// Decompiled with JetBrains decompiler
// Type: BlobulordSlam1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/Blobulord/Slam1")]
public class BlobulordSlam1 : Script
{
  private const int NumBullets = 32 /*0x20*/;
  private const int NumWaves = 4;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BlobulordSlam1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class SlamBullet : Bullet
  {
    private int m_spawnDelay;

    public SlamBullet(int spawnDelay)
      : base("slam")
    {
      this.m_spawnDelay = spawnDelay;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BlobulordSlam1.SlamBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
