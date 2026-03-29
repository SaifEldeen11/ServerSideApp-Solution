using Application.Dtos.Swimmer_Dto;
using Application.Exceptions;
using Application.Interfaces;
using Application.Pagination;
using Application.ServiceInterfaces;
using AutoMapper;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using System.Text.Json;

namespace Application.ServiceImplementation
{
    public class SwimmerService : ISwimmerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public SwimmerService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        #region Create
        public async Task<SwimmerDto> CreateSwimmerAsync(CreateSwimmerDto createSwimmerDto)
        {
            var swimmer = _mapper.Map<Swimmer>(createSwimmerDto);
            swimmer.IsActive = true;
            swimmer.CompetitionReadiness = CompetitionReadiness.NotReady;

            await _unitOfWork.Swimmers.AddAsync(swimmer);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateListCachesAsync();

            return _mapper.Map<SwimmerDto>(swimmer);
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteSwimmerAsync(int id)
        {
            var swimmer = await _unitOfWork.Swimmers.GetByIdAsync(id);
            if (swimmer is null)
                throw new SwimmerNotFoundException(id);

            swimmer.IsActive = false;
            await _unitOfWork.SaveChangesAsync();

            await InvalidateSingleCacheAsync(id);
            await InvalidateListCachesAsync();

            return true;
        }
        #endregion

        #region Get All
        public async Task<PaginatedResult<SwimmerDto>> GetAllSwimmersAsync(PaginationParams pagination)
        {
            var cacheKey = $"swimmers:all:page:{pagination.PageIndex}";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResult<SwimmerDto>>(cached)!;

            var swimmers = await _unitOfWork.Swimmers.GetAllAsync();
            swimmers = swimmers.Where(s => s.IsActive);

            var totalCount = swimmers.Count();
            var data = swimmers
                .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            var result = new PaginatedResult<SwimmerDto>(
                pagination.PageIndex,
                pagination.PageSize,
                totalCount,
                _mapper.Map<IEnumerable<SwimmerDto>>(data));

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Get By Id
        public async Task<SwimmerDto?> GetSwimmerByIdAsync(int id)
        {
            var cacheKey = $"swimmers:{id}";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<SwimmerDto>(cached)!;

            var swimmer = await _unitOfWork.Swimmers.GetByIdAsync(id);
            if (swimmer is null || !swimmer.IsActive)
                throw new SwimmerNotFoundException(id);

            var result = _mapper.Map<SwimmerDto>(swimmer);
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Dashboard
        public async Task<SwimmerDashboardDto?> GetSwimmerDashboardAsync(int swimmerId)
        {
            var cacheKey = $"swimmers:dashboard:{swimmerId}";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<SwimmerDashboardDto>(cached)!;

            var swimmer = await _unitOfWork.Swimmers.GetSwimmerWithPerformanceRecordsAsync(swimmerId);
            if (swimmer == null || !swimmer.IsActive)
                return null;

            var swimmerDto = _mapper.Map<SwimmerDto>(swimmer);
            var allRecords = swimmer.PerformanceRecords.OrderByDescending(r => r.RecordedDate).ToList();
            var recentRecords = _mapper.Map<List<RecentPerformanceDto>>(allRecords.Take(10));

            var swimmerWithNotes = await _unitOfWork.Swimmers.GetSwimmerWithNotesAsync(swimmerId);
            var recentNotes = swimmerWithNotes?.PerformanceNotes
                .OrderByDescending(n => n.NoteDate)
                .Take(5)
                .Select(n => new PerformanceNoteDto
                {
                    Id = n.Id,
                    Note = n.Note,
                    NoteDate = n.NoteDate,
                    CoachName = n.Coach.FullName
                })
                .ToList() ?? new List<PerformanceNoteDto>();

            var performanceByDistance = new List<DistancePerformanceDto>();
            var distances = new[] { EventDistance.Fifty, EventDistance.Hundred, EventDistance.TwoHundred, EventDistance.FourHundred };

            foreach (var distance in distances)
            {
                var distanceRecords = allRecords.Where(r => r.Distance == distance).ToList();
                if (!distanceRecords.Any()) continue;

                var bestRecord = distanceRecords.OrderBy(r => r.TimeInSeconds).First();
                var latestRecord = distanceRecords.OrderByDescending(r => r.RecordedDate).First();
                var avgTime = distanceRecords.Average(r => r.TimeInSeconds);

                decimal? improvementPercentage = null;
                if (distanceRecords.Count > 1)
                {
                    var firstRecord = distanceRecords.OrderBy(r => r.RecordedDate).First();
                    improvementPercentage = ((firstRecord.TimeInSeconds - bestRecord.TimeInSeconds) / firstRecord.TimeInSeconds) * 100;
                }

                performanceByDistance.Add(new DistancePerformanceDto
                {
                    Distance = (int)distance,
                    BestTime = bestRecord.TimeInSeconds,
                    BestTimeDate = bestRecord.RecordedDate,
                    AverageTime = avgTime,
                    LatestTime = latestRecord.TimeInSeconds,
                    TotalAttempts = distanceRecords.Count,
                    ImprovementPercentage = improvementPercentage,
                    History = _mapper.Map<List<PerformanceRecordSimpleDto>>(
                        distanceRecords.OrderByDescending(r => r.RecordedDate).Take(10))
                });
            }

            var mostImprovedDistance = performanceByDistance
                .Where(p => p.ImprovementPercentage.HasValue)
                .OrderByDescending(p => p.ImprovementPercentage)
                .FirstOrDefault();

            var statistics = new PerformanceStatisticsDto
            {
                TotalPerformances = allRecords.Count,
                TotalNotes = recentNotes.Count,
                OverallAverageTime = allRecords.Any() ? allRecords.Average(r => r.TimeInSeconds) : null,
                MostImprovedDistance = mostImprovedDistance != null ? $"{mostImprovedDistance.Distance}m" : null,
                MostImprovedPercentage = mostImprovedDistance?.ImprovementPercentage
            };

            var result = new SwimmerDashboardDto
            {
                Swimmer = swimmerDto,
                PerformanceByDistance = performanceByDistance,
                RecentPerformances = recentRecords,
                RecentNotes = recentNotes,
                Statistics = statistics
            };

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Get By Readiness
        public async Task<PaginatedResult<SwimmerDto>> GetSwimmersByReadinessAsync(CompetitionReadiness readiness, PaginationParams paginationParams)
        {
            var cacheKey = $"swimmers:readiness:{readiness}:page:{paginationParams.PageIndex}";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResult<SwimmerDto>>(cached)!;

            var swimmers = await _unitOfWork.Swimmers.GetSwimmersByReadinessAsync(readiness);
            swimmers = swimmers.Where(s => s.IsActive);

            var totalCount = swimmers.Count();
            var data = swimmers
                .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize);

            var result = new PaginatedResult<SwimmerDto>(
                paginationParams.PageIndex,
                paginationParams.PageSize,
                totalCount,
                _mapper.Map<IEnumerable<SwimmerDto>>(data));

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Get By Team
        public async Task<PaginatedResult<SwimmerDto>> GetSwimmersByTeamIdAsync(int teamId, PaginationParams paginationParams)
        {
            var cacheKey = $"swimmers:team:{teamId}:page:{paginationParams.PageIndex}";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResult<SwimmerDto>>(cached)!;

            var team = await _unitOfWork.Teams.GetByIdAsync(teamId);
            if (team == null || !team.IsActive)
                throw new TeamNotFoundException(teamId);

            var swimmers = await _unitOfWork.Swimmers.GetSwimmersByTeamAsync(teamId);
            swimmers = swimmers.Where(s => s.IsActive);

            var totalCount = swimmers.Count();
            var data = swimmers
                .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize);

            var result = new PaginatedResult<SwimmerDto>(
                paginationParams.PageIndex,
                paginationParams.PageSize,
                totalCount,
                _mapper.Map<IEnumerable<SwimmerDto>>(data));

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Update
        public async Task<SwimmerDto> UpdateSwimmerAsync(UpdateSwimmerDto updateSwimmerDto)
        {
            var existingSwimmer = await _unitOfWork.Swimmers.GetByIdAsync(updateSwimmerDto.Id);
            if (existingSwimmer == null || !existingSwimmer.IsActive)
                throw new SwimmerNotFoundException(updateSwimmerDto.Id);

            _mapper.Map(updateSwimmerDto, existingSwimmer);
            await _unitOfWork.Swimmers.UpdateAsync(existingSwimmer);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateSingleCacheAsync(updateSwimmerDto.Id);
            await InvalidateListCachesAsync();

            return _mapper.Map<SwimmerDto>(existingSwimmer);
        }
        #endregion

        #region Cache Invalidation
        private async Task InvalidateSingleCacheAsync(int id)
        {
            await _cacheService.DeleteAsync($"swimmers:{id}");
        }

        private async Task InvalidateListCachesAsync()
        {
            await _cacheService.DeleteByPatternAsync("swimmers:all:*");
            await _cacheService.DeleteByPatternAsync("swimmers:readiness:*");
            await _cacheService.DeleteByPatternAsync("swimmers:team:*");
        }
        #endregion
    }
}