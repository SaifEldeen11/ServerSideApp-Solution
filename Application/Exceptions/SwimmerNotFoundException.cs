using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public sealed class SwimmerNotFoundException(int id):NotFoundException($"Swimmer with id {id} was Not Found.")
    {
    }
}
