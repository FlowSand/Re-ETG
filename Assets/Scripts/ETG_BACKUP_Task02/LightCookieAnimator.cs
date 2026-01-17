// Decompiled with JetBrains decompiler
// Type: LightCookieAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LightCookieAnimator : MonoBehaviour
{
  public Texture2D[] frames;
  public float duration = 4f;
  public float initialDelay = 1f;
  public float additionalScreenShakeDelay = 0.1f;
  public bool doScreenShake;
  [ShowInInspectorIf("doScreenShake", false)]
  public ScreenShakeSettings screenShake;
  private float elapsed;
  private Light m_light;
  private bool m_hasTriggeredSS;
  public bool doVFX;
  public GameObject[] vfxs;
  public float[] vfxTimes;

  private void Start()
  {
    this.m_light = this.GetComponent<Light>();
    if (GameManager.Options.LightingQuality == GameOptions.GenericHighMedLowOption.LOW)
      this.m_light.enabled = false;
    this.elapsed = -1f * this.initialDelay;
  }

  private void Update()
  {
    this.elapsed += BraveTime.DeltaTime;
    float num1 = Mathf.SmoothStep(0.0f, 1f, Mathf.Clamp01(this.elapsed / this.duration));
    if ((double) this.elapsed >= (double) this.additionalScreenShakeDelay && this.doScreenShake && !this.m_hasTriggeredSS)
    {
      this.m_hasTriggeredSS = true;
      GameManager.Instance.MainCameraController.DoScreenShake(this.screenShake, new Vector2?());
      int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_moondoor_close_01", GameManager.Instance.PrimaryPlayer.gameObject);
    }
    if (this.doVFX)
    {
      for (int index = 0; index < this.vfxTimes.Length; ++index)
      {
        if ((Object) this.vfxs[index] != (Object) null && !this.vfxs[index].activeSelf && (double) this.elapsed > (double) this.vfxTimes[index])
        {
          this.vfxs[index].SetActive(true);
          this.vfxs[index] = (GameObject) null;
        }
      }
    }
    if ((double) num1 == 1.0)
      Object.Destroy((Object) this.gameObject);
    else
      this.m_light.cookie = (Texture) this.frames[Mathf.FloorToInt((float) this.frames.Length * num1)];
  }
}
