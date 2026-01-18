using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Ease base action - don't use!")]
  public abstract class EaseFsmAction : FsmStateAction
  {
    [RequiredField]
    public FsmFloat time;
    public FsmFloat speed;
    public FsmFloat delay;
    public EaseFsmAction.EaseType easeType = EaseFsmAction.EaseType.linear;
    public FsmBool reverse;
    [HutongGames.PlayMaker.Tooltip("Optionally send an Event when the animation finishes.")]
    public FsmEvent finishEvent;
    [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
    public bool realTime;
    protected EaseFsmAction.EasingFunction ease;
    protected float runningTime;
    protected float lastTime;
    protected float startTime;
    protected float deltaTime;
    protected float delayTime;
    protected float percentage;
    protected float[] fromFloats = new float[0];
    protected float[] toFloats = new float[0];
    protected float[] resultFloats = new float[0];
    protected bool finishAction;
    protected bool start;
    protected bool finished;
    protected bool isRunning;

    public override void Reset()
    {
      this.easeType = EaseFsmAction.EaseType.linear;
      this.time = new FsmFloat() { Value = 1f };
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.delay = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.speed = fsmFloat2;
      this.reverse = new FsmBool() { Value = false };
      this.realTime = false;
      this.finishEvent = (FsmEvent) null;
      this.ease = (EaseFsmAction.EasingFunction) null;
      this.runningTime = 0.0f;
      this.lastTime = 0.0f;
      this.percentage = 0.0f;
      this.fromFloats = new float[0];
      this.toFloats = new float[0];
      this.resultFloats = new float[0];
      this.finishAction = false;
      this.start = false;
      this.finished = false;
      this.isRunning = false;
    }

    public override void OnEnter()
    {
      this.finished = false;
      this.isRunning = false;
      this.SetEasingFunction();
      this.runningTime = 0.0f;
      this.percentage = !this.reverse.IsNone ? (!this.reverse.Value ? 0.0f : 1f) : 0.0f;
      this.finishAction = false;
      this.startTime = FsmTime.RealtimeSinceStartup;
      this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
      this.delayTime = !this.delay.IsNone ? (this.delayTime = this.delay.Value) : 0.0f;
      this.start = true;
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
      if (this.start && !this.isRunning)
      {
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
          this.startTime = FsmTime.RealtimeSinceStartup;
          this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
        }
      }
      if (!this.isRunning || this.finished)
        return;
      if ((!this.reverse.IsNone ? (this.reverse.Value ? 1 : 0) : 0) == 0)
      {
        this.UpdatePercentage();
        if ((double) this.percentage < 1.0)
        {
          for (int index = 0; index < this.fromFloats.Length; ++index)
            this.resultFloats[index] = this.ease(this.fromFloats[index], this.toFloats[index], this.percentage);
        }
        else
        {
          this.finishAction = true;
          this.finished = true;
          this.isRunning = false;
        }
      }
      else
      {
        this.UpdatePercentage();
        if ((double) this.percentage > 0.0)
        {
          for (int index = 0; index < this.fromFloats.Length; ++index)
            this.resultFloats[index] = this.ease(this.fromFloats[index], this.toFloats[index], this.percentage);
        }
        else
        {
          this.finishAction = true;
          this.finished = true;
          this.isRunning = false;
        }
      }
    }

    protected void UpdatePercentage()
    {
      if (this.realTime)
      {
        this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
        this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
        if (!this.speed.IsNone)
          this.runningTime += this.deltaTime * this.speed.Value;
        else
          this.runningTime += this.deltaTime;
      }
      else if (!this.speed.IsNone)
        this.runningTime += Time.deltaTime * this.speed.Value;
      else
        this.runningTime += Time.deltaTime;
      if ((!this.reverse.IsNone ? (this.reverse.Value ? 1 : 0) : 0) != 0)
        this.percentage = (float) (1.0 - (double) this.runningTime / (double) this.time.Value);
      else
        this.percentage = this.runningTime / this.time.Value;
    }

    protected void SetEasingFunction()
    {
      switch (this.easeType)
      {
        case EaseFsmAction.EaseType.easeInQuad:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInQuad);
          break;
        case EaseFsmAction.EaseType.easeOutQuad:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuad);
          break;
        case EaseFsmAction.EaseType.easeInOutQuad:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuad);
          break;
        case EaseFsmAction.EaseType.easeInCubic:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInCubic);
          break;
        case EaseFsmAction.EaseType.easeOutCubic:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutCubic);
          break;
        case EaseFsmAction.EaseType.easeInOutCubic:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCubic);
          break;
        case EaseFsmAction.EaseType.easeInQuart:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInQuart);
          break;
        case EaseFsmAction.EaseType.easeOutQuart:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuart);
          break;
        case EaseFsmAction.EaseType.easeInOutQuart:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuart);
          break;
        case EaseFsmAction.EaseType.easeInQuint:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInQuint);
          break;
        case EaseFsmAction.EaseType.easeOutQuint:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutQuint);
          break;
        case EaseFsmAction.EaseType.easeInOutQuint:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutQuint);
          break;
        case EaseFsmAction.EaseType.easeInSine:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInSine);
          break;
        case EaseFsmAction.EaseType.easeOutSine:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutSine);
          break;
        case EaseFsmAction.EaseType.easeInOutSine:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutSine);
          break;
        case EaseFsmAction.EaseType.easeInExpo:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInExpo);
          break;
        case EaseFsmAction.EaseType.easeOutExpo:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutExpo);
          break;
        case EaseFsmAction.EaseType.easeInOutExpo:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutExpo);
          break;
        case EaseFsmAction.EaseType.easeInCirc:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInCirc);
          break;
        case EaseFsmAction.EaseType.easeOutCirc:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutCirc);
          break;
        case EaseFsmAction.EaseType.easeInOutCirc:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutCirc);
          break;
        case EaseFsmAction.EaseType.linear:
          this.ease = new EaseFsmAction.EasingFunction(this.linear);
          break;
        case EaseFsmAction.EaseType.spring:
          this.ease = new EaseFsmAction.EasingFunction(this.spring);
          break;
        case EaseFsmAction.EaseType.bounce:
          this.ease = new EaseFsmAction.EasingFunction(this.bounce);
          break;
        case EaseFsmAction.EaseType.easeInBack:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInBack);
          break;
        case EaseFsmAction.EaseType.easeOutBack:
          this.ease = new EaseFsmAction.EasingFunction(this.easeOutBack);
          break;
        case EaseFsmAction.EaseType.easeInOutBack:
          this.ease = new EaseFsmAction.EasingFunction(this.easeInOutBack);
          break;
        case EaseFsmAction.EaseType.elastic:
          this.ease = new EaseFsmAction.EasingFunction(this.elastic);
          break;
      }
    }

    protected float linear(float start, float end, float value) => Mathf.Lerp(start, end, value);

    protected float clerp(float start, float end, float value)
    {
      float num1 = 0.0f;
      float num2 = 360f;
      float num3 = Mathf.Abs((float) (((double) num2 - (double) num1) / 2.0));
      float num4;
      if ((double) end - (double) start < -(double) num3)
      {
        float num5 = (num2 - start + end) * value;
        num4 = start + num5;
      }
      else if ((double) end - (double) start > (double) num3)
      {
        float num6 = (float) -((double) num2 - (double) end + (double) start) * value;
        num4 = start + num6;
      }
      else
        num4 = start + (end - start) * value;
      return num4;
    }

    protected float spring(float start, float end, float value)
    {
      value = Mathf.Clamp01(value);
      value = (float) (((double) Mathf.Sin((float) ((double) value * 3.1415927410125732 * (0.20000000298023224 + 2.5 * (double) value * (double) value * (double) value))) * (double) Mathf.Pow(1f - value, 2.2f) + (double) value) * (1.0 + 1.2000000476837158 * (1.0 - (double) value)));
      return start + (end - start) * value;
    }

    protected float easeInQuad(float start, float end, float value)
    {
      end -= start;
      return end * value * value + start;
    }

    protected float easeOutQuad(float start, float end, float value)
    {
      end -= start;
      return (float) (-(double) end * (double) value * ((double) value - 2.0)) + start;
    }

    protected float easeInOutQuad(float start, float end, float value)
    {
      value /= 0.5f;
      end -= start;
      if ((double) value < 1.0)
        return end / 2f * value * value + start;
      --value;
      return (float) (-(double) end / 2.0 * ((double) value * ((double) value - 2.0) - 1.0)) + start;
    }

    protected float easeInCubic(float start, float end, float value)
    {
      end -= start;
      return end * value * value * value + start;
    }

    protected float easeOutCubic(float start, float end, float value)
    {
      --value;
      end -= start;
      return end * (float) ((double) value * (double) value * (double) value + 1.0) + start;
    }

    protected float easeInOutCubic(float start, float end, float value)
    {
      value /= 0.5f;
      end -= start;
      if ((double) value < 1.0)
        return end / 2f * value * value * value + start;
      value -= 2f;
      return (float) ((double) end / 2.0 * ((double) value * (double) value * (double) value + 2.0)) + start;
    }

    protected float easeInQuart(float start, float end, float value)
    {
      end -= start;
      return end * value * value * value * value + start;
    }

    protected float easeOutQuart(float start, float end, float value)
    {
      --value;
      end -= start;
      return (float) (-(double) end * ((double) value * (double) value * (double) value * (double) value - 1.0)) + start;
    }

    protected float easeInOutQuart(float start, float end, float value)
    {
      value /= 0.5f;
      end -= start;
      if ((double) value < 1.0)
        return end / 2f * value * value * value * value + start;
      value -= 2f;
      return (float) (-(double) end / 2.0 * ((double) value * (double) value * (double) value * (double) value - 2.0)) + start;
    }

    protected float easeInQuint(float start, float end, float value)
    {
      end -= start;
      return end * value * value * value * value * value + start;
    }

    protected float easeOutQuint(float start, float end, float value)
    {
      --value;
      end -= start;
      return end * (float) ((double) value * (double) value * (double) value * (double) value * (double) value + 1.0) + start;
    }

    protected float easeInOutQuint(float start, float end, float value)
    {
      value /= 0.5f;
      end -= start;
      if ((double) value < 1.0)
        return end / 2f * value * value * value * value * value + start;
      value -= 2f;
      return (float) ((double) end / 2.0 * ((double) value * (double) value * (double) value * (double) value * (double) value + 2.0)) + start;
    }

    protected float easeInSine(float start, float end, float value)
    {
      end -= start;
      return -end * Mathf.Cos((float) ((double) value / 1.0 * 1.5707963705062866)) + end + start;
    }

    protected float easeOutSine(float start, float end, float value)
    {
      end -= start;
      return end * Mathf.Sin((float) ((double) value / 1.0 * 1.5707963705062866)) + start;
    }

    protected float easeInOutSine(float start, float end, float value)
    {
      end -= start;
      return (float) (-(double) end / 2.0 * ((double) Mathf.Cos((float) (3.1415927410125732 * (double) value / 1.0)) - 1.0)) + start;
    }

    protected float easeInExpo(float start, float end, float value)
    {
      end -= start;
      return end * Mathf.Pow(2f, (float) (10.0 * ((double) value / 1.0 - 1.0))) + start;
    }

    protected float easeOutExpo(float start, float end, float value)
    {
      end -= start;
      return end * (float) (-(double) Mathf.Pow(2f, (float) (-10.0 * (double) value / 1.0)) + 1.0) + start;
    }

    protected float easeInOutExpo(float start, float end, float value)
    {
      value /= 0.5f;
      end -= start;
      if ((double) value < 1.0)
        return end / 2f * Mathf.Pow(2f, (float) (10.0 * ((double) value - 1.0))) + start;
      --value;
      return (float) ((double) end / 2.0 * (-(double) Mathf.Pow(2f, -10f * value) + 2.0)) + start;
    }

    protected float easeInCirc(float start, float end, float value)
    {
      end -= start;
      return (float) (-(double) end * ((double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) - 1.0)) + start;
    }

    protected float easeOutCirc(float start, float end, float value)
    {
      --value;
      end -= start;
      return end * Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) + start;
    }

    protected float easeInOutCirc(float start, float end, float value)
    {
      value /= 0.5f;
      end -= start;
      if ((double) value < 1.0)
        return (float) (-(double) end / 2.0 * ((double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) - 1.0)) + start;
      value -= 2f;
      return (float) ((double) end / 2.0 * ((double) Mathf.Sqrt((float) (1.0 - (double) value * (double) value)) + 1.0)) + start;
    }

    protected float bounce(float start, float end, float value)
    {
      value /= 1f;
      end -= start;
      if ((double) value < 0.36363637447357178)
        return end * (121f / 16f * value * value) + start;
      if ((double) value < 0.72727274894714355)
      {
        value -= 0.545454562f;
        return end * (float) (121.0 / 16.0 * (double) value * (double) value + 0.75) + start;
      }
      if ((double) value < 10.0 / 11.0)
      {
        value -= 0.8181818f;
        return end * (float) (121.0 / 16.0 * (double) value * (double) value + 15.0 / 16.0) + start;
      }
      value -= 0.954545438f;
      return end * (float) (121.0 / 16.0 * (double) value * (double) value + 63.0 / 64.0) + start;
    }

    protected float easeInBack(float start, float end, float value)
    {
      end -= start;
      value /= 1f;
      float num = 1.70158f;
      return (float) ((double) end * (double) value * (double) value * (((double) num + 1.0) * (double) value - (double) num)) + start;
    }

    protected float easeOutBack(float start, float end, float value)
    {
      float num = 1.70158f;
      end -= start;
      value = (float) ((double) value / 1.0 - 1.0);
      return end * (float) ((double) value * (double) value * (((double) num + 1.0) * (double) value + (double) num) + 1.0) + start;
    }

    protected float easeInOutBack(float start, float end, float value)
    {
      float num1 = 1.70158f;
      end -= start;
      value /= 0.5f;
      if ((double) value < 1.0)
      {
        float num2 = num1 * 1.525f;
        return (float) ((double) end / 2.0 * ((double) value * (double) value * (((double) num2 + 1.0) * (double) value - (double) num2))) + start;
      }
      value -= 2f;
      float num3 = num1 * 1.525f;
      return (float) ((double) end / 2.0 * ((double) value * (double) value * (((double) num3 + 1.0) * (double) value + (double) num3) + 2.0)) + start;
    }

    protected float punch(float amplitude, float value)
    {
      if ((double) value == 0.0 || (double) value == 1.0)
        return 0.0f;
      float num1 = 0.3f;
      float num2 = num1 / 6.28318548f * Mathf.Asin(0.0f);
      return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((float) (((double) value * 1.0 - (double) num2) * 6.2831854820251465) / num1);
    }

    protected float elastic(float start, float end, float value)
    {
      end -= start;
      float num1 = 1f;
      float num2 = num1 * 0.3f;
      float num3 = 0.0f;
      if ((double) value == 0.0)
        return start;
      if ((double) (value /= num1) == 1.0)
        return start + end;
      float num4;
      if ((double) num3 == 0.0 || (double) num3 < (double) Mathf.Abs(end))
      {
        num3 = end;
        num4 = num2 / 4f;
      }
      else
        num4 = num2 / 6.28318548f * Mathf.Asin(end / num3);
      return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((float) (((double) value * (double) num1 - (double) num4) * 6.2831854820251465) / num2) + end + start;
    }

    protected delegate float EasingFunction(float start, float end, float value);

    public enum EaseType
    {
      easeInQuad,
      easeOutQuad,
      easeInOutQuad,
      easeInCubic,
      easeOutCubic,
      easeInOutCubic,
      easeInQuart,
      easeOutQuart,
      easeInOutQuart,
      easeInQuint,
      easeOutQuint,
      easeInOutQuint,
      easeInSine,
      easeOutSine,
      easeInOutSine,
      easeInExpo,
      easeOutExpo,
      easeInOutExpo,
      easeInCirc,
      easeOutCirc,
      easeInOutCirc,
      linear,
      spring,
      bounce,
      easeInBack,
      easeOutBack,
      easeInOutBack,
      elastic,
      punch,
    }
  }
}
