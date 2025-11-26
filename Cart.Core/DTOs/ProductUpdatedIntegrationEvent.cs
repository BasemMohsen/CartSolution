using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Core.DTOs
{
    public record ProductUpdatedIntegrationEvent(int ProductId, string Name, decimal Price, string? ImageUrl);
}
