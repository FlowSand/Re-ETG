using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/Backend/tk2dFont")]
public class tk2dFont : MonoBehaviour
  {
    public TextAsset bmFont;
    public Material material;
    public Texture texture;
    public Texture2D gradientTexture;
    public bool dupeCaps;
    public bool flipTextureY;
    [HideInInspector]
    public bool proxyFont;
    [SerializeField]
    [HideInInspector]
    private bool useTk2dCamera;
    [SerializeField]
    [HideInInspector]
    private int targetHeight = 640;
    [SerializeField]
    [HideInInspector]
    private float targetOrthoSize = 1f;
    public tk2dSpriteCollectionSize sizeDef = tk2dSpriteCollectionSize.Default();
    public int gradientCount = 1;
    public bool manageMaterial;
    [HideInInspector]
    public bool loadable;
    public int charPadX;
    public tk2dFontData data;
    public static int CURRENT_VERSION = 1;
    public int version;

    public void Upgrade()
    {
      if (this.version >= tk2dFont.CURRENT_VERSION)
        return;
      Debug.Log((object) $"Font '{this.name}' - Upgraded from version {this.version.ToString()}");
      if (this.version == 0)
        this.sizeDef.CopyFromLegacy(this.useTk2dCamera, this.targetOrthoSize, (float) this.targetHeight);
      this.version = tk2dFont.CURRENT_VERSION;
    }
  }

