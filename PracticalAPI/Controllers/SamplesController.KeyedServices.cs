using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace PracticalAPI.Controllers
{
    public partial class SamplesController
    {
        //[EnableRateLimiting("CustomPolicy")]
        [HttpGet]
        [Route("Greeting")]
        public ActionResult<string> Greet1()
        {
            _logger.LogInformation("Use Keyed DI 1");
            var firstName = _configuration["DemoApp:Testing:FirstName"];
            var lastName = _configuration["DemoApp:Testing:LastName"];
            var fullName = $"{lastName} {firstName}";

            return Ok(_welcoming.WelcomeFormalWay(fullName, "Hope you're doing well"));
        }

        //[EnableRateLimiting("User")]
        [HttpGet]
        [Route("Greeting2")]
        public ActionResult<string> Greet2()
        {
            return Ok(_welcoming.WelcomeInFormalWay("Victor"));
        }
    }
}
