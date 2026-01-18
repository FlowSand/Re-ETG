using System;
using UnityEngine;

#nullable disable

public class DFMaxScaler : MonoBehaviour
  {
    private dfControl m_control;

    private void Start()
    {
      this.m_control = this.GetComponent<dfControl>();
      this.m_control.ResolutionChangedPostLayout += new Action<dfControl, Vector3, Vector3>(this.m_control_SizeChanged);
      this.m_control_SizeChanged(this.m_control, Vector3.zero, Vector3.zero);
    }

    private void m_control_SizeChanged(dfControl control, Vector3 pre, Vector3 post)
    {
      if (!(control is dfSprite))
        return;
      dfSprite dfSprite = control as dfSprite;
      Vector2 screenSize = control.GetManager().GetScreenSize();
      Vector2 sizeInPixels = dfSprite.SpriteInfo.sizeInPixels;
      float num1 = screenSize.x / screenSize.y;
      float num2 = sizeInPixels.x / sizeInPixels.y;
      dfSprite.Anchor = dfAnchorStyle.None;
      if ((double) num1 > (double) num2)
      {
        Vector2 vector2 = new Vector2(screenSize.y * num2, screenSize.y);
        if (!(dfSprite.Size != vector2))
          return;
        dfSprite.Size = vector2;
        dfSprite.RelativePosition = new Vector3((float) (((double) screenSize.x - (double) vector2.x) / 2.0), 0.0f, dfSprite.RelativePosition.z);
      }
      else
      {
        Vector2 vector2 = new Vector2(screenSize.x, screenSize.x / num2);
        if (!(dfSprite.Size != vector2))
          return;
        dfSprite.Size = vector2;
        dfSprite.RelativePosition = new Vector3(0.0f, (float) (((double) screenSize.y - (double) vector2.y) / 2.0), dfSprite.RelativePosition.z);
      }
    }
  }

