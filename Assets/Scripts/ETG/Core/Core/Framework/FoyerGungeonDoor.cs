// Decompiled with JetBrains decompiler
// Type: FoyerGungeonDoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class FoyerGungeonDoor : BraveBehaviour
  {
    public bool LoadsCustomLevel;
    [ShowInInspectorIf("LoadsCustomLevel", false)]
    public string LevelNameToLoad = string.Empty;
    public bool LoadsCharacterSelect;
    public bool ReturnToFoyerFromTutorial;
    public bool southernDoor;
    private bool m_triggered;
    private bool m_coopTriggered;

    private void Start()
    {
      this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggered);
    }

    private void OnTriggered(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData)
    {
      if (this.LoadsCustomLevel && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        return;
      PlayerController component = specRigidbody.GetComponent<PlayerController>();
      if (this.ReturnToFoyerFromTutorial)
      {
        if (this.m_triggered || !((Object) component != (Object) null) || !((Object) component == (Object) GameManager.Instance.PrimaryPlayer))
          return;
        this.m_triggered = true;
        this.StartCoroutine(this.HandleLoading(component));
      }
      else if (!this.m_triggered && (Object) component != (Object) null && (Object) component == (Object) GameManager.Instance.PrimaryPlayer)
      {
        this.m_triggered = true;
        this.StartCoroutine(this.HandleLoading(component));
      }
      else
      {
        if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER || !((Object) component != (Object) null) || !((Object) component == (Object) GameManager.Instance.SecondaryPlayer) || !this.m_triggered || this.m_coopTriggered)
          return;
        this.m_coopTriggered = true;
        this.StartCoroutine(this.HandleCoopAnimation(component));
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleCoopAnimation(PlayerController p)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FoyerGungeonDoor__HandleCoopAnimationc__Iterator0()
      {
        p = p,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleLoading(PlayerController p)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FoyerGungeonDoor__HandleLoadingc__Iterator1()
      {
        p = p,
        _this = this
      };
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

