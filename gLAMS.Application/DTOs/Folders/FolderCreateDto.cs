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
    /// Data Transfer Object representing the incoming payload to create a new Folder.
    /// </summary>
    public class FolderCreateDto : BaseCreateDto, IMappableDto<Folder>
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;

        public FolderCreateDto() { }
        public FolderCreateDto(Folder folder) 
        {
            Id = folder.Id; 
            ParentId = folder.ParentId;
            Name = folder.Name;
        }
        public FolderCreateDto(FolderResponseDto folder)
        {
            Id = folder.Id;
            ParentId= folder.ParentId;
            Name = folder.Name;
        }

        public Folder ToEntity()
        {
            return new Folder
            {
                Id = this.Id,
                ParentId = this.ParentId,
                Name = this.Name,
            };
        }
    }
}
