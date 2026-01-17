// Decompiled with JetBrains decompiler
// Type: ChancebulonDice1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Chancebulon/Dice1")]
    public class ChancebulonDice1 : Script
    {
      public const float Radius = 2f;
      public const int GrowTime = 15;
      public const float RotationSpeed = 180f;
      public const float BulletSpeed = 10f;

      public float aimDirection { get; private set; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ChancebulonDice1__Topc__Iterator0()
        {
          _this = this
        };
      }

      private void FireSquare()
      {
        Vector2 vector2_1 = new Vector2(2.2f, 0.0f).Rotate(45f);
        Vector2 vector2_2 = new Vector2(2.2f, 0.0f).Rotate(135f);
        Vector2 vector2_3 = new Vector2(2.2f, 0.0f).Rotate(225f);
        Vector2 vector2_4 = new Vector2(2.2f, 0.0f).Rotate(-45f);
        this.FireExpandingLine(vector2_1, vector2_2, 5);
        this.FireExpandingLine(vector2_2, vector2_3, 5);
        this.FireExpandingLine(vector2_3, vector2_4, 5);
        this.FireExpandingLine(vector2_4, vector2_1, 5);
        this.Fire((Bullet) new ChancebulonDice1.ExpandingBullet(this, new Vector2(0.0f, 0.0f), new int?(0)));
        this.Fire((Bullet) new ChancebulonDice1.ExpandingBullet(this, new Vector2(0.0f, 0.0f), new int?(1)));
        this.Fire((Bullet) new ChancebulonDice1.ExpandingBullet(this, new Vector2(0.0f, 0.0f), new int?(2)));
        this.Fire((Bullet) new ChancebulonDice1.ExpandingBullet(this, new Vector2(0.0f, 0.0f), new int?(3)));
        this.Fire((Bullet) new ChancebulonDice1.ExpandingBullet(this, new Vector2(0.0f, 0.0f), new int?(4)));
        this.Fire((Bullet) new ChancebulonDice1.ExpandingBullet(this, new Vector2(0.0f, 0.0f), new int?(5)));
      }

      private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets)
      {
        for (int index = 0; index < numBullets; ++index)
          this.Fire((Bullet) new ChancebulonDice1.ExpandingBullet(this, Vector2.Lerp(start, end, (float) index / ((float) numBullets - 1f))));
      }

      public class ExpandingBullet : Bullet
      {
        private const int SingleFaceShowTime = 13;
        private ChancebulonDice1 m_parent;
        private Vector2 m_offset;
        private int? m_numeralIndex;
        private int m_currentNumeral;

        public ExpandingBullet(ChancebulonDice1 parent, Vector2 offset, int? numeralIndex = null)
          : base()
        {
          this.m_parent = parent;
          this.m_offset = offset;
          this.m_numeralIndex = numeralIndex;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new ChancebulonDice1.ExpandingBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
