// Decompiled with JetBrains decompiler
// Type: dfTweenComponent`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Text;
using UnityEngine;

#nullable disable
[Serializable]
public abstract class dfTweenComponent<T> : dfTweenComponentBase where T : struct
{
  [SerializeField]
  protected T startValue;
  [SerializeField]
  protected T endValue;
  [SerializeField]
  protected dfPlayDirection direction;
  private T actualStartValue;
  private T actualEndValue;
  private float startTime;
  private float pingPongDirection;

  public event TweenNotification TweenStarted;

  public event TweenNotification TweenStopped;

  public event TweenNotification TweenPaused;

  public event TweenNotification TweenResumed;

  public event TweenNotification TweenReset;

  public event TweenNotification TweenCompleted;

  public T StartValue
  {
    get => this.startValue;
    set
    {
      this.startValue = value;
      if (this.state == dfTweenState.Stopped)
        return;
      this.Stop();
      this.Play();
    }
  }

  public T EndValue
  {
    get => this.endValue;
    set
    {
      this.endValue = value;
      if (this.state == dfTweenState.Stopped)
        return;
      this.Stop();
      this.Play();
    }
  }

  public dfTweenState State => this.state;

  public static dfTweenComponent<T> Create(
    Component target,
    string propertyName,
    T startValue,
    T endValue,
    float length)
  {
    return dfTweenComponent<T>.Create(target, propertyName, startValue, endValue, length, dfEasingType.Linear);
  }

  public static dfTweenComponent<T> Create(
    Component target,
    string propertyName,
    T startValue,
    T endValue,
    float length,
    dfEasingType func)
  {
    if ((UnityEngine.Object) target == (UnityEngine.Object) null || (UnityEngine.Object) target.gameObject == (UnityEngine.Object) null)
      throw new ArgumentNullException(nameof (target));
    if (string.IsNullOrEmpty(propertyName))
      throw new ArgumentNullException(nameof (propertyName));
    dfTweenComponent<T> dfTweenComponent = (dfTweenComponent<T>) target.gameObject.AddComponent(typeof (T));
    dfTweenComponent.autoRun = false;
    dfTweenComponent.target = new dfComponentMemberInfo()
    {
      Component = target,
      MemberName = propertyName
    };
    dfTweenComponent.startValue = startValue;
    dfTweenComponent.endValue = endValue;
    dfTweenComponent.length = length;
    dfTweenComponent.easingType = func;
    return dfTweenComponent;
  }

  public override void Play()
  {
    if (this.state != dfTweenState.Stopped)
      this.Stop();
    if (!this.enabled || !this.gameObject.activeSelf || !this.gameObject.activeInHierarchy)
      return;
    if (this.target == null)
      throw new NullReferenceException("Tween target is NULL");
    this.boundProperty = this.target.IsValid ? this.target.GetProperty() : throw new InvalidOperationException($"Invalid property binding configuration on {this.getPath(this.gameObject.transform)} - {(object) this.target}");
    this.easingFunction = dfEasingFunctions.GetFunction(this.easingType);
    this.onStarted();
    this.actualStartValue = this.startValue;
    this.actualEndValue = this.endValue;
    if (this.syncStartWhenRun)
      this.actualStartValue = (T) this.boundProperty.Value;
    else if (this.startValueIsOffset)
      this.actualStartValue = this.offset(this.startValue, (T) this.boundProperty.Value);
    if (this.syncEndWhenRun)
      this.actualEndValue = (T) this.boundProperty.Value;
    else if (this.endValueIsOffset)
      this.actualEndValue = this.offset(this.endValue, (T) this.boundProperty.Value);
    this.boundProperty.Value = (object) this.actualStartValue;
    this.startTime = UnityEngine.Time.realtimeSinceStartup;
    this.state = dfTweenState.Started;
  }

  public override void Stop()
  {
    if (this.state == dfTweenState.Stopped)
      return;
    if (this.skipToEndOnStop)
      this.boundProperty.Value = (object) this.actualEndValue;
    this.state = dfTweenState.Stopped;
    this.onStopped();
    this.easingFunction = (dfEasingFunctions.EasingFunction) null;
    this.boundProperty = (dfObservableProperty) null;
  }

  public override void Reset()
  {
    if (this.boundProperty != null)
      this.boundProperty.Value = (object) this.actualStartValue;
    this.state = dfTweenState.Stopped;
    this.onReset();
    this.easingFunction = (dfEasingFunctions.EasingFunction) null;
    this.boundProperty = (dfObservableProperty) null;
  }

  public void Pause() => this.IsPaused = true;

  public void Resume() => this.IsPaused = false;

  public void Update()
  {
    if (this.state == dfTweenState.Stopped || this.state == dfTweenState.Paused)
      return;
    if (this.state == dfTweenState.Started)
    {
      if ((double) this.startTime + (double) this.StartDelay >= (double) UnityEngine.Time.realtimeSinceStartup)
        return;
      this.state = dfTweenState.Playing;
      this.startTime = UnityEngine.Time.realtimeSinceStartup;
      this.pingPongDirection = 0.0f;
    }
    float num = Mathf.Min(UnityEngine.Time.realtimeSinceStartup - this.startTime, this.length);
    if ((double) num >= (double) this.length)
    {
      if (this.loopType == dfTweenLoopType.Once)
      {
        this.boundProperty.Value = (object) this.actualEndValue;
        this.Stop();
        this.onCompleted();
      }
      else if (this.loopType == dfTweenLoopType.Loop)
      {
        this.startTime = UnityEngine.Time.realtimeSinceStartup;
      }
      else
      {
        if (this.loopType != dfTweenLoopType.PingPong)
          throw new NotImplementedException();
        this.startTime = UnityEngine.Time.realtimeSinceStartup;
        if ((double) this.pingPongDirection == 0.0)
          this.pingPongDirection = 1f;
        else
          this.pingPongDirection = 0.0f;
      }
    }
    else
    {
      float time = this.easingFunction(0.0f, 1f, Mathf.Abs(this.pingPongDirection - num / this.length));
      if (this.animCurve != null)
        time = this.animCurve.Evaluate(time);
      this.boundProperty.Value = (object) this.evaluate(this.actualStartValue, this.actualEndValue, time);
    }
  }

  public abstract T evaluate(T startValue, T endValue, float time);

  public abstract T offset(T value, T offset);

  public override string ToString()
  {
    return this.Target != null && this.Target.IsValid ? $"{this.TweenName} ({this.target.Component.name}.{this.target.MemberName})" : this.TweenName;
  }

  private string getPath(Transform obj)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (; (UnityEngine.Object) obj != (UnityEngine.Object) null; obj = obj.parent)
    {
      if (stringBuilder.Length > 0)
      {
        stringBuilder.Insert(0, "\\");
        stringBuilder.Insert(0, obj.name);
      }
      else
        stringBuilder.Append(obj.name);
    }
    return stringBuilder.ToString();
  }

  protected internal static float Lerp(float startValue, float endValue, float time)
  {
    return startValue + (endValue - startValue) * time;
  }

  protected internal override void onPaused()
  {
    this.SendMessage("TweenPaused", (object) this, SendMessageOptions.DontRequireReceiver);
    if (this.TweenPaused == null)
      return;
    this.TweenPaused((dfTweenPlayableBase) this);
  }

  protected internal override void onResumed()
  {
    this.SendMessage("TweenResumed", (object) this, SendMessageOptions.DontRequireReceiver);
    if (this.TweenResumed == null)
      return;
    this.TweenResumed((dfTweenPlayableBase) this);
  }

  protected internal override void onStarted()
  {
    this.SendMessage("TweenStarted", (object) this, SendMessageOptions.DontRequireReceiver);
    if (this.TweenStarted == null)
      return;
    this.TweenStarted((dfTweenPlayableBase) this);
  }

  protected internal override void onStopped()
  {
    this.SendMessage("TweenStopped", (object) this, SendMessageOptions.DontRequireReceiver);
    if (this.TweenStopped == null)
      return;
    this.TweenStopped((dfTweenPlayableBase) this);
  }

  protected internal override void onReset()
  {
    this.SendMessage("TweenReset", (object) this, SendMessageOptions.DontRequireReceiver);
    if (this.TweenReset == null)
      return;
    this.TweenReset((dfTweenPlayableBase) this);
  }

  protected internal override void onCompleted()
  {
    this.SendMessage("TweenCompleted", (object) this, SendMessageOptions.DontRequireReceiver);
    if (this.TweenCompleted == null)
      return;
    this.TweenCompleted((dfTweenPlayableBase) this);
  }
}
