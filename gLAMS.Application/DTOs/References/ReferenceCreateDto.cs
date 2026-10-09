using System;
using System.ComponentModel.DataAnnotations;
using gLAMS.Domain.Entities;
using gLAMS.Domain.Enums;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;

namespace gLAMS.Application.DTOs.References
{
    public class ReferenceCreateDto : BaseCreateDto, IMappableDto<Reference>
    {
        [Required(ErrorMessage = "Reference's base course is required.")]
        public Guid CourseId { get; set; }

        public Guid? LessonId { get; set; }

        [Required(ErrorMessage = "Reference's title is required.")]
        [MaxLength(200, ErrorMessage = "Reference's title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reference's type is required.")]
        public ReferenceType Type { get; set; }

        [Required(ErrorMessage = "Reference's content value is required.")]
        public string ContentValue { get; set; } = string.Empty;

        public Reference ToEntity()
        {
            return new Reference
            {
                Id = this.Id,
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                Title = this.Title,
                Type = this.Type,
                ContentValue = this.ContentValue
            };
        }
    }
}
