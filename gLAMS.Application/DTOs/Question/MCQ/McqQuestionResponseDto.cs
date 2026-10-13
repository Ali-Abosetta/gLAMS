using System;
using System.Collections.Generic;
using System.Linq;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Question.MCQ
{
    public class McqQuestionResponseDto : BaseResponseDto, IMappableDto<McqQuestion>
    {
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<McqOptionDto> Options { get; set; }

        public McqQuestionResponseDto()
        {
            Options = new List<McqOptionDto>();
        }

        public McqQuestionResponseDto(McqQuestion question)
        {
            this.Id = question.Id;
            this.CourseId = question.CourseId;
            this.LessonId = question.LessonId;
            this.QuestionText = question.QuestionText;
            this.Options = question.Options.Select(option => new McqOptionDto(option)).ToList();
            this.CreatedAt = question.CreatedAt;
            this.UpdatedAt = question.UpdatedAt;
            this.IsDeleted = question.IsDeleted;
        }

        public McqQuestion ToEntity()
        {
            return new McqQuestion
            {
                Id = this.Id,
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                QuestionText = this.QuestionText,
                Options = this.Options.Select(option => option.ToEntity(this.Id)).ToList(),
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
