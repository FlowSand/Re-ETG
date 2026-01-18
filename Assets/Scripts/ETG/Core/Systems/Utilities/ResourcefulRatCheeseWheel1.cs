using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/ResourcefulRat/CheeseWheel1")]
public class ResourcefulRatCheeseWheel1 : Script
  {
    private const float WallInset = 1.25f;
    private const int WallInsetTime = 40;
    private const int WallInsetTimeVariation = 20;
    private const int NumVerticalWallBullets = 20;
    private const int NumHorizontalWallBullets = 20;
    private const int MisfireBullets = 5;
    private const int TargetPoints = 8;
    private const float TargetAngleDelta = 45f;
    private const float TargetOffset = 0.875f;
    private const int MinTravelTime = 90;
    private const int MaxTravelTime = 135;
    private const int WaveDelay = 75;
    private const int NumWaves = 3;
    private static string[] TargetNames = new string[8]
    {
      "cheeseWedge0",
      "cheeseWedge1",
      "cheeseWedge2",
      "cheeseWedge3",
      "cheeseWedge4",
      "cheeseWedge5",
      "cheeseWedge6",
      "cheeseWedge7"
    };
    private static float[] RampHeights = new float[8]
    {
      2f,
      1f,
      0.0f,
      1f,
      2f,
      3f,
      4f,
      2f
    };
    private static Vector2[] TargetOffsets = new Vector2[8]
    {
      new Vector2(0.0f, 1f / 16f),
      new Vector2(1f / 16f, -1f / 16f),
      new Vector2(1f / 16f, 0.0f),
      new Vector2(1f / 16f, -1f / 16f),
      new Vector2(1f / 16f, 1f / 16f),
      new Vector2(0.0f, 0.0f),
      new Vector2(1f / 16f, 0.0f),
      new Vector2(0.125f, -0.125f)
    };

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ResourcefulRatCheeseWheel1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public override void OnForceEnded()
    {
      AIActor aiActor = this.BulletBank.aiActor;
      aiActor.IsGone = false;
      aiActor.specRigidbody.CollideWithOthers = true;
    }

    private void FireWallBullet(
      float facingDir,
      Vector2 spawnPos,
      Vector2 roomCenter,
      bool isMisfire)
    {
      int index = Mathf.RoundToInt(BraveMathCollege.ClampAngle360((spawnPos - roomCenter).ToAngle()) / 45f) % 8;
      float angle = (float) index * 45f;
      Vector2 targetPos = (roomCenter + BraveMathCollege.DegreesToVector(angle, 0.875f) + ResourcefulRatCheeseWheel1.TargetOffsets[index]).Quantize(1f / 16f);
      this.Fire(Offset.OverridePosition(spawnPos), new Brave.BulletScript.Direction(facingDir), new Brave.BulletScript.Speed(), (Bullet) new ResourcefulRatCheeseWheel1.CheeseWedgeBullet(this, ResourcefulRatCheeseWheel1.TargetNames[index], ResourcefulRatCheeseWheel1.RampHeights[index], targetPos, angle + 180f, isMisfire));
    }

    public class CheeseWedgeBullet : Bullet
    {
      private ResourcefulRatCheeseWheel1 m_parent;
      private Vector2 m_targetPos;
      private float m_endingAngle;
      private bool m_isMisfire;
      private float m_additionalRampHeight;

      public CheeseWedgeBullet(
        ResourcefulRatCheeseWheel1 parent,
        string bulletName,
        float additionalRampHeight,
        Vector2 targetPos,
        float endingAngle,
        bool isMisfire)
        : base(bulletName, true)
      {
        this.m_parent = parent;
        this.m_targetPos = targetPos;
        this.m_endingAngle = endingAngle;
        this.m_isMisfire = isMisfire;
        this.m_additionalRampHeight = additionalRampHeight;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatCheeseWheel1.CheeseWedgeBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class CheeseWheelBullet : Bullet
    {
      public CheeseWheelBullet()
        : base("cheeseWheel", true)
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatCheeseWheel1.CheeseWheelBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

