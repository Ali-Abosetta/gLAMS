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
    [Route("api/Folders")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IFolderRepository _folderRepository;
        public FoldersController(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [HttpGet(Name = "GetAllFolders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FolderResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
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
                return NotFound("No records found.");
            }
            else
            {
                IEnumerable<FolderResponseDto> dtos =
                    result.Data.Select((folder) => new FolderResponseDto(folder));

                return Ok(dtos);
            }
        }

        [HttpGet("Root", Name = "GetRootFolders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FolderResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
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
                return NotFound("No records found.");
            }
            else
            {
                IEnumerable<FolderResponseDto> dtos =
                    result.Data.Select((folder) => new FolderResponseDto(folder));

                return Ok(dtos);
            }
        }

        [HttpGet("{id:guid}", Name = "GetFolderById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FolderResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
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
                return NotFound("Record not found.");
            }
            else
            {
                return Ok(new FolderResponseDto(result.Data));
            }
        }

        [HttpGet("{parentId:guid}/SubFolders", Name = "GetSubFoldersByParentId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FolderResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
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
                return NotFound("No records found.");
            }
            else
            {
                IEnumerable<FolderResponseDto> dtos =
                    result.Data.Select((folder) => new FolderResponseDto(folder));

                return Ok(dtos);
            }
        }

        [HttpPost(Name = "AddNewFolder")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FolderResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddFolder([FromBody] FolderCreateDto newFolderDto)
        {
            if (newFolderDto == null || newFolderDto.Id == Guid.Empty)
            {
                return BadRequest("Invalid payload.");
            }

            Folder newFolder = newFolderDto.ToEntity();
            Result<Folder> result = await _folderRepository.AddAsync(newFolder);

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
                FolderResponseDto responseDto = new FolderResponseDto(result.Data);

                return CreatedAtAction(
                    nameof(GetFolderById),
                    new { id = responseDto.Id },
                    responseDto);
            }
        }

        [HttpPut("{id:guid}", Name = "UpdateFolder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FolderResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> UpdateFolder([FromRoute] Guid id, [FromBody] FolderUpdateDto folderDto)
        {
            if (folderDto == null || id == Guid.Empty)
            {
                return BadRequest("Invalid payload.");
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
                return NotFound("Record not found.");
            }
            else
            {
                return Ok(new FolderResponseDto(result.Data));
            }
        }

        [HttpDelete("{id:guid}", Name = "DeleteFolder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
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
                return NotFound("ID not found or already deleted.");
            }
            else
            {
                return NoContent();
            }
        }
    }
}
