using System;
namespace Doitsu.Ecommerce.Core.DeliveryIntegration.GHTK.Models
{
    public abstract class GHTKBaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}