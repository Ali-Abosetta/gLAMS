using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.FlashCards
{
    public class FlashCardCreateDto : BaseCreateDto, IMappableDto<FlashCard>
    {
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string FrontContent { get; set; } = string.Empty;
        public string BackContent { get; set; } = string.Empty;

        public FlashCardCreateDto() { }
        public FlashCardCreateDto(FlashCard flashCard)
        {
            this.Id = flashCard.Id;
            this.CourseId = flashCard.CourseId;
            this.LessonId = flashCard.LessonId;
            this.FrontContent = flashCard.FrontContent;
            this.BackContent = flashCard.BackContent;
        }

        public FlashCard ToEntity()
        {
            return new FlashCard
            {
                Id = this.Id,
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                FrontContent = this.FrontContent,
                BackContent = this.BackContent
            };
        }

    }
}
