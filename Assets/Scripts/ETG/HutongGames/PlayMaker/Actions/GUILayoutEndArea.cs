// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutEndArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUILayout)]
  [HutongGames.PlayMaker.Tooltip("Close a GUILayout group started with BeginArea.")]
  public class GUILayoutEndArea : FsmStateAction
  {
    public override void Reset()
    {
    }

    public override void OnGUI() => GUILayout.EndArea();
  }
}
