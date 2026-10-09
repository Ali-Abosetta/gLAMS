using gLAMS.Shared.Responses;
using gLAMS.Application.DTOs.References;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gLAMS.API.Controllers
{
    [Route("api/References")]
    [ApiController]
    public class ReferencesController : ControllerBase
    {
        private readonly IReferenceRepository _referenceRepository;

        public ReferencesController(IReferenceRepository referenceRepository)
        {
            _referenceRepository = referenceRepository;
        }

        /// <summary>
        /// Retrieves all references in the system.
        /// </summary>
        [HttpGet(Name = "GetAllReferences")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReferenceResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllReferences()
        {
            Result<IEnumerable<Reference>> result = await _referenceRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No references were found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<ReferenceResponseDto> dtos = 
                    result.Data.Select((reference) => new ReferenceResponseDto(reference));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves a specific reference by its unique ID.
        /// </summary>
        [HttpGet("{id:guid}", Name = "GetReferenceById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferenceResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetReferenceById([FromRoute] Guid id)
        {
            Result<Reference> result = await _referenceRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(
                    detail: "The requested reference was not found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                return Ok(new ReferenceResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Retrieves all references associated with a specific lesson.
        /// </summary>
        [HttpGet("Lesson/{lessonId:guid}", Name = "GetReferencesByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReferenceResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetReferencesByLessonId([FromRoute] Guid lessonId)
        {
            Result<IEnumerable<Reference>> result = await _referenceRepository.GetReferencesByLessonIdAsync(lessonId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No references were found for this lesson.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<ReferenceResponseDto> dtos =
                    result.Data.Select((reference) => new ReferenceResponseDto(reference));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves all course-level references (not attached to a specific lesson).
        /// </summary>
        [HttpGet("Course/{courseId:guid}", Name = "GetReferencesByCourseId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReferenceResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetReferencesByCourseId([FromRoute] Guid courseId)
        {
            Result<IEnumerable<Reference>> result = await _referenceRepository.GetCourseLevelReferencesAsync(courseId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No course-level references were found for this course.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<ReferenceResponseDto> dtos =
                    result.Data.Select((reference) => new ReferenceResponseDto(reference));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Creates a new reference.
        /// </summary>
        [HttpPost(Name = "AddNewReference")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReferenceResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddReference([FromBody] ReferenceCreateDto referenceDto)
        {
            if (referenceDto == null)
            {
                return ValidationProblem(detail: "The sent reference payload cannot be null.", title: "Invalid Payload");
            }
            else if (referenceDto.Id == Guid.Empty)
            {
                return ValidationProblem(detail: "The reference ID cannot be empty.", title: "Invalid Identifier");
            }

            Reference reference = referenceDto.ToEntity();
            Result<Reference> result = await _referenceRepository.AddAsync(reference);

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
                ReferenceResponseDto responseDto = new ReferenceResponseDto(result.Data);

                return CreatedAtAction(
                    nameof(GetReferenceById),
                    new { id = responseDto.Id },
                    responseDto);
            }
        }

        /// <summary>
        /// Updates an existing reference.
        /// </summary>
        [HttpPut("{id:guid}", Name = "UpdateReference")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferenceResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UpdateReference([FromRoute] Guid id, [FromBody] ReferenceUpdateDto referenceDto)
        {
            if (referenceDto == null)
            {
                return ValidationProblem(detail: "The sent reference payload cannot be null.", title: "Invalid Payload");
            }
            else if (id == Guid.Empty)
            {
                return ValidationProblem(detail: "The reference ID cannot be empty.", title: "Invalid Identifier");
            }

            Reference reference = referenceDto.ToEntity();
            reference.Id = id;

            Result<Reference> result = await _referenceRepository.UpdateAsync(reference);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(detail: "The requested reference was not found or has been deleted.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                return Ok(new ReferenceResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Deletes a reference.
        /// </summary>
        [HttpDelete("{id:guid}", Name = "DeleteReference")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> DeleteReference([FromRoute] Guid id)
        {
            Result<bool> result = await _referenceRepository.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == false)
            {
                return Problem(
                    detail: "The reference ID was not found or is already deleted.",
                    statusCode: 404,
                    title: "Resource Not Found"
                    );
            }
            else
            {
                return NoContent();
            }
        }
    }
}
