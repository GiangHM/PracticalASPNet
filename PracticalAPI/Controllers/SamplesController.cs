using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalAPI.Services;

namespace PracticalAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public partial class SamplesController : ControllerBase
    {
        readonly IWelcoming _welcoming;
        ILogger<SamplesController> _logger;
        public SamplesController(IWelcoming welcoming
            , ILogger<SamplesController> logger)
        {
            _welcoming = welcoming;
            _logger = logger;
        }
    }
}
