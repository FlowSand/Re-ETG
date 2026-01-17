// Decompiled with JetBrains decompiler
// Type: SpawnObjectOnReloadPressed
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SpawnObjectOnReloadPressed : MonoBehaviour
    {
      public GameObject SpawnObject;
      public float tossForce;
      public float DelayTime;
      public bool canBounce = true;
      [ShowInInspectorIf("tossForce", 0, false)]
      public bool orphaned = true;
      [ShowInInspectorIf("tossForce", 0, false)]
      public bool preventRotation;
      [CheckAnimation(null)]
      public string AnimToPlay;
      public bool RequiresSynergy;
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public bool RequiresActualReload;
      private Gun m_gun;
      private PlayerController m_playerOwner;
      private bool m_semaphore;

      private void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_gun.OnInitializedWithOwner += new Action<GameActor>(this.OnGunInitialized);
        this.m_gun.OnDropped += new System.Action(this.OnGunDroppedOrDestroyed);
        if (this.RequiresActualReload)
          this.m_gun.OnAutoReload += new Action<PlayerController, Gun>(this.HandleAutoReload);
        this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
        if (!((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null))
          return;
        this.OnGunInitialized(this.m_gun.CurrentOwner);
      }

      private void HandleAutoReload(PlayerController arg1, Gun arg2)
      {
        this.HandleReloadPressed(arg1, arg2, false);
      }

      private void HandleReloadPressed(PlayerController user, Gun sourceGun, bool actual)
      {
        if (this.RequiresSynergy && (!(bool) (UnityEngine.Object) user || !user.HasActiveBonusSynergy(this.RequiredSynergy)) || this.m_semaphore || this.RequiresActualReload && sourceGun.ClipShotsRemaining == sourceGun.ClipCapacity)
          return;
        this.m_semaphore = this.RequiresActualReload;
        if (!this.m_gun.IsFiring || this.RequiresActualReload)
        {
          if (!string.IsNullOrEmpty(this.AnimToPlay))
          {
            if (!sourceGun.spriteAnimator.IsPlaying(this.AnimToPlay))
              user.StartCoroutine(this.DoSpawn(user, 0.0f));
          }
          else
            user.StartCoroutine(this.DoSpawn(user, 0.0f));
        }
        if (!this.m_semaphore)
          return;
        user.StartCoroutine(this.HandleReloadDelay(sourceGun));
      }

      [DebuggerHidden]
      private IEnumerator HandleReloadDelay(Gun sourceGun)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SpawnObjectOnReloadPressed.\u003CHandleReloadDelay\u003Ec__Iterator0()
        {
          sourceGun = sourceGun,
          \u0024this = this
        };
      }

      [DebuggerHidden]
      protected IEnumerator DoSpawn(PlayerController user, float angleFromAim)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SpawnObjectOnReloadPressed.\u003CDoSpawn\u003Ec__Iterator1()
        {
          user = user,
          angleFromAim = angleFromAim,
          \u0024this = this
        };
      }

      private void OnGunInitialized(GameActor obj)
      {
        if ((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null)
          this.OnGunDroppedOrDestroyed();
        if ((UnityEngine.Object) obj == (UnityEngine.Object) null || !(obj is PlayerController))
          return;
        this.m_playerOwner = obj as PlayerController;
      }

      private void OnDestroy() => this.OnGunDroppedOrDestroyed();

      private void OnGunDroppedOrDestroyed()
      {
        if (!((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null))
          return;
        this.m_playerOwner = (PlayerController) null;
      }
    }

}
