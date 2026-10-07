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
    public class LessonResponseDto : BaseResponseDto, IMappableDto<Lesson>
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SortOrder { get; set; }

        public LessonResponseDto() { }

        public LessonResponseDto(Lesson lesson)
        {
            Id = lesson.Id;
            CourseId = lesson.CourseId;
            Title = lesson.Title;
            SortOrder = lesson.SortOrder;
            CreatedAt = lesson.CreatedAt;
            UpdatedAt = lesson.UpdatedAt;
            IsDeleted = lesson.IsDeleted;
        }

        public Lesson ToEntity()
        {
            return new Lesson
            {
                Id = this.Id,
                CourseId = this.CourseId,
                Title = this.Title,
                SortOrder = this.SortOrder,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
