// Decompiled with JetBrains decompiler
// Type: FortuneFavorItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class FortuneFavorItem : PlayerItem
{
  public float pushRadius = 4f;
  public float secondRadius = 6f;
  public float finalRadius = 8f;
  public float pushStrength = 10f;
  public float duration = 5f;
  public GameObject sparkOctantVFX;

  protected override void DoEffect(PlayerController user)
  {
    this.StartCoroutine(this.HandleShield(user));
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_fortune_shield_01", this.gameObject);
  }

  [DebuggerHidden]
  private IEnumerator HandleShield(PlayerController user)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new FortuneFavorItem.\u003CHandleShield\u003Ec__Iterator0()
    {
      user = user,
      \u0024this = this
    };
  }

  protected override void OnDestroy() => base.OnDestroy();
}
