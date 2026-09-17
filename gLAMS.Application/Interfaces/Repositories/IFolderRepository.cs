using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines specific data access operations for Folder entities.
    /// </summary>
    public interface IFolderRepository : IRepository<Folder>
    {
        /// <summary>
        /// Retrieves all top-level folders (where ParentFolderId is null).
        /// </summary>
        /// <returns>A Result containing the collection of root folders.</returns>
        Task<Result<IEnumerable<Folder>>> GetRootFoldersAsync();

        /// <summary>
        /// Retrieves all child folders contained within a specific parent folder.
        /// </summary>
        /// <param name="parentFolderId">The ID of the parent folder.</param>
        /// <returns>A Result containing the collection of child folders.</returns>
        Task<Result<IEnumerable<Folder>>> GetSubfoldersByParentIdAsync(Guid parentFolderId);
    }
}