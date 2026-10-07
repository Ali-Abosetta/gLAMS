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
    public class FlashCardRepository : BaseRepository, IFlashCardRepository
    {
        public FlashCardRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {

        }

        private FlashCard MapReaderToFlashCard(SqlDataReader reader)
        {
            return new FlashCard
            {
                Id = reader.GetGuid(reader.GetOrdinal("FlashCardID")),
                CourseId = reader.GetGuid(reader.GetOrdinal("CourseID")),
                LessonId = reader.IsDBNull(reader.GetOrdinal("LessonID"))
                    ? null
                    : reader.GetGuid(reader.GetOrdinal("LessonID")),
                FrontContent = reader.GetString(reader.GetOrdinal("FrontContent")),
                BackContent = reader.GetString(reader.GetOrdinal("BackContent")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        private async Task<Result<IEnumerable<FlashCard>>> GetFlashCardsByLessonIdInternalAsync(Guid lessonId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetFlashCardsByLessonId]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", lessonId);

            List<FlashCard> flashCards = await FetchListAsync(command, MapReaderToFlashCard);

            return Result<IEnumerable<FlashCard>>.Success(flashCards);
        }

        public async Task<Result<IEnumerable<FlashCard>>> GetFlashCardsByLessonIdAsync(Guid lessonId) 
        {
            return await ExecuteSafeAsync(() => GetFlashCardsByLessonIdInternalAsync(lessonId));
        }

        private async Task<Result<IEnumerable<FlashCard>>> GetFlashCardsByCourseIdInternalAsync(Guid courseId)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetFlashCardsByCourseId]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", courseId);

            List<FlashCard> flashCards = await FetchListAsync(command, MapReaderToFlashCard);

            return Result<IEnumerable<FlashCard>>.Success(flashCards);
        }
        public async Task<Result<IEnumerable<FlashCard>>> GetFlashCardsByCourseIdAsync(Guid courseId)
        {
            return await ExecuteSafeAsync(() => GetFlashCardsByCourseIdInternalAsync(courseId));
        }

        private async Task<Result<FlashCard>> GetByIdInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetFlashCardById]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@FlashCardID", id);

            List<FlashCard> flashCards = await FetchListAsync(command, MapReaderToFlashCard);
            FlashCard? flashCard = flashCards.FirstOrDefault();

            return Result<FlashCard>.Success(flashCard);
        }

        public async Task<Result<FlashCard>> GetByIdAsync(Guid id)
        {
            return await ExecuteSafeAsync(() => GetByIdInternalAsync(id));
        }

        private async Task<Result<IEnumerable<FlashCard>>> GetAllInternalAsync()
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_GetFlashCards]");
            command.CommandType = CommandType.StoredProcedure;

            List<FlashCard> flashCards = await FetchListAsync(command, MapReaderToFlashCard);

            return Result<IEnumerable<FlashCard>>.Success(flashCards);
        }

        public async Task<Result<IEnumerable<FlashCard>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(() => GetAllInternalAsync());
        }

        private async Task<Result<FlashCard>> AddInternalAsync(FlashCard flashCard)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_AddFlashCard]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FlashCardID", flashCard.Id);
            command.Parameters.AddWithValue("@CourseID", flashCard.CourseId);
            command.Parameters.AddWithValue("@LessonID", flashCard.LessonId);
            command.Parameters.AddWithValue("@FrontContent", flashCard.FrontContent);
            command.Parameters.AddWithValue("@BackContent", flashCard.BackContent);

            List<FlashCard> flashCards = await FetchListAsync(command, MapReaderToFlashCard);
            FlashCard? newFlashCard = flashCards.FirstOrDefault();

            return Result<FlashCard>.Success(newFlashCard);
        }

        public async Task<Result<FlashCard>> AddAsync(FlashCard flashCard)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(flashCard));
        }

        private async Task<Result<FlashCard>> UpdateInternalAsync(FlashCard flashCard)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_UpdateFlashCard]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FlashCardID", flashCard.Id);
            command.Parameters.AddWithValue("@CourseID", flashCard.CourseId);
            command.Parameters.AddWithValue("@LessonID", flashCard.LessonId);
            command.Parameters.AddWithValue("@FrontContent", flashCard.FrontContent);
            command.Parameters.AddWithValue("@BackContent", flashCard.BackContent);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            List<FlashCard> flashCards = await FetchListAsync(command, MapReaderToFlashCard);
            FlashCard? updatedFlashCard = flashCards.FirstOrDefault();

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0 || updatedFlashCard == null)
            {
                return Result<FlashCard>.Success(null);
            }
            return Result<FlashCard>.Success(updatedFlashCard);
        }

        public async Task<Result<FlashCard>> UpdateAsync(FlashCard flashCard)
        {
            return await ExecuteSafeAsync(() => UpdateInternalAsync(flashCard));
        }

        private async Task<Result<bool>> DeleteFlashCardByIdInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_DeleteFlashCard]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@FlashCardID", id);

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
            return await ExecuteSafeAsync(() => DeleteFlashCardByIdInternalAsync(id));
        }
    }
}
