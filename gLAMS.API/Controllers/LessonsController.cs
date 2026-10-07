using gLAMS.Application.DTOs.Lessons;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories;
using gLAMS.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace gLAMS.API.Controllers
{
    [Route("api/Lessons")]
    [ApiController]
    /// <summary>
    /// Controller responsible for managing Lessons.
    /// </summary>
    public class LessonsController : ControllerBase
    {
        private readonly ILessonRepository _lessonRepository;
        public LessonsController(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        /// <summary>
        /// Retrieves all lessons associated with a specific course.
        /// </summary>
        /// <param name="id">The GUID of the parent course.</param>
        [HttpGet("Course/{id:guid}", Name = "GetbyCourseId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LessonResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetLessonsByCourseId([FromRoute] Guid id)
        {
            Result<IEnumerable<Lesson>> result = await _lessonRepository.GetLessonsByCourseIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return NotFound("No lessons found in this course");
            }
            else
            {
                IEnumerable<LessonResponseDto> dtos =
                    result.Data.Select((lesson) => new LessonResponseDto(lesson));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves a specific lesson by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the lesson.</param>
        [HttpGet("{id:guid}", Name = "GetLessonById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LessonResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            Result<Lesson> result = await _lessonRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return NotFound(result.ErrorMessage);
            }
            else
            {
                return Ok(new LessonResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Retrieves all active lessons in the system.
        /// </summary>
        [HttpGet(Name = "GetAllLessons")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LessonResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetAllLessons()
        {
            Result<IEnumerable<Lesson>> result = await _lessonRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return NotFound(result.ErrorMessage);
            }
            else
            {
                IEnumerable<LessonResponseDto> dtos =
                    result.Data.Select((lesson) => new LessonResponseDto(lesson));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Creates a new lesson.
        /// </summary>
        /// <param name="lessonDto">The lesson creation payload.</param>
        [HttpPost(Name = "AddNewLesson")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LessonResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddLesson([FromBody] LessonCreateDto lessonDto)
        {
            if (lessonDto == null)
            {
                return BadRequest("Invalid payload: the sent lesson is null");
            }
            else if (lessonDto.Id == Guid.Empty)
            {
                return BadRequest("Invalid payload: The lesson id is empty");
            }

            Lesson lesson = lessonDto.ToEntity();
            Result<Lesson> result = await _lessonRepository.AddAsync(lesson);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return BadRequest("The database failed to insert the record.");
            }
            else
            {
                LessonResponseDto responseDto = new LessonResponseDto(result.Data);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = responseDto.Id },
                    responseDto);
            }
        }

        /// <summary>
        /// Updates an existing lesson's details, including its sort order.
        /// </summary>
        /// <param name="id">The GUID of the lesson to update.</param>
        /// <param name="lessonDto">The updated lesson data.</param>
        [HttpPut("{id:guid}",  Name = "UpdateLesson")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LessonResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> UpdateLesson([FromRoute] Guid id, [FromBody] LessonUpdateDto lessonDto)
        {
            if (lessonDto == null)
            {
                return BadRequest("Invalid payload: the sent lesson is null");
            }
            else if (id == Guid.Empty)
            {
                return BadRequest("Invalid payload: The lesson id is empty");
            }

            Lesson lesson = lessonDto.ToEntity();
            lesson.Id = id;

            Result<Lesson> result = await _lessonRepository.UpdateAsync(lesson);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return NotFound("Record not found.");
            }
            else
            {
                return Ok(new LessonResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Soft deletes a lesson by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the lesson to delete.</param>
        [HttpDelete("{id:guid}", Name = "DeleteLesson")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Result<bool>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> DeleteLesson([FromRoute] Guid id)
        {

            Result<bool> result = await _lessonRepository.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == false)
            {
                return NotFound("ID not found or already deleted.");
            }
            else
            {
                return NoContent();
            }
        }

    }

}
