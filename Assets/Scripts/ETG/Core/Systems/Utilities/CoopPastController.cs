// Decompiled with JetBrains decompiler
// Type: CoopPastController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class CoopPastController : MonoBehaviour
  {
    private Vector2 startingP1Position;
    private Vector2 startingP2Position;

    private void Start() => this.StartCoroutine(this.HandleIntro());

    private void Update()
    {
      GameManager.Instance.IsFoyer = false;
      if (!GameManager.PVP_ENABLED)
        return;
      if (GameManager.Instance.PrimaryPlayer.healthHaver.IsDead)
      {
        this.HandlePlayerTwoVictory();
        GameManager.PVP_ENABLED = false;
      }
      else
      {
        if (!GameManager.Instance.SecondaryPlayer.healthHaver.IsDead)
          return;
        this.HandlePlayerOneVictory();
        GameManager.PVP_ENABLED = false;
      }
    }

    private void HandlePlayerOneVictory() => this.StartCoroutine(this.HandleOutro(false));

    private void HandlePlayerTwoVictory() => this.StartCoroutine(this.HandleOutro(true));

    [DebuggerHidden]
    private IEnumerator HandleOutro(bool coopPlayerWon)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CoopPastController__HandleOutroc__Iterator0()
      {
        coopPlayerWon = coopPlayerWon,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleIntro()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CoopPastController__HandleIntroc__Iterator1()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    public IEnumerator DoAmbientTalk(
      Transform baseTransform,
      Vector3 offset,
      string stringKey,
      float duration,
      int specificStringIndex = -1,
      int OnlyThisPlayerInput = -1)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CoopPastController__DoAmbientTalkc__Iterator2()
      {
        baseTransform = baseTransform,
        offset = offset,
        duration = duration,
        specificStringIndex = specificStringIndex,
        stringKey = stringKey,
        OnlyThisPlayerInput = OnlyThisPlayerInput
      };
    }
  }

