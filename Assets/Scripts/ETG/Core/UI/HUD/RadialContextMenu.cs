// Decompiled with JetBrains decompiler
// Type: RadialContextMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Menus/Radial Context Menu Helper")]
public class RadialContextMenu : MonoBehaviour
  {
    public dfRadialMenu contextMenu;

    public void Start()
    {
      this.contextMenu.MenuClosed += (dfRadialMenu.CircularMenuEventHandler) (menu => menu.host.Hide());
    }

    public void OnMouseDown(dfControl control, dfMouseEventArgs args)
    {
      if (args.Used || args.Buttons != dfMouseButtons.Middle)
        return;
      if (this.contextMenu.IsOpen)
      {
        this.contextMenu.Close();
      }
      else
      {
        args.Use();
        Vector2 hitPosition = control.GetHitPosition(args);
        dfControl host = this.contextMenu.host;
        host.RelativePosition = (Vector3) (hitPosition - host.Size * 0.5f);
        host.BringToFront();
        host.Show();
        host.Focus(true);
        this.contextMenu.Open();
      }
    }
  }

