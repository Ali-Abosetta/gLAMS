using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.Base;
using gLAMS.Application.DTOs.Interfaces;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Folders
{
    /// <summary>
    /// Data Transfer Object representing the outgoing response payload for a Folder.
    /// </summary>
    public class FolderResponseDto : BaseResponseDto, IMappableDto<Folder>
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;

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
