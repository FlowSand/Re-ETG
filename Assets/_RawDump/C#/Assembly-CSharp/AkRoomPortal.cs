// Decompiled with JetBrains decompiler
// Type: AkRoomPortal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[DisallowMultipleComponent]
[RequireComponent(typeof (BoxCollider))]
[AddComponentMenu("Wwise/AkRoomPortal")]
public class AkRoomPortal : AkUnityEventHandler
{
  public const int MAX_ROOMS_PER_PORTAL = 2;
  private readonly AkVector extent = new AkVector();
  private readonly AkTransform portalTransform = new AkTransform();
  private ulong backRoomID = AkRoom.INVALID_ROOM_ID;
  public List<int> closePortalTriggerList = new List<int>();
  private ulong frontRoomID = AkRoom.INVALID_ROOM_ID;
  public AkRoom[] rooms = new AkRoom[2];

  public ulong GetID() => (ulong) this.GetInstanceID();

  protected override void Awake()
  {
    BoxCollider component = this.GetComponent<BoxCollider>();
    component.isTrigger = true;
    this.portalTransform.Set(component.bounds.center.x, component.bounds.center.y, component.bounds.center.z, this.transform.forward.x, this.transform.forward.y, this.transform.forward.z, this.transform.up.x, this.transform.up.y, this.transform.up.z);
    this.extent.X = (float) ((double) component.size.x * (double) this.transform.localScale.x / 2.0);
    this.extent.Y = (float) ((double) component.size.y * (double) this.transform.localScale.y / 2.0);
    this.extent.Z = (float) ((double) component.size.z * (double) this.transform.localScale.z / 2.0);
    this.frontRoomID = !((Object) this.rooms[1] == (Object) null) ? this.rooms[1].GetID() : AkRoom.INVALID_ROOM_ID;
    this.backRoomID = !((Object) this.rooms[0] == (Object) null) ? this.rooms[0].GetID() : AkRoom.INVALID_ROOM_ID;
    this.RegisterTriggers(this.closePortalTriggerList, new AkTriggerBase.Trigger(this.ClosePortal));
    base.Awake();
    if (!this.closePortalTriggerList.Contains(1151176110))
      return;
    this.ClosePortal((GameObject) null);
  }

  protected override void Start()
  {
    base.Start();
    if (!this.closePortalTriggerList.Contains(1281810935))
      return;
    this.ClosePortal((GameObject) null);
  }

  public override void HandleEvent(GameObject in_gameObject) => this.Open();

  public void ClosePortal(GameObject in_gameObject) => this.Close();

  protected override void OnDestroy()
  {
    base.OnDestroy();
    this.UnregisterTriggers(this.closePortalTriggerList, new AkTriggerBase.Trigger(this.ClosePortal));
    if (!this.closePortalTriggerList.Contains(-358577003))
      return;
    this.ClosePortal((GameObject) null);
  }

  public void Open() => this.ActivatePortal(true);

  public void Close() => this.ActivatePortal(false);

  private void ActivatePortal(bool active)
  {
    if (!this.enabled)
      return;
    if ((long) this.frontRoomID != (long) this.backRoomID)
    {
      int num = (int) AkSoundEngine.SetRoomPortal(this.GetID(), this.portalTransform, this.extent, active, this.frontRoomID, this.backRoomID);
    }
    else
      Debug.LogError((object) (this.name + " is not placed/oriented correctly"));
  }

  public void FindOverlappingRooms(AkRoom.PriorityList[] roomList)
  {
    BoxCollider component = this.gameObject.GetComponent<BoxCollider>();
    if ((Object) component == (Object) null)
      return;
    Vector3 halfExtents = new Vector3((float) ((double) component.size.x * (double) this.transform.localScale.x / 2.0), (float) ((double) component.size.y * (double) this.transform.localScale.y / 2.0), (float) ((double) component.size.z * (double) this.transform.localScale.z / 4.0));
    this.FillRoomList(Vector3.forward * -0.25f, halfExtents, roomList[0]);
    this.FillRoomList(Vector3.forward * 0.25f, halfExtents, roomList[1]);
  }

  private void FillRoomList(Vector3 center, Vector3 halfExtents, AkRoom.PriorityList list)
  {
    list.rooms.Clear();
    center = this.transform.TransformPoint(center);
    foreach (Component component1 in Physics.OverlapBox(center, halfExtents, this.transform.rotation, -1, QueryTriggerInteraction.Collide))
    {
      AkRoom component2 = component1.gameObject.GetComponent<AkRoom>();
      if ((Object) component2 != (Object) null && !list.Contains(component2))
        list.Add(component2);
    }
  }

  public void SetFrontRoom(AkRoom room)
  {
    this.rooms[1] = room;
    this.frontRoomID = !((Object) this.rooms[1] == (Object) null) ? this.rooms[1].GetID() : AkRoom.INVALID_ROOM_ID;
  }

  public void SetBackRoom(AkRoom room)
  {
    this.rooms[0] = room;
    this.backRoomID = !((Object) this.rooms[0] == (Object) null) ? this.rooms[0].GetID() : AkRoom.INVALID_ROOM_ID;
  }

  public void UpdateOverlappingRooms()
  {
    AkRoom.PriorityList[] roomList = new AkRoom.PriorityList[2]
    {
      new AkRoom.PriorityList(),
      new AkRoom.PriorityList()
    };
    this.FindOverlappingRooms(roomList);
    for (int index = 0; index < 2; ++index)
    {
      if (!roomList[index].Contains(this.rooms[index]))
        this.rooms[index] = roomList[index].GetHighestPriorityRoom();
    }
    this.frontRoomID = !((Object) this.rooms[1] == (Object) null) ? this.rooms[1].GetID() : AkRoom.INVALID_ROOM_ID;
    this.backRoomID = !((Object) this.rooms[0] == (Object) null) ? this.rooms[0].GetID() : AkRoom.INVALID_ROOM_ID;
  }
}
