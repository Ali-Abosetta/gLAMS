using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Folders
{
    public class FolderResponseDto
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public FolderResponseDto() { }
        public FolderResponseDto(Folder folder) 
        {
            Id = folder.Id;
            ParentId = folder.ParentId;
            Name = folder.Name;
            CreatedAt = folder.CreatedAt;
            UpdatedAt = folder.UpdatedAt;
            IsDeleted = folder.IsDeleted;
        }

        public Folder ToEntity()
        {
            return new Folder
            {
                Id = this.Id,
                ParentId = this.ParentId,
                Name = this.Name,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                IsDeleted = this.IsDeleted
            };
        }
    }
}
