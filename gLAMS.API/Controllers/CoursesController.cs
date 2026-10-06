using gLAMS.Application.DTOs.Courses;
using gLAMS.Application.DTOs.Folders;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories;
using gLAMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace gLAMS.API.Controllers
{
    [Route("api/Courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        [HttpGet("Folder/{folderId:guid}", Name = "GetCoursesByFolderId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Course>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetCoursesByFolderId([FromRoute] Guid folderId)
        {
            Result<IEnumerable<Course>> result = await _courseRepository.GetCoursesByFolderIdAsync(folderId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return NotFound("Record not found.");
            }
            else
            {
                IEnumerable<CourseResponseDto> dtos =
                    result.Data.Select((course) => new CourseResponseDto(course));

                return Ok(dtos);
            }
        }

        [HttpGet("Search/{Name}", Name = "SearchCoursesByName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CourseResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> SearchCourseByName([FromRoute] string Name)
        {
            Result<IEnumerable<Course>> result = await _courseRepository.SearchCoursesByNameAsync(Name);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return NotFound("Record not found.");
            }
            else
            {
                IEnumerable<CourseResponseDto> dtos =
                    result.Data.Select((course) => new CourseResponseDto(course));

                return Ok(dtos);
            }
        }

        [HttpGet("{id:guid}", Name = "GetCourseById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetCourseById([FromRoute] Guid id)
        {
            Result<Course> result = await _courseRepository.GetByIdAsync(id);

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
                return Ok(new CourseResponseDto(result.Data));
            }

        }

        [HttpGet(Name = "GetAllCourses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CourseResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetAllCourses()
        {
            Result<IEnumerable<Course>> result = await _courseRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return NotFound("Record not found.");
            }
            else
            {
                IEnumerable<CourseResponseDto> dtos =
                    result.Data.Select((course) => new CourseResponseDto(course));
                return Ok(dtos);
            }
        }

        [HttpPost(Name = "AddNewCourse")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> AddCourse([FromBody] CourseCreateDto newCourseDto)
        {

            if (newCourseDto == null)
            {
                return BadRequest("The sent course is null");
            }
            else if (newCourseDto.Id == Guid.Empty)
            {
                return BadRequest("The course id is empty");
            }

            Course course = newCourseDto.ToEntity();
            Result<Course> result = await _courseRepository.AddAsync(course);

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
                CourseResponseDto responseDto = new CourseResponseDto(result.Data);

                return CreatedAtAction(
                    nameof(GetCourseById),
                    new { id = responseDto.Id },
                    responseDto);
            }

        }

        [HttpPut("{id:guid}", Name = "UpdateCourse")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]

        public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] CourseUpdateDto courseDto)
        {
            if (courseDto == null)
            {
                return BadRequest("Invalid payload: the sent course is null");
            }
            else if (id == Guid.Empty)
            {
                return BadRequest("Invalid payload: The course id is empty");
            }

            Course course = courseDto.ToEntity();
            course.Id = id;

            Result<Course> result = await _courseRepository.UpdateAsync(course);

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
                return Ok(new CourseResponseDto(result.Data));
            }
        }

        [HttpDelete("{id:guid}", Name = "DeleteCourse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
        {

            Result<bool> result = await _courseRepository.DeleteAsync(id);

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
