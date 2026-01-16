// Decompiled with JetBrains decompiler
// Type: AkRoom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using AK.Wwise;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[DisallowMultipleComponent]
[RequireComponent(typeof (Collider))]
[AddComponentMenu("Wwise/AkRoom")]
public class AkRoom : MonoBehaviour
{
  public static ulong INVALID_ROOM_ID = ulong.MaxValue;
  private static int RoomCount;
  [Tooltip("Higher number has a higher priority")]
  public int priority;
  public AuxBus reverbAuxBus;
  [Range(0.0f, 1f)]
  public float reverbLevel = 1f;
  [Range(0.0f, 1f)]
  public float wallOcclusion = 1f;

  public static bool IsSpatialAudioEnabled
  {
    get
    {
      return (Object) AkSpatialAudioListener.TheSpatialAudioListener != (Object) null && AkRoom.RoomCount > 0;
    }
  }

  public ulong GetID() => (ulong) this.GetInstanceID();

  private void OnEnable()
  {
    AkRoomParams in_roomParams = new AkRoomParams();
    in_roomParams.Up.X = this.transform.up.x;
    in_roomParams.Up.Y = this.transform.up.y;
    in_roomParams.Up.Z = this.transform.up.z;
    in_roomParams.Front.X = this.transform.forward.x;
    in_roomParams.Front.Y = this.transform.forward.y;
    in_roomParams.Front.Z = this.transform.forward.z;
    in_roomParams.ReverbAuxBus = (uint) this.reverbAuxBus.ID;
    in_roomParams.ReverbLevel = this.reverbLevel;
    in_roomParams.WallOcclusion = this.wallOcclusion;
    ++AkRoom.RoomCount;
    int num = (int) AkSoundEngine.SetRoom(this.GetID(), in_roomParams, this.name);
  }

  private void OnDisable()
  {
    --AkRoom.RoomCount;
    int num = (int) AkSoundEngine.RemoveRoom(this.GetID());
  }

  private void OnTriggerEnter(Collider in_other)
  {
    AkSpatialAudioBase[] componentsInChildren = in_other.GetComponentsInChildren<AkSpatialAudioBase>();
    for (int index = 0; index < componentsInChildren.Length; ++index)
    {
      if (componentsInChildren[index].enabled)
        componentsInChildren[index].EnteredRoom(this);
    }
  }

  private void OnTriggerExit(Collider in_other)
  {
    AkSpatialAudioBase[] componentsInChildren = in_other.GetComponentsInChildren<AkSpatialAudioBase>();
    for (int index = 0; index < componentsInChildren.Length; ++index)
    {
      if (componentsInChildren[index].enabled)
        componentsInChildren[index].ExitedRoom(this);
    }
  }

  public class PriorityList
  {
    private static readonly AkRoom.PriorityList.CompareByPriority s_compareByPriority = new AkRoom.PriorityList.CompareByPriority();
    public List<AkRoom> rooms = new List<AkRoom>();

    public ulong GetHighestPriorityRoomID()
    {
      AkRoom highestPriorityRoom = this.GetHighestPriorityRoom();
      return (Object) highestPriorityRoom == (Object) null ? AkRoom.INVALID_ROOM_ID : highestPriorityRoom.GetID();
    }

    public AkRoom GetHighestPriorityRoom() => this.rooms.Count == 0 ? (AkRoom) null : this.rooms[0];

    public void Add(AkRoom room)
    {
      int num = this.BinarySearch(room);
      if (num >= 0)
        return;
      this.rooms.Insert(~num, room);
    }

    public void Remove(AkRoom room) => this.rooms.Remove(room);

    public bool Contains(AkRoom room) => this.BinarySearch(room) >= 0;

    public int BinarySearch(AkRoom room)
    {
      return this.rooms.BinarySearch(room, (IComparer<AkRoom>) AkRoom.PriorityList.s_compareByPriority);
    }

    private class CompareByPriority : IComparer<AkRoom>
    {
      public virtual int Compare(AkRoom a, AkRoom b)
      {
        int num = a.priority.CompareTo(b.priority);
        return num == 0 && (Object) a != (Object) b ? 1 : -num;
      }
    }
  }
}
