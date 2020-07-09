using System;
namespace Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration.ResponseModels
{
    public abstract class GHTKBaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}