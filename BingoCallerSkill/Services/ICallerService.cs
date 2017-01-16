using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingoCallerSkill.Services
{
  public interface ICallerService
  {
    IEnumerable<string> GetCallerIds();
    bool DoesCallerExist(string callerId);
    Caller GetCaller(string callerId);
    void CreateCaller(string callerId, string callsFilePath);
    bool RemoveCaller(string callerId);
  }
}
