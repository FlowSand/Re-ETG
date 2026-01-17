// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ResetGUIMatrix
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Resets the GUI matrix. Useful if you've rotated or scaled the GUI and now want to reset it.")]
  [ActionCategory(ActionCategory.GUI)]
  public class ResetGUIMatrix : FsmStateAction
  {
    public override void OnGUI()
    {
      Matrix4x4 identity = Matrix4x4.identity;
      GUI.matrix = identity;
      PlayMakerGUI.GUIMatrix = identity;
    }
  }
}
