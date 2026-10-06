using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gLAMS.Application.DTOs
{
    /// <summary>
    /// Abstract base class for all response DTOs, containing offline-sync metadata (timestamps and tombstone flags).
    /// </summary>
    public abstract class BaseResponseDto
    {

        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
