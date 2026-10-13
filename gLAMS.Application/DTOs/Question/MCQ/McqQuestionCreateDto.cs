using System;
using System.Collections.Generic;
using System.Linq;
using gLAMS.Domain.Entities;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;

namespace gLAMS.Application.DTOs.Question.MCQ
{
    public class McqQuestionCreateDto : BaseCreateDto, IMappableDto<McqQuestion>
    {
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<McqOptionDto> Options { get; set; }

        public McqQuestionCreateDto() 
        {
            Options = new List<McqOptionDto>();
        }

        public McqQuestionCreateDto(McqQuestion question)
        {
            this.Id = question.Id;
            this.CourseId = question.CourseId;
            this.LessonId = question.LessonId;
            this.QuestionText = question.QuestionText;
            this.Options = question.Options.Select(option => new McqOptionDto(option)).ToList();
        }

        public McqQuestion ToEntity()
        {
            return new McqQuestion
            {
                Id = this.Id,
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                QuestionText = this.QuestionText,
                Options = this.Options.Select(option => option.ToEntity(this.Id)).ToList()
            };
        }
    }
}
