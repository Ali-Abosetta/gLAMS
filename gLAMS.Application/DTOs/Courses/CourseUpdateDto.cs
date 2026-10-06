using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Courses
{
    /// <summary>
    /// Data Transfer Object representing the incoming payload to update an existing Course.
    /// </summary>
    public class CourseUpdateDto
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; } = string.Empty;

        public CourseUpdateDto() { }

        public CourseUpdateDto(Course course)
        {
            FolderId = course.FolderId;
            Name = course.Name;
        }

        public Course ToEntity()
        {
            return new Course
            {
                FolderId = this.FolderId,
                Name = this.Name,
            };
        }

    }
}
