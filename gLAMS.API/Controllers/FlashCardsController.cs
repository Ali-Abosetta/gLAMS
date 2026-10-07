using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gLAMS.Application.DTOs.FlashCards;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gLAMS.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing FlashCards.
    /// </summary>
    [Route("api/FlashCards")]
    [ApiController]
    public class FlashCardsController : ControllerBase
    {
        private readonly IFlashCardRepository _flashCardRepository;

        public FlashCardsController(IFlashCardRepository flashCardRepository)
        {
            _flashCardRepository = flashCardRepository;
        }

        /// <summary>
        /// Retrieves all flashcards associated with a specific course.
        /// </summary>
        /// <param name="courseId">The GUID of the parent course.</param>
        [HttpGet("Course/{courseId:guid}", Name = "GetFlashCardsByCourseId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FlashCardResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetFlashCardsByCourseId([FromRoute] Guid courseId)
        {
            Result<IEnumerable<FlashCard>> result = await _flashCardRepository.GetFlashCardsByCourseIdAsync(courseId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return NotFound(Problem(detail: "No flashcards found in this course.", title: "Resource Not Found"));
            }
            
            return Ok(result.Data.Select((flashCard) => new FlashCardResponseDto(flashCard)));
        }

        /// <summary>
        /// Retrieves all flashcards associated with a specific lesson.
        /// </summary>
        /// <param name="lessonId">The GUID of the parent lesson.</param>
        [HttpGet("Lesson/{lessonId:guid}", Name = "GetFlashCardsByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FlashCardResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetFlashCardsByLessonId([FromRoute] Guid lessonId)
        {
            Result<IEnumerable<FlashCard>> result = await _flashCardRepository.GetFlashCardsByLessonIdAsync(lessonId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return NotFound(Problem(detail: "No flashcards found in this lesson.", title: "Resource Not Found"));
            }
            
            return Ok(result.Data.Select((flashCard) => new FlashCardResponseDto(flashCard)));
        }

        /// <summary>
        /// Retrieves a specific flashcard by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the flashcard.</param>
        [HttpGet("{id:guid}", Name = "GetFlashCardById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FlashCardResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            Result<FlashCard> result = await _flashCardRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return NotFound(Problem(detail: "Flashcard not found.", title: "Resource Not Found"));
            }
            
            return Ok(new FlashCardResponseDto(result.Data));
        }

        /// <summary>
        /// Retrieves all active flashcards in the system.
        /// </summary>
        [HttpGet(Name = "GetAllFlashCards")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FlashCardResponseDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllFlashCards()
        {
            Result<IEnumerable<FlashCard>> result = await _flashCardRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return NotFound(Problem(detail: "No flashcards found.", title: "Resource Not Found"));
            }
            
            return Ok(result.Data.Select((flashCard) => new FlashCardResponseDto(flashCard)));
        }

        /// <summary>
        /// Creates a new flashcard.
        /// </summary>
        /// <param name="flashCardDto">The flashcard creation payload.</param>
        [HttpPost(Name = "AddNewFlashCard")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FlashCardResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddFlashCard([FromBody] FlashCardCreateDto flashCardDto)
        {
            if (flashCardDto == null)
            {
                return ValidationProblem(detail: "The FlashCard payload cannot be null.", title: "Invalid Payload");
            }
            else if (flashCardDto.Id == Guid.Empty)
            {
                return ValidationProblem(detail: "The FlashCard ID cannot be empty.", title: "Invalid Identifier");
            }

            FlashCard flashCard = flashCardDto.ToEntity();
            Result<FlashCard> result = await _flashCardRepository.AddAsync(flashCard);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return StatusCode(500, "The database failed to insert the record.");
            }

            FlashCardResponseDto responseDto = new FlashCardResponseDto(result.Data);

            return CreatedAtAction(
                nameof(GetById),
                new { id = responseDto.Id },
                responseDto);
        }

        /// <summary>
        /// Updates an existing flashcard's details.
        /// </summary>
        /// <param name="id">The GUID of the flashcard to update.</param>
        /// <param name="flashCardDto">The updated flashcard data.</param>
        [HttpPut("{id:guid}", Name = "UpdateFlashCard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FlashCardResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateFlashCard([FromRoute] Guid id, [FromBody] FlashCardUpdateDto flashCardDto)
        {
            if (flashCardDto == null)
            {
                return ValidationProblem(detail: "The FlashCard payload cannot be null.", title: "Invalid Payload");
            }
            else if (id == Guid.Empty)
            {
                return ValidationProblem(detail: "The FlashCard ID cannot be empty.", title: "Invalid Identifier");
            }

            FlashCard flashCard = flashCardDto.ToEntity();
            flashCard.Id = id;

            Result<FlashCard> result = await _flashCardRepository.UpdateAsync(flashCard);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return NotFound(Problem(detail: "Record not found or already deleted.", title: "Resource Not Found"));
            }
            
            return Ok(new FlashCardResponseDto(result.Data));
        }

        /// <summary>
        /// Soft deletes a flashcard by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the flashcard to delete.</param>
        [HttpDelete("{id:guid}", Name = "DeleteFlashCard")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteFlashCard([FromRoute] Guid id)
        {
            Result<bool> result = await _flashCardRepository.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == false)
            {
                return NotFound(Problem(detail: "ID not found or already deleted.", title: "Resource Not Found"));
            }
            
            return NoContent();
        }
    }
}
