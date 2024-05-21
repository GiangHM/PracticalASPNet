using Microsoft.AspNetCore.Mvc;

namespace PracticalAPI.Controllers
{
    public partial class SamplesController
    {
        /// <summary>
        /// Don't try ... catch in action method
        /// Global Exception Handler catches the ex and process
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ExceptionTest")]
        public ActionResult<string> ExceptionTest()
        {
            var names = new List<string> { "Victor", "Lina" };
            var family = "";
            for (int i = 0; i <= names.Count; i++)
            {
                family += names[i];
            }
            return Ok(_welcoming.WelcomeInFormalWay(family));
        }
        
    }
}
