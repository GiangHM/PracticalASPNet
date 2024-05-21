using Microsoft.AspNetCore.Mvc;

namespace PracticalAPI.Controllers
{
    public partial class ExamplesController
    {
        [HttpGet]
        [Route("Greeting")]
        public ActionResult<string> Greet1()
        {
            return Ok(_welcoming.WelcomeFormalWay("Victor", "Hope you're doing well"));
        }
        [HttpGet]
        [Route("Greeting2")]
        public ActionResult<string> Greet2()
        {
            return Ok(_welcoming.WelcomeInFormalWay("Victor"));
        }
    }
}
