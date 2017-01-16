using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingoCallerSkill.Models.Callers
{
  public class SessionDetail : SessionSummary
  {
    public int MaxNumber { get; set; }
    public int NumbersCalledCount { get; set; }
    public List<int> NumbersCalled { get; set; }
  }
}
