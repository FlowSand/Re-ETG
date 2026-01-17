// Decompiled with JetBrains decompiler
// Type: TemporaryRadialIgniter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TemporaryRadialIgniter : MonoBehaviour
    {
      public float Radius = 5f;
      public float Lifespan = 5f;
      public GameActorFireEffect igniteEffect;
      private bool m_radialIndicatorActive;
      private HeatIndicatorController m_radialIndicator;
      private Action<AIActor, float> AuraAction;

      private void Start()
      {
        this.HandleRadialIndicator();
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, this.Lifespan);
      }

      private void Update() => this.DoAura();

      protected virtual void DoAura()
      {
        if (this.AuraAction == null)
          this.AuraAction = (Action<AIActor, float>) ((actor, dist) => actor.ApplyEffect((GameActorEffect) this.igniteEffect));
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        if (absoluteRoom == null)
          return;
        if ((bool) (UnityEngine.Object) this.m_radialIndicator)
          this.m_radialIndicator.CurrentRadius = this.Radius;
        absoluteRoom.ApplyActionToNearbyEnemies((Vector2) this.transform.position, this.Radius, this.AuraAction);
      }

      private void OnDestroy() => this.UnhandleRadialIndicator();

      private void HandleRadialIndicator()
      {
        if (this.m_radialIndicatorActive)
          return;
        this.m_radialIndicatorActive = true;
        this.m_radialIndicator = ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), this.transform.position, Quaternion.identity, this.transform)).GetComponent<HeatIndicatorController>();
      }

      private void UnhandleRadialIndicator()
      {
        if (!this.m_radialIndicatorActive)
          return;
        this.m_radialIndicatorActive = false;
        if ((bool) (UnityEngine.Object) this.m_radialIndicator)
          this.m_radialIndicator.EndEffect();
        this.m_radialIndicator = (HeatIndicatorController) null;
      }
    }

}
