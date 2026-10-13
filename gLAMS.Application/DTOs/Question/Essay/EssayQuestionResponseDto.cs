using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Question.Essay
{
    public class EssayQuestionResponseDto : BaseResponseDto, IMappableDto<EssayQuestion>
    {
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string ModelAnswer { get; set; } = string.Empty;

        public EssayQuestionResponseDto() { }
        public EssayQuestionResponseDto(EssayQuestion question)
        {
            this.Id = question.Id;
            this.CourseId = question.CourseId;
            this.LessonId = question.LessonId;
            this.QuestionText = question.QuestionText;
            this.ModelAnswer = question.ModelAnswer;
            this.CreatedAt = question.CreatedAt;
            this.UpdatedAt = question.UpdatedAt;
            this.IsDeleted = question.IsDeleted;
        }

        public EssayQuestion ToEntity()
        {
            return new EssayQuestion
            {
                Id = this.Id,
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                QuestionText = this.QuestionText,
                ModelAnswer = this.ModelAnswer,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
