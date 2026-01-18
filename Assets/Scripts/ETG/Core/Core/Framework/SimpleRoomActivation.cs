using Dungeonator;
using UnityEngine;

#nullable disable

public class SimpleRoomActivation : BraveBehaviour, IPlaceConfigurable
  {
    public GameObject[] objectsToActivate;
    private bool m_active;

    public void ConfigureOnPlacement(RoomHandler room)
    {
      room.BecameVisible += new RoomHandler.OnBecameVisibleEventHandler(this.Activate);
    }

    protected void Activate(float delay)
    {
      if (this.m_active)
        return;
      this.m_active = true;
      for (int index = 0; index < this.objectsToActivate.Length; ++index)
        this.objectsToActivate[index].SetActive(true);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

