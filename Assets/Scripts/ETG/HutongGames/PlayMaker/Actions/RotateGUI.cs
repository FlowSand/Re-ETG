// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RotateGUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUI)]
  [HutongGames.PlayMaker.Tooltip("Rotates the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
  public class RotateGUI : FsmStateAction
  {
    [RequiredField]
    public FsmFloat angle;
    [RequiredField]
    public FsmFloat pivotX;
    [RequiredField]
    public FsmFloat pivotY;
    public bool normalized;
    public bool applyGlobally;
    private bool applied;

    public override void Reset()
    {
      this.angle = (FsmFloat) 0.0f;
      this.pivotX = (FsmFloat) 0.5f;
      this.pivotY = (FsmFloat) 0.5f;
      this.normalized = true;
      this.applyGlobally = false;
    }

    public override void OnGUI()
    {
      if (this.applied)
        return;
      Vector2 pivotPoint = new Vector2(this.pivotX.Value, this.pivotY.Value);
      if (this.normalized)
      {
        pivotPoint.x *= (float) Screen.width;
        pivotPoint.y *= (float) Screen.height;
      }
      GUIUtility.RotateAroundPivot(this.angle.Value, pivotPoint);
      if (!this.applyGlobally)
        return;
      PlayMakerGUI.GUIMatrix = GUI.matrix;
      this.applied = true;
    }

    public override void OnUpdate() => this.applied = false;
  }
}
