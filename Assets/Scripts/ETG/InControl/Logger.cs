using System;

#nullable disable
namespace InControl
{
  public class Logger
  {
    public static event Action<LogMessage> OnLogMessage;

    public static void LogInfo(string text)
    {
      if (Logger.OnLogMessage == null)
        return;
      LogMessage logMessage = new LogMessage()
      {
        text = text,
        type = LogMessageType.Info
      };
      Logger.OnLogMessage(logMessage);
    }

    public static void LogWarning(string text)
    {
      if (Logger.OnLogMessage == null)
        return;
      LogMessage logMessage = new LogMessage()
      {
        text = text,
        type = LogMessageType.Warning
      };
      Logger.OnLogMessage(logMessage);
    }

    public static void LogError(string text)
    {
      if (Logger.OnLogMessage == null)
        return;
      LogMessage logMessage = new LogMessage()
      {
        text = text,
        type = LogMessageType.Error
      };
      Logger.OnLogMessage(logMessage);
    }
  }
}
