using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Application.Interfaces.Logging;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories.Base;
using gLAMS.Shared.Responses;
using Microsoft.Data.SqlClient;

namespace gLAMS.Infrastructure.Repositories
{
    public class CourseRepository : BaseRepository, ICourseRepository
    {
        public CourseRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {

        }
        private Course MapReaderToCourse(SqlDataReader reader)
        {
            return new Course
            {
                Id = reader.GetGuid(reader.GetOrdinal("CourseID")),
                FolderId = reader.GetGuid(reader.GetOrdinal("FolderID")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        private async Task<Result<IEnumerable<Course>>> GetCoursesByFolderIdInternalAsync(Guid folderId)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_GetCoursesByFolderId]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@FolderID", folderId);

            List<Course> courses = await FetchListAsync(command, MapReaderToCourse);

            return Result<IEnumerable<Course>>.Success(courses);
        }

        public async Task<Result<IEnumerable<Course>>> GetCoursesByFolderIdAsync(Guid folderId)
        {
            return await ExecuteSafeAsync(() => GetCoursesByFolderIdInternalAsync(folderId));
        }

        private async Task<Result<IEnumerable<Course>>> SearchCoursesByNameInternalAsync(string CourseName)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_SearchCoursesByName]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Name", CourseName);

            List<Course> courses = await FetchListAsync(command, MapReaderToCourse);

            return Result<IEnumerable<Course>>.Success(courses);
        }

        public async Task<Result<IEnumerable<Course>>> SearchCoursesByNameAsync(string CourseName)
        {
            return await ExecuteSafeAsync(() => SearchCoursesByNameInternalAsync(CourseName));
        }

        private async Task<Result<Course>> GetCourseByIdInternalAsync(Guid id)
        {


            using SqlCommand command = new SqlCommand("[Structure].[usp_GetCourseById]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", id);

            List<Course> Courses = await FetchListAsync<Course>(command, MapReaderToCourse);
            Course? course = Courses.FirstOrDefault();

            return Result<Course>.Success(course);
        }

        public async Task<Result<Course>> GetByIdAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => GetCourseByIdInternalAsync(id));
        }

        private async Task<Result<IEnumerable<Course>>> GetAllCoursesInternalAsync()
        {

            using SqlCommand command = new SqlCommand("[Structure].[usp_GetCourses]");
            command.CommandType = CommandType.StoredProcedure;

            List<Course> Courses = await FetchListAsync(command, MapReaderToCourse);

            return Result<IEnumerable<Course>>.Success(Courses);
        }

        public async Task<Result<IEnumerable<Course>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(GetAllCoursesInternalAsync);
        }

        private async Task<Result<Course>> AddInternalAsync(Course course)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_AddCourse]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@CourseID", course.Id);
            command.Parameters.AddWithValue("@FolderID", course.FolderId);
            command.Parameters.AddWithValue("@Name", course.Name);

            List<Course> Courses = await FetchListAsync<Course>(command, MapReaderToCourse);
            Course? newCourse = Courses.FirstOrDefault();

            return Result<Course>.Success(newCourse);
        }

        public async Task<Result<Course>> AddAsync(Course course)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(course));
        }

        private async Task<Result<Course>> UpdateCourseInternalAsync(Course course)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_UpdateCourse]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@CourseID", course.Id);
            command.Parameters.AddWithValue("@FolderID", course.FolderId);
            command.Parameters.AddWithValue("@Name", course.Name);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            List<Course> courses = await FetchListAsync(command, MapReaderToCourse);
            Course? updatedCourse = courses.FirstOrDefault();

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0 || updatedCourse == null)
            {
                return Result<Course>.Success(null);
            }
            return Result<Course>.Success(updatedCourse);
        }

        public async Task<Result<Course>> UpdateAsync(Course course)
        {
            return await ExecuteSafeAsync(() => UpdateCourseInternalAsync(course));
        }

        private async Task<Result<bool>> DeleteCourseByIdInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Structure].[usp_DeleteCourse]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", id);

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
            return await ExecuteSafeAsync(() => DeleteCourseByIdInternalAsync((Guid)id));
        }

    }
}
