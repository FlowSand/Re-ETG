// Decompiled with JetBrains decompiler
// Type: SetCharacterColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Examples/Color Picker/Set Character Color")]
    public class SetCharacterColor : MonoBehaviour
    {
      public SkinnedMeshRenderer CharacterRenderer;

      public Color BeltColor
      {
        get => this.CharacterRenderer.material.GetColor("_TeamColor");
        set => this.CharacterRenderer.material.SetColor("_TeamColor", value);
      }
    }

}
