// Decompiled with JetBrains decompiler
// Type: ParadoxPortalController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ParadoxPortalController : DungeonPlaceableBehaviour, IPlayerInteractable
    {
      public Texture2D CosmicTex;
      private bool m_used;

      public float GetDistanceToPoint(Vector2 point)
      {
        return Vector2.Distance(point, this.transform.position.XY()) / 1.5f;
      }

      public float GetOverrideMaxDistance() => -1f;

      public void OnEnteredRange(PlayerController interactor)
      {
      }

      public void OnExitRange(PlayerController interactor)
      {
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public void Interact(PlayerController interactor)
      {
        if (this.m_used || !interactor.IsPrimaryPlayer)
          return;
        this.m_used = true;
        interactor.portalEeveeTex = this.CosmicTex;
        interactor.IsTemporaryEeveeForUnlock = true;
        this.transform.position.GetAbsoluteRoom().DeregisterInteractable((IPlayerInteractable) this);
        this.StartCoroutine(this.HandleDestroy());
      }

      [DebuggerHidden]
      private IEnumerator HandleDestroy()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ParadoxPortalController.\u003CHandleDestroy\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }
    }

}
