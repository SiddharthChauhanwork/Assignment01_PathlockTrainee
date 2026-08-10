using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Entities
{
    public class ApplicationClient : BaseEntity
    {
        public string ClientId { get; set; } = default!;
        public string SecretHash { get; set; } = default!;
        public string PrimaryDomain { get; set; } = default!;
        public string CompanyName { get; set; } = default!; // I think in db it looks great

        public bool IsActive { get; set; } = true;
    }
}
