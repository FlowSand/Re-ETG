// Decompiled with JetBrains decompiler
// Type: dfMouseTouchInputSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfMouseTouchInputSource : IDFTouchInputSource
    {
      private List<dfTouchInfo> activeTouches = new List<dfTouchInfo>();
      private dfTouchTrackingInfo touch;
      private dfTouchTrackingInfo altTouch;

      public bool MirrorAlt { get; set; }

      public bool ParallelAlt { get; set; }

      public int TouchCount
      {
        get
        {
          int touchCount = 0;
          if (this.touch != null)
            ++touchCount;
          if (this.altTouch != null)
            ++touchCount;
          return touchCount;
        }
      }

      public IList<dfTouchInfo> Touches
      {
        get
        {
          this.activeTouches.Clear();
          if (this.touch != null)
            this.activeTouches.Add((dfTouchInfo) this.touch);
          if (this.altTouch != null)
            this.activeTouches.Add((dfTouchInfo) this.altTouch);
          return (IList<dfTouchInfo>) this.activeTouches;
        }
      }

      public void Update()
      {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(1))
        {
          if (this.altTouch != null)
            this.altTouch.Phase = TouchPhase.Ended;
          else
            this.altTouch = new dfTouchTrackingInfo()
            {
              Phase = TouchPhase.Began,
              FingerID = 1,
              Position = (Vector2) Input.mousePosition
            };
        }
        else
        {
          if (Input.GetKeyUp(KeyCode.LeftAlt))
          {
            if (this.altTouch != null)
            {
              this.altTouch.Phase = TouchPhase.Ended;
              return;
            }
          }
          else if (this.altTouch != null)
          {
            if (this.altTouch.Phase == TouchPhase.Ended)
              this.altTouch = (dfTouchTrackingInfo) null;
            else if (this.altTouch.Phase == TouchPhase.Began || this.altTouch.Phase == TouchPhase.Moved)
              this.altTouch.Phase = TouchPhase.Stationary;
          }
          if (this.touch != null)
            this.touch.IsActive = false;
          if (this.touch != null && Input.GetKeyDown(KeyCode.Escape))
            this.touch.Phase = TouchPhase.Canceled;
          else if (this.touch == null || this.touch.Phase != TouchPhase.Canceled)
          {
            if (Input.GetMouseButtonUp(0))
            {
              if (this.touch != null)
                this.touch.Phase = TouchPhase.Ended;
            }
            else if (Input.GetMouseButtonDown(0))
              this.touch = new dfTouchTrackingInfo()
              {
                FingerID = 0,
                Phase = TouchPhase.Began,
                Position = (Vector2) Input.mousePosition
              };
            else if (this.touch != null && this.touch.Phase != TouchPhase.Ended)
            {
              Vector2 vector2 = (Vector2) Input.mousePosition - this.touch.Position;
              bool flag = (double) Vector2.Distance((Vector2) Input.mousePosition, this.touch.Position) > 1.4012984643248171E-45;
              this.touch.Position = (Vector2) Input.mousePosition;
              this.touch.Phase = !flag ? TouchPhase.Stationary : TouchPhase.Moved;
              if (flag && this.altTouch != null && (this.MirrorAlt || this.ParallelAlt))
              {
                if (this.MirrorAlt)
                  this.altTouch.Position -= vector2;
                else
                  this.altTouch.Position += vector2;
                this.altTouch.Phase = TouchPhase.Moved;
              }
            }
          }
          if (this.touch == null || this.touch.IsActive)
            return;
          this.touch = (dfTouchTrackingInfo) null;
        }
      }

      public dfTouchInfo GetTouch(int index) => this.Touches[index];
    }

}
