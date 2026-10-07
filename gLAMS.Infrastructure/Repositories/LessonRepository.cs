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
    public class LessonRepository : BaseRepository, ILessonRepository
    {

        public LessonRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {

        }
        private Lesson MapReaderToLesson(SqlDataReader reader)
        {
            return new Lesson()
            {
                Id = reader.GetGuid(reader.GetOrdinal("LessonID")),
                CourseId = reader.GetGuid(reader.GetOrdinal("CourseID")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                SortOrder = reader.GetInt32(reader.GetOrdinal("SortOrder")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        private async Task<Result<IEnumerable<Lesson>>> GetLessonsByCourseIdInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_GetLessonsByCourseId]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", id);

            List<Lesson> lessons = await FetchListAsync(command, MapReaderToLesson);

            return Result<IEnumerable<Lesson>>.Success(lessons);
        }

        public async Task<Result<IEnumerable<Lesson>>> GetLessonsByCourseIdAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => GetLessonsByCourseIdInternalAsync(id));
        }

        private async Task<Result<Lesson>> GetByIdInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_GetLessonById]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", id);

            List<Lesson> lessons = await FetchListAsync(command, MapReaderToLesson);
            Lesson? lesson = lessons.FirstOrDefault();

            return Result<Lesson>.Success(lesson);
        }

        public async Task<Result<Lesson>> GetByIdAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => GetByIdInternalAsync(id));
        }

        private async Task<Result<IEnumerable<Lesson>>> GetAllInternalAsync()
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_GetLessons]");
            command.CommandType = CommandType.StoredProcedure;

            List<Lesson> lessons = await FetchListAsync(command, MapReaderToLesson);

            return Result<IEnumerable<Lesson>>.Success(lessons);
        }

        public async Task<Result<IEnumerable<Lesson>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(() => GetAllInternalAsync());
        }

        private async Task<Result<Lesson>> AddInternalAsync(Lesson lesson)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_AddLesson]");

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", lesson.Id);
            command.Parameters.AddWithValue("@CourseID", lesson.CourseId);
            command.Parameters.AddWithValue("@Title", lesson.Title);

            List<Lesson> lessons = await FetchListAsync(command, MapReaderToLesson);
            Lesson? newLesson = lessons.FirstOrDefault();

            return Result<Lesson>.Success(newLesson);
        }

        public async Task<Result<Lesson>> AddAsync(Lesson lesson)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(lesson));
        }

        private async Task<Result<Lesson>> UpdateInternalAsync(Lesson lesson)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_UpdateLesson]");

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", lesson.Id);
            command.Parameters.AddWithValue("@CourseID", lesson.CourseId);
            command.Parameters.AddWithValue("@Title", lesson.Title);
            command.Parameters.AddWithValue("@SortOrder", lesson.SortOrder);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            List<Lesson> lessons = await FetchListAsync(command, MapReaderToLesson);
            Lesson? updatedLesson = lessons.FirstOrDefault();
            
            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0 || updatedLesson == null)
            {
                return Result<Lesson>.Success(null);
            }

            return Result<Lesson>.Success(updatedLesson);
        }

        public async Task<Result<Lesson>> UpdateAsync(Lesson lesson)
        {
            return await ExecuteSafeAsync(() =>  UpdateInternalAsync(lesson));
        }

        private async Task<Result<bool>> DeleteLessonByIdInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_DeleteLesson]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", id);

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
            return await ExecuteSafeAsync(() => DeleteLessonByIdInternalAsync(id));
        }
    }
}
