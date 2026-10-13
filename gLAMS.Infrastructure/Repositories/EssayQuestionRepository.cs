using Microsoft.Data.SqlClient;
using System.Data;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Application.Interfaces.Logging;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories.Base;
using gLAMS.Shared.Responses;

namespace gLAMS.Infrastructure.Repositories
{
    public class EssayQuestionRepository : BaseRepository, IEssayQuestionRepository
    {
        public EssayQuestionRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {

        }

        private EssayQuestion MapReaderToEssayQuestion(SqlDataReader reader)
        {
            return new EssayQuestion
            {
                Id = reader.GetGuid(reader.GetOrdinal("QuestionID")),
                CourseId = reader.GetGuid(reader.GetOrdinal("CourseID")),
                LessonId = reader.IsDBNull(reader.GetOrdinal("LessonID")) 
                    ? null 
                    : reader.GetGuid(reader.GetOrdinal("LessonID")),
                QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                ModelAnswer = reader.GetString(reader.GetOrdinal("ModelAnswer")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        private async Task<Result<IEnumerable<EssayQuestion>>> GetQuestionsByLessonIdInternalAsync(Guid lessonId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetEssayQuestionsByLessonID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", lessonId);

            List<EssayQuestion> essayQuestions = await FetchListAsync(command, MapReaderToEssayQuestion);

            return Result<IEnumerable<EssayQuestion>>.Success(essayQuestions);
        }

        public async Task<Result<IEnumerable<EssayQuestion>>> GetQuestionsByLessonIdAsync(Guid lessonId)
        {
            return await ExecuteSafeAsync(() => GetQuestionsByLessonIdInternalAsync(lessonId));
        }

        private async Task<Result<IEnumerable<EssayQuestion>>> GetQuestionsByCourseIdInternalAsync(Guid courseId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetEssayQuestionsByCourseID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", courseId);

            List<EssayQuestion> essayQuestions = await FetchListAsync(command, MapReaderToEssayQuestion);

            return Result<IEnumerable<EssayQuestion>>.Success(essayQuestions);
        }

        public async Task<Result<IEnumerable<EssayQuestion>>> GetQuestionsByCourseIdAsync(Guid courseId)
        {
            return await ExecuteSafeAsync(() => GetQuestionsByCourseIdInternalAsync(courseId));
        }

        private async Task<Result<EssayQuestion>> GetByIdInternalAsync(Guid questionId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetEssayQuestionById]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@QuestionID", questionId);

            List<EssayQuestion> essayQuestions = await FetchListAsync(command, MapReaderToEssayQuestion);
            EssayQuestion? essayQuestion = essayQuestions.FirstOrDefault();

            return Result<EssayQuestion>.Success(essayQuestion);
        }

        public async Task<Result<EssayQuestion>> GetByIdAsync(Guid questionId)
        {
            return await ExecuteSafeAsync(() => GetByIdInternalAsync(questionId));
        }

        private async Task<Result<IEnumerable<EssayQuestion>>> GetAllInternalAsync()
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetEssayQuestions]");
            command.CommandType = CommandType.StoredProcedure;

            List<EssayQuestion> essayQuestions = await FetchListAsync(command, MapReaderToEssayQuestion);

            return Result<IEnumerable<EssayQuestion>>.Success(essayQuestions);
        }

        public async Task<Result<IEnumerable<EssayQuestion>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(() => GetAllInternalAsync());
        }

        private async Task<Result<EssayQuestion>> AddInternalAsync(EssayQuestion essayQuestion)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_AddEssayQuestion]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@QuestionID", essayQuestion.Id);
            command.Parameters.AddWithValue("@CourseID", essayQuestion.CourseId);
            command.Parameters.AddWithValue("@LessonID", (object?)essayQuestion.LessonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuestionText", essayQuestion.QuestionText);
            command.Parameters.AddWithValue("@ModelAnswer", essayQuestion.ModelAnswer);

            List<EssayQuestion> essayQuestions = await FetchListAsync(command, MapReaderToEssayQuestion);
            EssayQuestion? newEssayQuestion = essayQuestions.FirstOrDefault();

            return Result<EssayQuestion>.Success(newEssayQuestion);
        }

        public async Task<Result<EssayQuestion>> AddAsync(EssayQuestion essayQuestion)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(essayQuestion));
        }

        private async Task<Result<EssayQuestion>> UpdateInternalAsync(EssayQuestion essayQuestion)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_UpdateEssayQuestion]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@QuestionID", essayQuestion.Id);
            command.Parameters.AddWithValue("@CourseID", essayQuestion.CourseId);
            command.Parameters.AddWithValue("@LessonID", (object?)essayQuestion.LessonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuestionText", essayQuestion.QuestionText);
            command.Parameters.AddWithValue("@ModelAnswer", essayQuestion.ModelAnswer);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            List<EssayQuestion> essayQuestions = await FetchListAsync(command, MapReaderToEssayQuestion);
            EssayQuestion? updatedQuestion = essayQuestions.FirstOrDefault();

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0 || updatedQuestion == null)
            {
                return Result<EssayQuestion>.Success(null);
            }

            return Result<EssayQuestion>.Success(updatedQuestion);
        }

        public async Task<Result<EssayQuestion>> UpdateAsync(EssayQuestion essayQuestion)
        {
            return await ExecuteSafeAsync(() => UpdateInternalAsync(essayQuestion));
        }

        private async Task<Result<bool>> DeleteInternalAsync(Guid questionId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_DeleteEssayQuestion]");
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

        public async Task<Result<bool>> DeleteAsync(Guid questionId)
        {
            return await ExecuteSafeAsync(() => DeleteInternalAsync(questionId));
        }
    }
}
