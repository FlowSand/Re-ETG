using FullSerializer;
using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal
{
  public static class fiLog
  {
    private static readonly List<string> _messages = new List<string>();

    public static void InsertAndClearMessagesTo(List<string> buffer)
    {
      lock ((object) typeof (fiLog))
      {
        buffer.AddRange((IEnumerable<string>) fiLog._messages);
        fiLog._messages.Clear();
      }
    }

    public static void Blank()
    {
      lock ((object) typeof (fiLog))
        fiLog._messages.Add(string.Empty);
    }

    private static string GetTag(object tag)
    {
      switch (tag)
      {
        case null:
          return string.Empty;
        case string _:
          return (string) tag;
        case Type _:
          return $"[{((Type) tag).CSharpName()}]: ";
        default:
          return $"[{tag.GetType().CSharpName()}]: ";
      }
    }

    public static void Log(object tag, string message)
    {
      lock ((object) typeof (fiLog))
        fiLog._messages.Add(fiLog.GetTag(tag) + message);
    }

    public static void Log(object tag, string format, params object[] args)
    {
      lock ((object) typeof (fiLog))
        fiLog._messages.Add(fiLog.GetTag(tag) + string.Format(format, args));
    }
  }
}
