// Decompiled with JetBrains decompiler
// Type: HoverAnimEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Sprites/Hover Animation Events")]
public class HoverAnimEvents : MonoBehaviour
  {
    public dfSpriteAnimation hoverAnimation;

    public void OnMouseEnter(dfControl control, dfMouseEventArgs mouseEvent)
    {
      this.hoverAnimation.PlayForward();
    }

    public void OnMouseLeave(dfControl control, dfMouseEventArgs mouseEvent)
    {
      this.hoverAnimation.PlayReverse();
    }
  }

