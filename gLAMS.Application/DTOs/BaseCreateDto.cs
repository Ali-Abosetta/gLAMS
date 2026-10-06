using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gLAMS.Application.DTOs
{
    /// <summary>
    /// Abstract base class for all creation DTOs, enforcing the inclusion of an explicit ID.
    /// </summary>
    public abstract class BaseCreateDto
    {
        public Guid Id { get; set; }
    }
}
