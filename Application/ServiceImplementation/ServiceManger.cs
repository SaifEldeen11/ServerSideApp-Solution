using Application.Interfaces;
using Application.ServiceImplementation;
using Application.ServiceInterfaces;
using AutoMapper;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class ServiceManager : IServiceManger
    {
        private readonly Lazy<ICoachService> _coachService;
        private readonly Lazy<ISwimmerService> _swimmerService;
        private readonly Lazy<ITeamService> _teamService;
        private readonly Lazy<IPerformanceRecordService> _performanceRecordService;
        private readonly Lazy<IPerformanceNoteService> _performanceNoteService;
        private readonly Lazy<IAuthService> _authService;

        public ServiceManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            ICacheService cacheService)
        {
            _coachService = new Lazy<ICoachService>(() =>
                new CoachService(unitOfWork, mapper, cacheService));

            _swimmerService = new Lazy<ISwimmerService>(() =>
                new SwimmerService(unitOfWork, mapper, cacheService));

            _teamService = new Lazy<ITeamService>(() =>
                new TeamService(unitOfWork, mapper, cacheService));

            _performanceRecordService = new Lazy<IPerformanceRecordService>(() =>
                new PerformanceRecordService(unitOfWork, mapper));

            _performanceNoteService = new Lazy<IPerformanceNoteService>(() =>
                new PerformanceNoteService(unitOfWork, mapper));

            _authService = new Lazy<IAuthService>(() =>
                new AuthService(userManager, signInManager, roleManager, tokenService));
        }

        public ICoachService CoachService => _coachService.Value;
        public ISwimmerService SwimmerService => _swimmerService.Value;
        public ITeamService TeamService => _teamService.Value;
        public IPerformanceRecordService PerformanceRecordService => _performanceRecordService.Value;
        public IPerformanceNoteService PerformanceNoteService => _performanceNoteService.Value;
        public IAuthService AuthService => _authService.Value;
    }
}