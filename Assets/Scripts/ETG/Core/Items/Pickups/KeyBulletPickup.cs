using Dungeonator;
using UnityEngine;

#nullable disable

public class KeyBulletPickup : PickupObject
  {
    public int numberKeyBullets = 1;
    public bool IsRatKey;
    public string overrideBloopSpriteName = string.Empty;
    private bool m_hasBeenPickedUp;
    private SpeculativeRigidbody m_srb;
    public GameObject minimapIcon;
    private RoomHandler m_minimapIconRoom;
    private GameObject m_instanceMinimapIcon;

    private void Start()
    {
      this.m_srb = this.GetComponent<SpeculativeRigidbody>();
      this.m_srb.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnPreCollision);
      if (!((Object) this.minimapIcon != (Object) null) || this.m_hasBeenPickedUp)
        return;
      this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
      this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon);
    }

    private void Update()
    {
      if ((Object) this.spriteAnimator != (Object) null && this.spriteAnimator.DefaultClip != null)
        this.spriteAnimator.SetFrame(Mathf.FloorToInt(UnityEngine.Time.time * this.spriteAnimator.DefaultClip.fps % (float) this.spriteAnimator.DefaultClip.frames.Length));
      if (!this.IsRatKey || GameManager.Instance.IsLoadingLevel || this.m_hasBeenPickedUp || !(bool) (Object) this || GameManager.Instance.IsAnyPlayerInRoom(this.transform.position.GetAbsoluteRoom()))
        return;
      PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
      if (!(bool) (Object) bestActivePlayer || bestActivePlayer.IsGhost || !bestActivePlayer.AcceptingAnyInput)
        return;
      this.m_hasBeenPickedUp = true;
      this.Pickup(bestActivePlayer);
    }

    private void GetRidOfMinimapIcon()
    {
      if (!((Object) this.m_instanceMinimapIcon != (Object) null))
        return;
      Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.m_instanceMinimapIcon);
      this.m_instanceMinimapIcon = (GameObject) null;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!Minimap.HasInstance)
        return;
      this.GetRidOfMinimapIcon();
    }

    private void OnPreCollision(
      SpeculativeRigidbody otherRigidbody,
      SpeculativeRigidbody source,
      CollisionData collisionData)
    {
      if (this.m_hasBeenPickedUp)
        return;
      PlayerController component = otherRigidbody.GetComponent<PlayerController>();
      if (!((Object) component != (Object) null))
        return;
      this.m_hasBeenPickedUp = true;
      this.Pickup(component);
    }

    public override void Pickup(PlayerController player)
    {
      player.HasGottenKeyThisRun = true;
      this.HandleEncounterable(player);
      this.GetRidOfMinimapIcon();
      if ((bool) (Object) this.spriteAnimator)
        this.spriteAnimator.StopAndResetFrame();
      player.BloopItemAboveHead(this.sprite, this.overrideBloopSpriteName);
      player.carriedConsumables.KeyBullets += this.numberKeyBullets;
      if (this.IsRatKey)
        ++player.carriedConsumables.ResourcefulRatKeys;
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_key_pickup_01", this.gameObject);
      Object.Destroy((Object) this.gameObject);
    }
  }

