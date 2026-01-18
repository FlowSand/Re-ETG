#nullable disable

public class EncounterOnRoomClear : BraveBehaviour
  {
    public void Start() => this.aiActor.ParentRoom.OnEnemiesCleared += new System.Action(this.RoomCleared);

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.ParentRoom != null)
        this.aiActor.ParentRoom.OnEnemiesCleared -= new System.Action(this.RoomCleared);
      base.OnDestroy();
    }

    private void RoomCleared()
    {
      if (!(bool) (UnityEngine.Object) this.encounterTrackable)
        return;
      GameStatsManager.Instance.HandleEncounteredObject(this.encounterTrackable);
    }
  }

