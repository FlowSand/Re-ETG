using UnityEngine;

#nullable disable

public class CircularMenuStateIcon : MonoBehaviour
    {
        public string OffSprite;
        public string OnSprite;
        public dfSprite sprite;
        public dfRadialMenu menu;

        public void OnEnable()
        {
            if ((Object) this.sprite == (Object) null)
                this.sprite = this.GetComponent<dfSprite>();
            if ((Object) this.menu == (Object) null)
                this.menu = this.GetComponent<dfRadialMenu>();
            this.sprite.SpriteName = !this.menu.IsOpen ? this.OffSprite : this.OnSprite;
            this.menu.MenuOpened += new dfRadialMenu.CircularMenuEventHandler(this.OnMenuOpened);
            this.menu.MenuClosed += new dfRadialMenu.CircularMenuEventHandler(this.OnMenuClosed);
        }

        public void OnMenuOpened(dfRadialMenu menu) => this.sprite.SpriteName = this.OnSprite;

        public void OnMenuClosed(dfRadialMenu menu) => this.sprite.SpriteName = this.OffSprite;
    }

