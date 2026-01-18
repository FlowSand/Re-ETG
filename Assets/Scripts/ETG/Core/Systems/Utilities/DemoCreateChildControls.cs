// Decompiled with JetBrains decompiler
// Type: DemoCreateChildControls
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Add-Remove Controls/Create Child Control")]
public class DemoCreateChildControls : MonoBehaviour
  {
    public dfScrollPanel target;
    private int colorNum;
    private Color32[] colors = new Color32[4]
    {
      (Color32) Color.white,
      (Color32) Color.red,
      (Color32) Color.green,
      (Color32) Color.black
    };

    public void Start()
    {
      if (!((Object) this.target == (Object) null))
        return;
      this.target = this.GetComponent<dfScrollPanel>();
    }

    public void OnClick()
    {
      for (int index = 0; index < 10; ++index)
      {
        dfButton dfButton = this.target.AddControl<dfButton>();
        dfButton.NormalBackgroundColor = this.colors[this.colorNum % this.colors.Length];
        dfButton.BackgroundSprite = "button-normal";
        dfButton.Text = $"Button {dfButton.ZOrder}";
        dfButton.Anchor = dfAnchorStyle.Left | dfAnchorStyle.Right;
        dfButton.Width = this.target.Width - (float) this.target.ScrollPadding.horizontal;
      }
      ++this.colorNum;
    }

    public void OnDoubleClick() => this.OnClick();
  }

