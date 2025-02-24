using MediatR;
using SeniorBackend.Core.Entities;
using SeniorBackend.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.Features.Users.Queries
{
    public class GetUserByIdQuery: IRequest<User>
    {
        public string Id { get; set; }
    }
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
    {
        private readonly IUserRepository _context;

        public GetUserByIdQueryHandler(IUserRepository context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.GetByIdAsync(request.Id);
            return user;
        }
    }
}
