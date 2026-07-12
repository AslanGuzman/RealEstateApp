using Microsoft.AspNetCore.Mvc;

namespace RealEstateApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
