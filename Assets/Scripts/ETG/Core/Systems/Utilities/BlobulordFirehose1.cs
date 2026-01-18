using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/Blobulord/Firehose1")]
public class BlobulordFirehose1 : Script
  {
    private const float SpawnVariance = 0.5f;
    private const float WobbleRange = 35f;
    private const float BreakAwayChance = 0.2f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BlobulordFirehose1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class FirehoseBullet : Bullet
    {
      private float m_direction;

      public FirehoseBullet(float direction)
        : base("firehose")
      {
        this.m_direction = direction;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlobulordFirehose1.FirehoseBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

