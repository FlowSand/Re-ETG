// Decompiled with JetBrains decompiler
// Type: DragunCracktonMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class DragunCracktonMap : MonoBehaviour
    {
      [SerializeField]
      public List<Texture> crackSprites;
      private List<string> m_crackSpriteNames;
      private Dictionary<tk2dSpriteCollectionData, Dictionary<int, Texture>> m_cracktonMap = new Dictionary<tk2dSpriteCollectionData, Dictionary<int, Texture>>();

      public void Start()
      {
        this.m_crackSpriteNames = new List<string>(this.crackSprites.Count);
        for (int index = 0; index < this.crackSprites.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.crackSprites[index])
            this.m_crackSpriteNames.Add(this.crackSprites[index].name);
        }
        tk2dSprite[] componentsInChildren = this.GetComponentsInChildren<tk2dSprite>(true);
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          componentsInChildren[index].GenerateUV2 = true;
          componentsInChildren[index].usesOverrideMaterial = true;
          componentsInChildren[index].SpriteChanged += new Action<tk2dBaseSprite>(this.HandleCracktonChanged);
          componentsInChildren[index].ForceBuild();
          this.HandleCracktonChanged((tk2dBaseSprite) componentsInChildren[index]);
        }
      }

      public void ConvertToCrackton()
      {
        this.StartCoroutine(this.HandleAmbient());
        this.StartCoroutine(this.HandleConversion());
      }

      [DebuggerHidden]
      private IEnumerator HandleAmbient()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DragunCracktonMap.<HandleAmbient>c__Iterator0()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleConversion()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DragunCracktonMap.<HandleConversion>c__Iterator1()
        {
          $this = this
        };
      }

      public void PreGold()
      {
        foreach (tk2dSprite componentsInChild in this.GetComponentsInChildren<tk2dSprite>(true))
        {
          if ((bool) (UnityEngine.Object) componentsInChild)
          {
            componentsInChild.renderer.material.SetFloat("_CharAmount", 1f);
            componentsInChild.renderer.material.SetFloat("_CrackAmount", 1f);
          }
        }
      }

      public void ConvertToGold()
      {
        this.StartCoroutine(this.HandleGoldAmbient());
        this.StartCoroutine(this.HandleGoldConversion());
      }

      [DebuggerHidden]
      private IEnumerator HandleGoldAmbient()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DragunCracktonMap.<HandleGoldAmbient>c__Iterator2()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleGoldConversion()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DragunCracktonMap.<HandleGoldConversion>c__Iterator3()
        {
          $this = this
        };
      }

      private void HandleCracktonChanged(tk2dBaseSprite obj)
      {
        tk2dSpriteCollectionData collection = obj.Collection;
        int spriteId = obj.spriteId;
        Dictionary<int, Texture> dictionary;
        if (!this.m_cracktonMap.TryGetValue(collection, out dictionary))
        {
          dictionary = new Dictionary<int, Texture>();
          this.m_cracktonMap.Add(collection, dictionary);
        }
        Texture texture;
        if (dictionary.TryGetValue(spriteId, out texture))
        {
          if (!((UnityEngine.Object) texture != (UnityEngine.Object) null))
            return;
          obj.renderer.material.SetTexture("_CracksTex", texture);
        }
        else
        {
          string name = obj.GetCurrentSpriteDef().name;
          int index1 = this.m_crackSpriteNames.IndexOf(name.Insert(name.Length - 4, "_crackton"));
          if (index1 >= 0)
          {
            dictionary.Add(spriteId, this.crackSprites[index1]);
            obj.renderer.material.SetTexture("_CracksTex", this.crackSprites[index1]);
          }
          else
          {
            int index2 = this.m_crackSpriteNames.IndexOf(name.Substring(0, name.Length - 4) + "_crackton_001");
            if (index2 >= 0)
            {
              dictionary.Add(spriteId, this.crackSprites[index2]);
              obj.renderer.material.SetTexture("_CracksTex", this.crackSprites[index2]);
            }
            else
            {
              if (name.Length > 12)
              {
                int index3 = this.m_crackSpriteNames.IndexOf(obj.GetCurrentSpriteDef().name.Insert(11, "_crack"));
                if (index3 >= 0)
                {
                  dictionary.Add(spriteId, this.crackSprites[index3]);
                  obj.renderer.material.SetTexture("_CracksTex", this.crackSprites[index3]);
                  return;
                }
              }
              dictionary.Add(spriteId, (Texture) null);
            }
          }
        }
      }
    }

}
