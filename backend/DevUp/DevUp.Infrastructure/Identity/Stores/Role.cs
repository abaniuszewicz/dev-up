using System;

namespace DevUp.Infrastructure.Identity.Stores
{
    internal class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}
