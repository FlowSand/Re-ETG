// Decompiled with JetBrains decompiler
// Type: InControl.Logger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace InControl;

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
