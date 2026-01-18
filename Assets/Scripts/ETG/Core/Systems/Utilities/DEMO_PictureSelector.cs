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

