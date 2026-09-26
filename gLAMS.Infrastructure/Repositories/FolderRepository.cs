using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Application.Interfaces.Logging;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories.Base;
using gLAMS.Shared.Responses;
using Microsoft.Data.SqlClient;

namespace gLAMS.Infrastructure.Repositories
{

    /// <summary>
    /// Implements the IFolderRepository using raw ADO.NET and Stored Procedures.
    /// Inherits from BaseRepository to utilize dry execution helpers and global error handling.
    /// </summary>
    public class FolderRepository : BaseRepository, IFolderRepository
    {
        private Folder MapReaderToFolder(SqlDataReader reader)
        {
            return new Folder
            {
                Id = reader.GetGuid(reader.GetOrdinal("FolderID")),
                ParentId = reader.IsDBNull(reader.GetOrdinal("ParentID"))
                            ? null
                            : reader.GetGuid(reader.GetOrdinal("ParentID")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        public FolderRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {

        }

        private async Task<Result<IEnumerable<Folder>>> GetRootFoldersInternalAsync()
        {
            List<Folder> folders = new List<Folder>();

                using SqlCommand command = new SqlCommand("[Structure].[usp_GetRootFolders]");
                command.CommandType = CommandType.StoredProcedure;

                folders = await FetchListAsync(command, MapReaderToFolder);

                if (folders == null)
                {
                    return Result<IEnumerable<Folder>>.Failure("No root folders was found");
                }

            return Result<IEnumerable<Folder>>.Success(folders);
        }
        
        public async Task<Result<IEnumerable<Folder>>> GetRootFoldersAsync()
        {
            return await ExecuteSafeAsync(() => GetRootFoldersInternalAsync());
        }

        private async Task<Result<IEnumerable<Folder>>> GetAllFoldersInternalAsync()
        {
            List<Folder> folders = new List<Folder>();

                using SqlCommand command = new SqlCommand("[Structure].[usp_GetFolders]");
                command.CommandType = CommandType.StoredProcedure;

                folders = await FetchListAsync(command, MapReaderToFolder);

                if (folders == null)
                {
                    return Result<IEnumerable<Folder>>.Failure("No folders was found");
                }

            return Result<IEnumerable<Folder>>.Success(folders);
        }
        public async Task<Result<IEnumerable<Folder>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(GetAllFoldersInternalAsync);
        }

        private async Task<Result<Folder>> GetFolderByIdInternalAsync(Guid id)
        {
            Folder? folder = new Folder();

                using (SqlCommand command = new SqlCommand("[Structure].[usp_GetFolderById]"))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FolderID", id);

                    List<Folder> folders = await FetchListAsync<Folder>(command, MapReaderToFolder);
                    folder = folders.FirstOrDefault();

                    if (folder == null)
                    {
                        return Result<Folder>.Failure($"No folder was found with id {id}");
                    }
                }

            return Result<Folder>.Success(folder);
        }

        public async Task<Result<Folder>> GetByIdAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => GetFolderByIdInternalAsync(id));
        }

        private async Task<Result<IEnumerable<Folder>>> GetSubFoldersByParentIdInternalAsync(Guid parentId)
        {
            List<Folder> folders = new List<Folder>();

                using SqlCommand command = new SqlCommand("[Structure].[usp_GetFoldersByParentId]");
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ParentID", parentId);

                folders = await FetchListAsync<Folder>(command, MapReaderToFolder);

                if (folders == null)
                {
                    return Result<IEnumerable<Folder>>.Failure($"No sub folders for the parent with id {parentId}");
                }

            return Result<IEnumerable<Folder>>.Success(folders);
        }

        public async Task<Result<IEnumerable<Folder>>> GetSubfoldersByParentIdAsync(Guid parentId)
        {
            return await ExecuteSafeAsync(() => GetSubFoldersByParentIdInternalAsync(parentId));
        }

        private async Task<Result<Folder>> AddInternalAsync(Folder folder)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_AddFolder]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FolderID", folder.Id);
            command.Parameters.AddWithValue("@ParentID", (object?)folder.ParentId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Name", folder.Name);

            List<Folder> folders = await FetchListAsync<Folder>(command, MapReaderToFolder);
            Folder? newFolder = folders.FirstOrDefault();

            if (newFolder == null)
            {
                return Result<Folder>.Failure("Fail to add the folder.");
            }

            return Result<Folder>.Success(newFolder);
        }

        public async Task<Result<Folder>> AddAsync(Folder folder)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(folder));
        }

        private async Task<Result<Folder>> UpdateFolderInternalAsync(Folder folder)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_UpdateFolder]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FolderID", folder.Id);
            command.Parameters.AddWithValue("@ParentID", (object?)folder.ParentId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Name", folder.Name);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            await ExecuteCommandAsync(command);

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0)
            {
                return Result<Folder>.Failure($"Failed to update folder. ID {folder.Id} not found or no changes made.");
            }
            return Result<Folder>.Success(folder);
        }

        public async Task<Result<Folder>> UpdateAsync(Folder folder)
        {
            return await ExecuteSafeAsync(() => UpdateFolderInternalAsync(folder));
        }

        private async Task<Result<bool>> DeleteFolderByIdInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_DeleteFolder]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("FolderID", id);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            await ExecuteCommandAsync(command);

            int rowsAffected = (int) rowsAffectedParam.Value;
            if (rowsAffected == 0)
            {
                return Result<bool>.Failure($"Failed to delete folder. ID {id} not found or no changes made.");
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => DeleteFolderByIdInternalAsync((Guid)id));
        }

    }
}
