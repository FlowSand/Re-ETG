// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetNpcVisibility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".NPCs")]
[HutongGames.PlayMaker.Tooltip("Sets the NPC's visibility (renderers and Speculative Rigidbody).")]
public class SetNpcVisibility : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Set visibility to this.")]
  public FsmBool visible;

  public override void Reset() => this.visible = (FsmBool) true;

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    if ((bool) (Object) component)
      SetNpcVisibility.SetVisible(component, this.visible.Value);
    this.Finish();
  }

  public static void SetVisible(TalkDoerLite talkDoer, bool visible)
  {
    talkDoer.renderer.enabled = visible;
    talkDoer.ShowOutlines = visible;
    if ((bool) (Object) talkDoer.shadow)
      talkDoer.shadow.GetComponent<Renderer>().enabled = visible;
    if ((bool) (Object) talkDoer.specRigidbody)
      talkDoer.specRigidbody.enabled = visible;
    if (!(bool) (Object) talkDoer.ultraFortunesFavor)
      return;
    talkDoer.ultraFortunesFavor.enabled = visible;
  }
}
