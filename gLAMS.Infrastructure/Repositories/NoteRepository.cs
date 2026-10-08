using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Application.Interfaces.Logging;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories.Base;
using gLAMS.Shared.Responses;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography.Pkcs;
using gLAMS.Application.Interfaces.Repositories;

namespace gLAMS.Infrastructure.Repositories
{
    public class NoteRepository : BaseRepository, INoteRepository
    {

        public NoteRepository(ISqlConnectionFactory connectionFactory, IAppLogger logger)
            : base(connectionFactory, logger)
        {

        }

        public Note MapReaderToNote(SqlDataReader reader)
        {
            return new Note()
            {
                Id = reader.GetGuid(reader.GetOrdinal("NoteID")),
                CourseId = reader.GetGuid(reader.GetOrdinal("CourseID")),
                LessonId = reader.GetGuid(reader.GetOrdinal("LessonID")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                NoteText = reader.GetString(reader.GetOrdinal("NoteText")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
            };
        }

        private async Task<Result<IEnumerable<Note>>> GetNotesByLessonIdInternalAsync(Guid lessonId)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetNotesByLessonID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@LessonID", lessonId);

            List<Note> notes = await FetchListAsync(command, MapReaderToNote);

            return Result<IEnumerable<Note>>.Success(notes);
        }

        public async Task<Result<IEnumerable<Note>>> GetNotesByLessonIdAsync(Guid lessonId)
        {
            return await ExecuteSafeAsync(() => GetNotesByLessonIdInternalAsync(lessonId));
        }

        private async Task<Result<IEnumerable<Note>>> GetCourseLevelNotesInternalAsync(Guid courseId)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetNotesByCourseID]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CourseID", courseId);

            List<Note> notes = await FetchListAsync(command, MapReaderToNote);

            return Result<IEnumerable<Note>>.Success(notes);
        }

        public async Task<Result<IEnumerable<Note>>> GetCourseLevelNotesAsync(Guid courseId)
        {
            return await ExecuteSafeAsync(() => GetCourseLevelNotesInternalAsync(courseId));
        }

        private async Task<Result<Note>> GetByIdInternalAsync(Guid noteId)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetNoteById]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@NoteID", noteId);

            List<Note> notes = await FetchListAsync(command, MapReaderToNote);
            Note? note = notes.FirstOrDefault();

            return Result<Note>.Success(note);
        }

        public async Task<Result<Note>> GetByIdAsync(Guid noteId)
        {
            return await ExecuteSafeAsync(() => GetByIdInternalAsync(noteId));
        }

        private async Task<Result<IEnumerable<Note>>> GetAllInteralAsync()
        {
            SqlCommand command = new SqlCommand("[Content].[usp_GetNotes]");
            command.CommandType = CommandType.StoredProcedure;

            List<Note> notes = await FetchListAsync(command, MapReaderToNote);

            return Result<IEnumerable<Note>>.Success(notes);
        }

        public async Task<Result<IEnumerable<Note>>> GetAllAsync()
        {
            return await ExecuteSafeAsync(() => GetAllInteralAsync());
        }

        private async Task<Result<Note>> AddInternalAsync(Note note)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_AddNote]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@NoteID", note.Id);
            command.Parameters.AddWithValue("@CourseID", note.CourseId);
            command.Parameters.AddWithValue("@LessonID", (object?)note.LessonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Title", note.Title);
            command.Parameters.AddWithValue("@NoteText", note.NoteText);

            List<Note> notes = await FetchListAsync(command, MapReaderToNote);
            Note? newNote = notes.FirstOrDefault();

            return Result<Note>.Success(newNote);
        }

        public async Task<Result<Note>> AddAsync(Note note)
        {
            return await ExecuteSafeAsync(() => AddInternalAsync(note));
        }

        private async Task<Result<Note>> UpdateInternalAsync(Note note)
        {
            SqlCommand command = new SqlCommand("[Content].[usp_UpdateNote]");
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@NoteID", note.Id);
            command.Parameters.AddWithValue("@CourseID", note.CourseId);
            command.Parameters.AddWithValue("@LessonID", (object?)note.LessonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Title", note.Title);
            command.Parameters.AddWithValue("@NoteText", note.NoteText);

            SqlParameter rowsAffectedParam = new("@RowsAffected", SqlDbType.Int);
            rowsAffectedParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(rowsAffectedParam);

            List<Note> notes = await FetchListAsync(command, MapReaderToNote);
            Note? updatedNote = notes.FirstOrDefault();

            int rowsAffected = (int)rowsAffectedParam.Value;
            if (rowsAffected == 0 || updatedNote == null)
            {
                return Result<Note>.Success(null);
            }

            return Result<Note>.Success(updatedNote);
        }

        public async Task<Result<Note>> UpdateAsync(Note note)
        {
            return await ExecuteSafeAsync(() => UpdateInternalAsync(note));
        }

        private async Task<Result<bool>> DeleteInternalAsync(Guid id)
        {
            using SqlCommand command = new SqlCommand("[Content].[usp_DeleteNote]");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@NoteID", id);

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
