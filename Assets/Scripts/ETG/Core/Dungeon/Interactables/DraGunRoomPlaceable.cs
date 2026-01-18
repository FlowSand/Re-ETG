// Decompiled with JetBrains decompiler
// Type: DraGunRoomPlaceable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class DraGunRoomPlaceable : DungeonPlaceableBehaviour, IPlaceConfigurable
  {
    public static int HallHeight = 18;
    public float roomEmbers = 100f;
    public float pitEmbers = 300f;
    public float nearDeathPitEmbers = 600f;
    public float idlePitEmbers = 100f;
    public float transitioningEmbers = 500f;
    private RoomHandler m_room;
    private DraGunController m_dragunController;
    private MovingPlatform m_deathBridge;
    private Vector2 m_pitMin;
    private Vector2 m_pitMax;
    private Vector2 m_roomMin;
    private Vector2 m_roomMax;
    private Vector2 m_bodyMin;
    private Vector2 m_bodyMax;

    public bool UseInvariantTime { get; set; }

    public bool DraGunKilled { get; set; }

    [DebuggerHidden]
    public IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunRoomPlaceable__Startc__Iterator0()
      {
        _this = this
      };
    }

    public void Update()
    {
      if (!(bool) (Object) this.m_dragunController && !this.DraGunKilled && this.m_room.GetComponentsAbsoluteInRoom<DraGunController>().Count > 0)
        this.m_dragunController = this.m_room.GetComponentsAbsoluteInRoom<DraGunController>()[0];
      if ((bool) (Object) this.m_dragunController && !this.m_dragunController.HasDoneIntro && GameManager.Instance.PrimaryPlayer.CurrentRoom == this.m_room)
        GameManager.Instance.MainCameraController.OverrideZoomScale = Mathf.Lerp(1f, 0.75f, (GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter.y - this.m_roomMin.y) / (float) DraGunRoomPlaceable.HallHeight);
      if (GameManager.Instance.IsLoadingLevel || !GameManager.Instance.IsAnyPlayerInRoom(this.m_room) || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
        return;
      float num1 = !this.UseInvariantTime ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME;
      float num2 = !(bool) (Object) this.m_dragunController || !this.m_dragunController.healthHaver.IsAlive ? this.idlePitEmbers : (!this.m_dragunController.IsNearDeath ? this.pitEmbers : this.nearDeathPitEmbers);
      float num3 = 1f;
      if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM)
        num3 = 0.25f;
      GlobalSparksDoer.DoRandomParticleBurst((int) ((double) num2 * (double) num1 * (double) num3), this.m_pitMin.ToVector3ZUp(100f), this.m_pitMax.ToVector3ZUp(100f), Vector3.up, 90f, 0.5f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
      if ((bool) (Object) this.m_dragunController && this.m_dragunController.healthHaver.IsAlive)
        GlobalSparksDoer.DoRandomParticleBurst((int) ((double) this.roomEmbers * (double) num1 * (double) num3), this.m_roomMin.ToVector3ZisY(), this.m_roomMax.ToVector3ZisY(), Vector3.up, 90f, 0.5f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
      if ((bool) (Object) this.m_dragunController && this.m_dragunController.IsTransitioning)
        GlobalSparksDoer.DoRandomParticleBurst((int) ((double) this.transitioningEmbers * (double) num1 * (double) num3), this.m_bodyMin.ToVector3ZUp(this.m_bodyMin.y - 5f), this.m_bodyMax.ToVector3ZUp(this.m_bodyMin.y - 5f), Vector3.up * 1.5f, 180f, 0.5f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
      if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH || !(bool) (Object) GlobalSparksDoer.EmberParticles)
        return;
      GlobalSparksDoer.EmberParticles.maxParticles = 10000;
    }

    protected override void OnDestroy() => base.OnDestroy();

    public void ExtendDeathBridge()
    {
      this.m_deathBridge.specRigidbody.enabled = true;
      this.m_deathBridge.specRigidbody.Initialize();
      this.m_deathBridge.spriteAnimator.Play();
      this.m_deathBridge.MarkCells();
    }

    public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;

    private void FindPitBounds()
    {
      IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
      this.m_pitMin = intVector2.ToVector2() + new Vector2(0.0f, 14f);
      this.m_pitMax = intVector2.ToVector2() + new Vector2(36f, 29f);
      this.m_roomMin = this.m_room.area.UnitBottomLeft;
      this.m_roomMax = this.m_room.area.UnitTopRight;
      this.m_bodyMin = this.m_pitMin + new Vector2(15f, 0.0f);
      this.m_bodyMax = this.m_pitMin + new Vector2(21f, 15f);
    }
  }

