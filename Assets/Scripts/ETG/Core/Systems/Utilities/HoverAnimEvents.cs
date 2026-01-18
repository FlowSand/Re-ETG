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

