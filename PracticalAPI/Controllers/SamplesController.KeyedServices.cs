using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace PracticalAPI.Controllers
{
    public partial class SamplesController
    {
        [EnableRateLimiting("CustomPolicy")]
        [HttpGet]
        [Route("Greeting")]
        public ActionResult<string> Greet1()
        {
            return Ok(_welcoming.WelcomeFormalWay("Victor", "Hope you're doing well"));
        }

        [EnableRateLimiting("User")]
        [HttpGet]
        [Route("Greeting2")]
        public ActionResult<string> Greet2()
        {
            return Ok(_welcoming.WelcomeInFormalWay("Victor"));
        }
    }
}
