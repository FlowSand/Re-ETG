using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BloodbulonController : BraveBehaviour
  {
    public float bloodbulubMoveSpeed = 2.5f;
    public float bloodbuburstMoveSpeed = 1.5f;
    public float bloodbulubGoopSize = 1.25f;
    public float bloodbuburstGoopSize = 2f;
    private GoopDoer m_goopDoer;
    private tk2dSpriteAnimator m_shadowAnimator;
    private BloodbulonController.State m_state;
    private bool m_isTransitioning;

    public void Start()
    {
      this.healthHaver.minimumHealth = 0.5f;
      GoopDoer[] components = this.GetComponents<GoopDoer>();
      for (int index = 0; index < components.Length; ++index)
      {
        if (components[index].updateTiming == GoopDoer.UpdateTiming.Always)
        {
          this.m_goopDoer = components[index];
          break;
        }
      }
      this.m_shadowAnimator = this.aiActor.ShadowObject.GetComponent<tk2dSpriteAnimator>();
    }

    public void Update()
    {
      if (!(bool) (Object) this.aiActor || !(bool) (Object) this.healthHaver || this.healthHaver.IsDead || this.m_isTransitioning)
        return;
      if (this.m_state == BloodbulonController.State.Small && (double) this.healthHaver.GetCurrentHealthPercentage() <= 0.66600000858306885)
      {
        this.StartCoroutine(this.GetBigger());
      }
      else
      {
        if (this.m_state != BloodbulonController.State.Medium || (double) this.healthHaver.GetCurrentHealthPercentage() >= 0.33300000429153442)
          return;
        this.StartCoroutine(this.GetBigger());
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

    [DebuggerHidden]
    private IEnumerator GetBigger()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BloodbulonController__GetBiggerc__Iterator0()
      {
        _this = this
      };
    }

    private enum State
    {
      Small,
      Medium,
      Large,
    }
  }

