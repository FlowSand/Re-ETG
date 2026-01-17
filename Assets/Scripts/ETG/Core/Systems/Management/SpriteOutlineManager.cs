// Decompiled with JetBrains decompiler
// Type: SpriteOutlineManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public static class SpriteOutlineManager
    {
      private static string[] m_outlineNames = new string[4]
      {
        "OutlineSprite0",
        "OutlineSprite1",
        "OutlineSprite2",
        "OutlineSprite3"
      };
      private static int m_atlasDataID = -1;

      public static void AddSingleOutlineToSprite(
        tk2dBaseSprite targetSprite,
        IntVector2 pixelOffset,
        Color outlineColor)
      {
        SpriteOutlineManager.AddSingleOutlineToSprite<tk2dSprite>(targetSprite, pixelOffset, outlineColor, 0.4f);
      }

      public static void AddOutlineToSprite(tk2dBaseSprite targetSprite, Color outlineColor)
      {
        SpriteOutlineManager.AddOutlineToSprite(targetSprite, outlineColor, 0.4f);
      }

      public static void AddOutlineToSprite<T>(tk2dBaseSprite targetSprite, Color outlineColor) where T : tk2dBaseSprite
      {
        SpriteOutlineManager.AddOutlineToSprite<T>(targetSprite, outlineColor, 0.4f);
      }

      public static void AddOutlineToSprite<T>(
        tk2dBaseSprite targetSprite,
        Color outlineColor,
        Material overrideOutlineMaterial)
        where T : tk2dBaseSprite
      {
        SpriteOutlineManager.AddOutlineToSprite<T>(targetSprite, outlineColor, 0.4f, overrideOutlineMaterial: overrideOutlineMaterial);
      }

      public static bool HasOutline(tk2dBaseSprite targetSprite)
      {
        foreach (tk2dBaseSprite componentsInChild in targetSprite.GetComponentsInChildren<tk2dBaseSprite>(true))
        {
          if (!((UnityEngine.Object) componentsInChild.transform.parent != (UnityEngine.Object) targetSprite.transform) && componentsInChild.IsOutlineSprite)
            return true;
        }
        return false;
      }

      public static Material GetOutlineMaterial(tk2dBaseSprite targetSprite)
      {
        if ((UnityEngine.Object) targetSprite == (UnityEngine.Object) null)
          return (Material) null;
        Transform transform = targetSprite.transform.Find("BraveOutlineSprite");
        if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
          return transform.GetComponent<tk2dBaseSprite>().renderer.sharedMaterial;
        foreach (tk2dBaseSprite componentsInChild in targetSprite.GetComponentsInChildren<tk2dBaseSprite>(true))
        {
          if (!((UnityEngine.Object) componentsInChild.transform.parent != (UnityEngine.Object) targetSprite.transform) && componentsInChild.IsOutlineSprite)
            return componentsInChild.renderer.sharedMaterial;
        }
        return (Material) null;
      }

      public static tk2dSprite[] GetOutlineSprites(tk2dBaseSprite targetSprite)
      {
        return SpriteOutlineManager.GetOutlineSprites<tk2dSprite>(targetSprite);
      }

      public static int ChangeOutlineLayer(tk2dBaseSprite targetSprite, int targetLayer)
      {
        tk2dSprite[] outlineSprites = SpriteOutlineManager.GetOutlineSprites(targetSprite);
        int num = -1;
        if (outlineSprites != null)
        {
          for (int index = 0; index < outlineSprites.Length; ++index)
          {
            if ((bool) (UnityEngine.Object) outlineSprites[index])
            {
              num = outlineSprites[index].gameObject.layer;
              outlineSprites[index].gameObject.layer = targetLayer;
            }
          }
        }
        return num;
      }

      public static T[] GetOutlineSprites<T>(tk2dBaseSprite targetSprite) where T : tk2dBaseSprite
      {
        if ((UnityEngine.Object) targetSprite == (UnityEngine.Object) null)
          return (T[]) null;
        Transform transform = targetSprite.transform.Find("BraveOutlineSprite");
        if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
          return new T[1]{ transform.GetComponent<T>() };
        T[] componentsInChildren = targetSprite.GetComponentsInChildren<T>(true);
        T[] outlineSprites = new T[4];
        int index1 = 0;
        for (int index2 = 0; index2 < componentsInChildren.Length; ++index2)
        {
          T obj = componentsInChildren[index2];
          if (!((UnityEngine.Object) obj.transform.parent != (UnityEngine.Object) targetSprite.transform) && obj.IsOutlineSprite)
          {
            outlineSprites[index1] = obj;
            ++index1;
          }
        }
        return outlineSprites;
      }

      public static void UpdateSingleOutlineSprite(
        tk2dBaseSprite targetSprite,
        IntVector2 newPixelOffset)
      {
        Transform transform = targetSprite.transform.Find("OutlineSprite0");
        if (!((UnityEngine.Object) transform != (UnityEngine.Object) null))
          return;
        transform.localPosition = newPixelOffset.ToVector3() * (1f / 16f);
      }

      public static void AddSingleOutlineToSprite<T>(
        tk2dBaseSprite targetSprite,
        IntVector2 pixelOffset,
        Color outlineColor,
        float zOffset,
        float luminanceCutoff = 0.0f)
        where T : tk2dBaseSprite
      {
        SpriteOutlineManager.HandleSingleOutlineAddition<T>(targetSprite, (Material) null, pixelOffset, 0, outlineColor, zOffset, luminanceCutoff);
      }

      private static void HandleInitialLayer(tk2dBaseSprite sourceSprite, GameObject outlineObject)
      {
        int num = sourceSprite.gameObject.layer;
        if (num == 22)
          num = 21;
        outlineObject.layer = num;
      }

      private static void HandleBraveOutlineAddition<T>(
        tk2dBaseSprite targetSprite,
        Color outlineColor,
        float zOffset,
        float luminanceCutoff = 0.0f)
        where T : tk2dBaseSprite
      {
        Transform transform1 = targetSprite.transform;
        GameObject outlineObject = new GameObject("BraveOutlineSprite");
        Transform transform2 = outlineObject.transform;
        transform2.parent = transform1;
        transform2.localPosition = Vector3.zero;
        transform2.localRotation = Quaternion.identity;
        if (targetSprite.ignoresTiltworldDepth)
          transform2.localPosition = transform2.localPosition.WithZ(1f);
        T attachment = outlineObject.AddComponent<T>();
        attachment.IsOutlineSprite = true;
        attachment.IsBraveOutlineSprite = true;
        attachment.usesOverrideMaterial = true;
        attachment.depthUsesTrimmedBounds = targetSprite.depthUsesTrimmedBounds;
        attachment.SetSprite(targetSprite.Collection, targetSprite.spriteId);
        attachment.ignoresTiltworldDepth = targetSprite.ignoresTiltworldDepth;
        SpriteOutlineManager.HandleInitialLayer(targetSprite, outlineObject);
        attachment.scale = targetSprite.scale;
        Material material = new Material(ShaderCache.Acquire("Brave/Internal/SinglePassOutline"));
        material.mainTexture = targetSprite.renderer.sharedMaterial.mainTexture;
        material.SetColor("_OverrideColor", outlineColor);
        material.SetFloat("_LuminanceCutoff", luminanceCutoff);
        material.DisableKeyword("OUTLINE_OFF");
        material.EnableKeyword("OUTLINE_ON");
        attachment.renderer.material = material;
        attachment.HeightOffGround = -zOffset;
        targetSprite.AttachRenderer((tk2dBaseSprite) attachment);
        targetSprite.UpdateZDepth();
        SpriteOutlineManager.HandleSpriteChanged(targetSprite);
      }

      private static Material HandleSingleOutlineAddition<T>(
        tk2dBaseSprite targetSprite,
        Material sharedMaterialToUse,
        IntVector2 pixelOffset,
        int outlineIndex,
        Color outlineColor,
        float zOffset,
        float luminanceCutoff = 0.0f)
        where T : tk2dBaseSprite
      {
        Transform transform1 = targetSprite.transform;
        Vector3 a = pixelOffset.ToVector3() * (1f / 16f);
        GameObject outlineObject = new GameObject(SpriteOutlineManager.m_outlineNames[outlineIndex]);
        Transform transform2 = outlineObject.transform;
        transform2.parent = transform1;
        transform2.localPosition = Vector3.Scale(a, targetSprite.scale);
        transform2.localRotation = Quaternion.identity;
        if (targetSprite.ignoresTiltworldDepth)
          transform2.localPosition = transform2.localPosition.WithZ(1f);
        T attachment = outlineObject.AddComponent<T>();
        attachment.IsOutlineSprite = true;
        attachment.usesOverrideMaterial = true;
        attachment.depthUsesTrimmedBounds = targetSprite.depthUsesTrimmedBounds;
        attachment.SetSprite(targetSprite.Collection, targetSprite.spriteId);
        attachment.ignoresTiltworldDepth = targetSprite.ignoresTiltworldDepth;
        SpriteOutlineManager.HandleInitialLayer(targetSprite, outlineObject);
        attachment.scale = targetSprite.scale;
        Material material = sharedMaterialToUse;
        if ((UnityEngine.Object) material == (UnityEngine.Object) null)
        {
          material = new Material(ShaderCache.Acquire("tk2d/SpriteOutlineCutout"));
          material.mainTexture = targetSprite.renderer.sharedMaterial.mainTexture;
          material.SetColor("_OverrideColor", outlineColor);
          material.SetFloat("_LuminanceCutoff", luminanceCutoff);
        }
        attachment.renderer.material = material;
        attachment.HeightOffGround = -zOffset;
        targetSprite.AttachRenderer((tk2dBaseSprite) attachment);
        targetSprite.UpdateZDepth();
        return material;
      }

      private static Material HandleSingleScaledOutlineAddition<T>(
        tk2dBaseSprite targetSprite,
        Material sharedMaterialToUse,
        IntVector2 pixelOffset,
        int outlineIndex,
        Color outlineColor,
        float zOffset,
        float luminanceCutoff = 0.0f)
        where T : tk2dBaseSprite
      {
        Transform transform1 = targetSprite.transform;
        Vector3 a = pixelOffset.ToVector3() * (1f / 16f);
        bool flipX = targetSprite.FlipX;
        bool flipY = targetSprite.FlipY;
        Vector3 vector3 = Vector3.Scale(new Vector3(!flipX ? 1f : -1f, !flipY ? 1f : -1f, 1f), targetSprite.scale);
        GameObject outlineObject = new GameObject(SpriteOutlineManager.m_outlineNames[outlineIndex]);
        Transform transform2 = outlineObject.transform;
        transform2.parent = transform1;
        transform2.localPosition = Vector3.Scale(a, targetSprite.scale);
        transform2.localRotation = Quaternion.identity;
        transform2.localScale = Vector3.one;
        if (targetSprite.ignoresTiltworldDepth)
          transform2.localPosition = transform2.localPosition.WithZ(1f);
        T attachment = outlineObject.AddComponent<T>();
        attachment.IsOutlineSprite = true;
        attachment.usesOverrideMaterial = true;
        attachment.depthUsesTrimmedBounds = targetSprite.depthUsesTrimmedBounds;
        attachment.SetSprite(targetSprite.Collection, targetSprite.spriteId);
        attachment.ignoresTiltworldDepth = targetSprite.ignoresTiltworldDepth;
        SpriteOutlineManager.HandleInitialLayer(targetSprite, outlineObject);
        attachment.scale = vector3;
        Material material = sharedMaterialToUse;
        if ((UnityEngine.Object) material == (UnityEngine.Object) null)
        {
          material = new Material(ShaderCache.Acquire("tk2d/SpriteOutlineCutout"));
          material.mainTexture = targetSprite.renderer.sharedMaterial.mainTexture;
          material.SetColor("_OverrideColor", outlineColor);
          material.SetFloat("_LuminanceCutoff", luminanceCutoff);
        }
        attachment.renderer.material = material;
        attachment.HeightOffGround = -zOffset;
        targetSprite.AttachRenderer((tk2dBaseSprite) attachment);
        targetSprite.UpdateZDepth();
        return material;
      }

      public static void ForceRebuildMaterial(
        tk2dBaseSprite outlineSprite,
        tk2dBaseSprite sourceSprite,
        Color c,
        float luminanceCutoff = 0.0f)
      {
        Material material = (Material) null;
        if ((UnityEngine.Object) material == (UnityEngine.Object) null)
        {
          material = new Material(ShaderCache.Acquire("tk2d/SpriteOutlineCutout"));
          material.mainTexture = sourceSprite.renderer.sharedMaterial.mainTexture;
          material.SetColor("_OverrideColor", c);
          material.SetFloat("_LuminanceCutoff", luminanceCutoff);
        }
        outlineSprite.renderer.material = material;
      }

      public static void ForceUpdateOutlineMaterial(
        tk2dBaseSprite outlineSprite,
        tk2dBaseSprite sourceSprite)
      {
        if (!(bool) (UnityEngine.Object) sourceSprite || !(bool) (UnityEngine.Object) outlineSprite)
          return;
        Material sharedMaterial = outlineSprite.renderer.sharedMaterial;
        sharedMaterial.mainTexture = sourceSprite.renderer.sharedMaterial.mainTexture;
        outlineSprite.renderer.sharedMaterial = sharedMaterial;
      }

      public static void AddScaledOutlineToSprite<T>(
        tk2dBaseSprite targetSprite,
        Color outlineColor,
        float zOffset,
        float luminanceCutoff)
        where T : tk2dBaseSprite
      {
        if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null && GameManager.Instance.Dungeon.debugSettings.DISABLE_OUTLINES)
          return;
        if (SpriteOutlineManager.HasOutline(targetSprite))
          SpriteOutlineManager.RemoveOutlineFromSprite(targetSprite, true);
        IntVector2[] cardinals = IntVector2.Cardinals;
        Material sharedMaterialToUse = (Material) null;
        for (int outlineIndex = 0; outlineIndex < 4; ++outlineIndex)
          sharedMaterialToUse = SpriteOutlineManager.HandleSingleScaledOutlineAddition<T>(targetSprite, sharedMaterialToUse, cardinals[outlineIndex], outlineIndex, outlineColor, zOffset, luminanceCutoff);
        tk2dBaseSprite tk2dBaseSprite = targetSprite;
        // ISSUE: reference to a compiler-generated field
        if (SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache0 = new Action<tk2dBaseSprite>(SpriteOutlineManager.HandleSpriteChanged);
        }
        // ISSUE: reference to a compiler-generated field
        Action<tk2dBaseSprite> fMgCache0 = SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache0;
        tk2dBaseSprite.SpriteChanged += fMgCache0;
        targetSprite.UpdateZDepth();
        SpriteOutlineManager.HandleSpriteChanged(targetSprite);
      }

      public static void AddOutlineToSprite(
        tk2dBaseSprite targetSprite,
        Color outlineColor,
        float zOffset,
        float luminanceCutoff = 0.0f,
        SpriteOutlineManager.OutlineType outlineType = SpriteOutlineManager.OutlineType.NORMAL)
      {
        if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null && GameManager.Instance.Dungeon.debugSettings.DISABLE_OUTLINES)
          return;
        if (SpriteOutlineManager.HasOutline(targetSprite))
          SpriteOutlineManager.RemoveOutlineFromSprite(targetSprite, true);
        switch (outlineType)
        {
          case SpriteOutlineManager.OutlineType.NORMAL:
            SpriteOutlineManager.HandleBraveOutlineAddition<tk2dSprite>(targetSprite, outlineColor, zOffset, luminanceCutoff);
            break;
          case SpriteOutlineManager.OutlineType.EEVEE:
            IntVector2[] cardinals = IntVector2.Cardinals;
            Material sharedMaterialToUse = (Material) null;
            for (int outlineIndex = 0; outlineIndex < 4; ++outlineIndex)
              sharedMaterialToUse = SpriteOutlineManager.HandleSingleOutlineAddition<tk2dSprite>(targetSprite, sharedMaterialToUse, cardinals[outlineIndex], outlineIndex, outlineColor, zOffset, luminanceCutoff);
            sharedMaterialToUse.shader = Shader.Find("Brave/PlayerShaderEevee");
            sharedMaterialToUse.SetTexture("_EeveeTex", (Texture) targetSprite.transform.parent.GetComponent<CharacterAnimationRandomizer>().CosmicTex);
            break;
        }
        tk2dBaseSprite tk2dBaseSprite = targetSprite;
        // ISSUE: reference to a compiler-generated field
        if (SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache1 = new Action<tk2dBaseSprite>(SpriteOutlineManager.HandleSpriteChanged);
        }
        // ISSUE: reference to a compiler-generated field
        Action<tk2dBaseSprite> fMgCache1 = SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache1;
        tk2dBaseSprite.SpriteChanged += fMgCache1;
        targetSprite.UpdateZDepth();
        SpriteOutlineManager.HandleSpriteChanged(targetSprite);
        if (targetSprite.renderer.enabled)
          return;
        SpriteOutlineManager.ToggleOutlineRenderers(targetSprite, false);
      }

      public static void AddOutlineToSprite<T>(
        tk2dBaseSprite targetSprite,
        Color outlineColor,
        float zOffset,
        float luminanceCutoff = 0.0f,
        Material overrideOutlineMaterial = null)
        where T : tk2dBaseSprite
      {
        if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null && GameManager.Instance.Dungeon.debugSettings.DISABLE_OUTLINES)
          return;
        if (SpriteOutlineManager.HasOutline(targetSprite))
          SpriteOutlineManager.RemoveOutlineFromSprite(targetSprite, true);
        IntVector2[] cardinals = IntVector2.Cardinals;
        Material sharedMaterialToUse = overrideOutlineMaterial;
        for (int outlineIndex = 0; outlineIndex < 4; ++outlineIndex)
          sharedMaterialToUse = SpriteOutlineManager.HandleSingleOutlineAddition<T>(targetSprite, sharedMaterialToUse, cardinals[outlineIndex], outlineIndex, outlineColor, zOffset, luminanceCutoff);
        tk2dBaseSprite tk2dBaseSprite = targetSprite;
        // ISSUE: reference to a compiler-generated field
        if (SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache2 = new Action<tk2dBaseSprite>(SpriteOutlineManager.HandleSpriteChanged);
        }
        // ISSUE: reference to a compiler-generated field
        Action<tk2dBaseSprite> fMgCache2 = SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache2;
        tk2dBaseSprite.SpriteChanged += fMgCache2;
        targetSprite.UpdateZDepth();
        SpriteOutlineManager.HandleSpriteChanged(targetSprite);
      }

      public static void HandleSpriteChanged(tk2dBaseSprite targetSprite)
      {
        if (SpriteOutlineManager.m_atlasDataID == -1)
          SpriteOutlineManager.m_atlasDataID = Shader.PropertyToID("_AtlasData");
        Transform transform1 = targetSprite.transform;
        Vector3 scale = targetSprite.scale;
        bool flag = false;
        for (int index = 0; index < transform1.childCount; ++index)
        {
          Transform child = transform1.GetChild(index);
          tk2dBaseSprite component1 = child.GetComponent<tk2dBaseSprite>();
          if ((bool) (UnityEngine.Object) component1 && component1.IsBraveOutlineSprite)
          {
            flag = true;
            tk2dSpriteDefinition currentSpriteDef = targetSprite.GetCurrentSpriteDef();
            Vector4 vector4 = new Vector4(1f, 1f, 0.0f, 0.0f);
            if (currentSpriteDef.flipped == tk2dSpriteDefinition.FlipMode.Tk2d)
              vector4 = new Vector4(vector4.x, vector4.y, 1f, 1f);
            vector4.x *= targetSprite.scale.x;
            vector4.y *= targetSprite.scale.y;
            tk2dBaseSprite component2 = child.GetComponent<tk2dBaseSprite>();
            component2.scale = scale;
            component2.SetSprite(targetSprite.Collection, targetSprite.spriteId);
            component2.renderer.material.SetVector(SpriteOutlineManager.m_atlasDataID, vector4);
          }
        }
        if (flag)
          return;
        for (int index = 0; index < 4; ++index)
        {
          Transform transform2 = transform1.Find(SpriteOutlineManager.m_outlineNames[index]);
          if ((UnityEngine.Object) transform2 != (UnityEngine.Object) null)
          {
            tk2dBaseSprite component = transform2.GetComponent<tk2dBaseSprite>();
            component.scale = scale;
            component.SetSprite(targetSprite.Collection, targetSprite.spriteId);
          }
        }
      }

      public static void ToggleOutlineRenderers(tk2dBaseSprite targetSprite, bool value)
      {
        foreach (tk2dBaseSprite componentsInChild in targetSprite.GetComponentsInChildren<tk2dBaseSprite>(true))
        {
          if (componentsInChild.IsOutlineSprite)
            componentsInChild.renderer.enabled = value;
        }
      }

      public static void RemoveOutlineFromSprite(tk2dBaseSprite targetSprite, bool deparent = false)
      {
        Transform transform1 = targetSprite.transform;
        tk2dBaseSprite tk2dBaseSprite = targetSprite;
        // ISSUE: reference to a compiler-generated field
        if (SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache3 = new Action<tk2dBaseSprite>(SpriteOutlineManager.HandleSpriteChanged);
        }
        // ISSUE: reference to a compiler-generated field
        Action<tk2dBaseSprite> fMgCache3 = SpriteOutlineManager.\u003C\u003Ef__mg\u0024cache3;
        tk2dBaseSprite.SpriteChanged -= fMgCache3;
        bool flag = false;
        for (int index = 0; index < transform1.childCount; ++index)
        {
          Transform child = transform1.GetChild(index);
          tk2dBaseSprite component = child.GetComponent<tk2dBaseSprite>();
          if ((bool) (UnityEngine.Object) component && component.IsBraveOutlineSprite)
          {
            flag = true;
            if (deparent)
              child.parent = (Transform) null;
            UnityEngine.Object.Destroy((UnityEngine.Object) child.gameObject);
          }
        }
        if (flag)
          return;
        for (int index = 0; index < 4; ++index)
        {
          Transform transform2 = transform1.Find(SpriteOutlineManager.m_outlineNames[index]);
          if (!((UnityEngine.Object) transform2 != (UnityEngine.Object) null))
            break;
          if (deparent)
            transform2.parent = (Transform) null;
          UnityEngine.Object.Destroy((UnityEngine.Object) transform2.gameObject);
        }
      }

      public enum OutlineType
      {
        NORMAL,
        EEVEE,
      }
    }

}
