using Brave.BulletScript;
using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/NegativeSpace1")]
public class DraGunNegativeSpace1 : ScriptLite
  {
    private const int NumPlatforms = 10;
    private const int NumBullets = 19;
    private const int RowDelay = 16 /*0x10*/;
    private const float HalfRoomWidth = 17f;
    private const int PlatformRadius = 4;
    private static float[] PlatformAngles = new float[5]
    {
      155f,
      125f,
      90f,
      55f,
      25f
    };
    private static float[] PlatformDistances = new float[5]
    {
      1f,
      2.5f,
      3f,
      2.5f,
      1f
    };
    private static List<int> s_validPlatformIndices = new List<int>();
    private float ActivePlatformRadius;
    private List<Vector2> m_platformCenters;
    private float m_verticalGap;
    private float m_lastCenterHeight;
    private float m_rowHeight;

    public override void Start()
    {
      this.ActivePlatformRadius = !ChallengeManager.CHALLENGE_MODE_ACTIVE ? 4f : 3.75f;
      int capacity = 10;
      this.m_platformCenters = new List<Vector2>(capacity);
      this.m_platformCenters.Add(new Vector2(Random.Range(-17f, 17f), 0.0f));
      for (int index1 = 1; index1 < capacity; ++index1)
      {
        Vector2 platformCenter = this.m_platformCenters[index1 - 1];
        DraGunNegativeSpace1.s_validPlatformIndices.Clear();
        if (index1 % 3 == 0 && !ChallengeManager.CHALLENGE_MODE_ACTIVE)
        {
          DraGunNegativeSpace1.s_validPlatformIndices.Add(2);
        }
        else
        {
          for (int index2 = 0; index2 < DraGunNegativeSpace1.PlatformAngles.Length; ++index2)
          {
            if (index2 != 2)
            {
              Vector2 vector2 = platformCenter + BraveMathCollege.DegreesToVector(DraGunNegativeSpace1.PlatformAngles[index2], 2f * this.ActivePlatformRadius + DraGunNegativeSpace1.PlatformDistances[index2]);
              if ((double) vector2.x > -17.0 && (double) vector2.x < 17.0)
                DraGunNegativeSpace1.s_validPlatformIndices.Add(index2);
            }
          }
        }
        int index3 = BraveUtility.RandomElement<int>(DraGunNegativeSpace1.s_validPlatformIndices);
        this.m_platformCenters.Add(platformCenter + BraveMathCollege.DegreesToVector(DraGunNegativeSpace1.PlatformAngles[index3], 2f * this.ActivePlatformRadius + DraGunNegativeSpace1.PlatformDistances[index3]));
      }
      this.m_verticalGap = 1.6f;
      this.m_lastCenterHeight = this.m_platformCenters[this.m_platformCenters.Count - 1].y;
      this.m_rowHeight = 0.0f;
    }

    public override int Update(ref int state)
    {
      if (state != 0)
        return this.Done();
      if ((double) this.m_rowHeight < (double) this.m_lastCenterHeight)
      {
        for (int i = 0; i < 19; ++i)
        {
          float x = this.SubdivideRange(-17f, 17f, 19, i);
          Vector2 a = new Vector2(x, this.m_rowHeight);
          bool suppressOffset = false;
          for (int index = 0; index < this.m_platformCenters.Count; ++index)
          {
            if ((double) Vector2.Distance(a, this.m_platformCenters[index]) < (double) this.ActivePlatformRadius)
            {
              Vector2 i1;
              Vector2 i2;
              x = BraveMathCollege.LineCircleIntersections(this.m_platformCenters[index], this.ActivePlatformRadius, new Vector2(-17f, this.m_rowHeight), new Vector2(17f, this.m_rowHeight), out i1, out i2) != 1 ? ((double) Mathf.Abs(x - i1.x) >= (double) Mathf.Abs(x - i2.x) ? i2.x : i1.x) : i1.x;
              suppressOffset = true;
            }
          }
          this.Fire(new Offset(x, 18f, transform: string.Empty), new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(6f), (Bullet) new DraGunNegativeSpace1.WiggleBullet(suppressOffset));
        }
        this.m_rowHeight += this.m_verticalGap;
        return this.Wait(16 /*0x10*/);
      }
      ++state;
      return this.Wait(120);
    }

    public class WiggleBullet : BulletLite
    {
      private bool m_suppressOffset;
      private Vector2 m_truePosition;
      private Vector2 m_offset;
      private float m_xMagnitude;
      private float m_xPeriod;
      private float m_yMagnitude;
      private float m_yPeriod;
      private Vector2 m_delta;

      public WiggleBullet(bool suppressOffset)
        : base("default_novfx")
      {
        this.m_suppressOffset = suppressOffset;
      }

      public override void Start()
      {
        this.ManualControl = true;
        this.m_truePosition = this.Position;
        this.m_offset = Vector2.zero;
        this.m_xMagnitude = Random.Range(0.0f, 0.6f);
        this.m_xPeriod = Random.Range(1f, 2.5f);
        this.m_yMagnitude = Random.Range(0.0f, 0.4f);
        this.m_yPeriod = Random.Range(1f, 2.5f);
        this.m_delta = BraveMathCollege.DegreesToVector(this.Direction, this.Speed / 60f);
      }

      public override int Update(ref int state)
      {
        if (this.Tick >= 360)
        {
          this.Vanish();
          return this.Done();
        }
        if (!this.m_suppressOffset)
        {
          float num1 = Mathf.Clamp01(1f - Mathf.Abs(Mathf.Repeat((float) (0.5 + (double) this.Tick / 60.0 * (double) this.m_xPeriod), 2f) - 1f));
          float num2 = (float) (-2.0 * (double) num1 * (double) num1 * (double) num1 + 3.0 * (double) num1 * (double) num1);
          this.m_offset.x = (float) ((double) this.m_xMagnitude * (double) num2 + -(double) this.m_xMagnitude * (1.0 - (double) num2));
          float num3 = Mathf.Clamp01(1f - Mathf.Abs(Mathf.Repeat((float) (0.5 + (double) this.Tick / 60.0 * (double) this.m_yPeriod), 2f) - 1f));
          float num4 = (float) (-2.0 * (double) num3 * (double) num3 * (double) num3 + 3.0 * (double) num3 * (double) num3);
          this.m_offset.y = (float) ((double) this.m_yMagnitude * (double) num4 + -(double) this.m_yMagnitude * (1.0 - (double) num4));
        }
        this.m_truePosition += this.m_delta;
        this.Position = this.m_truePosition + this.m_offset;
        return this.Wait(1);
      }
    }
  }

