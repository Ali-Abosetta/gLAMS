using System;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Question.MCQ
{
    public class McqOptionDto    
    {
        public Guid? Id { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int SortOrder { get; set; }

        public McqOptionDto()
        {
        }

        public McqOptionDto(McqOption option)
        {
            Id = option.Id;
            AnswerText = option.AnswerText;
            IsCorrect = option.IsCorrect;
            SortOrder = option.SortOrder;
        }

        public McqOption ToEntity(Guid questionId = default)
        {
            return new McqOption
            {
                Id = Id ?? Guid.NewGuid(),
                QuestionId = questionId,
                AnswerText = AnswerText,
                IsCorrect = IsCorrect,
                SortOrder = SortOrder
            };
        }
    }
}
