using System;
namespace Doitsu.Ecommerce.DeliveryIntegration.GHTK.Models
{
    public abstract class GHTKBaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}