using System;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;
using gLAMS.Domain.Enums;

namespace gLAMS.Application.DTOs.References
{
    public class ReferenceResponseDto : BaseResponseDto, IMappableDto<Reference>
    {
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public ReferenceType Type { get; set; }
        public string ContentValue { get; set; } = string.Empty;
        
        public ReferenceResponseDto() { }
        public ReferenceResponseDto(Reference reference)
        {
            this.Id = reference.Id;
            this.CourseId = reference.CourseId;
            this.LessonId = reference.LessonId;
            this.Title = reference.Title;
            this.Type = reference.Type;
            this.ContentValue = reference.ContentValue;
            this.CreatedAt = reference.CreatedAt;
            this.UpdatedAt = reference.UpdatedAt;
            this.IsDeleted = reference.IsDeleted;
        }

        public Reference ToEntity()
        {
            return new Reference
            {
                Id = this.Id,
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                Title = this.Title,
                Type = this.Type,
                ContentValue = this.ContentValue,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
