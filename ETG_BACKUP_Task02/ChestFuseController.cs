// Decompiled with JetBrains decompiler
// Type: ChestFuseController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ChestFuseController : MonoBehaviour
{
  public tk2dTiledSprite[] fuseSegmentsInOrderOfAppearance;
  public GameObject sparksVFXPrefab;
  private Transform sparksInstance;
  private float totalLength = -1f;
  private float m_accumParticles;

  private void CalcLength()
  {
    this.totalLength = 0.0f;
    for (int index = 0; index < this.fuseSegmentsInOrderOfAppearance.Length; ++index)
      this.totalLength += this.fuseSegmentsInOrderOfAppearance[index].dimensions.x;
  }

  public Vector2? SetFuseCompletion(float t)
  {
    if ((double) this.totalLength < 0.0)
      this.CalcLength();
    float x = Mathf.Clamp01(1f - t) * this.totalLength;
    Vector2? nullable = new Vector2?();
    for (int index = 0; index < this.fuseSegmentsInOrderOfAppearance.Length && (double) x >= 0.0; ++index)
    {
      if ((double) x > (double) this.fuseSegmentsInOrderOfAppearance[index].dimensions.x)
      {
        x -= this.fuseSegmentsInOrderOfAppearance[index].dimensions.x;
      }
      else
      {
        this.fuseSegmentsInOrderOfAppearance[index].dimensions = this.fuseSegmentsInOrderOfAppearance[index].dimensions.WithX(x);
        this.m_accumParticles += 30f * BraveTime.DeltaTime;
        int num = Mathf.FloorToInt(this.m_accumParticles);
        this.m_accumParticles -= (float) num;
        Vector3 vector = this.fuseSegmentsInOrderOfAppearance[index].transform.position + (Quaternion.Euler(0.0f, 0.0f, this.fuseSegmentsInOrderOfAppearance[index].transform.eulerAngles.z) * this.fuseSegmentsInOrderOfAppearance[index].dimensions.ToVector3ZUp() * (1f / 16f)).XY().ToVector3ZisY();
        nullable = new Vector2?(vector.XY());
        float y = (double) this.fuseSegmentsInOrderOfAppearance[index].transform.eulerAngles.z != 0.0 ? 0.0f : -1f / 16f;
        GlobalSparksDoer.DoRandomParticleBurst(num, vector + new Vector3(-0.125f, y - 0.125f, 0.0f), vector + new Vector3(0.0f, y, 0.0f), Vector3.up, 180f, 0.25f, startColor: new Color?(Color.yellow));
      }
    }
    return nullable;
  }
}
