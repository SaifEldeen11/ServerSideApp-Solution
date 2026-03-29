using Application.Dtos.TeamDto;
using Application.Exceptions;
using Application.Interfaces;
using Application.ServiceInterfaces;
using AutoMapper;
using Core.Interfaces;
using Core.Models;
using System.Text.Json;

namespace Application.ServiceImplementation
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public TeamService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        #region Create
        public async Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            var coachExists = await _unitOfWork.Coaches.GetByIdAsync(createTeamDto.CoachId);
            if (coachExists is null)
                throw new CoachNotFoundException(createTeamDto.CoachId);

            var team = _mapper.Map<Team>(createTeamDto);
            team.IsActive = true;
            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateListCachesAsync();

            return _mapper.Map<TeamDto>(team);
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team is null)
                throw new TeamNotFoundException(id);

            team.IsActive = false;
            await _unitOfWork.SaveChangesAsync();

            await InvalidateSingleCacheAsync(id);
            await InvalidateListCachesAsync();

            return true;
        }
        #endregion

        #region Get All
        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            var cacheKey = "teams:all";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<TeamDto>>(cached)!;

            var teams = await _unitOfWork.Teams.GetAllAsync();
            var activeTeams = teams.Where(t => t.IsActive);

            var result = _mapper.Map<IEnumerable<TeamDto>>(activeTeams);
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Get By Id
        public async Task<TeamDto?> GetTeamByIdAsync(int id)
        {
            var cacheKey = $"teams:{id}";
            var cached = await _cacheService.GetAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<TeamDto>(cached)!;

            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team is null || !team.IsActive)
                throw new TeamNotFoundException(id);

            var result = _mapper.Map<TeamDto>(team);
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        #endregion

        #region Update
        public async Task<TeamDto> UpdateTeamAsync(UpdateTeamDto updateTeamDto)
        {
            var existingTeam = await _unitOfWork.Teams.GetByIdAsync(updateTeamDto.Id);
            if (existingTeam is null)
                throw new TeamNotFoundException(updateTeamDto.Id);

            _mapper.Map(updateTeamDto, existingTeam);
            await _unitOfWork.Teams.UpdateAsync(existingTeam);
            await _unitOfWork.SaveChangesAsync();

            await InvalidateSingleCacheAsync(updateTeamDto.Id);
            await InvalidateListCachesAsync();

            return _mapper.Map<TeamDto>(existingTeam);
        }
        #endregion

        #region Cache Invalidation
        private async Task InvalidateSingleCacheAsync(int id)
        {
            await _cacheService.DeleteAsync($"teams:{id}");
        }

        private async Task InvalidateListCachesAsync()
        {
            await _cacheService.DeleteByPatternAsync("teams:all*");
        }
        #endregion
    }
}