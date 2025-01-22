using Kanban.Models;
using Kanban.Services;
using Microsoft.AspNetCore.Mvc;
using Kanban.Enums;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

namespace Kanban.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IService<User> userService) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Produces<List<User>>]
        [HttpGet(Name = "GetUser")]
        public async Task<IActionResult> GetUser()
        {
            List<User> users = await userService.GetAll();

            if (users.Count > 0)
            {
                return Ok(users);
            }

            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [HttpPost(Name = "PostUser")]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            User? newUser = await userService.Create(user);

            if (newUser != null)
            {
                return CreatedAtAction(nameof(PostUser),
                    new { id = newUser.Id }, newUser);
            }
            else
            {
                return BadRequest();
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [Consumes("application/json-patch+json")]
        [HttpPatch("{id}", Name = "PatchUser")]
        public async Task<IActionResult> PatchUser(int id, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            User? existingUser = await userService.GetById(requestUser.Id);

            if (existingUser != null)
            {
                User? updatedUser = await userService.Update(requestUser);
                if (updatedUser != null)
                {
                    return Ok(updatedUser);
                }
                return BadRequest();
            }

            User? createdUser = await userService.Create(requestUser);
            if (createdUser != null)
            {
                if (user != null)
                {
                    try
                    {
                        patchDoc.ApplyTo(user);
                    }
                    catch (JsonPatchException)
                    {
                        result = OperationResult.Error;
                    }
                }
            });

            switch (result)
            {
                case OperationResult.Success:
                    return NoContent();
                case OperationResult.Error:
                    return BadRequest();
                case OperationResult.NotFound:
                    return NotFound();
                default:
                    return BadRequest();
            }
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete(Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            DeleteResult result = await userService.Delete(id);

            switch (result)
            {
                case DeleteResult.Success:
                    return NoContent();
                case DeleteResult.Error:
                    return BadRequest();
                case DeleteResult.NotFound:
                    return NotFound();
                default:
                    return BadRequest();
            }
        }
    }
}
