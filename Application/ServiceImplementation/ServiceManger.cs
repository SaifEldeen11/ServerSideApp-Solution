using Application.Interfaces;
using Application.ServiceInterfaces;
using AutoMapper;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceImplementation
{
    public class ServiceManger(IUnitOfWork unitOfWork, IMapper mapper,
                               SignInManager<ApplicationUser>_signInManager, UserManager<ApplicationUser> _userManager
                                ,RoleManager<IdentityRole> _roleManager,ITokenService _tokenService) : IServiceManger
    {
        private readonly Lazy<ICoachService> _coachService = new Lazy<ICoachService>(() => new CoachService(unitOfWork, mapper));
        private readonly Lazy<ISwimmerService> _swimmerService = new Lazy<ISwimmerService>(() => new SwimmerService(unitOfWork, mapper));
        private readonly Lazy<ITeamService> _teamService = new Lazy<ITeamService>(() => new TeamService(unitOfWork, mapper));
        private readonly Lazy<IPerformanceRecordService> _performanceRecordService = new Lazy<IPerformanceRecordService>(() => new PerformanceRecordService(unitOfWork, mapper));
        private readonly Lazy<IPerformanceNoteService> _performanceNoteService = new Lazy<IPerformanceNoteService>(() => new PerformanceNoteService(unitOfWork, mapper));
        private readonly Lazy<IAuthService> _AuthService = new Lazy<IAuthService>(() => new AuthService(_userManager, _signInManager, _roleManager, _tokenService));
        public ICoachService CoachService => _coachService.Value;
        public ISwimmerService SwimmerService => _swimmerService.Value;
        public ITeamService TeamService => _teamService.Value;
        public IPerformanceRecordService PerformanceRecordService => _performanceRecordService.Value;
        public IPerformanceNoteService PerformanceNoteService => _performanceNoteService.Value;

        public IAuthService AuthService => _AuthService.Value;
    }
}
