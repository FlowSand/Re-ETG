// Decompiled with JetBrains decompiler
// Type: dfControlEventArgs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public class dfControlEventArgs
{
  internal dfControlEventArgs(dfControl Target) => this.Source = Target;

  public dfControl Source { get; internal set; }

  public bool Used { get; private set; }

  public void Use() => this.Used = true;
}
