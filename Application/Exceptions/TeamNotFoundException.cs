using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public sealed class TeamNotFoundException(int id): NotFoundException($"team with id: {id} was Not Found.")
    {
    }
}
