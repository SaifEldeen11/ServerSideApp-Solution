using Application.Dtos.Coach_Dto;
using Application.Dtos.PerformanceNote;
using Application.Dtos.PerformanceRecord;
using Application.Dtos.Swimmer_Dto;
using Application.Dtos.TeamDto;
using AutoMapper;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PerformanceNoteDto = Application.Dtos.PerformanceNote.PerformanceNoteDto;

namespace Application
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ========== COACH MAPPINGS ==========
            CreateMap<Coach, CoachDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.TeamCount,
                    opt => opt.MapFrom(src => src.Teams != null
                        ? src.Teams.Count(t => t.IsActive)
                        : 0));

            CreateMap<CreateCoachDto, Coach>();
            CreateMap<UpdateCoachDto, Coach>();

            // ========== SWIMMER MAPPINGS ==========
            CreateMap<Swimmer, SwimmerDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => DateTime.UtcNow.Year - src.DateOfBirth.Year))
                .ForMember(dest => dest.CompetitionReadinessName,
                    opt => opt.MapFrom(src => src.CompetitionReadiness.ToString()))
                .ForMember(dest => dest.TeamName,
                    opt => opt.MapFrom(src => src.Team != null ? src.Team.Name : null));

            CreateMap<CreateSwimmerDto, Swimmer>();
            CreateMap<UpdateSwimmerDto, Swimmer>();

            // ========== TEAM MAPPINGS ==========
            CreateMap<Team, TeamDto>()
                .ForMember(dest => dest.CoachName,
                    opt => opt.MapFrom(src => src.Coach != null ? src.Coach.FullName : null))
                .ForMember(dest => dest.SwimmerCount,
                    opt => opt.MapFrom(src => src.Swimmers != null
                        ? src.Swimmers.Count(s => s.IsActive)
                        : 0));

            CreateMap<CreateTeamDto, Team>();
            CreateMap<UpdateTeamDto, Team>();

            // ========== PERFORMANCE RECORD MAPPINGS ==========
            CreateMap<PerformanceRecord, PerformanceRecordDto>()
                .ForMember(dest => dest.SwimmerName,
                    opt => opt.MapFrom(src => src.Swimmer != null ? src.Swimmer.FullName : null))
                .ForMember(dest => dest.RecordedByCoachName,
                    opt => opt.MapFrom(src => src.RecordedByCoach != null ? src.RecordedByCoach.FullName : null));

            CreateMap<CreatePerformanceRecordDto, PerformanceRecord>();
            CreateMap<UpdatePerformanceRecordDto, PerformanceRecord>();

            // ========== PERFORMANCE NOTE MAPPINGS ==========
            CreateMap<PerformanceNote, PerformanceNoteDto>()
                .ForMember(dest => dest.SwimmerName,
                    opt => opt.MapFrom(src => src.Swimmer != null ? src.Swimmer.FullName : null))
                .ForMember(dest => dest.CoachName,
                    opt => opt.MapFrom(src => src.Coach != null ? src.Coach.FullName : null));

            CreateMap<CreatePerformanceNoteDto, PerformanceNote>();

            // ========== DASHBOARD MAPPINGS ==========
            CreateMap<PerformanceRecord, RecentPerformanceDto>()
                .ForMember(dest => dest.Distance,
                    opt => opt.MapFrom(src => (int)src.Distance))
                .ForMember(dest => dest.RecordedByCoachName,
                    opt => opt.MapFrom(src => src.RecordedByCoach != null ? src.RecordedByCoach.FullName : null));

            CreateMap<PerformanceRecord, PerformanceRecordSimpleDto>();
        }
    }
}