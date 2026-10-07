using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Lessons
{
    public class LessonCreateDto : BaseCreateDto, IMappableDto<Lesson>
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;

        public LessonCreateDto() { }

        public LessonCreateDto(Lesson lesson)
        {
            Id = lesson.Id;
            CourseId = lesson.CourseId;
            Title = lesson.Title;
        }

        public Lesson ToEntity()
        {
            return new Lesson
            {
                Id = this.Id,
                CourseId = this.CourseId,
                Title = this.Title,
            };
        }
    }
}
