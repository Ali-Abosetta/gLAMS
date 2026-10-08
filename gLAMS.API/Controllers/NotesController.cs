using gLAMS.Shared.Responses;
using gLAMS.Application.DTOs.Notes;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gLAMS.API.Controllers
{
    [Route("api/Notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        /// <summary>
        /// Retrieves all notes in the system.
        /// </summary>
        [HttpGet(Name = "GetAllNotes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NoteResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllNotes()
        {
            Result<IEnumerable<Note>> result = await _noteRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No notes were found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<NoteResponseDto> dtos = 
                    result.Data.Select((note) => new NoteResponseDto(note));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves a specific note by its unique ID.
        /// </summary>
        [HttpGet("{id:guid}", Name = "GetNoteById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetNoteById([FromRoute] Guid id)
        {
            Result<Note> result = await _noteRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(
                    detail: "The requested note was not found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                return Ok(new NoteResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Retrieves all notes associated with a specific lesson.
        /// </summary>
        [HttpGet("Lesson/{lessonId:guid}", Name = "GetNotesByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NoteResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetNotesByLessonId([FromRoute] Guid lessonId)
        {
            Result<IEnumerable<Note>> result = await _noteRepository.GetNotesByLessonIdAsync(lessonId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No notes were found for this lesson.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<NoteResponseDto> dtos =
                    result.Data.Select((note) => new NoteResponseDto(note));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves all course-level notes (not attached to a specific lesson).
        /// </summary>
        [HttpGet("Course/{courseId:guid}", Name = "GetNotesByCourseId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NoteResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetNotesByCourseId([FromRoute] Guid courseId)
        {
            Result<IEnumerable<Note>> result = await _noteRepository.GetCourseLevelNotesAsync(courseId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No course-level notes were found for this course.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<NoteResponseDto> dtos =
                    result.Data.Select((note) => new NoteResponseDto(note));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Creates a new note.
        /// </summary>
        [HttpPost(Name = "AddNewNote")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NoteResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddNote([FromBody] NoteCreateDto noteDto)
        {
            if (noteDto == null)
            {
                return ValidationProblem(detail: "The sent note payload cannot be null.", title: "Invalid Payload");
            }
            else if (noteDto.Id == Guid.Empty)
            {
                return ValidationProblem(detail: "The note ID cannot be empty.", title: "Invalid Identifier");
            }

            Note note = noteDto.ToEntity();
            Result<Note> result = await _noteRepository.AddAsync(note);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return StatusCode(500, "The database failed to insert the record.");
            }
            else
            {
                NoteResponseDto responseDto = new NoteResponseDto(result.Data);

                return CreatedAtAction(
                    nameof(GetNoteById),
                    new { id = responseDto.Id },
                    responseDto);
            }
        }

        /// <summary>
        /// Updates an existing note.
        /// </summary>
        [HttpPut("{id:guid}", Name = "UpdateNote")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoteResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] NoteUpdateDto noteDto)
        {
            if (noteDto == null)
            {
                return ValidationProblem(detail: "The sent note payload cannot be null.", title: "Invalid Payload");
            }
            else if (id == Guid.Empty)
            {
                return ValidationProblem(detail: "The note ID cannot be empty.", title: "Invalid Identifier");
            }

            Note note = noteDto.ToEntity();
            note.Id = id;

            Result<Note> result = await _noteRepository.UpdateAsync(note);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(detail: "The requested note was not found or has been deleted.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                return Ok(new NoteResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Deletes a note.
        /// </summary>
        [HttpDelete("{id:guid}", Name = "DeleteNote")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> DeleteNote([FromRoute] Guid id)
        {
            Result<bool> result = await _noteRepository.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == false)
            {
                return Problem(detail: "The note ID was not found or is already deleted.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                return NoContent();
            }
        }
    }
}
