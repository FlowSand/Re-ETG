// Decompiled with JetBrains decompiler
// Type: DEMO_PictureSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class DEMO_PictureSelector : MonoBehaviour
  {
    public dfTextureSprite DisplayImage;
    protected dfTextureSprite myImage;

    public void OnEnable() => this.myImage = this.GetComponent<dfTextureSprite>();

    [DebuggerHidden]
    public IEnumerator OnDoubleTapGesture()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DEMO_PictureSelector__OnDoubleTapGesturec__Iterator0()
      {
        _this = this
      };
    }

    private static Vector2 fitImage(
      float maxWidth,
      float maxHeight,
      float imageWidth,
      float imageHeight)
    {
      float num = Mathf.Min(maxWidth / imageWidth, maxHeight / imageHeight);
      return new Vector2(Mathf.Floor(imageWidth * num), Mathf.Ceil(imageHeight * num));
    }
  }

