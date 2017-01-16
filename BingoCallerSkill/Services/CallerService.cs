using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingoCallerSkill.Services
{
  public class CallerService : ICallerService
  {
    private Dictionary<string, Caller> sessionIdToCallerMap;

    public CallerService()
    {
      sessionIdToCallerMap = new Dictionary<string, Caller>();
    }

    public IEnumerable<string> GetCallerIds()
    {
      return sessionIdToCallerMap.Keys;
    }

    public bool DoesCallerExist(string callerId)
    {
      return sessionIdToCallerMap.ContainsKey(callerId);
    }

    public Caller GetCaller(string callerId)
    {
      Caller caller;
      if (sessionIdToCallerMap.TryGetValue(callerId, out caller))
        return caller;

      return null;
    }

    public void CreateCaller(string callerId, string callsFilePath)
    {
      sessionIdToCallerMap.Add(callerId, new Caller(callsFilePath));
    }

    public bool RemoveCaller(string callerId)
    {
      return sessionIdToCallerMap.Remove(callerId);
    }
  }
}
