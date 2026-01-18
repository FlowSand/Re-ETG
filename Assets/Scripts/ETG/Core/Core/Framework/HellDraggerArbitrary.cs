// Decompiled with JetBrains decompiler
// Type: HellDraggerArbitrary
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class HellDraggerArbitrary : BraveBehaviour
  {
    public GameObject HellDragVFX;

    [DebuggerHidden]
    private IEnumerator HandleGrabbyGrab(PlayerController grabbedPlayer)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HellDraggerArbitrary__HandleGrabbyGrabc__Iterator0()
      {
        grabbedPlayer = grabbedPlayer
      };
    }

    private void GrabPlayer(PlayerController enteredPlayer)
    {
      GameObject gameObject = enteredPlayer.PlayEffectOnActor(this.HellDragVFX, new Vector3(0.0f, -0.25f, 0.0f), false);
      gameObject.transform.position = new Vector3((float) Mathf.RoundToInt(gameObject.transform.position.x + 3f / 16f) - 3f / 16f, (float) Mathf.RoundToInt(gameObject.transform.position.y - 0.375f) + 0.375f, gameObject.transform.position.z);
      this.StartCoroutine(this.HandleGrabbyGrab(enteredPlayer));
    }

    public void Do(PlayerController enteredPlayer)
    {
      if (!(bool) (Object) enteredPlayer)
        return;
      this.GrabPlayer(enteredPlayer);
    }
  }

