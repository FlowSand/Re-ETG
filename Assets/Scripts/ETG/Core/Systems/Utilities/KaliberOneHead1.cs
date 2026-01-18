// Decompiled with JetBrains decompiler
// Type: KaliberOneHead1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Kaliber/OneHead1")]
public class KaliberOneHead1 : Script
  {
    private const int NumBullets = 60;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KaliberOneHead1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void GetOffset(out Vector2 offset, out float angle, bool left)
    {
      int num = this.Tick % 40;
      this.GetFrameOffset(num / 5, out offset, left);
      if (left)
      {
        if (num <= 25)
          angle = Mathf.Lerp(70f, 290f, (float) num / 25f);
        else
          angle = Mathf.Lerp(290f, 70f, (float) (num - 25) / 15f);
      }
      else if (num <= 25)
        angle = Mathf.Lerp(-110f, 110f, (float) num / 25f);
      else
        angle = Mathf.Lerp(110f, -110f, (float) (num - 25) / 15f);
    }

    private void GetFrameOffset(int frame, out Vector2 offset, bool left)
    {
      IntVector2 intVector2 = IntVector2.Zero;
      if (left)
      {
        switch (frame)
        {
          case 0:
            intVector2 = new IntVector2(13, 30);
            break;
          case 1:
            intVector2 = new IntVector2(12, 28);
            break;
          case 2:
            intVector2 = new IntVector2(13, 20);
            break;
          case 3:
            intVector2 = new IntVector2(12, 12);
            break;
          case 4:
            intVector2 = new IntVector2(11, 4);
            break;
          case 5:
            intVector2 = new IntVector2(11, 3);
            break;
          case 6:
            intVector2 = new IntVector2(9, 4);
            break;
          case 7:
            intVector2 = new IntVector2(11, 12);
            break;
        }
      }
      else
      {
        switch (frame)
        {
          case 0:
            intVector2 = new IntVector2(59, 3);
            break;
          case 1:
            intVector2 = new IntVector2(61, 4);
            break;
          case 2:
            intVector2 = new IntVector2(62, 11);
            break;
          case 3:
            intVector2 = new IntVector2(61, 20);
            break;
          case 4:
            intVector2 = new IntVector2(60, 28);
            break;
          case 5:
            intVector2 = new IntVector2(58, 31 /*0x1F*/);
            break;
          case 6:
            intVector2 = new IntVector2(60, 28);
            break;
          case 7:
            intVector2 = new IntVector2(61, 21);
            break;
        }
      }
      IntVector2 pixel = intVector2 - new IntVector2(36, 13);
      offset = PhysicsEngine.PixelToUnit(pixel);
    }
  }

