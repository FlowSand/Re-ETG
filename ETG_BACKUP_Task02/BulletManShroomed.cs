// Decompiled with JetBrains decompiler
// Type: BulletManShroomed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;

#nullable disable
public class BulletManShroomed : Script
{
  protected override IEnumerator Top()
  {
    this.Fire(new Brave.BulletScript.Direction(-20f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
    this.Fire(new Brave.BulletScript.Direction(20f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
    return (IEnumerator) null;
  }
}
