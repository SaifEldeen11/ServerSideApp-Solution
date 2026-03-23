using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public sealed class CoachNotFoundException(int id): NotFoundException($"coach with id {id} was Not Found.")
    {
    }
}
