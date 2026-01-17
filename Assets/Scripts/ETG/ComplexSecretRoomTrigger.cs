// Decompiled with JetBrains decompiler
// Type: ComplexSecretRoomTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ComplexSecretRoomTrigger : BraveBehaviour, IPlayerInteractable
{
  public string introStringKey;
  [PickupIdentifier]
  public List<int> RequiredObjectIds;
  public List<string> RequiredItemSupplyKeys;
  public Transform speakPoint;
  private List<bool> SuppliedObjects = new List<bool>();
  public SecretRoomManager referencedSecretRoom;
  public RoomHandler parentRoom;
  private bool m_isInteracting;

  public void Initialize(RoomHandler room)
  {
    this.parentRoom = room;
    this.parentRoom.RegisterInteractable((IPlayerInteractable) this);
    List<PickupObject> requiredObjects = new List<PickupObject>();
    for (int index = 0; index < this.RequiredObjectIds.Count; ++index)
    {
      this.SuppliedObjects.Add(false);
      requiredObjects.Add(PickupObjectDatabase.GetById(this.RequiredObjectIds[index]));
    }
    GameManager.Instance.Dungeon.data.DistributeComplexSecretPuzzleItems(requiredObjects, this.parentRoom);
  }

  public float GetDistanceToPoint(Vector2 point)
  {
    return Vector2.Distance(point, this.sprite.WorldCenter);
  }

  public float GetOverrideMaxDistance() => -1f;

  public void OnEnteredRange(PlayerController interactor)
  {
    if (this.referencedSecretRoom.IsOpen || !(bool) (Object) this)
      return;
    SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
    this.sprite.UpdateZDepth();
  }

  public void OnExitRange(PlayerController interactor)
  {
    if (!(bool) (Object) this)
      return;
    SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
  }

  public void Interact(PlayerController interactor)
  {
    if (this.m_isInteracting)
      return;
    this.m_isInteracting = true;
    this.StartCoroutine(this.HandleDialog(interactor));
  }

  [DebuggerHidden]
  private IEnumerator HandleDialog(PlayerController player)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ComplexSecretRoomTrigger.\u003CHandleDialog\u003Ec__Iterator0()
    {
      player = player,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator WaitForPlayerYesNo(PlayerController player, int i, int j)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ComplexSecretRoomTrigger.\u003CWaitForPlayerYesNo\u003Ec__Iterator1()
    {
      player = player,
      i = i,
      j = j,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator WaitForPlayer()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ComplexSecretRoomTrigger.\u003CWaitForPlayer\u003Ec__Iterator2 playerCIterator2 = new ComplexSecretRoomTrigger.\u003CWaitForPlayer\u003Ec__Iterator2();
    return (IEnumerator) playerCIterator2;
  }

  protected void AttemptSupplyObjects(PlayerController player)
  {
  }

  public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
  {
    shouldBeFlipped = false;
    return string.Empty;
  }

  protected override void OnDestroy() => base.OnDestroy();
}
