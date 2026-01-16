// Decompiled with JetBrains decompiler
// Type: QuitOnClick
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/General/Quit On Click")]
public class QuitOnClick : MonoBehaviour
{
  private void OnClick() => Application.Quit();
}
