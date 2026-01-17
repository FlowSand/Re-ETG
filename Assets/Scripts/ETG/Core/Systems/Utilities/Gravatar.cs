// Decompiled with JetBrains decompiler
// Type: Gravatar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Examples/Game Menu/Gravatar")]
    [Serializable]
    public class Gravatar : MonoBehaviour
    {
      private static Regex validator = new Regex("^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$", RegexOptions.IgnoreCase);
      public dfWebSprite Sprite;
      [SerializeField]
      protected string email = string.Empty;

      private void OnEnable()
      {
        if (!Gravatar.validator.IsMatch(this.email) || !((UnityEngine.Object) this.Sprite != (UnityEngine.Object) null))
          return;
        this.updateImage();
      }

      public string EmailAddress
      {
        get => this.email;
        set
        {
          if (!(value != this.email))
            return;
          this.email = value;
          this.updateImage();
        }
      }

      private void updateImage()
      {
        if ((UnityEngine.Object) this.Sprite == (UnityEngine.Object) null)
          return;
        if (Gravatar.validator.IsMatch(this.email))
          this.Sprite.URL = $"http://www.gravatar.com/avatar/{this.MD5(this.email.Trim().ToLower())}";
        else
          this.Sprite.Texture = (Texture) this.Sprite.LoadingImage;
      }

      public string MD5(string strToEncrypt)
      {
        byte[] hash = new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(strToEncrypt));
        string empty = string.Empty;
        for (int index = 0; index < hash.Length; ++index)
          empty += Convert.ToString(hash[index], 16 /*0x10*/).PadLeft(2, '0');
        return empty.PadLeft(32 /*0x20*/, '0');
      }
    }

}
