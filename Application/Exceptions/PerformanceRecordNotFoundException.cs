using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public sealed class PerformanceRecordNotFoundException(int id):NotFoundException($"Record with Id: {id} was Not Found.")
    {
    }
}
