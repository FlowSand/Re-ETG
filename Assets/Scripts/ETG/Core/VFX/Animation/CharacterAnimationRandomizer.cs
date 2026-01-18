using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class CharacterAnimationRandomizer : MonoBehaviour
  {
    public List<tk2dSpriteAnimation> AnimationLibraries;
    public Color[] PrimaryColors;
    public Texture2D CosmicTex;
    private PlayerController m_player;
    private tk2dBaseSprite m_sprite;
    private tk2dSpriteAnimator m_animator;
    private Material m_material;
    private int m_shaderID;

    public void Start()
    {
      this.m_player = this.GetComponent<PlayerController>();
      this.m_sprite = this.m_player.sprite;
      this.m_animator = this.m_player.spriteAnimator;
      this.m_material = this.m_sprite.renderer.sharedMaterial;
      this.m_shaderID = Shader.PropertyToID("_EeveeColor");
      this.m_material.SetTexture("_EeveeTex", (Texture) this.CosmicTex);
      this.m_animator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.HandleAnimationCompletedSwap);
    }

    private void HandleAnimationCompletedSwap(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
    {
      if (!this.m_player.IsVisible)
        return;
      int num = UnityEngine.Random.Range(0, this.AnimationLibraries.Count);
      this.m_animator.Library = this.AnimationLibraries[num];
      this.m_material.SetColor(this.m_shaderID, this.PrimaryColors[Mathf.Min(num, this.PrimaryColors.Length - 1)]);
      this.m_material.SetTexture("_EeveeTex", (Texture) this.CosmicTex);
    }

    public void AddOverrideAnimLibrary(tk2dSpriteAnimation library)
    {
      if (this.AnimationLibraries.Contains(library))
        return;
      this.AnimationLibraries.Add(library);
    }

    public void RemoveOverrideAnimLibrary(tk2dSpriteAnimation library)
    {
      if (!this.AnimationLibraries.Contains(library))
        return;
      this.AnimationLibraries.Remove(library);
    }
  }

