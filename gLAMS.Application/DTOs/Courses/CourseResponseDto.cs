using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Courses
{
    /// <summary>
    /// Data Transfer Object representing the outgoing response payload for a Course.
    /// </summary>
    public class CourseResponseDto : BaseResponseDto
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; } = string.Empty;

        public CourseResponseDto() { }
        public CourseResponseDto(Course course)
        {
            Id = course.Id;    
            FolderId = course.FolderId;
            Name = course.Name;
            CreatedAt = course.CreatedAt;
            UpdatedAt = course.UpdatedAt;
            IsDeleted = course.IsDeleted;
        }
    }
}
