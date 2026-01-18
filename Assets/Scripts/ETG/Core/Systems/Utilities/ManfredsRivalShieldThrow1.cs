// Decompiled with JetBrains decompiler
// Type: ManfredsRivalShieldThrow1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("ManfredsRival/ShieldThrow1")]
public class ManfredsRivalShieldThrow1 : Script
  {
    private const int WaitTime = 70;
    private const int TravelTime = 90;
    private const int HoldTime = 480;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ManfredsRivalShieldThrow1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void FireShield(Vector2 endOffset)
    {
      this.FireExpandingLine(new Vector2(-0.5f, -1f), new Vector2(0.5f, -1f), 4, endOffset);
      this.FireExpandingLine(new Vector2(-0.8f, -0.7f), new Vector2(-0.8f, 0.2f), 4, endOffset);
      this.FireExpandingLine(new Vector2(0.8f, -0.7f), new Vector2(0.8f, 0.2f), 4, endOffset);
      this.FireExpandingLine(new Vector2(-0.8f, 0.2f), new Vector2(-0.15f, 1f), 4, endOffset);
      this.FireExpandingLine(new Vector2(0.8f, 0.2f), new Vector2(0.15f, 1f), 4, endOffset);
    }

    private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets, Vector2 endOffset)
    {
      start *= 0.5f;
      end *= 0.5f;
      for (int index = 0; index < numBullets; ++index)
      {
        float t = numBullets > 1 ? (float) index / ((float) numBullets - 1f) : 0.5f;
        Vector2 vector = Vector2.Lerp(start, end, t);
        vector.y *= -1f;
        this.Fire(new Offset(vector * 4f, transform: string.Empty), new Brave.BulletScript.Direction(vector.ToAngle()), (Bullet) new ManfredsRivalShieldThrow1.ShieldBullet(endOffset));
      }
    }

    public class ShieldBullet : Bullet
    {
      private Vector2 m_endOffset;

      public ShieldBullet(Vector2 endOffset)
        : base("shield")
      {
        this.m_endOffset = endOffset;
        this.SuppressVfx = true;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ManfredsRivalShieldThrow1.ShieldBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

