// Decompiled with JetBrains decompiler
// Type: CharacterSelectButton_Listener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CharacterSelectButton_Listener : MonoBehaviour
{
  public GameObject playerToSelect;

  private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
  {
    GameManager.PlayerPrefabForNewGame = this.playerToSelect;
    GameManager.Instance.LoadNextLevel();
  }
}
