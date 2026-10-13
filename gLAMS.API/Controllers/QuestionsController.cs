using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using gLAMS.Application.DTOs.Question.Essay;
using gLAMS.Application.DTOs.Question.MCQ;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;

namespace gLAMS.API.Controllers
{
    [Route("api/Questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMcqQuestionRepository _mcqRepository;
        private readonly IEssayQuestionRepository _essayRepository;

        public QuestionsController(
            IMcqQuestionRepository mcqRepository,
            IEssayQuestionRepository essayRepository)
        {
            _mcqRepository = mcqRepository;
            _essayRepository = essayRepository;
        }

        // ==========================================
        // MCQ ENDPOINTS
        // ==========================================

        /// <summary>
        /// Retrieves all MCQ questions in the system.
        /// </summary>
        [HttpGet("MCQ", Name = "GetAllMcqQuestions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<McqQuestionResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllMcqQuestions()
        {
            Result<IEnumerable<McqQuestion>> result = await _mcqRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No MCQ questions were found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<McqQuestionResponseDto> dtos = 
                    result.Data.Select((question) => new McqQuestionResponseDto(question));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves a specific MCQ question by its unique ID.
        /// </summary>
        [HttpGet("MCQ/{id:guid}", Name = "GetMcqQuestionById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(McqQuestionResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetMcqQuestionById([FromRoute] Guid id)
        {
            Result<McqQuestion> result = await _mcqRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(
                    detail: "The requested MCQ question was not found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                return Ok(new McqQuestionResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Retrieves all MCQ questions associated with a specific lesson.
        /// </summary>
        [HttpGet("MCQ/Lesson/{lessonId:guid}", Name = "GetMcqQuestionsByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<McqQuestionResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetMcqQuestionsByLessonId([FromRoute] Guid lessonId)
        {
            Result<IEnumerable<McqQuestion>> result = await _mcqRepository.GetQuestionsByLessonIdAsync(lessonId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No MCQ questions were found for this lesson.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<McqQuestionResponseDto> dtos = 
                    result.Data.Select((question) => new McqQuestionResponseDto(question));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves all MCQ questions associated with a specific course.
        /// </summary>
        [HttpGet("MCQ/Course/{courseId:guid}", Name = "GetMcqQuestionsByCourseId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<McqQuestionResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetMcqQuestionsByCourseId([FromRoute] Guid courseId)
        {
            Result<IEnumerable<McqQuestion>> result = await _mcqRepository.GetQuestionsByCourseIdAsync(courseId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No MCQ questions were found for this course.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<McqQuestionResponseDto> dtos = 
                    result.Data.Select((question) => new McqQuestionResponseDto(question));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Creates a new MCQ question with its choices.
        /// </summary>
        [HttpPost("MCQ", Name = "CreateMcqQuestion")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(McqQuestionResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> CreateMcqQuestion([FromBody] McqQuestionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            McqQuestion entity = dto.ToEntity();
            Result<McqQuestion> result = await _mcqRepository.AddAsync(entity);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else
            {
                McqQuestionResponseDto responseDto = new McqQuestionResponseDto(result.Data);

                return CreatedAtRoute(
                    "GetMcqQuestionById",
                    new { id = responseDto.Id },
                    responseDto
                );
            }
        }

        /// <summary>
        /// Updates an existing MCQ question and its choices.
        /// </summary>
        [HttpPut("MCQ/{id:guid}", Name = "UpdateMcqQuestion")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(McqQuestionResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UpdateMcqQuestion([FromRoute] Guid id, [FromBody] McqQuestionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            McqQuestion entity = dto.ToEntity();
            entity.Id = id;

            Result<McqQuestion> result = await _mcqRepository.UpdateAsync(entity);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(
                    detail: "The requested MCQ question was not found to update.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                return Ok(new McqQuestionResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Soft deletes an MCQ question and its choices.
        /// </summary>
        [HttpDelete("MCQ/{id:guid}", Name = "DeleteMcqQuestion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> DeleteMcqQuestion([FromRoute] Guid id)
        {
            Result<bool> result = await _mcqRepository.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (!result.Data)
            {
                return Problem(
                    detail: "The requested MCQ question was not found to delete.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                return NoContent();
            }
        }

        // ==========================================
        // ESSAY ENDPOINTS
        // ==========================================

        /// <summary>
        /// Retrieves all Essay questions in the system.
        /// </summary>
        [HttpGet("Essay", Name = "GetAllEssayQuestions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EssayQuestionResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllEssayQuestions()
        {
            Result<IEnumerable<EssayQuestion>> result = await _essayRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No Essay questions were found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<EssayQuestionResponseDto> dtos = 
                    result.Data.Select((question) => new EssayQuestionResponseDto(question));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves a specific Essay question by its unique ID.
        /// </summary>
        [HttpGet("Essay/{id:guid}", Name = "GetEssayQuestionById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EssayQuestionResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetEssayQuestionById([FromRoute] Guid id)
        {
            Result<EssayQuestion> result = await _essayRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(
                    detail: "The requested Essay question was not found.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                return Ok(new EssayQuestionResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Retrieves all Essay questions associated with a specific lesson.
        /// </summary>
        [HttpGet("Essay/Lesson/{lessonId:guid}", Name = "GetEssayQuestionsByLessonId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EssayQuestionResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetEssayQuestionsByLessonId([FromRoute] Guid lessonId)
        {
            Result<IEnumerable<EssayQuestion>> result = await _essayRepository.GetQuestionsByLessonIdAsync(lessonId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No Essay questions were found for this lesson.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<EssayQuestionResponseDto> dtos = 
                    result.Data.Select((question) => new EssayQuestionResponseDto(question));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves all Essay questions associated with a specific course.
        /// </summary>
        [HttpGet("Essay/Course/{courseId:guid}", Name = "GetEssayQuestionsByCourseId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EssayQuestionResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetEssayQuestionsByCourseId([FromRoute] Guid courseId)
        {
            Result<IEnumerable<EssayQuestion>> result = await _essayRepository.GetQuestionsByCourseIdAsync(courseId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(
                    detail: "No Essay questions were found for this course.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                IEnumerable<EssayQuestionResponseDto> dtos = 
                    result.Data.Select((question) => new EssayQuestionResponseDto(question));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Creates a new Essay question with its model answer.
        /// </summary>
        [HttpPost("Essay", Name = "CreateEssayQuestion")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EssayQuestionResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> CreateEssayQuestion([FromBody] EssayQuestionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EssayQuestion entity = dto.ToEntity();
            Result<EssayQuestion> result = await _essayRepository.AddAsync(entity);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else
            {
                EssayQuestionResponseDto responseDto = new EssayQuestionResponseDto(result.Data);

                return CreatedAtRoute(
                    "GetEssayQuestionById",
                    new { id = responseDto.Id },
                    responseDto
                );
            }
        }

        /// <summary>
        /// Updates an existing Essay question and its model answer.
        /// </summary>
        [HttpPut("Essay/{id:guid}", Name = "UpdateEssayQuestion")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EssayQuestionResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UpdateEssayQuestion([FromRoute] Guid id, [FromBody] EssayQuestionUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EssayQuestion entity = dto.ToEntity();
            entity.Id = id;

            Result<EssayQuestion> result = await _essayRepository.UpdateAsync(entity);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null)
            {
                return Problem(
                    detail: "The requested Essay question was not found to update.",
                    statusCode: 404,
                    title: "Resource Not Found"
                );
            }
            else
            {
                return Ok(new EssayQuestionResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Soft deletes an Essay question.
        /// </summary>
        [HttpDelete("Essay/{id:guid}", Name = "DeleteEssayQuestion")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> DeleteEssayQuestion([FromRoute] Guid id)
        {
            Result<bool> result = await _essayRepository.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (!result.Data)
            {
                return Problem(
                    detail: "The requested Essay question was not found to delete.",
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
