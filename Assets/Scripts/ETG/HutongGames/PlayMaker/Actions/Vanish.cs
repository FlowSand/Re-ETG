// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Vanish
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Removes the NPC with flair.")]
  [ActionCategory(".NPCs")]
  public class Vanish : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Seconds to wait before vanishing (not including the vanish animation).")]
    public FsmFloat delay;
    [HutongGames.PlayMaker.Tooltip("Animation to play before vanishing.")]
    public FsmString vanishAnim;
    [HutongGames.PlayMaker.Tooltip("Add GameObjects here to leave behind after vanishing.")]
    public FsmGameObject[] itemsToLeaveBehind;
    private float m_vanishTimer;

    public override void Reset()
    {
      this.delay = (FsmFloat) 0.0f;
      this.vanishAnim = (FsmString) string.Empty;
      this.itemsToLeaveBehind = new FsmGameObject[0];
    }

    public override string ErrorCheck()
    {
      string str = string.Empty;
      tk2dSpriteAnimator component1 = this.Owner.GetComponent<tk2dSpriteAnimator>();
      AIAnimator component2 = this.Owner.GetComponent<AIAnimator>();
      if (!(bool) (Object) component1 && !(bool) (Object) component2)
        return "Requires a 2D Toolkit animator or an AI Animator.\n";
      if ((bool) (Object) component2)
      {
        if (!component2.HasDirectionalAnimation(this.vanishAnim.Value))
          str = $"{str}Unknown animation {this.vanishAnim.Value}.\n";
      }
      else if ((bool) (Object) component1 && component1.GetClipByName(this.vanishAnim.Value) == null)
        str = $"{str}Unknown animation {this.vanishAnim.Value}.\n";
      return str;
    }

    public override void OnEnter()
    {
      if ((double) this.delay.Value <= 0.0)
      {
        this.DoVanish();
        this.Finish();
      }
      else
        this.m_vanishTimer = this.delay.Value;
    }

    public override void OnUpdate()
    {
      this.m_vanishTimer -= BraveTime.DeltaTime;
      if ((double) this.m_vanishTimer > 0.0)
        return;
      this.DoVanish();
      this.Finish();
    }

    private void DoVanish()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      GameManager.Instance.Dungeon.GetRoomFromPosition(component.transform.position.IntXY(VectorConversions.Floor)).DeregisterInteractable((IPlayerInteractable) component);
      component.CloseTextBox(true);
      if ((Object) component.specRigidbody != (Object) null)
        component.specRigidbody.enabled = false;
      for (int index = 0; index < this.itemsToLeaveBehind.Length; ++index)
        this.itemsToLeaveBehind[index].Value.transform.parent = component.transform.parent;
      for (int index = 0; index < component.itemsToLeaveBehind.Count; ++index)
        component.itemsToLeaveBehind[index].transform.parent = component.transform.parent;
      if ((bool) (Object) component.aiAnimator)
      {
        component.aiAnimator.PlayUntilFinished(this.vanishAnim.Value);
        Object.Destroy((Object) component.gameObject, component.spriteAnimator.CurrentClip.BaseClipLength);
      }
      else if ((bool) (Object) component.spriteAnimator && component.spriteAnimator.GetClipByName(this.vanishAnim.Value) != null)
        component.spriteAnimator.PlayAndDestroyObject(this.vanishAnim.Value);
      else
        Object.Destroy((Object) component.gameObject);
    }
  }
}
