using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalAPI.Services;

namespace PracticalAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExamplesController : ControllerBase
    {
        readonly IWelcoming _welcoming;
        public ExamplesController(IWelcoming welcoming)
        {
            _welcoming = welcoming;
        }

        [HttpGet]
        [Route("Greeting")]
        public ActionResult<string> Greet1()
        {
            return Ok(_welcoming.Welcome("Victor", "Hope you're doing well"));
        }
        [HttpGet]
        [Route("Greeting2")]
        public ActionResult<string> Greet2()
        {
            return Ok(_welcoming.Welcome("Victor", "Hope you're doing well endpoint2"));
        }
    }
}
