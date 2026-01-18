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
      return (IEnumerator) new BlobulordSlam1__Topc__Iterator0()
      {
        _this = this
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
        return (IEnumerator) new BlobulordSlam1.SlamBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

