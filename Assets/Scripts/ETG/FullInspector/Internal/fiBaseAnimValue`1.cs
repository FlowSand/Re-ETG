using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  public abstract class fiBaseAnimValue<T>
  {
    private double m_LerpPosition = 1.0;
    public float speed = 2f;
    private T m_Start;
    [SerializeField]
    private T m_Target;
    private double m_LastTime;
    private bool m_Animating;

    protected fiBaseAnimValue(T value)
    {
      this.m_Start = value;
      this.m_Target = value;
    }

    public bool isAnimating => this.m_Animating;

    protected float lerpPosition
    {
      get
      {
        double num = 1.0 - this.m_LerpPosition;
        return (float) (1.0 - num * num * num * num);
      }
    }

    protected T start => this.m_Start;

    public T target
    {
      get => this.m_Target;
      set
      {
        if (this.m_Target.Equals((object) value))
          return;
        this.BeginAnimating(value, this.value);
      }
    }

    public T value
    {
      get => this.GetValue();
      set => this.StopAnim(value);
    }

    private static T2 Clamp<T2>(T2 val, T2 min, T2 max) where T2 : IComparable<T2>
    {
      if (val.CompareTo(min) < 0)
        return min;
      return val.CompareTo(max) > 0 ? max : val;
    }

    protected void BeginAnimating(T newTarget, T newStart)
    {
      this.m_Start = newStart;
      this.m_Target = newTarget;
      fiLateBindings.EditorApplication.AddUpdateFunc(new Action(this.Update));
      this.m_Animating = true;
      this.m_LastTime = fiLateBindings.EditorApplication.timeSinceStartup;
      this.m_LerpPosition = 0.0;
    }

    private void Update()
    {
      if (!this.m_Animating)
        return;
      this.UpdateLerpPosition();
      if ((double) this.lerpPosition < 1.0)
        return;
      this.m_Animating = false;
      fiLateBindings.EditorApplication.RemUpdateFunc(new Action(this.Update));
    }

    private void UpdateLerpPosition()
    {
      double timeSinceStartup = fiLateBindings.EditorApplication.timeSinceStartup;
      this.m_LerpPosition = fiBaseAnimValue<T>.Clamp<double>(this.m_LerpPosition + (timeSinceStartup - this.m_LastTime) * (double) this.speed, 0.0, 1.0);
      this.m_LastTime = timeSinceStartup;
    }

    protected void StopAnim(T newValue)
    {
      bool flag = false;
      if (!newValue.Equals((object) this.GetValue()) || this.m_LerpPosition < 1.0)
        flag = true;
      this.m_Target = newValue;
      this.m_Start = newValue;
      this.m_LerpPosition = 1.0;
      this.m_Animating = false;
      if (flag)
        ;
    }

    protected abstract T GetValue();
  }
}
