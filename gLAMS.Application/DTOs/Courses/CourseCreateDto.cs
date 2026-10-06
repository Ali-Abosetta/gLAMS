using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Courses
{
    public class CourseCreateDto : BaseCreateDto
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
