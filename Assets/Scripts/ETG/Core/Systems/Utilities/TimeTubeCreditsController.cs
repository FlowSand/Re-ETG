// Decompiled with JetBrains decompiler
// Type: TimeTubeCreditsController
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
    public class TimeTubeCreditsController
    {
      public static bool IsTimeTubing;
      protected Material m_decayMaterial;
      public static GameObject PreAcquiredTunnelInstance;
      public static GameObject PreAcquiredPastDiorama;
      [NonSerialized]
      public bool ForceNoTimefallForCoop;
      private bool m_shouldTimefall;
      private float m_timefallJitterMultiplier = 1f;

      protected Vector4 GetCenterPointInScreenUV(Vector2 centerPoint, float dIntensity, float dRadius)
      {
        Vector3 viewportPoint = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
        return new Vector4(viewportPoint.x, viewportPoint.y, dRadius, dIntensity);
      }

      public void Cleanup()
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          GameManager.Instance.AllPlayers[index].ClearInputOverride("time tube");
        Pixelator.Instance.AdditionalCoreStackRenderPass = (Material) null;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_decayMaterial);
      }

      public void CleanupLich()
      {
        Pixelator.Instance.AdditionalCoreStackRenderPass = (Material) null;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_decayMaterial);
      }

      public static void AcquireTunnelInstanceInAdvance()
      {
        TimeTubeCreditsController.PreAcquiredTunnelInstance = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Global Prefabs/ModTunnel_02"), new Vector3(-100f, -700f, 0.0f), Quaternion.identity);
        TimeTubeCreditsController.PreAcquiredTunnelInstance.SetActive(false);
      }

      public static void AcquirePastDioramaInAdvance()
      {
        GameObject original = BraveResources.Load(!GameManager.IsGunslingerPast ? "GungeonPastDiorama" : "GungeonTruePastDiorama") as GameObject;
        TimeTubeCreditsController.PreAcquiredPastDiorama = UnityEngine.Object.Instantiate<GameObject>(original, original.transform.position, Quaternion.identity).GetComponent<TitleDioramaController>().gameObject;
        TimeTubeCreditsController.PreAcquiredPastDiorama.SetActive(false);
      }

      public static void ClearPerLevelData()
      {
        TimeTubeCreditsController.PreAcquiredTunnelInstance = (GameObject) null;
        TimeTubeCreditsController.PreAcquiredPastDiorama = (GameObject) null;
      }

      public void ClearDebris()
      {
        for (int index = 0; index < StaticReferenceManager.AllDebris.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) StaticReferenceManager.AllDebris[index] && GameManager.Instance.MainCameraController.PointIsVisible(StaticReferenceManager.AllDebris[index].transform.position.XY()))
            StaticReferenceManager.AllDebris[index].TriggerDestruction();
        }
      }

      [DebuggerHidden]
      public IEnumerator HandleTimeTubeLightFX()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TimeTubeCreditsController__HandleTimeTubeLightFXc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      public IEnumerator HandleTimeTubeCredits(
        Vector2 decayCenter,
        bool skipCredits,
        tk2dSpriteAnimator optionalAnimatorToDisable,
        int shotPlayerID,
        bool quickEndShatter = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TimeTubeCreditsController__HandleTimeTubeCreditsc__Iterator1()
        {
          decayCenter = decayCenter,
          shotPlayerID = shotPlayerID,
          skipCredits = skipCredits,
          optionalAnimatorToDisable = optionalAnimatorToDisable,
          quickEndShatter = quickEndShatter,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleTimefallCorpse(
        PlayerController sourcePlayer,
        bool isShotPlayer,
        Camera TunnelCamera,
        Transform TunnelTransform)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TimeTubeCreditsController__HandleTimefallCorpsec__Iterator2()
        {
          sourcePlayer = sourcePlayer,
          isShotPlayer = isShotPlayer,
          TunnelCamera = TunnelCamera,
          TunnelTransform = TunnelTransform,
          _this = this
        };
      }
    }

}
