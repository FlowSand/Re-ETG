using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

public class BreakableColumn : DungeonPlaceableBehaviour
  {
    [FormerlySerializedAs("damagedAnimation")]
    public string damagedSprite;
    [FormerlySerializedAs("destroyAnimation")]
    public string destroyedSprite;
    [Header("Flake Data")]
    public GameObject flake;
    public VFXPool puff;
    public int flakeCount;
    public float flakeAreaWidth;
    public float flakeAreaHeight;
    public float flakeSpawnDuration;
    [Header("Explosion Data")]
    public ExplosionData explosionData;
    private BreakableColumn.State m_state;

    public void Start()
    {
      this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
    }

    public void Update()
    {
    }

    protected override void OnDestroy() => base.OnDestroy();

    private void OnPreRigidbodyCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if (!(bool) (Object) otherRigidbody.projectile || !otherRigidbody.name.StartsWith("TankTreader_Fast_Projectile") && !otherRigidbody.name.StartsWith("TankTreader_Scatter_Projectile") && !otherRigidbody.name.StartsWith("TankTreader_Spawn_Projectile") && !otherRigidbody.name.StartsWith("TankTreader_Rocket_Projectile"))
        return;
      if (this.m_state == BreakableColumn.State.Default)
      {
        this.sprite.SetSprite(this.damagedSprite);
        this.m_state = BreakableColumn.State.Damaged;
        this.SpawnFlakes();
        if (!PhysicsEngine.PendingCastResult.Overlap)
          return;
      }
      if (this.m_state != BreakableColumn.State.Damaged)
        return;
      PhysicsEngine.SkipCollision = true;
      Exploder.Explode((Vector3) PhysicsEngine.PendingCastResult.Contact, this.explosionData, PhysicsEngine.PendingCastResult.Normal);
      this.sprite.SetSprite(this.destroyedSprite);
      this.specRigidbody.enabled = false;
      this.SetAreaPassable();
      this.sprite.IsPerpendicular = false;
      this.sprite.HeightOffGround = -1.95f;
      this.sprite.UpdateZDepth();
      this.gameObject.layer = LayerMask.NameToLayer("BG_Critical");
      BreakableChunk component = this.GetComponent<BreakableChunk>();
      if ((bool) (Object) component)
        component.Trigger(false, new Vector3?((Vector3) PhysicsEngine.PendingCastResult.Contact));
      this.m_state = BreakableColumn.State.Destroyed;
    }

    private void SpawnFlakes()
    {
      if (this.flakeCount <= 0)
        return;
      for (int index = 0; index < this.flakeCount; ++index)
      {
        if ((double) this.flakeSpawnDuration == 0.0)
          this.SpawnRandomizeFlakes();
        else
          this.Invoke("SpawnRandomizeFlakes", Random.Range(0.0f, this.flakeSpawnDuration));
      }
    }

    private void SpawnRandomizeFlakes()
    {
      Vector3 position = this.transform.position + new Vector3(Random.Range(0.0f, this.flakeAreaWidth), Random.Range(0.0f, this.flakeAreaHeight));
      this.puff.SpawnAtPosition(position, sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero));
      tk2dSprite component = Object.Instantiate<GameObject>(this.flake, position, Quaternion.identity).GetComponent<tk2dSprite>();
      component.HeightOffGround = 0.1f;
      this.sprite.AttachRenderer((tk2dBaseSprite) component);
      component.UpdateZDepth();
    }

    private enum State
    {
      Default,
      Damaged,
      Destroyed,
    }
  }

