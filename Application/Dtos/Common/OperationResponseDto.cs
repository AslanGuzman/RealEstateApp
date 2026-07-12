namespace RealEstateApp.Core.Application.Dtos.Common
{
    public class OperationResponseDto
    {
        public bool HasError { get; set; }
        public string? Error { get; set; }
    }
}
