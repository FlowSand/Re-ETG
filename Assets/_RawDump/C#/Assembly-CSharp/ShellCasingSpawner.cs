// Decompiled with JetBrains decompiler
// Type: ShellCasingSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class ShellCasingSpawner : BraveBehaviour
{
  public GameObject shellCasing;
  public bool inheritRotationAsDirection;
  public int shellsToLaunch;
  public float minForce = 1f;
  public float maxForce = 2.5f;
  public float angleVariance = 10f;
  private bool m_shouldSpawn;

  public void Start() => this.m_shouldSpawn = true;

  public void OnSpawned() => this.m_shouldSpawn = true;

  public void Update()
  {
    if (!this.m_shouldSpawn)
      return;
    this.SpawnShells();
    this.m_shouldSpawn = false;
  }

  protected override void OnDestroy() => base.OnDestroy();

  private void SpawnShells()
  {
    if (GameManager.Options.DebrisQuantity == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.DebrisQuantity == GameOptions.GenericHighMedLowOption.VERY_LOW)
      return;
    for (int index = 0; index < this.shellsToLaunch; ++index)
      this.SpawnShellCasingAtPosition(this.transform.position);
  }

  private void SpawnShellCasingAtPosition(Vector3 position)
  {
    if (!((Object) this.shellCasing != (Object) null))
      return;
    float z = !this.inheritRotationAsDirection ? Random.Range(-180f, 180f) : this.transform.eulerAngles.z;
    GameObject gameObject = SpawnManager.SpawnDebris(this.shellCasing, position, Quaternion.Euler(0.0f, 0.0f, z));
    ShellCasing component1 = gameObject.GetComponent<ShellCasing>();
    if ((Object) component1 != (Object) null)
      component1.Trigger();
    DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
    if (!((Object) component2 != (Object) null))
      return;
    Vector3 vector3Zup = BraveMathCollege.DegreesToVector(z + this.angleVariance * Random.Range(-1f, 1f), Mathf.Lerp(this.minForce, this.maxForce, Random.value)).ToVector3ZUp((float) ((double) Random.value * 1.5 + 1.0));
    float y = this.transform.position.y;
    float num = 0.2f;
    float startingHeight = (float) ((double) component2.transform.position.y - (double) y + (double) Random.value * 0.5);
    component2.additionalHeightBoost = num - startingHeight;
    if ((double) z > 25.0 && (double) z < 155.0)
      component2.additionalHeightBoost += -0.25f;
    else
      component2.additionalHeightBoost += 0.25f;
    component2.Trigger(vector3Zup, startingHeight);
  }
}
