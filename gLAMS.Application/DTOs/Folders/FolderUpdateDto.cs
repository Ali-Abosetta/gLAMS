using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;

namespace gLAMS.Application.DTOs.Folders
{
    /// <summary>
    /// Data Transfer Object representing the incoming payload to update an existing Folder.
    /// </summary>
    public class FolderUpdateDto
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;

        public FolderUpdateDto() { }
        public FolderUpdateDto(Folder folder)
        {
            ParentId = folder.ParentId;
            Name = folder.Name;
        }
        public FolderUpdateDto(FolderResponseDto folder)
        {
            ParentId = folder.ParentId;
            Name = folder.Name;
        }

        public Folder ToEntity()
        {
            return new Folder
            {
                ParentId = this.ParentId,
                Name = this.Name,
            };
        }
    }
}
