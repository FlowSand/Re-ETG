using UnityEngine;

#nullable disable

public class NoBlankChecker : BraveBehaviour
  {
    public void Update()
    {
      if (!((Object) GameManager.Instance.BestActivePlayer != (Object) null) || GameManager.Instance.BestActivePlayer.Blanks != 0)
        return;
      GameManager.BroadcastRoomTalkDoerFsmEvent("hasNoBlanks");
    }
  }

