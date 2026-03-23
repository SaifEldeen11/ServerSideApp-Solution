using Application.Dtos.Coach_Dto;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceImplementation
{
    public class CoachService : ICoachService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CoachService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CoachExistsAsync(int id)
        {
            var result =  await _unitOfWork.Coaches.ExistsAsync(id);
            if(result == false)
            {
                throw new CoachNotFoundException(id);
            }
            return result;
        }

        public async Task<CoachDto> CreateCoachAsync(CreateCoachDto createCoachDto)
        {
            var caoch = _mapper.Map<Coach>(createCoachDto);
            caoch.IsActive = true;
            await _unitOfWork.Coaches.AddAsync(caoch);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CoachDto>(caoch);
        }

        public async Task<bool> DeleteCoachAsync(int id)
        {
            var coach = await _unitOfWork.Coaches.GetByIdAsync(id);
            if (coach == null)
                throw new CoachNotFoundException(id);

            coach.IsActive = false; // soft delete
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CoachDto>> GetAllCoachesAsync()
        {
            var coaches = await _unitOfWork.Coaches.GetAllAsync();
            coaches = coaches.Where(c => c.IsActive);
            return _mapper.Map<IEnumerable<CoachDto>>(coaches);
        }

        public async Task<CoachDto> GetCoachByIdAsync(int id)
        {
            var coach =  await _unitOfWork.Coaches.GetByIdAsync(id);
            if (coach == null || !coach.IsActive)
                throw new CoachNotFoundException(id);

            return _mapper.Map<CoachDto>(coach);
        }

        public async Task<CoachDto> UpdateCoachAsync(UpdateCoachDto updateCoachDto)
        {
            var existingCoach = await _unitOfWork.Coaches.GetByIdAsync(updateCoachDto.Id);
            if (existingCoach == null || !existingCoach.IsActive)
            {
                throw new CoachNotFoundException(updateCoachDto.Id);
            }
            _mapper.Map(updateCoachDto, existingCoach);
            await _unitOfWork.Coaches.UpdateAsync(existingCoach);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CoachDto>(existingCoach);


        }
    }
}
