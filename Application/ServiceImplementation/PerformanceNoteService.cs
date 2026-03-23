using Application.Dtos.PerformanceNote;
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
    public class PerformanceNoteService : IPerformanceNoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PerformanceNoteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PerformanceNoteDto> CreateNoteAsync(CreatePerformanceNoteDto createNoteDto)
        {
            var swimmer = await _unitOfWork.Swimmers.GetByIdAsync(createNoteDto.SwimmerId);
            if (swimmer is null || !swimmer.IsActive)
                throw new SwimmerNotFoundException(createNoteDto.SwimmerId);

            var coach = await _unitOfWork.Coaches.GetByIdAsync(createNoteDto.CoachId);
            if (coach is null)
                throw new CoachNotFoundException(createNoteDto.CoachId);

            var note = _mapper.Map<PerformanceNote>(createNoteDto);
            note.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.PerformanceNotes.AddAsync(note);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PerformanceNoteDto>(note);
        }

        public async Task<bool> DeleteNoteAsync(int id)
        {
            var note = await _unitOfWork.PerformanceNotes.GetByIdAsync(id);
            if (note == null)
                throw new PerformanceNoteNotFoundException(id);

            note.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PerformanceNoteDto>> GetAllNotesAsync()
        {
            var notes = await _unitOfWork.PerformanceNotes.GetAllAsync();
            notes = notes.Where(n => !n.IsDeleted);
            return _mapper.Map<IEnumerable<PerformanceNoteDto>>(notes);
        }

        public async Task<PerformanceNoteDto?> GetNoteByIdAsync(int id)
        {
            var note =  await _unitOfWork.PerformanceNotes.GetByIdAsync(id);
            if (note == null || note.IsDeleted)
                throw new PerformanceNoteNotFoundException(id);

            return _mapper.Map<PerformanceNoteDto>(note);
        }

        public async Task<IEnumerable<PerformanceNoteDto>> GetNotesBySwimmerAsync(int swimmerId)
        {
            var ExistingSwimmer = await _unitOfWork.Swimmers.GetByIdAsync(swimmerId);
            if (ExistingSwimmer is null || !ExistingSwimmer.IsActive)
                throw new SwimmerNotFoundException(swimmerId);

            var notes = await _unitOfWork.PerformanceNotes.FindAsync(n => n.SwimmerId == swimmerId);
            notes = notes.Where(n => !n.IsDeleted);
            return _mapper.Map<IEnumerable<PerformanceNoteDto>>(notes.OrderByDescending(n => n.NoteDate));
        }
    }
}
