using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface IServiceManger
    {
        ICoachService CoachService { get; }
        ISwimmerService SwimmerService { get; }
        ITeamService TeamService { get; }
        IPerformanceRecordService PerformanceRecordService { get; }
        IPerformanceNoteService PerformanceNoteService { get; }

        IAuthService AuthService { get; }

    }
}
