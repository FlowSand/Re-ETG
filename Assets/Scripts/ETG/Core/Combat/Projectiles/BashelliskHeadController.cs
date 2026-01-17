// Decompiled with JetBrains decompiler
// Type: BashelliskHeadController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class BashelliskHeadController : BashelliskSegment
    {
      [Header("Head-Specific Data")]
      public BashelliskBodyPickupController pickupPrefab;
      public List<BashelliskBodyController> segmentPrefabs;
      public List<int> segmentCounts;
      public int startingSegments;
      [Header("Spawn Pickup Data")]
      public float initialSpawnDelay = 20f;
      public float minSpawnDelay = 20f;
      public float maxSpawnDelay = 40f;
      public float pickupHealthScaler = 1f;
      public float pickupLurchSpeed = 13f;
      [NonSerialized]
      public LinkedList<BashelliskSegment> Body = new LinkedList<BashelliskSegment>();
      [NonSerialized]
      public readonly PooledLinkedList<BashelliskBodyPickupController> AvailablePickups = new PooledLinkedList<BashelliskBodyPickupController>();
      private readonly PooledLinkedList<Vector2> m_path = new PooledLinkedList<Vector2>();
      private float m_pathSegmentLength;
      private float m_spawnTimer;
      private float m_nextSpawnHealthThreshold = 0.8f;
      private List<Vector2> m_pickupLocations = new List<Vector2>();
      private List<Projectile> m_collidedProjectiles = new List<Projectile>();

      public bool CanPickup { get; set; }

      public bool ReinitMovementDirection { get; set; }

      public bool IsMidPickup { get; set; }

      public void Start()
      {
        this.Body.AddFirst((BashelliskSegment) this);
        this.healthHaver.bodyRigidbodies = new List<SpeculativeRigidbody>();
        this.healthHaver.bodyRigidbodies.Add(this.specRigidbody);
        this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
        this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
        PhysicsEngine.Instance.OnPostRigidbodyMovement += new System.Action(this.OnPostRigidbodyMovement);
        for (int index = 0; index < this.startingSegments; ++index)
          this.Grow();
        this.m_path.AddLast((Vector2) this.center.position);
        this.m_path.AddLast((Vector2) this.center.position);
        this.m_pathSegmentLength = 0.0f;
        this.m_spawnTimer = this.initialSpawnDelay;
        List<BashelliskPickupSpawnPoint> componentsInRoom = this.aiActor.ParentRoom.GetComponentsInRoom<BashelliskPickupSpawnPoint>();
        for (int index = 0; index < componentsInRoom.Count; ++index)
          this.m_pickupLocations.Add((Vector2) (componentsInRoom[index].transform.position + new Vector3(-5f / 16f, 0.0f)));
      }

      public void Update()
      {
        for (LinkedListNode<BashelliskSegment> linkedListNode = this.Body.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
        {
          if ((UnityEngine.Object) linkedListNode.Value != (UnityEngine.Object) null && (bool) (UnityEngine.Object) linkedListNode.Value.specRigidbody)
            linkedListNode.Value.specRigidbody.CollideWithOthers = this.specRigidbody.CollideWithOthers;
        }
        if (this.aiActor.enabled)
        {
          bool flag = false;
          if ((double) this.m_spawnTimer <= 0.0)
            flag = true;
          if ((double) this.healthHaver.GetCurrentHealthPercentage() < (double) this.m_nextSpawnHealthThreshold)
            flag = true;
          if (flag)
          {
            this.SpawnBodyPickup();
            this.m_spawnTimer = UnityEngine.Random.Range(this.minSpawnDelay, this.maxSpawnDelay);
            this.m_nextSpawnHealthThreshold -= 0.2f;
          }
          if (this.AvailablePickups.Count == 0)
            this.m_spawnTimer -= BraveTime.DeltaTime;
        }
        for (int index1 = this.m_collidedProjectiles.Count - 1; index1 >= 0; --index1)
        {
          Projectile collidedProjectile = this.m_collidedProjectiles[index1];
          if (!(bool) (UnityEngine.Object) collidedProjectile || !(bool) (UnityEngine.Object) collidedProjectile.gameObject || !collidedProjectile.gameObject.activeSelf || collidedProjectile.specRigidbody.PrimaryPixelCollider == null)
          {
            this.m_collidedProjectiles.RemoveAt(index1);
          }
          else
          {
            bool flag = false;
            LinkedListNode<BashelliskSegment> linkedListNode = this.Body.First;
            PixelCollider primaryPixelCollider = collidedProjectile.specRigidbody.PrimaryPixelCollider;
            for (; linkedListNode != null && !flag; linkedListNode = linkedListNode.Next)
            {
              if ((UnityEngine.Object) linkedListNode.Value != (UnityEngine.Object) null && (UnityEngine.Object) linkedListNode.Value.specRigidbody != (UnityEngine.Object) null)
              {
                SpeculativeRigidbody specRigidbody = linkedListNode.Value.specRigidbody;
                for (int index2 = 0; index2 < specRigidbody.PixelColliders.Count; ++index2)
                {
                  PixelCollider pixelCollider = specRigidbody.PixelColliders[index2];
                  if (pixelCollider.Enabled && pixelCollider.Overlaps(primaryPixelCollider))
                  {
                    flag = true;
                    break;
                  }
                }
              }
            }
            if (!flag)
              this.m_collidedProjectiles.RemoveAt(index1);
          }
        }
      }

      protected override void OnDestroy()
      {
        if (GameManager.HasInstance && (bool) (UnityEngine.Object) this)
        {
          while (this.Body.Count > 0)
          {
            if ((bool) (UnityEngine.Object) this.Body.First.Value)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.Body.First.Value.gameObject);
            this.Body.RemoveFirst();
          }
          while (this.AvailablePickups.Count > 0)
          {
            if ((bool) (UnityEngine.Object) this.AvailablePickups.First.Value)
              UnityEngine.Object.Destroy((UnityEngine.Object) this.AvailablePickups.First.Value.gameObject);
            this.AvailablePickups.RemoveFirst();
          }
        }
        this.m_path.ClearPool();
        if (PhysicsEngine.HasInstance)
          PhysicsEngine.Instance.OnPostRigidbodyMovement -= new System.Action(this.OnPostRigidbodyMovement);
        base.OnDestroy();
      }

      public void OnPostRigidbodyMovement()
      {
        if (!this.enabled || this.IsMidPickup)
          return;
        this.UpdatePath((Vector2) this.center.position);
        if ((bool) (UnityEngine.Object) this.next)
          this.next.UpdatePosition(this.m_path, this.m_path.First, 0.0f, 0.0f);
        this.aiAnimator.FacingDirection = (this.Body.First.Value.center.position - this.Body.First.Next.Value.center.position).XY().ToAngle();
        if (this.CanPickup)
        {
          PixelCollider hitboxPixelCollider1 = this.specRigidbody.HitboxPixelCollider;
          LinkedListNode<BashelliskBodyPickupController> node1 = this.AvailablePickups.First;
          while (node1 != null)
          {
            if (!(bool) (UnityEngine.Object) node1.Value)
            {
              LinkedListNode<BashelliskBodyPickupController> node2 = node1;
              node1 = node1.Next;
              this.AvailablePickups.Remove(node2, true);
            }
            else
            {
              PixelCollider hitboxPixelCollider2 = node1.Value.specRigidbody.HitboxPixelCollider;
              if (hitboxPixelCollider2 != null && hitboxPixelCollider1.Overlaps(hitboxPixelCollider2))
              {
                this.StartCoroutine(this.PickupCR(node1.Value));
                this.AvailablePickups.Remove(node1, true);
                break;
              }
              node1 = node1.Next;
            }
          }
        }
        else
        {
          if (this.AvailablePickups.Count <= 0)
            return;
          LinkedListNode<BashelliskBodyPickupController> linkedListNode = this.AvailablePickups.First;
          while (linkedListNode != null)
          {
            if (!(bool) (UnityEngine.Object) linkedListNode.Value)
            {
              LinkedListNode<BashelliskBodyPickupController> node = linkedListNode;
              linkedListNode = linkedListNode.Next;
              this.AvailablePickups.Remove(node, true);
            }
            else
              linkedListNode = linkedListNode.Next;
          }
        }
      }

      public void Grow()
      {
        Vector3 position = this.center.position;
        int num = this.Body.Count - 1;
        int b = this.segmentCounts.Count - 1;
        for (int index = this.segmentCounts.Count - 1; index >= 0 && num >= 0; --index)
        {
          num -= this.segmentCounts[index];
          b = index;
        }
        BashelliskBodyController bashelliskBodyController = UnityEngine.Object.Instantiate<BashelliskBodyController>(this.segmentPrefabs[Mathf.Max(0, b)], this.center.position, Quaternion.identity);
        bashelliskBodyController.transform.position = position - bashelliskBodyController.center.localPosition;
        bashelliskBodyController.Init(this);
        if (this.Body.Count > 1)
        {
          bashelliskBodyController.next = this.Body.First.Next.Value;
          this.Body.First.Next.Value.previous = (BashelliskSegment) bashelliskBodyController;
        }
        this.next = (BashelliskSegment) bashelliskBodyController;
        bashelliskBodyController.previous = (BashelliskSegment) this;
        this.Body.AddAfter(this.Body.First, (BashelliskSegment) bashelliskBodyController);
        this.healthHaver.bodyRigidbodies.Add(bashelliskBodyController.specRigidbody);
        bashelliskBodyController.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
        bashelliskBodyController.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
      }

      [DebuggerHidden]
      private IEnumerator PickupCR(BashelliskBodyPickupController pickup)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BashelliskHeadController.<PickupCR>c__Iterator0()
        {
          pickup = pickup,
          _this = this
        };
      }

      private void OnPreRigidbodyCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        Projectile projectile = otherRigidbody.projectile;
        if (!(bool) (UnityEngine.Object) projectile || !this.m_collidedProjectiles.Contains(projectile))
          return;
        PhysicsEngine.SkipCollision = true;
      }

      private void OnRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        Projectile projectile = rigidbodyCollision.OtherRigidbody.projectile;
        if (!(bool) (UnityEngine.Object) projectile)
          return;
        this.m_collidedProjectiles.Add(projectile);
      }

      private void SpawnBodyPickup()
      {
        List<Vector2> list = new List<Vector2>();
        list.AddRange((IEnumerable<Vector2>) this.m_pickupLocations.FindAll((Predicate<Vector2>) (pos =>
        {
          for (LinkedListNode<BashelliskBodyPickupController> linkedListNode = this.AvailablePickups.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
          {
            if ((bool) (UnityEngine.Object) linkedListNode.Value && (double) Vector2.Distance(pos, linkedListNode.Value.center.transform.position.XY()) < 3.0)
              return false;
          }
          return true;
        })));
        Vector2 position;
        if (list.Count > 0)
        {
          position = BraveUtility.RandomElement<Vector2>(list);
        }
        else
        {
          Vector2 worldpoint1 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
          Vector2 worldpoint2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay);
          IntVector2 bottomLeft = worldpoint1.ToIntVector2(VectorConversions.Ceil);
          IntVector2 topRight = worldpoint2.ToIntVector2(VectorConversions.Floor) - IntVector2.One;
          IntVector2? randomAvailableCell = this.aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(new IntVector2(2, 2)), new CellTypes?(CellTypes.FLOOR), cellValidator: (CellValidator) (c =>
          {
            for (int index1 = 0; index1 < 2; ++index1)
            {
              for (int index2 = 0; index2 < 2; ++index2)
              {
                if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
                  return false;
              }
            }
            return c.x >= bottomLeft.x && c.y >= bottomLeft.y && c.x + 1 <= topRight.x && c.y + 1 <= topRight.y;
          }));
          position = !randomAvailableCell.HasValue ? (Vector2) this.transform.position : randomAvailableCell.Value.ToVector2();
        }
        AIActor aiActor = AIActor.Spawn(this.pickupPrefab.aiActor, position, this.aiActor.ParentRoom);
        aiActor.transform.position += new Vector3(1.25f, 0.0f);
        aiActor.specRigidbody.Reinitialize();
        this.AvailablePickups.AddLast(aiActor.GetComponent<BashelliskBodyPickupController>());
        this.specRigidbody.RegisterSpecificCollisionException(aiActor.specRigidbody);
      }

      private void SpawnBodyPickupAtMouse()
      {
        this.AvailablePickups.AddLast(UnityEngine.Object.Instantiate<BashelliskBodyPickupController>(this.pickupPrefab, (Vector3) this.aiActor.ParentRoom.GetBestRewardLocation(new IntVector2(2, 2), (Vector2) BraveUtility.GetMousePosition(), false).ToVector2(), Quaternion.identity));
      }

      private void UpdatePath(Vector2 newPosition)
      {
        float num = Vector2.Distance(newPosition, this.m_path.First.Value);
        for (LinkedListNode<BashelliskSegment> next = this.Body.First.Next; next != null; next = next.Next)
          next.Value.PathDist += num;
        if ((double) this.m_pathSegmentLength > 0.5)
          this.m_path.AddFirst(newPosition);
        else
          this.m_path.First.Value = newPosition;
        this.m_pathSegmentLength = Vector2.Distance(this.m_path.First.Value, this.m_path.First.Next.Value);
      }
    }

}
