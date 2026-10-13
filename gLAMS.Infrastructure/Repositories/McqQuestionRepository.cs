using Microsoft.Data.SqlClient;
using System.Data;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Application.Interfaces.Logging;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories.Base;
using gLAMS.Shared.Responses;
using System.Net.Http.Headers;
using System.Text.Json;

namespace gLAMS.Infrastructure.Repositories
{
    public class McqQuestionRepository : BaseRepository, IMcqQuestionRepository
    {
        public McqQuestionRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {

        }

        private McqQuestion MapReaderToMcqQuestion(SqlDataReader reader)
        {
            int optionsOrdinal = reader.GetOrdinal("OptionsJson");

            string? json = reader.IsDBNull(optionsOrdinal) ? 
                null : reader.GetString(optionsOrdinal);

            return new McqQuestion
            {
                Id = reader.GetGuid(reader.GetOrdinal("QuestionID")),
                CourseId = reader.GetGuid(reader.GetOrdinal("CourseID")),
                LessonId = reader.IsDBNull(reader.GetOrdinal("LessonID"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("LessonID")),
                QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),

                Options = string.IsNullOrEmpty(json)
                    ? new List<McqOption>()
                    : JsonSerializer.Deserialize<List<McqOption>>(json) ?? new List<McqOption>()
            };
        }

        private async Task<Result<IEnumerable<McqQuestion>>> GetQuestionsByLessonIdInternalAsync(Guid lessonId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetMcqQuestionsByLessonID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", lessonId);

            List<McqQuestion> mcqQuestions = await FetchListAsync(command, MapReaderToMcqQuestion);

            return Result<IEnumerable<McqQuestion>>.Success(mcqQuestions);
        }
        
        public async Task<Result<IEnumerable<McqQuestion>>> GetQuestionsByLessonIdAsync(Guid lessonId)
        {
            return await ExecuteSafeAsync(() => GetQuestionsByLessonIdInternalAsync(lessonId));
        }

        private async Task<Result<IEnumerable<McqQuestion>>> GetQuestionsByCourseIdInternalAsync(Guid courseId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetMcqQuestionsByCourseID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", courseId);

            List<McqQuestion> mcqQuestions = await FetchListAsync(command, MapReaderToMcqQuestion);

            return Result<IEnumerable<McqQuestion>>.Success(mcqQuestions);
        }

        public async Task<Result<IEnumerable<McqQuestion>>> GetQuestionsByCourseIdAsync(Guid courseId)
        {
            return await ExecuteSafeAsync(() => GetQuestionsByCourseIdInternalAsync(courseId));
        }

        private async Task<Result<McqQuestion>> GetByIdInternalAsync(Guid questionId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetMcqQuestionById]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@QuestionID", questionId);

            List<McqQuestion> mcqQuestions = await FetchListAsync(command, MapReaderToMcqQuestion);
            McqQuestion? mcqQuestion = mcqQuestions.FirstOrDefault();

            return Result<McqQuestion>.Success(mcqQuestion);
        }

        public async Task<Result<McqQuestion>> GetByIdAsync(Guid questionId)
        {
            return await ExecuteSafeAsync(() => GetByIdInternalAsync(questionId));
        }

        private async Task<Result<IEnumerable<McqQuestion>>> GetAllInternalAsync()
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetMcqQuestions]");
            command.CommandType = CommandType.StoredProcedure;

            List<McqQuestion> mcqQuestions = await FetchListAsync(command, MapReaderToMcqQuestion);

            return Result<IEnumerable<McqQuestion>>.Success(mcqQuestions);
        }

        public async Task<Result<IEnumerable<McqQuestion>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(() => GetAllInternalAsync());
        }

        private async Task<Result<McqQuestion>> AddInternalAsync(McqQuestion mcqQuestion)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_AddMcqQuestion]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@QuestionID", mcqQuestion.Id);
            command.Parameters.AddWithValue("@CourseID", mcqQuestion.CourseId);
            command.Parameters.AddWithValue("@LessonID", mcqQuestion.LessonId);
            command.Parameters.AddWithValue("@QuestionText", mcqQuestion.QuestionText);

            string optionsJson = JsonSerializer.Serialize(mcqQuestion.Options);
            command.Parameters.AddWithValue("@OptionsJSON", optionsJson);

            List<McqQuestion> mcqQuestions = await FetchListAsync(command, MapReaderToMcqQuestion);
            McqQuestion? newMcqQuestion = mcqQuestions.FirstOrDefault();

            if (newMcqQuestion == null)
            {
                return Result<McqQuestion>.Success(null);
            }

            return Result<McqQuestion>.Success(newMcqQuestion);
        }

        public async Task<Result<McqQuestion>> AddAsync(McqQuestion mcqQuestion)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(mcqQuestion));
        }

        private async Task<Result<McqQuestion>> UpdateInternalAsync(McqQuestion mcqQuestion)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_UpdateMcqQuestion]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@QuestionID", mcqQuestion.Id);
            command.Parameters.AddWithValue("@CourseID", mcqQuestion.CourseId);
            command.Parameters.AddWithValue("@LessonID", (object?)mcqQuestion.LessonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuestionText", mcqQuestion.QuestionText);

            string optionsJson = JsonSerializer.Serialize(mcqQuestion.Options);
            command.Parameters.AddWithValue("@OptionsJSON", optionsJson);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            List<McqQuestion> mcqQuestions = await FetchListAsync(command, MapReaderToMcqQuestion);
            McqQuestion? updatedQuestion = mcqQuestions.FirstOrDefault();

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0 || updatedQuestion == null)
            {
                return Result<McqQuestion>.Success(null);
            }

            return Result<McqQuestion>.Success(updatedQuestion);
        }

        public async Task<Result<McqQuestion>> UpdateAsync(McqQuestion mcqQuestion)
        {
            return await ExecuteSafeAsync(() => UpdateInternalAsync(mcqQuestion));
        }

        private async Task<Result<bool>> DeleteInternalAsync(Guid questionId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_DeleteMcqQuestion]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@QuestionID", questionId);
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

        public async Task<Result<bool>> DeleteAsync(Guid quesiotnId)
        {
            return await ExecuteSafeAsync(() => DeleteInternalAsync(quesiotnId));
        }

    }
}
