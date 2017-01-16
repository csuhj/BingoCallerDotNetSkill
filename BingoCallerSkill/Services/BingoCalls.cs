using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BingoCallerSkill.Services
{
  public class BingoCalls
  {
    private string[] callsList;

    public int MaxNumber
    {
      get { return callsList.Length; }
    }

    public BingoCalls()
      : this("BingoCalls.txt", 90)
    {
    }

    public BingoCalls(int maxNumber)
      : this("BingoCalls.txt", maxNumber)
    {
    }

    public BingoCalls(string callsFilePath, int maxNumber)
    {
      callsList = new string[maxNumber];

      Dictionary<int, string> numberToCallMap = ParseCallsFile(callsFilePath);
      LoadCalls(numberToCallMap);
      PopulateMissingCalls();
    }

    public BingoCalls(string callsFilePath)
    {
      Dictionary<int, string> numberToCallMap = ParseCallsFile(callsFilePath);
      int maxNumber = numberToCallMap.Keys.Max();

      callsList = new string[maxNumber];
      LoadCalls(numberToCallMap);
      PopulateMissingCalls();
    }

    public string GetCall(int number)
    {
      if ((number > 0) && (number <= callsList.Length))
        return callsList[number - 1] + " " + number;

      throw new ArgumentOutOfRangeException(nameof(number),
          "There are only calls for numbers between 1 and " + callsList.Length + " (inclusive) - "+number+" is out of range.");
    }

    private void LoadCalls(Dictionary<int, string> numberToCallMap)
    {
      foreach (int number in numberToCallMap.Keys)
      {
        if ((number > 0) && (number <= callsList.Length))
          callsList[number - 1] = numberToCallMap[number];
      }
    }

    private static Dictionary<int, string> ParseCallsFile(string filePath)
    {
      Dictionary<int, string> numberToCallMap = new Dictionary<int, string>();

      using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        using (StreamReader sr = new StreamReader(fs))
        {
          string line;
          while ((line = sr.ReadLine()) != null)
          {
            string[] lineParts = line.Split('\t');
            if (lineParts.Length < 2)
              continue;

            int number;
            if (int.TryParse(lineParts[0], out number))
            {
              numberToCallMap[number] = lineParts[1];
            }
          }
        }
      }

      return numberToCallMap;
    }

    private void PopulateMissingCalls()
    {
      for (int i = 0; i < callsList.Length; i++)
      {
        if (string.IsNullOrEmpty(callsList[i]))
          callsList[i] = CreateDefaultCall(i + 1);
      }
    }

    private string CreateDefaultCall(int number)
    {
      if (number < 0)
        return null;

      if (number < 10)
        return "Number";

      if (number < 100)
        return GetDigitWord(number, 1) + " and " + GetDigitWord(number, 2);

      StringBuilder callBuilder = new StringBuilder();
      for (int i = 1; i <= GetNumberOfDigits(number); i++)
        callBuilder.Append(GetDigitWord(number, i)).Append(" ");

      return callBuilder.ToString();
    }

    private int GetDigit(int number, int index)
    {
      if (number < 0)
        throw new ArgumentOutOfRangeException(nameof(number),
            "The given number (" + number + ") must be greater than or equal to 0");

      string numberString = number.ToString();

      if (index < 1 || index > numberString.Length)
        throw new ArgumentOutOfRangeException(nameof(index),
            "The index of the digit in number (" + number + ") must be between 1 and "+ numberString.Length+" (inclusive)");

      char digitChar = numberString[index - 1];
      return digitChar - '0';
    }

    private int GetNumberOfDigits(int number)
    {
      return (int)Math.Floor(Math.Log10(number)) + 1;
    }

    private string GetDigitWord(int number, int index)
    {
      return GetNumberWord(GetDigit(number, index));
    }

    private string GetNumberWord(int number)
    {
      switch (number)
      {
        case 0:
          return "zero";

        case 1:
          return "one";

        case 2:
          return "two";

        case 3:
          return "three";

        case 4:
          return "four";

        case 5:
          return "five";

        case 6:
          return "six";

        case 7:
          return "seven";

        case 8:
          return "eight";

        case 9:
          return "nine";

        default:
          throw new ArgumentOutOfRangeException(nameof(number),
            "The given number (" + number + ") must be between 0 and 9 (inclusive)");
      }
    }
  }
}
