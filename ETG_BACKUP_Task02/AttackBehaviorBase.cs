// Decompiled with JetBrains decompiler
// Type: AttackBehaviorBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public abstract class AttackBehaviorBase : BehaviorBase
{
  public abstract bool IsReady();

  public abstract float GetMinReadyRange();

  public abstract float GetMaxRange();
}
