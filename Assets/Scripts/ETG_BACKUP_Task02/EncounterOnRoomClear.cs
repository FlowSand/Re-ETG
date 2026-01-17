// Decompiled with JetBrains decompiler
// Type: EncounterOnRoomClear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
