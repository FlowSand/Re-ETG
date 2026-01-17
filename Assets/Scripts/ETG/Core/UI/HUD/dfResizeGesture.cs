// Decompiled with JetBrains decompiler
// Type: dfResizeGesture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [AddComponentMenu("Daikon Forge/Input/Gestures/Resize")]
    public class dfResizeGesture : dfGestureBase
    {
      private float lastDistance;

      public event dfGestureEventHandler<dfResizeGesture> ResizeGestureStart;

      public event dfGestureEventHandler<dfResizeGesture> ResizeGestureUpdate;

      public event dfGestureEventHandler<dfResizeGesture> ResizeGestureEnd;

      public float SizeDelta { get; protected set; }

      protected void Start()
      {
      }

      public void OnMultiTouchEnd() => this.endGesture();

      public void OnMultiTouch(dfControl sender, dfTouchEventArgs args)
      {
        List<dfTouchInfo> touches = args.Touches;
        if (this.State == dfGestureState.None || this.State == dfGestureState.Cancelled || this.State == dfGestureState.Ended)
          this.State = dfGestureState.Possible;
        else if (this.State == dfGestureState.Possible)
        {
          if (!this.isResizeMovement(args.Touches))
            return;
          this.State = dfGestureState.Began;
          Vector2 center = this.getCenter(touches);
          this.CurrentPosition = center;
          this.StartPosition = center;
          this.lastDistance = Vector2.Distance(touches[0].position, touches[1].position);
          this.SizeDelta = 0.0f;
          if (this.ResizeGestureStart != null)
            this.ResizeGestureStart(this);
          this.gameObject.Signal("OnResizeGestureStart", (object) this);
        }
        else
        {
          if (this.State != dfGestureState.Began && this.State != dfGestureState.Changed || !this.isResizeMovement(touches))
            return;
          this.State = dfGestureState.Changed;
          this.CurrentPosition = this.getCenter(touches);
          float num = Vector2.Distance(touches[0].position, touches[1].position);
          this.SizeDelta = num - this.lastDistance;
          this.lastDistance = num;
          if (this.ResizeGestureUpdate != null)
            this.ResizeGestureUpdate(this);
          this.gameObject.Signal("OnResizeGestureUpdate", (object) this);
        }
      }

      private Vector2 getCenter(List<dfTouchInfo> list)
      {
        Vector2 zero = Vector2.zero;
        for (int index = 0; index < list.Count; ++index)
          zero += list[index].position;
        return zero / (float) list.Count;
      }

      private bool isResizeMovement(List<dfTouchInfo> list)
      {
        if (list.Count < 2)
          return false;
        dfTouchInfo dfTouchInfo1 = list[0];
        Vector2 normalized1 = (dfTouchInfo1.deltaPosition * (BraveTime.DeltaTime / dfTouchInfo1.deltaTime)).normalized;
        dfTouchInfo dfTouchInfo2 = list[1];
        Vector2 normalized2 = (dfTouchInfo2.deltaPosition * (BraveTime.DeltaTime / dfTouchInfo2.deltaTime)).normalized;
        float f1 = Vector2.Dot(normalized1, (dfTouchInfo1.position - dfTouchInfo2.position).normalized);
        float f2 = Vector2.Dot(normalized2, (dfTouchInfo2.position - dfTouchInfo1.position).normalized);
        return (double) Mathf.Abs(f1) >= 0.21460184454917908 || (double) Mathf.Abs(f2) >= 0.21460184454917908;
      }

      private void endGesture()
      {
        if (this.State == dfGestureState.Began || this.State == dfGestureState.Changed)
        {
          if (this.State == dfGestureState.Began)
            this.State = dfGestureState.Cancelled;
          else
            this.State = dfGestureState.Ended;
          float num = 0.0f;
          this.SizeDelta = num;
          this.lastDistance = num;
          if (this.ResizeGestureEnd != null)
            this.ResizeGestureEnd(this);
          this.gameObject.Signal("OnResizeGestureEnd", (object) this);
        }
        else
          this.State = dfGestureState.None;
      }
    }

}
