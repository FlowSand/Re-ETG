using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class RemoteMineController : BraveBehaviour
  {
    public ExplosionData explosionData;
    [CheckAnimation(null)]
    public string explodeAnimName;

    public void Detonate()
    {
      if (!string.IsNullOrEmpty(this.explodeAnimName))
      {
        this.StartCoroutine(this.DelayDetonateFrame());
      }
      else
      {
        Exploder.Explode(this.transform.position, this.explosionData, Vector2.zero);
        Object.Destroy((Object) this.gameObject);
      }
    }

    [DebuggerHidden]
    private IEnumerator DelayDetonateFrame()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new RemoteMineController__DelayDetonateFramec__Iterator0()
      {
        _this = this
      };
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

