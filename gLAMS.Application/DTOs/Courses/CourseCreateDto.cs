using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Courses
{
    /// <summary>
    /// Data Transfer Object representing the incoming payload to create a new Course.
    /// </summary>
    public class CourseCreateDto : BaseCreateDto, IMappableDto<Course>
    {

        public Guid FolderId { get; set; }
        public string Name { get; set; } = string.Empty;

        public CourseCreateDto() { }

        public CourseCreateDto(Course course)
        {
            Id = course.Id;
            FolderId = course.FolderId;
            Name = course.Name;
        }

        public Course ToEntity()
        {
            return new Course
            {
                Id = this.Id,
                FolderId = this.FolderId,
                Name = this.Name,
            };
        }
    }
}
