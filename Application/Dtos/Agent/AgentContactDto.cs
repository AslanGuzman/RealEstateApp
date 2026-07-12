namespace RealEstateApp.Core.Application.Dtos.Agent
{
    public class AgentContactDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ProfileImage { get; set; }
    }
}
