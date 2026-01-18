using UnityEngine;

#nullable disable

public class dfJumpButtonEvents : MonoBehaviour
  {
    public bool isMouseDown;

    public void OnMouseDown(dfControl control, dfMouseEventArgs mouseEvent)
    {
      this.isMouseDown = true;
    }

    public void OnMouseUp(dfControl control, dfMouseEventArgs mouseEvent) => this.isMouseDown = false;
  }

