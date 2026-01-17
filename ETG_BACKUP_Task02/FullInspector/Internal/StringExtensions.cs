// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.StringExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace FullInspector.Internal;

internal static class StringExtensions
{
  public static string F(this string format, params object[] args) => string.Format(format, args);
}
