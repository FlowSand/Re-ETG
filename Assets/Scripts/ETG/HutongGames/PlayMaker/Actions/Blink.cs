// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Blink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Effects)]
  [HutongGames.PlayMaker.Tooltip("Turns a Game Object on/off in a regular repeating pattern.")]
  public class Blink : ComponentAction<Renderer>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to blink on/off.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Time to stay off in seconds.")]
    [HasFloatSlider(0.0f, 5f)]
    public FsmFloat timeOff;
    [HutongGames.PlayMaker.Tooltip("Time to stay on in seconds.")]
    [HasFloatSlider(0.0f, 5f)]
    public FsmFloat timeOn;
    [HutongGames.PlayMaker.Tooltip("Should the object start in the active/visible state?")]
    public FsmBool startOn;
    [HutongGames.PlayMaker.Tooltip("Only effect the renderer, keeping other components active.")]
    public bool rendererOnly;
    [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
    public bool realTime;
    private float startTime;
    private float timer;
    private bool blinkOn;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.timeOff = (FsmFloat) 0.5f;
      this.timeOn = (FsmFloat) 0.5f;
      this.rendererOnly = true;
      this.startOn = (FsmBool) false;
      this.realTime = false;
    }

    public override void OnEnter()
    {
      this.startTime = FsmTime.RealtimeSinceStartup;
      this.timer = 0.0f;
      this.UpdateBlinkState(this.startOn.Value);
    }

    public override void OnUpdate()
    {
      if (this.realTime)
        this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
      else
        this.timer += Time.deltaTime;
      if (this.blinkOn && (double) this.timer > (double) this.timeOn.Value)
        this.UpdateBlinkState(false);
      if (this.blinkOn || (double) this.timer <= (double) this.timeOff.Value)
        return;
      this.UpdateBlinkState(true);
    }

    private void UpdateBlinkState(bool state)
    {
      GameObject go = this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner;
      if ((Object) go == (Object) null)
        return;
      if (this.rendererOnly)
      {
        if (this.UpdateCache(go))
          this.renderer.enabled = state;
      }
      else
        go.SetActive(state);
      this.blinkOn = state;
      this.startTime = FsmTime.RealtimeSinceStartup;
      this.timer = 0.0f;
    }
  }
}
