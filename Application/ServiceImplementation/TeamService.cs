using Application.Dtos.TeamDto;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Core.Models;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceImplementation
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            var coachExists = await _unitOfWork.Coaches.GetByIdAsync(createTeamDto.CoachId);
            if(coachExists is null)
            {
                throw new CoachNotFoundException(createTeamDto.CoachId);
            }
            var team = _mapper.Map<Team>(createTeamDto);
            team.IsActive = true;
            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TeamDto>(team);

        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team =  await _unitOfWork.Teams.GetByIdAsync(id);
            if(team is null)
            {
                throw new TeamNotFoundException(id);
            }
            team.IsActive = false; // soft delete
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            var teams = await _unitOfWork.Teams.GetAllAsync();
            var activeTeams = teams.Where(t => t.IsActive); // Active teams only 
            return _mapper.Map<IEnumerable<TeamDto>>(activeTeams);
        }

        public async Task<TeamDto?> GetTeamByIdAsync(int id)
        {
            var team =  await _unitOfWork.Teams.GetByIdAsync(id);
            if(team is null || !team.IsActive)
            {
                throw new TeamNotFoundException(id);
            }
            return _mapper.Map<TeamDto>(team);
        }

        public async Task<TeamDto> UpdateTeamAsync(UpdateTeamDto updateTeamDto)
        {
            var existingTeam =  await _unitOfWork.Teams.GetByIdAsync(updateTeamDto.Id);
            if(existingTeam is null)
            {
                throw new TeamNotFoundException(updateTeamDto.Id);
            }
            _mapper.Map(updateTeamDto, existingTeam);
            await _unitOfWork.Teams.UpdateAsync(existingTeam);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TeamDto>(existingTeam);
        }
    }
}
