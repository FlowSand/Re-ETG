// Decompiled with JetBrains decompiler
// Type: DemoDieListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class DemoDieListener : MonoBehaviour
  {
    private void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
    {
      dfGUIManager.PopModal();
      GameManager.Instance.PrimaryPlayer.healthHaver.Die(Vector2.zero);
      this.transform.parent.gameObject.SetActive(false);
    }
  }

