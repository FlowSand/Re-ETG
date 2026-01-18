using Dungeonator;

#nullable disable

public class TrapEnemyConfigurator : BraveBehaviour
  {
    private bool m_isActive;
    private RoomHandler m_parentRoom;

    private void Start()
    {
      this.m_parentRoom = this.transform.position.GetAbsoluteRoom();
      this.m_parentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.Activate);
    }

    private void Activate(PlayerController p)
    {
      if (this.m_isActive)
        return;
      this.m_isActive = true;
      this.behaviorSpeculator.enabled = true;
    }

    private void Update()
    {
      if (!this.m_isActive || GameManager.Instance.IsAnyPlayerInRoom(this.m_parentRoom))
        return;
      this.m_isActive = false;
      this.behaviorSpeculator.enabled = false;
    }
  }

