using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalAPI.Services;

namespace PracticalAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public partial class ExamplesController : ControllerBase
    {
        readonly IWelcoming _welcoming;
        public ExamplesController(IWelcoming welcoming)
        {
            _welcoming = welcoming;
        }
    }
}
