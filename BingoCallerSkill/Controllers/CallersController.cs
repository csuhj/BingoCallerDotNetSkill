using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BingoCallerSkill.Models.Callers;
using BingoCallerSkill.Services;

namespace BingoCallerSkill.Controllers
{
  [RoutePrefix("api/callers")]
  public class CallersController : ApiController
  {
    private ICallerService callerService;

    public CallersController(ICallerService callerService)
    {
      this.callerService = callerService;
    }

    // GET: api/callers
    [HttpGet]
    public IHttpActionResult Get()
    {
      return Ok(callerService.GetCallerIds().Select(id => new SessionSummary() {Id = id }));
    }

    // GET api/callers/5
    [HttpGet]
    [Route("{id}")]
    public IHttpActionResult Get(string id)
    {
      Caller caller = callerService.GetCaller(id);
      if (caller == null)
        return NotFound();

      List<int> numbersCalled = caller.NumbersCalled;

      return Ok(new SessionDetail()
      {
        Id = id,
        MaxNumber = caller.MaxNumber,
        NumbersCalledCount = numbersCalled.Count,
        NumbersCalled = numbersCalled
      });
    }

    // POST api/callers
    [HttpPost]
    public IHttpActionResult Post()
    {
      string id = Guid.NewGuid().ToString();
      var callsFilePath = HttpContext.Current.Server.MapPath("/App_Data/BingoCalls.txt");

      callerService.CreateCaller(id, callsFilePath);

      return Get(id);
    }

    // POST api/callers/5
    [HttpPost]
    [Route("{id}")]
    public IHttpActionResult Post(string id)
    {
      Caller caller = callerService.GetCaller(id);
      if (caller == null)
        return NotFound();

      string call = caller.GetNextNumberCall();

      return Ok(new Call()
      {
        Text = call
      });
    }

    // DELETE api/callers/5
    [HttpDelete]
    [Route("{id}")]
    public IHttpActionResult Delete(string id)
    {
      bool removed = callerService.RemoveCaller(id);
      return removed ? (IHttpActionResult)Ok() : NotFound();
    }
  }
}
