// Decompiled with JetBrains decompiler
// Type: ActionbarsHoverEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Actionbar/Hover Events")]
public class ActionbarsHoverEvents : MonoBehaviour
{
  private dfControl actionBar;
  private dfControl lastTarget;
  private dfControl target;
  private bool isTooltipVisible;

  public void Start() => this.actionBar = this.GetComponent<dfControl>();

  public void OnMouseHover(dfControl control, dfMouseEventArgs mouseEvent)
  {
    if (this.isTooltipVisible)
      return;
    if (this.actionBar.Controls.Contains(mouseEvent.Source))
    {
      this.target = mouseEvent.Source;
      if ((Object) this.target == (Object) this.lastTarget)
        return;
      this.lastTarget = this.target;
      this.isTooltipVisible = true;
      SpellSlot componentInChildren = this.target.GetComponentInChildren<SpellSlot>();
      if (string.IsNullOrEmpty(componentInChildren.Spell))
        return;
      SpellDefinition byName = SpellDefinition.FindByName(componentInChildren.Spell);
      if (byName == null)
        return;
      ActionbarsTooltip.Show(byName);
    }
    else
      this.lastTarget = (dfControl) null;
  }

  public void OnMouseDown()
  {
    this.isTooltipVisible = false;
    ActionbarsTooltip.Hide();
    this.target = (dfControl) null;
  }

  public void OnMouseLeave()
  {
    if ((Object) this.target == (Object) null)
      return;
    Vector3 mousePosition = Input.mousePosition;
    mousePosition.y = (float) Screen.height - mousePosition.y;
    if (this.target.GetScreenRect().Contains(mousePosition, true))
      return;
    this.isTooltipVisible = false;
    ActionbarsTooltip.Hide();
    this.target = (dfControl) null;
  }
}
