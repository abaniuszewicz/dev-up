using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class UserIdNotFoundException : IdentityNotFoundException
    {
        public UserId UserId { get; }

        public UserIdNotFoundException(UserId userId)
            : base("User with this id does not exist.")
        {
            UserId = userId;
        }
    }
}
