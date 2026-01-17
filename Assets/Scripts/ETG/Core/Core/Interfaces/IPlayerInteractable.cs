// Decompiled with JetBrains decompiler
// Type: IPlayerInteractable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Interfaces
{
    public interface IPlayerInteractable
    {
      float GetDistanceToPoint(Vector2 point);

      void OnEnteredRange(PlayerController interactor);

      void OnExitRange(PlayerController interactor);

      void Interact(PlayerController interactor);

      string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped);

      float GetOverrideMaxDistance();
    }

}
