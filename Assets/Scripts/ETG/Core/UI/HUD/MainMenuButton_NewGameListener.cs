// Decompiled with JetBrains decompiler
// Type: MainMenuButton_NewGameListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public class MainMenuButton_NewGameListener : MonoBehaviour
    {
      private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
      {
        GameManager.Instance.LoadCharacterSelect();
      }
    }

}
