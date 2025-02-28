using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorBackend.Core.DTOs.User;
using SeniorBackend.Core.Entities;
using SeniorBackend.Core.Features.Users.Commands;
using SeniorBackend.Core.Features.Users.Commands.UpdateUserCommand;
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

        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> GetUserById(UpdateUserRequest updatereq,string id)
        { 
            if (updatereq.Id != id) { return BadRequest(); }

            UpdateUserCommand command=new UpdateUserCommand();
            command.Id = id;
            command.PhoneNum = updatereq.PhoneNum;
            command.Surname = updatereq.Surname;
            command.Name= updatereq.Name;

            

            return Ok( await Mediator.Send(command));
        }

    }
}
