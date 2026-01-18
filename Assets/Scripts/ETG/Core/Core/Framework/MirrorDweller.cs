using UnityEngine;

#nullable disable

public class MirrorDweller : BraveBehaviour
  {
    public tk2dBaseSprite TargetSprite;
    public PlayerController TargetPlayer;
    public tk2dBaseSprite MirrorSprite;
    public bool UsesOverrideTintColor;
    public Color OverrideTintColor;

    private void Start()
    {
      this.sprite.usesOverrideMaterial = true;
      this.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Effects/StencilMasked");
      this.sprite.renderer.material.SetColor("_TintColor", new Color(0.4f, 0.4f, 0.8f, 0.5f));
      if (!this.UsesOverrideTintColor)
        return;
      this.sprite.renderer.material.SetColor("_TintColor", this.OverrideTintColor);
    }

    private void LateUpdate()
    {
      if ((Object) this.TargetSprite != (Object) null && (bool) (Object) this.MirrorSprite)
      {
        if ((double) Mathf.Abs(this.transform.position.x - this.TargetSprite.transform.position.x) < 5.0)
        {
          this.sprite.renderer.enabled = true;
          this.sprite.SetSprite(this.TargetSprite.Collection, this.TargetSprite.spriteId);
          float num = (this.MirrorSprite.transform.position.y - this.TargetSprite.transform.position.y) / 2f + 0.5f;
          this.transform.position = this.transform.position.WithX(this.TargetSprite.transform.position.x).WithY(this.MirrorSprite.transform.position.y + num).Quantize(1f / 16f);
        }
        else
          this.sprite.renderer.enabled = false;
      }
      else
      {
        if (!((Object) this.TargetPlayer != (Object) null) || !(bool) (Object) this.MirrorSprite)
          return;
        if ((double) Mathf.Abs(this.transform.position.x - this.TargetPlayer.sprite.transform.position.x) < 5.0)
        {
          this.sprite.renderer.enabled = true;
          this.sprite.SetSprite(this.TargetPlayer.sprite.Collection, this.TargetPlayer.GetMirrorSpriteID());
          float num = (this.MirrorSprite.transform.position.y - this.TargetPlayer.transform.position.y) / 2f + 0.5f;
          this.transform.position = this.transform.position.WithX(this.TargetPlayer.transform.position.x).WithY(this.MirrorSprite.transform.position.y + num).Quantize(1f / 16f);
          this.sprite.HeightOffGround = num - 0.5f;
          this.sprite.FlipX = this.TargetPlayer.sprite.FlipX;
          if (!this.sprite.FlipX)
            return;
          this.transform.position += new Vector3(this.TargetPlayer.sprite.GetBounds().size.x, 0.0f, 0.0f);
        }
        else
          this.sprite.renderer.enabled = false;
      }
    }
  }

