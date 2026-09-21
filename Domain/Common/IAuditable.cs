using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Domain.Common
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        int? CreatedBy { get; set; }
        DateTime? LastModifiedAt { get; set; }
        int? LastModifiedBy { get; set; }
    }
}
