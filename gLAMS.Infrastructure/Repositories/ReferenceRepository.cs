using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Application.Interfaces.Logging;
using gLAMS.Domain.Entities;
using gLAMS.Domain.Enums;
using gLAMS.Infrastructure.Repositories.Base;
using gLAMS.Shared.Responses;
using Microsoft.Data.SqlClient;
using gLAMS.Application.Interfaces.Repositories;

namespace gLAMS.Infrastructure.Repositories
{
    public class ReferenceRepository : BaseRepository, IReferenceRepository
    {
        public ReferenceRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {
        }

        public Reference MapReaderToReference(SqlDataReader reader)
        {
            return new Reference()
            {
                Id = reader.GetGuid(reader.GetOrdinal("ReferenceID")),
                CourseId = reader.GetGuid(reader.GetOrdinal("CourseID")),
                LessonId = reader.IsDBNull(reader.GetOrdinal("LessonID")) ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("LessonID")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Type = (ReferenceType)reader.GetByte(reader.GetOrdinal("ReferenceType")),
                ContentValue = reader.GetString(reader.GetOrdinal("ContentValue")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        private async Task<Result<IEnumerable<Reference>>> GetReferencesByLessonIdInternalAsync(Guid lessonId)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetReferencesByLessonID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", lessonId);

            List<Reference> references = await FetchListAsync(command, MapReaderToReference);
            return Result<IEnumerable<Reference>>.Success(references);
        }

        public async Task<Result<IEnumerable<Reference>>> GetReferencesByLessonIdAsync(Guid lessonId)
        {
            return await ExecuteSafeAsync(() => GetReferencesByLessonIdInternalAsync(lessonId));
        }

        private async Task<Result<IEnumerable<Reference>>> GetCourseLevelReferencesInternalAsync(Guid courseId)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetReferencesByCourseID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", courseId);

            List<Reference> references = await FetchListAsync(command, MapReaderToReference);
            return Result<IEnumerable<Reference>>.Success(references);
        }

        public async Task<Result<IEnumerable<Reference>>> GetCourseLevelReferencesAsync(Guid courseId)
        {
            return await ExecuteSafeAsync(() => GetCourseLevelReferencesInternalAsync(courseId));
        }

        private async Task<Result<Reference>> GetByIdInternalAsync(Guid referenceId)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetReferenceById]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ReferenceID", referenceId);

            List<Reference> references = await FetchListAsync(command, MapReaderToReference);
            Reference? reference = references.FirstOrDefault();
            return Result<Reference>.Success(reference);
        }

        public async Task<Result<Reference>> GetByIdAsync(Guid referenceId)
        {
            return await ExecuteSafeAsync(() => GetByIdInternalAsync(referenceId));
        }

        private async Task<Result<IEnumerable<Reference>>> GetAllInternalAsync()
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetReferences]");
            command.CommandType = CommandType.StoredProcedure;

            List<Reference> references = await FetchListAsync(command, MapReaderToReference);
            return Result<IEnumerable<Reference>>.Success(references);
        }

        public async Task<Result<IEnumerable<Reference>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(() => GetAllInternalAsync());
        }

        private async Task<Result<Reference>> AddInternalAsync(Reference reference)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_AddReference]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ReferenceID", reference.Id);
            command.Parameters.AddWithValue("@CourseID", reference.CourseId);
            command.Parameters.AddWithValue("@LessonID", (object?)reference.LessonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Title", reference.Title);
            command.Parameters.AddWithValue("@ReferenceType", (byte)reference.Type);
            command.Parameters.AddWithValue("@ContentValue", reference.ContentValue);

            List<Reference> references = await FetchListAsync(command, MapReaderToReference);
            Reference? newReference = references.FirstOrDefault();
            return Result<Reference>.Success(newReference);
        }

        public async Task<Result<Reference>> AddAsync(Reference reference)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(reference));
        }

        private async Task<Result<Reference>> UpdateInternalAsync(Reference reference)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_UpdateReference]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ReferenceID", reference.Id);
            command.Parameters.AddWithValue("@CourseID", reference.CourseId);
            command.Parameters.AddWithValue("@LessonID", (object?)reference.LessonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Title", reference.Title);
            command.Parameters.AddWithValue("@ReferenceType", (byte)reference.Type);
            command.Parameters.AddWithValue("@ContentValue", reference.ContentValue);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            List<Reference> references = await FetchListAsync(command, MapReaderToReference);
            Reference? updatedReference = references.FirstOrDefault();

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0 || updatedReference == null)
            {
                return Result<Reference>.Success(null);
            }

            return Result<Reference>.Success(updatedReference);
        }

        public async Task<Result<Reference>> UpdateAsync(Reference reference)
        {
            return await ExecuteSafeAsync(() => UpdateInternalAsync(reference));
        }

        private async Task<Result<bool>> DeleteInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_DeleteReference]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ReferenceID", id);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            await ExecuteCommandAsync(command);

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0)
            {
                return Result<bool>.Success(false);
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => DeleteInternalAsync(id));
        }
    }
}
