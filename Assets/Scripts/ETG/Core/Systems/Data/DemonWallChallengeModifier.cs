using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class DemonWallChallengeModifier : ChallengeModifier
  {
    [EnemyIdentifier]
    public string SniperGuyGuid;
    public float SniperCooldown = 2.4f;
    private AIActor m_sniper1;
    private AIActor m_sniper2;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DemonWallChallengeModifier__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void LateUpdate()
    {
      if ((bool) (Object) this.m_sniper1)
        this.m_sniper1.sprite.UpdateZDepth();
      if (!(bool) (Object) this.m_sniper2)
        return;
      this.m_sniper2.sprite.UpdateZDepth();
    }
  }

