using UnityEngine;

#nullable disable

public class PlayerHandController : BraveBehaviour
  {
    public bool ForceRenderersOff;
    public Transform attachPoint;
    public float handHeightFromGun = 0.05f;
    protected float OUTLINE_DEPTH = 0.1f;
    protected PlayerController m_ownerPlayer;
    private bool IsPlayerPrimary;
    protected Shader m_cachedShader;
    private bool m_hasAlteredHeight;
    private Vector3 m_cachedStartPosition;
    private tk2dSprite[] outlineSprites;

    public void InitializeWithPlayer(PlayerController p, bool isPrimary)
    {
      this.m_ownerPlayer = p;
      this.IsPlayerPrimary = isPrimary;
    }

    private void Start()
    {
      this.m_cachedStartPosition = this.transform.localPosition;
      this.sprite.HeightOffGround = this.handHeightFromGun;
      DepthLookupManager.ProcessRenderer(this.renderer);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, this.OUTLINE_DEPTH);
      this.m_cachedShader = this.sprite.renderer.material.shader;
    }

    private void ToggleRenderers(bool e)
    {
      if (this.outlineSprites == null || this.outlineSprites.Length == 0)
        this.outlineSprites = SpriteOutlineManager.GetOutlineSprites(this.sprite);
      this.renderer.enabled = e;
      for (int index = 0; index < this.outlineSprites.Length; ++index)
        this.outlineSprites[index].renderer.enabled = e;
    }

    private void LateUpdate()
    {
      if (!(bool) (Object) this.attachPoint || !this.attachPoint.gameObject.activeSelf)
      {
        this.ToggleRenderers(false);
        this.transform.localPosition = this.m_cachedStartPosition;
      }
      else
      {
        this.ToggleRenderers(!this.ForceRenderersOff);
        this.transform.position = BraveUtility.QuantizeVector(this.attachPoint.position, 16f);
      }
      if ((bool) (Object) this.m_ownerPlayer && (bool) (Object) this.m_ownerPlayer.CurrentGun && this.m_ownerPlayer.CurrentGun.OnlyUsesIdleInWeaponBox)
      {
        float num = 0.0f;
        float currentAngle = this.m_ownerPlayer.CurrentGun.CurrentAngle;
        if (this.m_ownerPlayer.CurrentGun.IsFiring)
        {
          if ((double) currentAngle <= 155.0 && (double) currentAngle >= 25.0)
          {
            num = 0.0f;
          }
          else
          {
            this.m_hasAlteredHeight = true;
            num = !this.IsPlayerPrimary ? 1.5f : 0.5f;
          }
        }
        this.sprite.HeightOffGround = this.handHeightFromGun + num;
      }
      else if (this.m_hasAlteredHeight)
      {
        this.sprite.HeightOffGround = this.handHeightFromGun;
        this.m_hasAlteredHeight = false;
      }
      this.sprite.UpdateZDepth();
    }

    public Material SetOverrideShader(Shader overrideShader)
    {
      Debug.Log((object) "overriding hand shader");
      this.sprite.renderer.material.shader = overrideShader;
      return this.sprite.renderer.material;
    }

    public void ClearOverrideShader() => this.sprite.renderer.material.shader = this.m_cachedShader;

    protected override void OnDestroy() => base.OnDestroy();
  }

