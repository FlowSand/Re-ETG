using tk2dRuntime;
using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/Sprite/tk2dSpriteFromTexture")]
public class tk2dSpriteFromTexture : MonoBehaviour
  {
    public Texture texture;
    public tk2dSpriteCollectionSize spriteCollectionSize = new tk2dSpriteCollectionSize();
    public tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter;
    public string CustomShaderResource;
    private tk2dSpriteCollectionData spriteCollection;
    private tk2dBaseSprite _sprite;

    private tk2dBaseSprite Sprite
    {
      get
      {
        if ((Object) this._sprite == (Object) null)
        {
          this._sprite = this.GetComponent<tk2dBaseSprite>();
          if ((Object) this._sprite == (Object) null)
          {
            Debug.Log((object) "tk2dSpriteFromTexture - Missing sprite object. Creating.");
            this._sprite = (tk2dBaseSprite) this.gameObject.AddComponent<tk2dSprite>();
          }
        }
        return this._sprite;
      }
    }

    private void Awake() => this.Create(this.spriteCollectionSize, this.texture, this.anchor);

    public bool HasSpriteCollection => (Object) this.spriteCollection != (Object) null;

    private void OnDestroy()
    {
      this.DestroyInternal();
      if (!((Object) this.GetComponent<Renderer>() != (Object) null))
        return;
      this.GetComponent<Renderer>().material = (Material) null;
    }

    public void Create(
      tk2dSpriteCollectionSize spriteCollectionSize,
      Texture texture,
      tk2dBaseSprite.Anchor anchor)
    {
      this.DestroyInternal();
      if (!((Object) texture != (Object) null))
        return;
      this.spriteCollectionSize.CopyFrom(spriteCollectionSize);
      this.texture = texture;
      this.anchor = anchor;
      GameObject parentObject = new GameObject("tk2dSpriteFromTexture - " + texture.name);
      parentObject.transform.localPosition = Vector3.zero;
      parentObject.transform.localRotation = Quaternion.identity;
      parentObject.transform.localScale = Vector3.one;
      parentObject.hideFlags = HideFlags.DontSave;
      Vector2 anchorOffset = tk2dSpriteGeomGen.GetAnchorOffset(anchor, (float) texture.width, (float) texture.height);
      this.spriteCollection = SpriteCollectionGenerator.CreateFromTexture(parentObject, texture, spriteCollectionSize, new Vector2((float) texture.width, (float) texture.height), new string[1]
      {
        "unnamed"
      }, new Rect[1]
      {
        new Rect(0.0f, 0.0f, (float) texture.width, (float) texture.height)
      }, (Rect[]) null, new Vector2[1]{ anchorOffset }, new bool[1], this.CustomShaderResource);
      string str = "SpriteFromTexture " + texture.name;
      this.spriteCollection.spriteCollectionName = str;
      this.spriteCollection.spriteDefinitions[0].material.name = str;
      this.spriteCollection.spriteDefinitions[0].material.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
      this.Sprite.SetSprite(this.spriteCollection, 0);
    }

    public void Clear() => this.DestroyInternal();

    public void ForceBuild()
    {
      this.DestroyInternal();
      this.Create(this.spriteCollectionSize, this.texture, this.anchor);
    }

    private void DestroyInternal()
    {
      if (!((Object) this.spriteCollection != (Object) null))
        return;
      if ((Object) this.spriteCollection.spriteDefinitions[0].material != (Object) null)
        Object.DestroyImmediate((Object) this.spriteCollection.spriteDefinitions[0].material);
      Object.DestroyImmediate((Object) this.spriteCollection.gameObject);
      this.spriteCollection = (tk2dSpriteCollectionData) null;
    }
  }

