using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Domain.Entities.Base;

namespace gLAMS.Application.DTOs.Notes
{
    public class NoteResponseDto : BaseResponseDto, IMappableDto<Note>
    {
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string NoteText { get; set; } = string.Empty;
        
        public NoteResponseDto() { }
        public NoteResponseDto(Note note)
        {
            this.Id = note.Id;
            this.CourseId = note.CourseId;
            this.LessonId = note.LessonId;
            this.Title = note.Title;
            this.NoteText = note.NoteText;
            this.CreatedAt = note.CreatedAt;
            this.UpdatedAt = note.UpdatedAt;
            this.IsDeleted = note.IsDeleted;
        }

        public Note ToEntity()
        {
            return new Note
            {
                Id = this.Id,
                CourseId = this.CourseId,
                LessonId = this.LessonId,
                Title = this.Title,
                NoteText = this.NoteText,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
