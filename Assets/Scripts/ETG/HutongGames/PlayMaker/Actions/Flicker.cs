// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Flicker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Effects)]
[HutongGames.PlayMaker.Tooltip("Randomly flickers a Game Object on/off.")]
public class Flicker : ComponentAction<Renderer>
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to flicker.")]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [HutongGames.PlayMaker.Tooltip("The frequency of the flicker in seconds.")]
  [HasFloatSlider(0.0f, 1f)]
  public FsmFloat frequency;
  [HutongGames.PlayMaker.Tooltip("Amount of time flicker is On (0-1). E.g. Use 0.95 for an occasional flicker.")]
  [HasFloatSlider(0.0f, 1f)]
  public FsmFloat amountOn;
  [HutongGames.PlayMaker.Tooltip("Only effect the renderer, leaving other components active.")]
  public bool rendererOnly;
  [HutongGames.PlayMaker.Tooltip("Ignore time scale. Useful if flickering UI when the game is paused.")]
  public bool realTime;
  private float startTime;
  private float timer;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.frequency = (FsmFloat) 0.1f;
    this.amountOn = (FsmFloat) 0.5f;
    this.rendererOnly = true;
    this.realTime = false;
  }

  public override void OnEnter()
  {
    this.startTime = FsmTime.RealtimeSinceStartup;
    this.timer = 0.0f;
  }

  public override void OnUpdate()
  {
    GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
    if ((Object) ownerDefaultTarget == (Object) null)
      return;
    if (this.realTime)
      this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
    else
      this.timer += Time.deltaTime;
    if ((double) this.timer <= (double) this.frequency.Value)
      return;
    bool flag = (double) Random.Range(0.0f, 1f) < (double) this.amountOn.Value;
    if (this.rendererOnly)
    {
      if (this.UpdateCache(ownerDefaultTarget))
        this.renderer.enabled = flag;
    }
    else
      ownerDefaultTarget.SetActive(flag);
    this.startTime = this.timer;
    this.timer = 0.0f;
  }
}
