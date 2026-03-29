using Application.Dtos.Coach_Dto;
using Application.Exceptions;
using Application.Interfaces;
using Application.ServiceInterfaces;
using AutoMapper;
using Core.Interfaces;
using Core.Models;
using System.Text.Json;

namespace Application.ServiceImplementation
{
    public class CoachService : ICoachService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public CoachService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        #region Exists
        public async Task<bool> CoachExistsAsync(int id)
        {
            var result = await _unitOfWork.Coaches.GetByIdAsync(id);
            if (result is null)
                throw new CoachNotFoundException(id);
            return true;
        }
        #endregion

        #region Create
        public async Task<CoachDto> CreateCoachAsync(CreateCoachDto createCoachDto)
        {
            var coach = _mapper.Map<Coach>(createCoachDto);
            coach.IsActive = true;
            await _unitOfWork.Coaches.AddAsync(coach);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateListCachesAsync();

            return _mapper.Map<CoachDto>(coach);
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteCoachAsync(int id)
        {
            var coach = await _unitOfWork.Coaches.GetByIdAsync(id);
            if (coach == null)
                throw new CoachNotFoundException(id);

            coach.IsActive = false;
            await _unitOfWork.SaveChangesAsync();

            await InvalidateSingleCacheAsync(id);
            await InvalidateListCachesAsync();

            return true;
        }
        #endregion

        #region Get All
        public async Task<IEnumerable<CoachDto>> GetAllCoachesAsync()
        {
            var cacheKey = "coaches:all";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<CoachDto>>(cached)!;

            var coaches = await _unitOfWork.Coaches.GetAllAsync();
            coaches = coaches.Where(c => c.IsActive);

            var result = _mapper.Map<IEnumerable<CoachDto>>(coaches);
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Get By Id
        public async Task<CoachDto> GetCoachByIdAsync(int id)
        {
            var cacheKey = $"coaches:{id}";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<CoachDto>(cached)!;

            var coach = await _unitOfWork.Coaches.GetByIdAsync(id);
            if (coach == null || !coach.IsActive)
                throw new CoachNotFoundException(id);

            var result = _mapper.Map<CoachDto>(coach);
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Update
        public async Task<CoachDto> UpdateCoachAsync(UpdateCoachDto updateCoachDto)
        {
            var existingCoach = await _unitOfWork.Coaches.GetByIdAsync(updateCoachDto.Id);
            if (existingCoach == null || !existingCoach.IsActive)
                throw new CoachNotFoundException(updateCoachDto.Id);

            _mapper.Map(updateCoachDto, existingCoach);
            await _unitOfWork.Coaches.UpdateAsync(existingCoach);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateSingleCacheAsync(updateCoachDto.Id);
            await InvalidateListCachesAsync();

            return _mapper.Map<CoachDto>(existingCoach);
        }
        #endregion

        #region Cache Invalidation
        private async Task InvalidateSingleCacheAsync(int id)
        {
            await _cacheService.DeleteAsync($"coaches:{id}");
        }

        private async Task InvalidateListCachesAsync()
        {
            await _cacheService.DeleteByPatternAsync("coaches:all*");
        }
        #endregion
    }
}