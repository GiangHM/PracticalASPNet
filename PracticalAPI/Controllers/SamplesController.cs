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
        readonly ILogger<SamplesController> _logger;
        readonly IConfiguration _configuration;
        public SamplesController(IWelcoming welcoming
            , ILogger<SamplesController> logger
            , IConfiguration configuration)
        {
            _welcoming = welcoming;
            _logger = logger;
            _configuration = configuration;
        }
    }
}
