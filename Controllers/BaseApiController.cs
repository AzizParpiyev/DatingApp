using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/users
    public class BaseApiController : ControllerBase
    {
    }
}
