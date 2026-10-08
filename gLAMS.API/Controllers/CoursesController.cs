using gLAMS.Application.DTOs.Courses;
using gLAMS.Application.DTOs.Folders;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Infrastructure.Repositories;
using gLAMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gLAMS.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing Courses.
    /// </summary>
    [Route("api/Courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        
        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        /// <summary>
        /// Retrieves all courses associated with a specific folder.
        /// </summary>
        /// <param name="folderId">The GUID of the parent folder.</param>
        [HttpGet("Folder/{folderId:guid}", Name = "GetCoursesByFolderId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CourseResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetCoursesByFolderId([FromRoute] Guid folderId)
        {
            Result<IEnumerable<Course>> result = await _courseRepository.GetCoursesByFolderIdAsync(folderId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(detail: "Record not found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                IEnumerable<CourseResponseDto> dtos =
                    result.Data.Select((course) => new CourseResponseDto(course));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Searches for active courses by name using relevance sorting.
        /// </summary>
        /// <param name="Name">The search keyword.</param>
        [HttpGet("Search/{Name}", Name = "SearchCoursesByName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CourseResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> SearchCourseByName([FromRoute] string Name)
        {
            Result<IEnumerable<Course>> result = await _courseRepository.SearchCoursesByNameAsync(Name);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(detail: "Record not found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                IEnumerable<CourseResponseDto> dtos =
                    result.Data.Select((course) => new CourseResponseDto(course));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves a specific course by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the course.</param>
        [HttpGet("{id:guid}", Name = "GetCourseById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetCourseById([FromRoute] Guid id)
        {
            Result<Course> result = await _courseRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(detail: "Record not found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                return Ok(new CourseResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Retrieves all active courses in the system.
        /// </summary>
        [HttpGet(Name = "GetAllCourses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CourseResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllCourses()
        {
            Result<IEnumerable<Course>> result = await _courseRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(detail: "Record not found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                IEnumerable<CourseResponseDto> dtos =
                    result.Data.Select((course) => new CourseResponseDto(course));
                
                return Ok(dtos);
            }
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="newCourseDto">The course creation payload.</param>
        [HttpPost(Name = "AddNewCourse")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CourseResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> AddCourse([FromBody] CourseCreateDto newCourseDto)
        {
            if (newCourseDto == null)
            {
                return ValidationProblem(detail: "The sent course payload cannot be null.", title: "Invalid Payload");
            }
            else if (newCourseDto.Id == Guid.Empty)
            {
                return ValidationProblem(detail: "The course id cannot be empty.", title: "Invalid Identifier");
            }

            Course course = newCourseDto.ToEntity();
            Result<Course> result = await _courseRepository.AddAsync(course);

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
                CourseResponseDto responseDto = new CourseResponseDto(result.Data);

                return CreatedAtAction(
                    nameof(GetCourseById),
                    new { id = responseDto.Id },
                    responseDto);
            }
        }

        /// <summary>
        /// Updates an existing course's details.
        /// </summary>
        /// <param name="id">The GUID of the course to update.</param>
        /// <param name="courseDto">The updated course data.</param>
        [HttpPut("{id:guid}", Name = "UpdateCourse")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] CourseUpdateDto courseDto)
        {
            if (courseDto == null)
            {
                return ValidationProblem(detail: "The sent course payload cannot be null.", title: "Invalid Payload");
            }
            else if (id == Guid.Empty)
            {
                return ValidationProblem(detail: "The course id cannot be empty.", title: "Invalid Identifier");
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
                return Problem(detail: "Record not found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                return Ok(new CourseResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Soft deletes a course by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the course to delete.</param>
        [HttpDelete("{id:guid}", Name = "DeleteCourse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
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
                return Problem(detail: "ID not found or already deleted.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                return NoContent();
            }
        }
    }
}
