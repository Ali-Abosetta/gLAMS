using System;
using System.Collections.Generic;
using System.Linq;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Question.MCQ
{
    public class McqQuestionUpdateDto : IMappableDto<McqQuestion>
    {
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<McqOptionDto> Options { get; set; }

        public McqQuestionUpdateDto()
        {
            Options = new List<McqOptionDto>();
        }

        public McqQuestionUpdateDto(McqQuestion question)
        {
            this.CourseId = question.CourseId;
            this.LessonId = question.LessonId;
            this.QuestionText = question.QuestionText;
            this.Options = question.Options.Select(option => new McqOptionDto(option)).ToList();
        }

        public McqQuestion ToEntity()
        {
            return new McqQuestion
            {
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                QuestionText = this.QuestionText,
                Options = this.Options.Select(option => option.ToEntity()).ToList()
            };
        }
    }
}
