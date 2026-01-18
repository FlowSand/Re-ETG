using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ForgeFlamePipeController : BraveBehaviour, IPlaceConfigurable
  {
    public float DamageToEnemies = 6f;
    public float TimeToSpew = 10f;
    public float ConeAngle = 10f;
    public float TimeBetweenBullets = 0.05f;
    public DungeonData.Direction DirectionToSpew = DungeonData.Direction.EAST;
    public Transform ShootPoint;
    public string EndSpriteName;
    public string LoopAnimationName;
    public string OutAnimationName;
    public tk2dSpriteAnimator[] vfxAnimators;
    private bool m_hasBurst;

    public void Start()
    {
      this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
      this.specRigidbody.OnBeamCollision += new SpeculativeRigidbody.OnBeamCollisionDelegate(this.HandleBeamCollision);
    }

    private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
    {
      if (this.m_hasBurst || !(bool) (Object) rigidbodyCollision.OtherRigidbody.projectile)
        return;
      this.m_hasBurst = true;
      this.StartCoroutine(this.HandleBurst());
    }

    private void HandleBeamCollision(BeamController beamController)
    {
      if (this.m_hasBurst)
        return;
      this.m_hasBurst = true;
      this.StartCoroutine(this.HandleBurst());
    }

    [DebuggerHidden]
    private IEnumerator HandleBurst()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ForgeFlamePipeController__HandleBurstc__Iterator0()
      {
        _this = this
      };
    }

    private void FireBullet()
    {
      this.bulletBank.CreateProjectileFromBank((Vector2) this.ShootPoint.position, BraveMathCollege.Atan2Degrees(DungeonData.GetIntVector2FromDirection(this.DirectionToSpew).ToVector2()) + Random.Range(-this.ConeAngle, this.ConeAngle), "default");
    }

    protected override void OnDestroy()
    {
      this.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
      this.specRigidbody.OnBeamCollision -= new SpeculativeRigidbody.OnBeamCollisionDelegate(this.HandleBeamCollision);
      base.OnDestroy();
    }

    public void ConfigureOnPlacement(RoomHandler room)
    {
      IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
      for (int y = 0; y < 2; ++y)
        GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(0, y)].cellVisualData.containsObjectSpaceStamp = true;
    }
  }

