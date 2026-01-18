using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Actionbar/Drag Cursor")]
public class ActionbarsDragCursor : MonoBehaviour
    {
        private static dfSprite _sprite;
        private static Vector2 _cursorOffset;

        public void Start()
        {
            ActionbarsDragCursor._sprite = this.GetComponent<dfSprite>();
            ActionbarsDragCursor._sprite.Hide();
            ActionbarsDragCursor._sprite.IsInteractive = false;
            ActionbarsDragCursor._sprite.IsEnabled = false;
        }

        public void Update()
        {
            if (!ActionbarsDragCursor._sprite.IsVisible)
                return;
            ActionbarsDragCursor.setPosition((Vector2) Input.mousePosition);
        }

        public static void Show(dfSprite sprite, Vector2 position, Vector2 offset)
        {
            ActionbarsDragCursor._cursorOffset = offset;
            ActionbarsDragCursor.setPosition(position);
            ActionbarsDragCursor._sprite.Size = sprite.Size;
            ActionbarsDragCursor._sprite.Atlas = sprite.Atlas;
            ActionbarsDragCursor._sprite.SpriteName = sprite.SpriteName;
            ActionbarsDragCursor._sprite.IsVisible = true;
            ActionbarsDragCursor._sprite.BringToFront();
        }

        public static void Hide() => ActionbarsDragCursor._sprite.IsVisible = false;

        private static void setPosition(Vector2 position)
        {
            position = ActionbarsDragCursor._sprite.GetManager().ScreenToGui(position);
            ActionbarsDragCursor._sprite.RelativePosition = (Vector3) (position - ActionbarsDragCursor._cursorOffset);
        }
    }

