using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using AlexaSkillsKit.Slu;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using BingoCallerSkill.Services;
using Newtonsoft.Json;

namespace BingoCallerSkill.Speechlets
{
  public class BingoSpeechlet : SpeechletAsync
  {
    private const string NumberKey = "Number";
    private const string CallForNumberIntentKey = "CallForNumberIntent";
    private const string StartGameIntentKey = "StartGameIntent";
    private const string NextNumberIntentKey = "NextNumberIntent";
    private const string QuitIntentKey = "QuitIntent";

    private ICallerService callerService;

    public BingoSpeechlet(ICallerService callerService)
    {
      this.callerService = callerService;
    }

    public override Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session)
    {
      Caller caller = EnsureCallerIsCreated(session.SessionId);

      return Task.FromResult(GetWelcomeResponse(caller));
    }

    public override Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session)
    {
      // Get intent from the request object.
      var intent = intentRequest.Intent;
      var intentName = intent?.Name;

      // Note: If the session is started with an intent, no welcome message will be rendered;
      // rather, the intent specific response will be returned.
      if (CallForNumberIntentKey.Equals(intentName))
      {
        Caller caller = EnsureCallerIsCreated(session.SessionId);

        return Task.FromResult(GetCallForNumberResponse(caller, intent));
      }
      else if (StartGameIntentKey.Equals(intentName))
      {
        Caller caller = EnsureCallerIsCreated(session.SessionId);

        return Task.FromResult(GetStartGameNumberResponse(caller));
      }
      else if (NextNumberIntentKey.Equals(intentName))
      {
        Caller caller = EnsureCallerIsCreated(session.SessionId);

        return Task.FromResult(GetNextNumberResponse(caller));
      }
      else if (QuitIntentKey.Equals(intentName))
      {
        callerService.RemoveCaller(session.SessionId);

        return Task.FromResult(GetQuitResponse());
      }

      throw new SpeechletException("Invalid Intent");
    }

    public override Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session)
    {
      return Task.FromResult(0);
    }

    public override Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session)
    {
      return Task.FromResult(0);
    }

    private SpeechletResponse GetWelcomeResponse(Caller caller)
    {
      var output = "Start a game or ask for a call between 1 and "+ caller.MaxNumber;
      return BuildSpeechletResponse("Welcome to Bingo Caller", output, false);
    }

    private SpeechletResponse GetCallForNumberResponse(Caller caller, Intent intent)
    {
      // Retrieve number from the intent slot
      var numberSlot = intent.Slots[NumberKey];

      // Create response
      string title;
      string output;
      int number;
      if (numberSlot != null && int.TryParse(numberSlot.Value, out number))
      {
        title = number.ToString();
        output = caller.GetCall(number);
      }
      else
      {
        title = "Bingo call";
        // Render an error since we don't know what the number was
        output = "I didn't catch that, please try again.";
      }

      // Here we are setting shouldEndSession to false to not end the session and
      // prompt the user for input
      return BuildSpeechletResponse(title, output, false);
    }

    private SpeechletResponse GetStartGameNumberResponse(Caller caller)
    {
      caller.Reset();

      string output = "Lets start the game. "+caller.GetNextNumberCall();
      return BuildSpeechletResponse("Start game", output, false);
    }
    private SpeechletResponse GetNextNumberResponse(Caller caller)
    {
      string output = caller.GetNextNumberCall();
      return BuildSpeechletResponse("Start game", output, false);
    }

    private SpeechletResponse GetQuitResponse()
    {
      var output = "Thanks for playing";
      return BuildSpeechletResponse("Finish game", output, true);
    }

    private Caller EnsureCallerIsCreated(string sessionId)
    {
      var callsFilePath = HttpContext.Current.Server.MapPath("/App_Data/BingoCalls.txt");

      if (!callerService.DoesCallerExist(sessionId))
        callerService.CreateCaller(sessionId, callsFilePath);

      return callerService.GetCaller(sessionId);
    }

    /// <summary>
    /// Creates and returns the visual and spoken response with shouldEndSession flag
    /// </summary>
    /// <param name="title">Title for the companion application home card</param>
    /// <param name="output">Output content for speech and companion application home card</param>
    /// <param name="shouldEndSession">Should the session be closed</param>
    /// <returns>SpeechletResponse spoken and visual response for the given input</returns>
    private SpeechletResponse BuildSpeechletResponse(string title, string output, bool shouldEndSession)
    {
      // Create the Simple card content
      var card = new SimpleCard
      {
        Title = title,
        Content = output
      };

      // Create the plain text output
      var speech = new PlainTextOutputSpeech { Text = output };

      // Create the speechlet response
      var response = new SpeechletResponse
      {
        ShouldEndSession = shouldEndSession,
        OutputSpeech = speech,
        Card = card
      };
      return response;
    }
  }
}
