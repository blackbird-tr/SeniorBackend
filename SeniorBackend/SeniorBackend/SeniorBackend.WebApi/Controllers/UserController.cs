using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorBackend.Core.Entities;
using SeniorBackend.Core.Features.Users.Commands;
using SeniorBackend.Core.Features.Users.Queries;

namespace SeniorBackend.WebApi.Controllers
{
    [ApiVersion("1.0")]
    public class UserController:BaseApiController
    {
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await Mediator.Send(new GetUserByIdQuery() { Id=id});
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = await Mediator.Send(command);
            return Ok(userId);
        }
    }
}
