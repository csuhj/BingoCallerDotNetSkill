using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BingoCallerSkill.Services;
using BingoCallerSkill.Speechlets;

namespace BingoCallerSkill.Controllers
{
  public class SpeechletController : ApiController
  {
    private ICallerService callerService;

    public SpeechletController(ICallerService callerService)
    {
      this.callerService = callerService;
    }

    [HttpPost]
    [Route("api/bingospeechlet")]
    public async Task<HttpResponseMessage> Menu()
    {
      var speechlet = new BingoSpeechlet(callerService);
      return await speechlet.GetResponseAsync(Request);
    }
  }
}
