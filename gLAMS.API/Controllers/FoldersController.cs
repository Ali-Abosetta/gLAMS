using gLAMS.Application.DTOs.Folders;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Domain.Entities;
using gLAMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gLAMS.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing Folder hierarchies.
    /// </summary>
    [Route("api/Folders")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IFolderRepository _folderRepository;
        public FoldersController(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        /// <summary>
        /// Retrieves all active folders in the system.
        /// </summary>
        [HttpGet(Name = "GetAllFolders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FolderResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllFolders()
        {
            Result<IEnumerable<Folder>> result = await _folderRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(detail: "No records found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                IEnumerable<FolderResponseDto> dtos =
                    result.Data.Select((folder) => new FolderResponseDto(folder));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves all root folders (folders without a parent).
        /// </summary>
        [HttpGet("Root", Name = "GetRootFolders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FolderResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetRootFolders()
        {
            Result<IEnumerable<Folder>> result = await _folderRepository.GetRootFoldersAsync();

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(detail: "No records found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                IEnumerable<FolderResponseDto> dtos =
                    result.Data.Select((folder) => new FolderResponseDto(folder));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Retrieves a specific folder by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the folder.</param>
        [HttpGet("{id:guid}", Name = "GetFolderById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FolderResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetFolderById([FromRoute] Guid id)
        {
            Result<Folder> result = await _folderRepository.GetByIdAsync(id);

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
                return Ok(new FolderResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Retrieves all direct sub-folders for a given parent folder.
        /// </summary>
        /// <param name="parentId">The GUID of the parent folder.</param>
        [HttpGet("{parentId:guid}/SubFolders", Name = "GetSubFoldersByParentId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FolderResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetSubFoldersByParentId([FromRoute] Guid parentId)
        {
            Result<IEnumerable<Folder>> result = await _folderRepository.GetSubfoldersByParentIdAsync(parentId);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.ErrorMessage);
            }
            else if (result.Data == null || !result.Data.Any())
            {
                return Problem(detail: "No records found.", statusCode: 404, title: "Resource Not Found");
            }
            else
            {
                IEnumerable<FolderResponseDto> dtos =
                    result.Data.Select((folder) => new FolderResponseDto(folder));

                return Ok(dtos);
            }
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        /// <param name="newFolderDto">The folder creation payload.</param>
        [HttpPost(Name = "AddNewFolder")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FolderResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddFolder([FromBody] FolderCreateDto newFolderDto)
        {
            if (newFolderDto == null)
            {
                return ValidationProblem(detail: "Invalid payload: The sent folder is null.", title: "Invalid Payload");
            }
            else if (newFolderDto.Id == Guid.Empty)
            {
                return ValidationProblem(detail: "Invalid payload: cannot have an empty id.", title: "Invalid Identifier");
            }

            Folder newFolder = newFolderDto.ToEntity();
            Result<Folder> result = await _folderRepository.AddAsync(newFolder);

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
                FolderResponseDto responseDto = new FolderResponseDto(result.Data);

                return CreatedAtAction(
                    nameof(GetFolderById),
                    new { id = responseDto.Id },
                    responseDto);
            }
        }

        /// <summary>
        /// Updates an existing folder's details.
        /// </summary>
        /// <param name="id">The GUID of the folder to update.</param>
        /// <param name="folderDto">The updated folder data.</param>
        [HttpPut("{id:guid}", Name = "UpdateFolder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FolderResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UpdateFolder([FromRoute] Guid id, [FromBody] FolderUpdateDto folderDto)
        {
            if (folderDto == null)
            {
                return ValidationProblem(detail: "Invalid payload: the sent folder is null.", title: "Invalid Payload");
            }
            else if (id == Guid.Empty)
            {
                return ValidationProblem(detail: "Invalid payload: The folder id is empty.", title: "Invalid Identifier");
            }
            
            Folder folder = folderDto.ToEntity();
            folder.Id = id;

            Result<Folder> result = await _folderRepository.UpdateAsync(folder);

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
                return Ok(new FolderResponseDto(result.Data));
            }
        }

        /// <summary>
        /// Soft deletes a folder by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the folder to delete.</param>
        [HttpDelete("{id:guid}", Name = "DeleteFolder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> DeleteFolder([FromRoute] Guid id)
        {
            Result<bool> result = await _folderRepository.DeleteAsync(id);

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
