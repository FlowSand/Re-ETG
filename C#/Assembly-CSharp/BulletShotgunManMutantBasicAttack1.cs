// Decompiled with JetBrains decompiler
// Type: BulletShotgunManMutantBasicAttack1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable
[InspectorDropdownName("BulletShotgunMan/MutantBasicAttack1")]
public class BulletShotgunManMutantBasicAttack1 : Script
{
  protected override IEnumerator Top()
  {
    for (int index = -2; index <= 2; ++index)
      this.Fire(new Brave.BulletScript.Direction((float) (index * 6), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
    return (IEnumerator) null;
  }
}
