// Decompiled with JetBrains decompiler
// Type: BrazierController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class BrazierController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
  {
    public DebrisDirectionalAnimationInfo directionalAnimationInfo;
    public GoopDefinition goop;
    [DwarfConfigurable]
    public float goopLength = 6f;
    [DwarfConfigurable]
    public float goopWidth = 2f;
    [DwarfConfigurable]
    public float goopTime = 1f;
    public string BreakAnimName;
    public DebrisDirectionalAnimationInfo directionalBreakAnims;
    private float m_accumParticleCount;
    private bool m_flipped;
    private float m_flipTime = -1f;
    private Vector2 m_cachedFlipVector;

    public float GetDistanceToPoint(Vector2 point)
    {
      Bounds bounds = this.sprite.GetBounds();
      bounds.SetMinMax(bounds.min + this.sprite.transform.position, bounds.max + this.sprite.transform.position);
      float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
      float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
      return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
    }

    public float GetOverrideMaxDistance() => -1f;

    private void Start()
    {
      this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision);
    }

    private void HandlePreCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if (!(bool) (Object) otherRigidbody.gameActor || !(otherRigidbody.gameActor is PlayerController))
        return;
      this.OnPlayerCollision(otherRigidbody.gameActor as PlayerController);
    }

    private void OnPlayerCollision(PlayerController p)
    {
      if (!((Object) p != (Object) null) || !p.IsDodgeRolling && !this.m_flipped || !p.IsDodgeRolling && (double) UnityEngine.Time.realtimeSinceStartup - (double) this.m_flipTime < 0.25)
        return;
      if (this.m_flipped)
        this.spriteAnimator.Play(this.directionalBreakAnims.GetAnimationForVector(this.m_cachedFlipVector));
      else
        this.spriteAnimator.Play(this.BreakAnimName);
      this.sprite.IsPerpendicular = false;
      this.sprite.HeightOffGround = -1.25f;
      this.sprite.UpdateZDepth();
      this.specRigidbody.enabled = false;
      this.transform.position.GetAbsoluteRoom().DeregisterInteractable((IPlayerInteractable) this);
      this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision);
    }

    private void Update()
    {
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || this.m_flipped || !this.specRigidbody.enabled)
        return;
      this.m_accumParticleCount += BraveTime.DeltaTime * 10f;
      if ((double) this.m_accumParticleCount <= 1.0)
        return;
      int num = Mathf.FloorToInt(this.m_accumParticleCount);
      this.m_accumParticleCount -= (float) num;
      GlobalSparksDoer.DoRandomParticleBurst(num, this.specRigidbody.UnitBottomLeft.ToVector3ZisY(), this.specRigidbody.UnitTopRight.ToVector3ZisY(), Vector3.up, 120f, 0.5f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
    }

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (Object) this)
        return;
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (Object) this)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
    }

    public void Interact(PlayerController interactor)
    {
      this.m_flipped = true;
      Vector2 normalized1 = (this.specRigidbody.UnitCenter - interactor.specRigidbody.UnitCenter).normalized;
      GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY()).DeregisterInteractable((IPlayerInteractable) this);
      this.m_cachedFlipVector = normalized1;
      this.spriteAnimator.Play(this.directionalAnimationInfo.GetAnimationForVector(normalized1));
      Vector2 normalized2 = BraveUtility.GetMajorAxis(normalized1).normalized;
      Vector2 vector2 = this.specRigidbody.UnitCenter + normalized2;
      if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW)
        GlobalSparksDoer.DoRandomParticleBurst(Random.Range(25, 40), this.specRigidbody.UnitBottomLeft.ToVector3ZisY(), this.specRigidbody.UnitTopRight.ToVector3ZisY(), Vector3.up, 120f, 0.5f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
      DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goop).TimedAddGoopLine(vector2, vector2 + normalized2 * this.goopLength, this.goopWidth / 2f, this.goopTime);
      this.m_flipTime = UnityEngine.Time.realtimeSinceStartup;
      DeadlyDeadlyGoopManager.IgniteGoopsCircle(vector2, 1.5f);
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      switch (DungeonData.GetCardinalFromVector2(this.specRigidbody.UnitCenter - interactor.specRigidbody.UnitCenter))
      {
        case DungeonData.Direction.NORTH:
          return "tablekick_up";
        case DungeonData.Direction.EAST:
          return "tablekick_right";
        case DungeonData.Direction.SOUTH:
          return "tablekick_down";
        case DungeonData.Direction.WEST:
          shouldBeFlipped = true;
          return "tablekick_right";
        default:
          return "tablekick_right";
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

    public void ConfigureOnPlacement(RoomHandler room)
    {
      this.PlacedPosition = this.transform.position.IntXY(VectorConversions.Floor);
      for (int x = this.PlacedPosition.x; x < this.PlacedPosition.x + 2; ++x)
      {
        for (int y = this.PlacedPosition.y; y < this.PlacedPosition.y + 2; ++y)
          GameManager.Instance.Dungeon.data[x, y].isOccupied = true;
      }
    }
  }

