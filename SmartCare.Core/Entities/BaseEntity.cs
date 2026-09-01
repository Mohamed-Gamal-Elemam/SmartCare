using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SmartCare.Core.Entities
{
    internal class BaseEntity<TId>
    {
        public TId Id { get; set; } = default!;
    }
}
