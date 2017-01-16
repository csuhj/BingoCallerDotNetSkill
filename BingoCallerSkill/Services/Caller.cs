using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingoCallerSkill.Services
{
  public class Caller
  {
    private BingoCalls bingoCalls;
    private bool[] numbers;

    public int MaxNumber
    {
      get { return bingoCalls.MaxNumber; }
    }

    public int NumbersCalledCount
    {
      get { return numbers.Count(n => true); }
    }

    public List<int> NumbersCalled
    {
      get
      {
        return numbers.Select((item, index) => new { HasBeenCalled = item, Index = index })
                  .Where(o => o.HasBeenCalled)
                  .Select(o => o.Index + 1)
                  .ToList();
      }
    }

    public Caller(string callsFilePath)
      : this(new List<int>(), new BingoCalls(callsFilePath))
    {
    }

    public Caller(IList<int> usedNumbers, BingoCalls bingoCalls)
    {
      this.bingoCalls = bingoCalls;
      this.numbers = new bool[bingoCalls.MaxNumber];
      foreach (int usedNumber in usedNumbers)
      {
        if ((usedNumber > 0) && (usedNumber <= bingoCalls.MaxNumber))
          numbers[usedNumber - 1] = true;
      }
    }

    public int? GetNextNumber()
    {
      List<int> availableNumbers = new List<int>();
      for (int i = 0; i < numbers.Length; i++)
      {
        if (!numbers[i])
          availableNumbers.Add(i);
      }

      if (availableNumbers.Count == 0)
        return null;

      int nextNumber = availableNumbers[new Random().Next(availableNumbers.Count - 1)];
      numbers[nextNumber] = true;
      return nextNumber + 1;
    }

    public string GetNextNumberCall()
    {
      int? nextNumber = GetNextNumber();
      if (!nextNumber.HasValue)
        return null;

      return bingoCalls.GetCall(nextNumber.Value);
    }

    public string GetCall(int number)
    {
      return bingoCalls.GetCall(number);
    }
  }
}
