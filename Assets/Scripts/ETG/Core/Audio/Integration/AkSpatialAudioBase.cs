using UnityEngine;

#nullable disable

  public abstract class AkSpatialAudioBase : MonoBehaviour
  {
    private readonly AkRoom.PriorityList roomPriorityList = new AkRoom.PriorityList();

    protected void SetGameObjectInHighestPriorityRoom()
    {
      int num = (int) AkSoundEngine.SetGameObjectInRoom(this.gameObject, this.roomPriorityList.GetHighestPriorityRoomID());
    }

    public void EnteredRoom(AkRoom room)
    {
      this.roomPriorityList.Add(room);
      this.SetGameObjectInHighestPriorityRoom();
    }

    public void ExitedRoom(AkRoom room)
    {
      this.roomPriorityList.Remove(room);
      this.SetGameObjectInHighestPriorityRoom();
    }

    public void SetGameObjectInRoom()
    {
      foreach (Component component1 in Physics.OverlapSphere(this.transform.position, 0.0f))
      {
        AkRoom component2 = component1.gameObject.GetComponent<AkRoom>();
        if ((Object) component2 != (Object) null)
          this.roomPriorityList.Add(component2);
      }
      this.SetGameObjectInHighestPriorityRoom();
    }
  }

