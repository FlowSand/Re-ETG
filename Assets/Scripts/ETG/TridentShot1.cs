// Decompiled with JetBrains decompiler
// Type: TridentShot1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;

#nullable disable
public class TridentShot1 : Script
{
  protected override IEnumerator Top()
  {
    this.Fire(new Brave.BulletScript.Direction(-12f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(10f), (Bullet) null);
    this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(10f), (Bullet) null);
    this.Fire(new Brave.BulletScript.Direction(12f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(10f), (Bullet) null);
    return (IEnumerator) null;
  }
}
