namespace RealEstateApp.Core.Application.Dtos.Email
{
    public class EmailRequestDto
    {
        public required string To { get; set; }
        public List<string>? ToRange { get; set; }
        public required string Subject { get; set; }
        public required string HtmlBody { get; set; }
    }
}
