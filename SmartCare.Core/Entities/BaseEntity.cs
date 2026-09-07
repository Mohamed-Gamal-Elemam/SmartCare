using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SmartCare.Core.Entities
{
    public class BaseEntity<TId>
    {
        public TId Id { get; set; } = default!;
    }
}
