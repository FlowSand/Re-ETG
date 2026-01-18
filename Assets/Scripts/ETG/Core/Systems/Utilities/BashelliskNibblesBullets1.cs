// Decompiled with JetBrains decompiler
// Type: BashelliskNibblesBullets1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Bashellisk/NibblesBullets1")]
public class BashelliskNibblesBullets1 : Script
  {
    private const int NumBullets = 8;
    private const int BulletSpeed = 12;
    private const int NibblesTickTime = 3;
    private const int NibblesTurnCooldown = 200;
    private const float NibblesTurnChance = 0.07f;

    protected override IEnumerator Top()
    {
      float input = BraveMathCollege.QuantizeFloat(this.GetAimDirection(1f, 12f), 90f);
      BashelliskNibblesBullets1.NibblesBullet parent = (BashelliskNibblesBullets1.NibblesBullet) null;
      for (int delay = 0; delay < 8; ++delay)
      {
        BashelliskNibblesBullets1.NibblesBullet nibblesBullet = new BashelliskNibblesBullets1.NibblesBullet(delay, parent);
        this.Fire(new Brave.BulletScript.Direction(BraveMathCollege.QuantizeFloat(input, 90f)), new Brave.BulletScript.Speed(12f), (Bullet) nibblesBullet);
        parent = nibblesBullet;
      }
      return (IEnumerator) null;
    }

    public class NibblesBullet : Bullet
    {
      private int delay;
      private BashelliskNibblesBullets1.NibblesBullet parent;
      private BashelliskNibblesBullets1.NibblesBullet child;
      private float prevDirection;
      private Vector2 prevPosition;
      private int turnCooldown;

      public NibblesBullet(int delay, BashelliskNibblesBullets1.NibblesBullet parent)
        : base("nibblesBullet")
      {
        this.delay = delay;
        this.parent = parent;
        if (parent == null)
          return;
        parent.child = this;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BashelliskNibblesBullets1.NibblesBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

