// Decompiled with JetBrains decompiler
// Type: GameStat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public class GameStat
{
  public string statName = string.Empty;
  public float statValue;

  public GameStat(string name, float val)
  {
    this.statName = name;
    this.statValue = val;
  }

  public void Modify(float change) => this.statValue += change;
}
