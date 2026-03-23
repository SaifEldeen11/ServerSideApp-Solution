using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public sealed class PerformanceNoteNotFoundException(int id):NotFoundException($"Note with Id: {id} was Not Found.")
    {
    }
}
