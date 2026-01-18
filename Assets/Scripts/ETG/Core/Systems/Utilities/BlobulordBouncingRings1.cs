using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Blobulord/BouncingRings1")]
public class BlobulordBouncingRings1 : Script
  {
    private const int NumBlobs = 8;
    private const int NumBullets = 18;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BlobulordBouncingRings1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class BouncingRingBullet : Bullet
    {
      private Vector2 m_desiredOffset;

      public BouncingRingBullet(string name, Vector2 desiredOffset)
        : base(name)
      {
        this.m_desiredOffset = desiredOffset;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlobulordBouncingRings1.BouncingRingBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

