using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.Agent
{
    public class ChangeAgentStatusDto
    {
        [Required(ErrorMessage = "Debe indicar el estado del agente")]
        public required bool Status { get; set; }
    }
}
