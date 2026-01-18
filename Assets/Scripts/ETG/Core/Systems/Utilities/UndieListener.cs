// Decompiled with JetBrains decompiler
// Type: UndieListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class UndieListener : MonoBehaviour
  {
    private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
      dfGUIManager.PopModal();
      Pixelator.Instance.LerpToLetterbox(0.5f, 0.0f);
      GameManager.Instance.PrimaryPlayer.healthHaver.FullHeal();
      this.transform.parent.gameObject.SetActive(false);
      GameManager.Instance.Unpause();
      GameManager.Instance.PrimaryPlayer.ClearDeadFlags();
    }
  }

