// Decompiled with JetBrains decompiler
// Type: ShadowSystem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ShadowSystem : BraveBehaviour
    {
      private static List<ShadowSystem> m_allLights = new List<ShadowSystem>();
      public static bool DisabledLightsRequireBoost = false;
      [NonSerialized]
      public bool IsDirty;
      public bool IsDynamic;
      public float lightRadius = 10f;
      public bool ignoreUnityLight;
      public Color uLightColor;
      public float uLightIntensity;
      public float uLightRange;
      public Texture2D uLightCookie;
      public float uLightCookieAngle;
      public bool ignoreCustomFloorLight;
      [SerializeField]
      private float minLuminance = 0.01f;
      [SerializeField]
      private float shadowBias = 1f / 1000f;
      [SerializeField]
      private Camera shadowCamera;
      [SerializeField]
      private bool highQuality;
      [SerializeField]
      private Shader lightDistanceShader;
      [SerializeField]
      private Shader transparentShader;
      [SerializeField]
      private Shader casterShader;
      [SerializeField]
      private int shadowMapSize = 512 /*0x0200*/;
      [SerializeField]
      public bool CoronalLight;
      [SerializeField]
      public List<Renderer> PersonalCookies = new List<Renderer>();
      private RenderTexture _texTarget;
      private Dictionary<Shader, Material> _shaderMap = new Dictionary<Shader, Material>();
      private List<RenderTexture> _tempRenderTextures = new List<RenderTexture>();
      private bool m_initialized;
      private Vector3 m_cachedPosition;
      private bool m_locallyDisabled;
      private static int m_numberLightsUpdatedThisFrame = 0;

      public static void ForceAllLightsUpdate()
      {
        for (int index = 0; index < ShadowSystem.m_allLights.Count; ++index)
        {
          ShadowSystem.m_allLights[index].IsDirty = true;
          ShadowSystem.m_allLights[index].renderer.enabled = true;
        }
      }

      public static void ForceRoomLightsUpdate(RoomHandler room, float duration)
      {
        for (int index = 0; index < ShadowSystem.m_allLights.Count; ++index)
        {
          if (GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(ShadowSystem.m_allLights[index].transform.position.IntXY(VectorConversions.Floor)) == room)
            ShadowSystem.m_allLights[index].TriggerTemporalUpdate(duration);
        }
      }

      public static void ClearPerLevelData() => ShadowSystem.m_allLights.Clear();

      public static List<ShadowSystem> AllLights => ShadowSystem.m_allLights;

      private int ModifiedShadowMapSize => this.shadowMapSize;

      private Material GetMaterial(Shader shader)
      {
        Material material1;
        if (this._shaderMap.TryGetValue(shader, out material1))
          return material1;
        Material material2 = new Material(shader);
        this._shaderMap.Add(shader, material2);
        return material2;
      }

      private void PreprocessAttachedUnityLight()
      {
        Light componentInChildren = this.transform.parent.GetComponentInChildren<Light>();
        if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
          return;
        this.uLightColor = componentInChildren.color;
        this.uLightIntensity = componentInChildren.intensity;
        this.uLightRange = componentInChildren.range;
        LightPulser component = componentInChildren.GetComponent<LightPulser>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.AssignShadowSystem(this);
        UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren);
      }

      private void Awake()
      {
        this.renderer.enabled = false;
        if (ShadowSystem.m_allLights.Contains(this))
          return;
        ShadowSystem.m_allLights.Add(this);
      }

      private void Start()
      {
        if (!this.ignoreUnityLight)
          this.PreprocessAttachedUnityLight();
        Material material = this.renderer.material;
        SceneLightManager component = this.GetComponent<SceneLightManager>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          Color validColor = component.validColors[UnityEngine.Random.Range(0, component.validColors.Length)];
          material.SetColor("_TintColor", validColor);
        }
        else
          material.SetColor("_TintColor", Color.white);
      }

      private void CleanupLightsForLowLighting()
      {
        ShadowSystem.DisabledLightsRequireBoost = true;
        if ((UnityEngine.Object) this._texTarget != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this._texTarget);
        this.ReleaseAllRenderTextures();
        this.renderer.enabled = false;
        this.transform.parent.gameObject.SetActive(false);
      }

      private void ReturnFromDead()
      {
        ShadowSystem.DisabledLightsRequireBoost = false;
        this.shadowMapSize = Mathf.NextPowerOfTwo(this.shadowMapSize);
        this.shadowMapSize = Mathf.Clamp(this.shadowMapSize, 8, 2048 /*0x0800*/);
        this._texTarget = !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) ? new RenderTexture(this.ModifiedShadowMapSize, this.ModifiedShadowMapSize, 0, RenderTextureFormat.Default) : new RenderTexture(this.ModifiedShadowMapSize, this.ModifiedShadowMapSize, 0, RenderTextureFormat.ARGBHalf);
        this._texTarget.useMipMap = false;
        this._texTarget.autoGenerateMips = false;
        this.shadowCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        this.transform.localScale = Vector3.one * this.shadowCamera.orthographicSize / 5f;
        this.transform.localScale = this.transform.localScale.WithZ(this.transform.localScale.z * 1.414f);
        this.renderer.material.mainTexture = (Texture) this._texTarget;
        this.IsDirty = true;
        this.renderer.enabled = true;
      }

      private void OnEnable()
      {
        if (GameManager.Options.LightingQuality == GameOptions.GenericHighMedLowOption.LOW || this.ignoreCustomFloorLight || this.m_locallyDisabled)
        {
          if (this.ignoreCustomFloorLight)
            this.PreprocessAttachedUnityLight();
          this.CleanupLightsForLowLighting();
        }
        else
          this.ReturnFromDead();
      }

      protected override void OnDestroy()
      {
        if (ShadowSystem.m_allLights != null && ShadowSystem.m_allLights.Contains(this))
          ShadowSystem.m_allLights.Remove(this);
        foreach (KeyValuePair<Shader, Material> shader in this._shaderMap)
          UnityEngine.Object.Destroy((UnityEngine.Object) shader.Value);
        this._shaderMap.Clear();
        if ((UnityEngine.Object) this._texTarget != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) this._texTarget);
        this.ReleaseAllRenderTextures();
      }

      private void TriggerTemporalUpdate(float duration)
      {
        this.StartCoroutine(this.HandleTemporalUpdate(duration));
      }

      [DebuggerHidden]
      private IEnumerator HandleTemporalUpdate(float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShadowSystem.\u003CHandleTemporalUpdate\u003Ec__Iterator0()
        {
          duration = duration,
          \u0024this = this
        };
      }

      private RenderTextureFormat IdealFormat
      {
        get
        {
          return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;
        }
      }

      private bool RequiresCasterDepthBuffer()
      {
        if (StaticReferenceManager.AllShadowSystemDepthHavers.Count > 0)
        {
          Vector2 vector2 = this.transform.PositionVector2();
          float num = this.lightRadius * this.lightRadius;
          for (int index = 0; index < StaticReferenceManager.AllShadowSystemDepthHavers.Count; ++index)
          {
            Transform systemDepthHaver = StaticReferenceManager.AllShadowSystemDepthHavers[index];
            if ((double) (vector2 - systemDepthHaver.PositionVector2()).sqrMagnitude < (double) num)
              return true;
          }
        }
        return false;
      }

      private void RenderFullShadowMap()
      {
        if (GameManager.Options.LightingQuality == GameOptions.GenericHighMedLowOption.LOW || this.ignoreCustomFloorLight)
        {
          if (this.ignoreCustomFloorLight)
            this.PreprocessAttachedUnityLight();
          this.CleanupLightsForLowLighting();
        }
        else
        {
          if (!this.transform.parent.gameObject.activeSelf)
          {
            this.transform.parent.gameObject.SetActive(true);
            ShadowSystem.DisabledLightsRequireBoost = false;
            this.ReturnFromDead();
          }
          for (int index = 0; index < this.PersonalCookies.Count; ++index)
            this.PersonalCookies[index].enabled = true;
          this.transform.position = this.transform.position.WithZ(this.transform.position.y - 2.5f);
          tk2dBaseSprite tk2dBaseSprite = (tk2dBaseSprite) null;
          int layer1 = -1;
          int layer2 = -1;
          if (GameManager.Instance.IsFoyer && (UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null)
          {
            tk2dBaseSprite = GameManager.Instance.PrimaryPlayer.sprite;
            layer1 = tk2dBaseSprite.gameObject.layer;
            tk2dBaseSprite.gameObject.SetLayerRecursively(LayerMask.NameToLayer("PlayerAndProjectiles"));
            if ((UnityEngine.Object) GameManager.Instance.SecondaryPlayer != (UnityEngine.Object) null)
            {
              layer2 = GameManager.Instance.SecondaryPlayer.sprite.gameObject.layer;
              GameManager.Instance.SecondaryPlayer.sprite.gameObject.SetLayerRecursively(LayerMask.NameToLayer("PlayerAndProjectiles"));
            }
          }
          RenderTexture source = this.PushRenderTexture(this.ModifiedShadowMapSize, this.ModifiedShadowMapSize, !this.RequiresCasterDepthBuffer() ? 0 : 16 /*0x10*/, this.IdealFormat);
          source.filterMode = FilterMode.Point;
          source.wrapMode = TextureWrapMode.Clamp;
          this.shadowCamera.targetTexture = source;
          if ((UnityEngine.Object) this.casterShader != (UnityEngine.Object) null)
            this.shadowCamera.RenderWithShader(this.casterShader, string.Empty);
          else
            this.shadowCamera.Render();
          if (GameManager.Instance.IsFoyer && (UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null)
          {
            tk2dBaseSprite.gameObject.SetLayerRecursively(layer1);
            if ((UnityEngine.Object) GameManager.Instance.SecondaryPlayer != (UnityEngine.Object) null)
              GameManager.Instance.SecondaryPlayer.sprite.gameObject.SetLayerRecursively(layer2);
          }
          Material material = this.GetMaterial(this.lightDistanceShader);
          material.SetFloat("_MinLuminance", this.minLuminance);
          material.SetFloat("_ShadowOffset", this.shadowBias);
          material.SetFloat("_Resolution", (float) this.ModifiedShadowMapSize);
          material.SetFloat("_LightRadius", this.lightRadius);
          RenderTexture renderTexture = this.PushRenderTexture(this.ModifiedShadowMapSize, 1, format: this.IdealFormat);
          Graphics.Blit((Texture) source, renderTexture, material, 0);
          Graphics.Blit((Texture) renderTexture, this._texTarget, material, !this.highQuality ? 1 : 2);
          this.ReleaseAllRenderTextures();
          this.m_initialized = true;
          this.m_cachedPosition = this.transform.position;
          for (int index = 0; index < this.PersonalCookies.Count; ++index)
            this.PersonalCookies[index].enabled = false;
          if (this.renderer.enabled)
            return;
          this.renderer.enabled = true;
        }
      }

      private void Update() => ShadowSystem.m_numberLightsUpdatedThisFrame = 0;

      private void LateUpdate()
      {
        if (!this.renderer.isVisible && false)
          return;
        bool flag1 = !this._texTarget.IsCreated();
        bool flag2 = this.renderer.isVisible && this.IsDynamic;
        if (this.m_initialized && !this.IsDirty && !(this.transform.position != this.m_cachedPosition) && !flag2 && !flag1)
          return;
        if (!flag2 && !flag1)
        {
          if (ShadowSystem.m_numberLightsUpdatedThisFrame >= 3)
            return;
          this.IsDirty = false;
          ++ShadowSystem.m_numberLightsUpdatedThisFrame;
          this.RenderFullShadowMap();
        }
        else
        {
          this.IsDirty = false;
          this.RenderFullShadowMap();
        }
      }

      private RenderTexture PushRenderTexture(
        int width,
        int height,
        int depth = 0,
        RenderTextureFormat format = RenderTextureFormat.ARGBHalf)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(width, height, depth, format);
        temporary.filterMode = FilterMode.Point;
        temporary.wrapMode = TextureWrapMode.Clamp;
        this._tempRenderTextures.Add(temporary);
        return temporary;
      }

      private void ReleaseAllRenderTextures()
      {
        if (this._tempRenderTextures == null || this._tempRenderTextures.Count == 0)
          return;
        foreach (RenderTexture tempRenderTexture in this._tempRenderTextures)
          RenderTexture.ReleaseTemporary(tempRenderTexture);
        this._tempRenderTextures.Clear();
      }
    }

}
