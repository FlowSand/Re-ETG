// Decompiled with JetBrains decompiler
// Type: dfMarkupAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public class dfMarkupAttribute
{
  public dfMarkupAttribute(string name, string value)
  {
    this.Name = name;
    this.Value = value;
  }

  public string Name { get; set; }

  public string Value { get; set; }

  public override string ToString() => $"{this.Name}='{this.Value}'";
}
