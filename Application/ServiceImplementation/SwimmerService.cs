using Application.Dtos.Swimmer_Dto;
using Application.Exceptions;
using Application.Interfaces;
using Application.Pagination;
using AutoMapper;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceImplementation
{
    public class SwimmerService : ISwimmerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SwimmerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region Create
        public async Task<SwimmerDto> CreateSwimmerAsync(CreateSwimmerDto createSwimmerDto)
        {
            var swimmer = _mapper.Map<Swimmer>(createSwimmerDto);
            swimmer.IsActive = true;
            swimmer.CompetitionReadiness = CompetitionReadiness.NotReady;

            await _unitOfWork.Swimmers.AddAsync(swimmer);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SwimmerDto>(swimmer);

        } 
        #endregion

        #region Delete
        public async Task<bool> DeleteSwimmerAsync(int id)
        {
            var swimmer = await _unitOfWork.Swimmers.GetByIdAsync(id);
            if (swimmer is null)
            {
                throw new SwimmerNotFoundException(id);
            }
            swimmer.IsActive = false; // soft delete
            await _unitOfWork.SaveChangesAsync();
            return true;
        } 
        #endregion

        #region Get All
        public async Task<PaginatedResult<SwimmerDto>> GetAllSwimmersAsync(PaginationParams pagination)
        {
            var swimmers = await _unitOfWork.Swimmers.GetAllAsync();
            swimmers = swimmers.Where(s => s.IsActive);

            var totalCount = swimmers.Count();
            var data = swimmers
                .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                .Take(pagination.PageSize);

            return new PaginatedResult<SwimmerDto>(
                pagination.PageIndex,
                pagination.PageSize,
                totalCount,
                _mapper.Map<IEnumerable<SwimmerDto>>(data));
        }
        #endregion

        #region Get By Id
        public async Task<SwimmerDto?> GetSwimmerByIdAsync(int id)
        {
            var swimmer = await _unitOfWork.Swimmers.GetByIdAsync(id);
            if (swimmer is null || !swimmer.IsActive)
            {
                throw new SwimmerNotFoundException(id);
            }
            return _mapper.Map<SwimmerDto>(swimmer);

        }
        #endregion

        #region Dashboard
        public async Task<SwimmerDashboardDto?> GetSwimmerDashboardAsync(int swimmerId)
        {
            var swimmer = await _unitOfWork.Swimmers.GetSwimmerWithPerformanceRecordsAsync(swimmerId);
            if (swimmer == null || !swimmer.IsActive)
                return null;

            var swimmerDto = _mapper.Map<SwimmerDto>(swimmer);
            var allRecords = swimmer.PerformanceRecords.OrderByDescending(r => r.RecordedDate).ToList();
            var recentRecords = _mapper.Map<List<RecentPerformanceDto>>(allRecords.Take(10));

            // Get notes
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

            // Calculate performance by distance
            var performanceByDistance = new List<DistancePerformanceDto>();
            var distances = new[] { EventDistance.Fifty, EventDistance.Hundred, EventDistance.TwoHundred, EventDistance.FourHundred };

            foreach (var distance in distances)
            {
                var distanceRecords = allRecords.Where(r => r.Distance == distance).ToList();
                if (!distanceRecords.Any())
                    continue;

                var bestRecord = distanceRecords.OrderBy(r => r.TimeInSeconds).First();
                var latestRecord = distanceRecords.OrderByDescending(r => r.RecordedDate).First();
                var avgTime = distanceRecords.Average(r => r.TimeInSeconds);

                // Calculate improvement
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

            // Calculate statistics
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

            return new SwimmerDashboardDto
            {
                Swimmer = swimmerDto,
                PerformanceByDistance = performanceByDistance,
                RecentPerformances = recentRecords,
                RecentNotes = recentNotes,
                Statistics = statistics
            };
        }
        #endregion

        public async Task<PaginatedResult<SwimmerDto>> GetSwimmersByReadinessAsync(CompetitionReadiness readiness, PaginationParams paginationParams)
        {
            var swimmers = await _unitOfWork.Swimmers.GetSwimmersByReadinessAsync(readiness);
            swimmers = swimmers.Where(s => s.IsActive);

            var totalCount = swimmers.Count();
            var data = swimmers
                .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize);

            return new PaginatedResult<SwimmerDto>(
                paginationParams.PageIndex,
                paginationParams.PageSize,
                totalCount,
                _mapper.Map<IEnumerable<SwimmerDto>>(data));
        }

        public async Task<PaginatedResult<SwimmerDto>> GetSwimmersByTeamIdAsync(int teamId, PaginationParams paginationParams)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(teamId);
            if (team == null || !team.IsActive)
                throw new TeamNotFoundException(teamId);

            var swimmers = await _unitOfWork.Swimmers.GetSwimmersByTeamAsync(teamId);
            swimmers = swimmers.Where(s => s.IsActive);

            var totalCount = swimmers.Count();
            var data = swimmers
                .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize);

            return new PaginatedResult<SwimmerDto>(
                paginationParams.PageIndex,
                paginationParams.PageSize,
                totalCount,
                _mapper.Map<IEnumerable<SwimmerDto>>(data));
        }

        #region Update
        public async Task<SwimmerDto> UpdateSwimmerAsync(UpdateSwimmerDto updateSwimmerDto)
        {
            var existingSwimmer = await _unitOfWork.Swimmers.GetByIdAsync(updateSwimmerDto.Id);
            if (existingSwimmer == null || !existingSwimmer.IsActive)
                throw new SwimmerNotFoundException(existingSwimmer!.Id);

            _mapper.Map(updateSwimmerDto, existingSwimmer);
            await _unitOfWork.Swimmers.UpdateAsync(existingSwimmer);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SwimmerDto>(existingSwimmer);
        } 
        #endregion
    }
}
