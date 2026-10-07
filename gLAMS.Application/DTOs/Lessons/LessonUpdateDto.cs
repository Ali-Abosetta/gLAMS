using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Lessons
{
    public class LessonUpdateDto : IMappableDto<Lesson>
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }

        public LessonUpdateDto() { }

        public LessonUpdateDto(Lesson lesson)
        {
            CourseId = lesson.CourseId;
            Title = lesson.Title;
            SortOrder = lesson.SortOrder;
        }

        public Lesson ToEntity()
        {
            return new Lesson
            {
                CourseId = this.CourseId,
                Title = this.Title,
                SortOrder = this.SortOrder,
            };
        }
    }
}
