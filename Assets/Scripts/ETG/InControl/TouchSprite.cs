using System;

using UnityEngine;

#nullable disable
namespace InControl
{
    [Serializable]
    public class TouchSprite
    {
        [SerializeField]
        private Sprite idleSprite;
        [SerializeField]
        private Sprite busySprite;
        [SerializeField]
        private Color idleColor = new Color(1f, 1f, 1f, 0.5f);
        [SerializeField]
        private Color busyColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField]
        private TouchSpriteShape shape;
        [SerializeField]
        private TouchUnitType sizeUnitType;
        [SerializeField]
        private Vector2 size = new Vector2(10f, 10f);
        [SerializeField]
        private bool lockAspectRatio = true;
        [SerializeField]
        [HideInInspector]
        private Vector2 worldSize;
        private Transform spriteParentTransform;
        private GameObject spriteGameObject;
        private SpriteRenderer spriteRenderer;
        private bool state;

        public TouchSprite()
        {
        }

        public TouchSprite(float size) => this.size = Vector2.one * size;

        public bool Dirty { get; set; }

        public bool Ready { get; set; }

        public void Create(string gameObjectName, Transform parentTransform, int sortingOrder)
        {
            this.spriteGameObject = this.CreateSpriteGameObject(gameObjectName, parentTransform);
            this.spriteRenderer = this.CreateSpriteRenderer(this.spriteGameObject, this.idleSprite, sortingOrder);
            this.spriteRenderer.color = this.idleColor;
            this.Ready = true;
        }

        public void Delete()
        {
            this.Ready = false;
            UnityEngine.Object.Destroy((UnityEngine.Object) this.spriteGameObject);
        }

        public void Update() => this.Update(false);

        public void Update(bool forceUpdate)
        {
            if (this.Dirty || forceUpdate)
            {
                if ((UnityEngine.Object) this.spriteRenderer != (UnityEngine.Object) null)
                    this.spriteRenderer.sprite = !this.State ? this.idleSprite : this.busySprite;
                if (this.sizeUnitType == TouchUnitType.Pixels)
                {
                    Vector2 size = TouchUtility.RoundVector(this.size);
                    this.ScaleSpriteInPixels(this.spriteGameObject, this.spriteRenderer, size);
                    this.worldSize = size * TouchManager.PixelToWorld;
                }
                else
                {
                    this.ScaleSpriteInPercent(this.spriteGameObject, this.spriteRenderer, this.size);
                    this.worldSize = !this.lockAspectRatio ? Vector2.Scale(this.size, (Vector2) TouchManager.ViewSize) : this.size * TouchManager.PercentToWorld;
                }
                this.Dirty = false;
            }
            if (!((UnityEngine.Object) this.spriteRenderer != (UnityEngine.Object) null))
                return;
            Color color1 = !this.State ? this.idleColor : this.busyColor;
            if (!(this.spriteRenderer.color != color1))
                return;
            this.spriteRenderer.color = Utility.MoveColorTowards(this.spriteRenderer.color, color1, 5f * Time.deltaTime);
        }

        private GameObject CreateSpriteGameObject(string name, Transform parentTransform)
        {
            return new GameObject(name)
            {
                transform = {
                    parent = parentTransform,
                    localPosition = Vector3.zero,
                    localScale = Vector3.one
                },
                layer = parentTransform.gameObject.layer
            };
        }

        private SpriteRenderer CreateSpriteRenderer(
            GameObject spriteGameObject,
            Sprite sprite,
            int sortingOrder)
        {
            SpriteRenderer spriteRenderer = spriteGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            spriteRenderer.sharedMaterial.SetFloat("PixelSnap", 1f);
            return spriteRenderer;
        }

        private void ScaleSpriteInPixels(
            GameObject spriteGameObject,
            SpriteRenderer spriteRenderer,
            Vector2 size)
        {
            if ((UnityEngine.Object) spriteGameObject == (UnityEngine.Object) null || (UnityEngine.Object) spriteRenderer == (UnityEngine.Object) null || (UnityEngine.Object) spriteRenderer.sprite == (UnityEngine.Object) null)
                return;
            float num = TouchManager.PixelToWorld * (spriteRenderer.sprite.rect.width / spriteRenderer.sprite.bounds.size.x);
            float x = num * size.x / spriteRenderer.sprite.rect.width;
            float y = num * size.y / spriteRenderer.sprite.rect.height;
            spriteGameObject.transform.localScale = new Vector3(x, y);
        }

        private void ScaleSpriteInPercent(
            GameObject spriteGameObject,
            SpriteRenderer spriteRenderer,
            Vector2 size)
        {
            if ((UnityEngine.Object) spriteGameObject == (UnityEngine.Object) null || (UnityEngine.Object) spriteRenderer == (UnityEngine.Object) null || (UnityEngine.Object) spriteRenderer.sprite == (UnityEngine.Object) null)
                return;
            if (this.lockAspectRatio)
            {
                float num = Mathf.Min(TouchManager.ViewSize.x, TouchManager.ViewSize.y);
                float x = num * size.x / spriteRenderer.sprite.bounds.size.x;
                float y = num * size.y / spriteRenderer.sprite.bounds.size.y;
                spriteGameObject.transform.localScale = new Vector3(x, y);
            }
            else
            {
                float x = TouchManager.ViewSize.x * size.x / spriteRenderer.sprite.bounds.size.x;
                float y = TouchManager.ViewSize.y * size.y / spriteRenderer.sprite.bounds.size.y;
                spriteGameObject.transform.localScale = new Vector3(x, y);
            }
        }

        public bool Contains(Vector2 testWorldPoint)
        {
            if (this.shape == TouchSpriteShape.Oval)
            {
                float num1 = (testWorldPoint.x - this.Position.x) / this.worldSize.x;
                float num2 = (testWorldPoint.y - this.Position.y) / this.worldSize.y;
                return (double) num1 * (double) num1 + (double) num2 * (double) num2 < 0.25;
            }
            float num3 = Utility.Abs(testWorldPoint.x - this.Position.x) * 2f;
            float num4 = Utility.Abs(testWorldPoint.y - this.Position.y) * 2f;
            return (double) num3 <= (double) this.worldSize.x && (double) num4 <= (double) this.worldSize.y;
        }

        public bool Contains(Touch touch)
        {
            return this.Contains((Vector2) TouchManager.ScreenToWorldPoint(touch.position));
        }

        public void DrawGizmos(Vector3 position, Color color)
        {
            if (this.shape == TouchSpriteShape.Oval)
                Utility.DrawOvalGizmo((Vector2) position, this.WorldSize, color);
            else
                Utility.DrawRectGizmo((Vector2) position, this.WorldSize, color);
        }

        public bool State
        {
            get => this.state;
            set
            {
                if (this.state == value)
                    return;
                this.state = value;
                this.Dirty = true;
            }
        }

        public Sprite BusySprite
        {
            get => this.busySprite;
            set
            {
                if (!((UnityEngine.Object) this.busySprite != (UnityEngine.Object) value))
                    return;
                this.busySprite = value;
                this.Dirty = true;
            }
        }

        public Sprite IdleSprite
        {
            get => this.idleSprite;
            set
            {
                if (!((UnityEngine.Object) this.idleSprite != (UnityEngine.Object) value))
                    return;
                this.idleSprite = value;
                this.Dirty = true;
            }
        }

        public Sprite Sprite
        {
            set
            {
                if ((UnityEngine.Object) this.idleSprite != (UnityEngine.Object) value)
                {
                    this.idleSprite = value;
                    this.Dirty = true;
                }
                if (!((UnityEngine.Object) this.busySprite != (UnityEngine.Object) value))
                    return;
                this.busySprite = value;
                this.Dirty = true;
            }
        }

        public Color BusyColor
        {
            get => this.busyColor;
            set
            {
                if (!(this.busyColor != value))
                    return;
                this.busyColor = value;
                this.Dirty = true;
            }
        }

        public Color IdleColor
        {
            get => this.idleColor;
            set
            {
                if (!(this.idleColor != value))
                    return;
                this.idleColor = value;
                this.Dirty = true;
            }
        }

        public TouchSpriteShape Shape
        {
            get => this.shape;
            set
            {
                if (this.shape == value)
                    return;
                this.shape = value;
                this.Dirty = true;
            }
        }

        public TouchUnitType SizeUnitType
        {
            get => this.sizeUnitType;
            set
            {
                if (this.sizeUnitType == value)
                    return;
                this.sizeUnitType = value;
                this.Dirty = true;
            }
        }

        public Vector2 Size
        {
            get => this.size;
            set
            {
                if (!(this.size != value))
                    return;
                this.size = value;
                this.Dirty = true;
            }
        }

        public Vector2 WorldSize => this.worldSize;

        public Vector3 Position
        {
            get
            {
                return (bool) (UnityEngine.Object) this.spriteGameObject ? this.spriteGameObject.transform.position : Vector3.zero;
            }
            set
            {
                if (!(bool) (UnityEngine.Object) this.spriteGameObject)
                    return;
                this.spriteGameObject.transform.position = value;
            }
        }
    }
}
