// Decompiled with JetBrains decompiler
// Type: DarknessChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class DarknessChallengeModifier : ChallengeModifier
{
  public Shader DarknessEffectShader;
  public float FlashlightAngle = 25f;
  private AdditionalBraveLight[] flashlights;
  private int m_valueMinID;
  private RoomHandler m_room;
  private Material m_material;

  private void Start()
  {
    this.m_material = new Material(this.DarknessEffectShader);
    Pixelator.Instance.AdditionalCoreStackRenderPass = this.m_material;
  }

  private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
  {
    Vector3 viewportPoint = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp());
    return new Vector4(viewportPoint.x, viewportPoint.y, 0.0f, 0.0f);
  }

  private void LateUpdate()
  {
    if (!((Object) this.m_material != (Object) null))
      return;
    float facingDirection1 = GameManager.Instance.PrimaryPlayer.FacingDirection;
    if ((double) facingDirection1 > 270.0)
      facingDirection1 -= 360f;
    if ((double) facingDirection1 < -270.0)
      facingDirection1 += 360f;
    this.m_material.SetFloat("_ConeAngle", this.FlashlightAngle);
    Vector4 centerPointInScreenUv = this.GetCenterPointInScreenUV(GameManager.Instance.PrimaryPlayer.CenterPosition) with
    {
      z = facingDirection1
    };
    Vector4 vector4 = centerPointInScreenUv;
    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
    {
      float facingDirection2 = GameManager.Instance.SecondaryPlayer.FacingDirection;
      if ((double) facingDirection2 > 270.0)
        facingDirection2 -= 360f;
      if ((double) facingDirection2 < -270.0)
        facingDirection2 += 360f;
      vector4 = this.GetCenterPointInScreenUV(GameManager.Instance.SecondaryPlayer.CenterPosition) with
      {
        z = facingDirection2
      };
    }
    this.m_material.SetVector("_Player1ScreenPosition", centerPointInScreenUv);
    this.m_material.SetVector("_Player2ScreenPosition", vector4);
  }

  [DebuggerHidden]
  private IEnumerator LerpFlashlight(AdditionalBraveLight abl, bool turnOff)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DarknessChallengeModifier.\u003CLerpFlashlight\u003Ec__Iterator0()
    {
      abl = abl,
      turnOff = turnOff
    };
  }

  private void OnDestroy()
  {
    if (!(bool) (Object) Pixelator.Instance)
      return;
    Pixelator.Instance.AdditionalCoreStackRenderPass = (Material) null;
  }
}
