using Application.Dtos.PerformanceRecord;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Core.Enums;
using Core.Models;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceImplementation
{
    public class PerformanceRecordService : IPerformanceRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PerformanceRecordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PerformanceRecordDto> CreateRecordAsync(CreatePerformanceRecordDto createRecordDto)
        {
            var existingSwimmer = await _unitOfWork.Swimmers.GetByIdAsync(createRecordDto.SwimmerId);
            if(existingSwimmer is null || !existingSwimmer.IsActive)
            {
                throw new SwimmerNotFoundException(createRecordDto.SwimmerId);
            }
            var coach = await _unitOfWork.Coaches.GetByIdAsync(createRecordDto.RecordedByCoachId);
            if(coach is null || !coach.IsActive)
            {
                throw new CoachNotFoundException(createRecordDto.RecordedByCoachId);
            }

            var record = _mapper.Map<PerformanceRecord>(createRecordDto);
            await _unitOfWork.PerformanceRecords.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PerformanceRecordDto>(record);
        }

        public async Task<bool> DeleteRecordAsync(int id)
        {
            var record = await _unitOfWork.PerformanceRecords.GetByIdAsync(id);
            if(record is null)
            {
                throw new PerformanceRecordNotFoundException(id);
            }
            record!.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;

        }

        public async Task<IEnumerable<PerformanceRecordDto>> GetAllRecordsAsync()
        {
            var records = await _unitOfWork.PerformanceRecords.GetAllAsync();
            records = records.Where(r => r.IsDeleted == false);
            return _mapper.Map<IEnumerable<PerformanceRecordDto>>(records);
        }

        public async Task<PerformanceRecordDto?> GetRecordByIdAsync(int id)
        {
            var record =  await _unitOfWork.PerformanceRecords.GetByIdAsync(id);
            if(record is null || record.IsDeleted)
            {
                throw new PerformanceRecordNotFoundException(id);
            }
            return _mapper.Map<PerformanceRecordDto>(record);
        }

        public async Task<IEnumerable<PerformanceRecordDto>> GetRecordsBySwimmerAndDistanceAsync(int swimmerId, EventDistance distance)
        {
            var ExistingSwimmer =  await _unitOfWork.Swimmers.GetByIdAsync(swimmerId);
            if(ExistingSwimmer is null || !ExistingSwimmer.IsActive)
            {
                throw new SwimmerNotFoundException(swimmerId);
            }
            var records =  await _unitOfWork.PerformanceRecords.GetRecordsBySwimmerAndDistanceAsync(swimmerId, distance);
            records = records.Where(r => r.IsDeleted == false);
            return _mapper.Map<IEnumerable<PerformanceRecordDto>>(records);
        }

        public async Task<IEnumerable<PerformanceRecordDto>> GetRecordsBySwimmerAsync(int swimmerId)
        {
            var ExistingSwimmer =  await _unitOfWork.Swimmers.GetByIdAsync(swimmerId);
            if(ExistingSwimmer is null || !ExistingSwimmer.IsActive)
            {
                throw new SwimmerNotFoundException(swimmerId);
            }
            var records =  await _unitOfWork.PerformanceRecords.GetRecordsBySwimmerAsync(swimmerId);
            records = records.Where(r => r.IsDeleted == false);
            return _mapper.Map<IEnumerable<PerformanceRecordDto>>(records);
        }

        public async Task<PerformanceRecordDto> UpdateRecordAsync(UpdatePerformanceRecordDto updateRecordDto)
        {
            var existingRecord = await _unitOfWork.PerformanceRecords.GetByIdAsync(updateRecordDto.Id);
            if(existingRecord is null || existingRecord.IsDeleted)
            {
                throw new PerformanceRecordNotFoundException(updateRecordDto.Id);
            }
            _mapper.Map(updateRecordDto, existingRecord);
            existingRecord.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.PerformanceRecords.UpdateAsync(existingRecord);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PerformanceRecordDto>(existingRecord);

        }
    }
}
