using MediatR;
using SeniorBackend.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.Features.Users.Commands.UpdateUserCommand
{
    public class UpdateUserCommand:IRequest<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNum { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, string>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
           
        }
        public async Task<string> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user=await _userRepository.GetByIdAsync(request.Id);
            if (user == null) { throw new Exception("user cannot find "); }

            user.FirstName = request.Name;
            user.LastName = request.Surname;
            user.PhoneNumber = request.PhoneNum;
            try
            {
                await _userRepository.UpdateAsync(user);
                return user.Id;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
           
        }
    }
}
