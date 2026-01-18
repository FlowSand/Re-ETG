// Decompiled with JetBrains decompiler
// Type: dfLoadLevelByName
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/Load Level On Click")]
[Serializable]
public class dfLoadLevelByName : MonoBehaviour
  {
    public string LevelName;

    private void OnClick()
    {
      if (string.IsNullOrEmpty(this.LevelName))
        return;
      SceneManager.LoadScene(this.LevelName);
    }
  }

