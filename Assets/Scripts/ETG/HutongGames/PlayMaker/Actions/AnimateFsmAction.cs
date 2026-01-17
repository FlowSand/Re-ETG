// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnimateFsmAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public abstract class AnimateFsmAction : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Define time to use your curve scaled to be stretched or shrinked.")]
    public FsmFloat time;
    [HutongGames.PlayMaker.Tooltip("If you define speed, your animation will be speeded up or slowed down.")]
    public FsmFloat speed;
    [HutongGames.PlayMaker.Tooltip("Delayed animimation start.")]
    public FsmFloat delay;
    [HutongGames.PlayMaker.Tooltip("Animation curve start from any time. If IgnoreCurveOffset is true the animation starts right after the state become entered.")]
    public FsmBool ignoreCurveOffset;
    [HutongGames.PlayMaker.Tooltip("Optionally send an Event when the animation finishes.")]
    public FsmEvent finishEvent;
    [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
    public bool realTime;
    private float startTime;
    private float currentTime;
    private float[] endTimes;
    private float lastTime;
    private float deltaTime;
    private float delayTime;
    private float[] keyOffsets;
    protected AnimationCurve[] curves;
    protected AnimateFsmAction.Calculation[] calculations;
    protected float[] resultFloats;
    protected float[] fromFloats;
    protected float[] toFloats;
    protected bool finishAction;
    protected bool isRunning;
    protected bool looping;
    private bool start;
    private float largestEndTime;

    public override void Reset()
    {
      this.finishEvent = (FsmEvent) null;
      this.realTime = false;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.time = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.speed = fsmFloat2;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.delay = fsmFloat3;
      this.ignoreCurveOffset = new FsmBool() { Value = true };
      this.resultFloats = new float[0];
      this.fromFloats = new float[0];
      this.toFloats = new float[0];
      this.endTimes = new float[0];
      this.keyOffsets = new float[0];
      this.curves = new AnimationCurve[0];
      this.finishAction = false;
      this.start = false;
    }

    public override void OnEnter()
    {
      this.startTime = FsmTime.RealtimeSinceStartup;
      this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
      this.deltaTime = 0.0f;
      this.currentTime = 0.0f;
      this.isRunning = false;
      this.finishAction = false;
      this.looping = false;
      this.delayTime = !this.delay.IsNone ? (this.delayTime = this.delay.Value) : 0.0f;
      this.start = true;
    }

    protected void Init()
    {
      this.endTimes = new float[this.curves.Length];
      this.keyOffsets = new float[this.curves.Length];
      this.largestEndTime = 0.0f;
      for (int index = 0; index < this.curves.Length; ++index)
      {
        if (this.curves[index] != null && this.curves[index].keys.Length > 0)
        {
          this.keyOffsets[index] = this.curves[index].keys.Length <= 0 ? 0.0f : (!this.time.IsNone ? this.time.Value / this.curves[index].keys[this.curves[index].length - 1].time * this.curves[index].keys[0].time : this.curves[index].keys[0].time);
          this.currentTime = !this.ignoreCurveOffset.IsNone ? (!this.ignoreCurveOffset.Value ? 0.0f : this.keyOffsets[index]) : 0.0f;
          this.endTimes[index] = this.time.IsNone ? this.curves[index].keys[this.curves[index].length - 1].time : this.time.Value;
          if ((double) this.largestEndTime < (double) this.endTimes[index])
            this.largestEndTime = this.endTimes[index];
          if (!this.looping)
            this.looping = ActionHelpers.IsLoopingWrapMode(this.curves[index].postWrapMode);
        }
        else
          this.endTimes[index] = -1f;
      }
      for (int index = 0; index < this.curves.Length; ++index)
      {
        if ((double) this.largestEndTime > 0.0 && (double) this.endTimes[index] == -1.0)
          this.endTimes[index] = this.largestEndTime;
        else if ((double) this.largestEndTime == 0.0 && (double) this.endTimes[index] == -1.0)
          this.endTimes[index] = !this.time.IsNone ? this.time.Value : 1f;
      }
      this.UpdateAnimation();
    }

    public override void OnUpdate()
    {
      this.CheckStart();
      if (!this.isRunning)
        return;
      this.UpdateTime();
      this.UpdateAnimation();
      this.CheckFinished();
    }

    private void CheckStart()
    {
      if (this.isRunning || !this.start)
        return;
      if ((double) this.delayTime >= 0.0)
      {
        if (this.realTime)
        {
          this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
          this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
          this.delayTime -= this.deltaTime;
        }
        else
          this.delayTime -= Time.deltaTime;
      }
      else
      {
        this.isRunning = true;
        this.start = false;
      }
    }

    private void UpdateTime()
    {
      if (this.realTime)
      {
        this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
        this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
        if (!this.speed.IsNone)
          this.currentTime += this.deltaTime * this.speed.Value;
        else
          this.currentTime += this.deltaTime;
      }
      else if (!this.speed.IsNone)
        this.currentTime += Time.deltaTime * this.speed.Value;
      else
        this.currentTime += Time.deltaTime;
    }

    public void UpdateAnimation()
    {
      for (int index = 0; index < this.curves.Length; ++index)
      {
        if (this.curves[index] != null && this.curves[index].keys.Length > 0)
        {
          if (this.calculations[index] != AnimateFsmAction.Calculation.None)
          {
            switch (this.calculations[index])
            {
              case AnimateFsmAction.Calculation.SetValue:
                this.resultFloats[index] = this.time.IsNone ? this.curves[index].Evaluate(this.currentTime) : this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time);
                continue;
              case AnimateFsmAction.Calculation.AddToValue:
                this.resultFloats[index] = this.time.IsNone ? this.fromFloats[index] + this.curves[index].Evaluate(this.currentTime) : this.fromFloats[index] + this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time);
                continue;
              case AnimateFsmAction.Calculation.SubtractFromValue:
                this.resultFloats[index] = this.time.IsNone ? this.fromFloats[index] - this.curves[index].Evaluate(this.currentTime) : this.fromFloats[index] - this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time);
                continue;
              case AnimateFsmAction.Calculation.SubtractValueFromCurve:
                this.resultFloats[index] = this.time.IsNone ? this.curves[index].Evaluate(this.currentTime) - this.fromFloats[index] : this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time) - this.fromFloats[index];
                continue;
              case AnimateFsmAction.Calculation.MultiplyValue:
                this.resultFloats[index] = this.time.IsNone ? this.curves[index].Evaluate(this.currentTime) * this.fromFloats[index] : this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time) * this.fromFloats[index];
                continue;
              case AnimateFsmAction.Calculation.DivideValue:
                this.resultFloats[index] = this.time.IsNone ? ((double) this.curves[index].Evaluate(this.currentTime) == 0.0 ? float.MaxValue : this.fromFloats[index] / this.curves[index].Evaluate(this.currentTime)) : ((double) this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time) == 0.0 ? float.MaxValue : this.fromFloats[index] / this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time));
                continue;
              case AnimateFsmAction.Calculation.DivideCurveByValue:
                this.resultFloats[index] = this.time.IsNone ? ((double) this.fromFloats[index] == 0.0 ? float.MaxValue : this.curves[index].Evaluate(this.currentTime) / this.fromFloats[index]) : ((double) this.fromFloats[index] == 0.0 ? float.MaxValue : this.curves[index].Evaluate(this.currentTime / this.time.Value * this.curves[index].keys[this.curves[index].length - 1].time) / this.fromFloats[index]);
                continue;
              default:
                continue;
            }
          }
          else
            this.resultFloats[index] = this.fromFloats[index];
        }
        else
          this.resultFloats[index] = this.fromFloats[index];
      }
    }

    private void CheckFinished()
    {
      if (!this.isRunning || this.looping)
        return;
      this.finishAction = true;
      for (int index = 0; index < this.endTimes.Length; ++index)
      {
        if ((double) this.currentTime < (double) this.endTimes[index])
          this.finishAction = false;
      }
      this.isRunning = !this.finishAction;
    }

    public enum Calculation
    {
      None,
      SetValue,
      AddToValue,
      SubtractFromValue,
      SubtractValueFromCurve,
      MultiplyValue,
      DivideValue,
      DivideCurveByValue,
    }
  }
}
