// Decompiled with JetBrains decompiler
// Type: BundleOfWandsLightController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BundleOfWandsLightController : MonoBehaviour
{
  public Color baseColor;
  private Light m_light;

  private void Start() => this.m_light = this.GetComponent<Light>();

  private Vector3 shift_col(Vector3 RGB, Vector3 shift)
  {
    Vector3 vector3 = new Vector3(RGB.x, RGB.y, RGB.z);
    float num1 = shift.z * shift.y * Mathf.Cos((float) ((double) shift.x * 3.1415927410125732 / 180.0));
    float num2 = shift.z * shift.y * Mathf.Sin((float) ((double) shift.x * 3.1415927410125732 / 180.0));
    vector3.x = (float) ((0.29899999499320984 * (double) shift.z + 0.70099997520446777 * (double) num1 + 0.1679999977350235 * (double) num2) * (double) RGB.x + (0.58700001239776611 * (double) shift.z - 0.58700001239776611 * (double) num1 + 0.33000001311302185 * (double) num2) * (double) RGB.y + (57.0 / 500.0 * (double) shift.z - 57.0 / 500.0 * (double) num1 - 0.49700000882148743 * (double) num2) * (double) RGB.z);
    vector3.y = (float) ((0.29899999499320984 * (double) shift.z - 0.29899999499320984 * (double) num1 - 0.328000009059906 * (double) num2) * (double) RGB.x + (0.58700001239776611 * (double) shift.z + 0.41299998760223389 * (double) num1 + 0.035000000149011612 * (double) num2) * (double) RGB.y + (57.0 / 500.0 * (double) shift.z - 57.0 / 500.0 * (double) num1 + 0.29199999570846558 * (double) num2) * (double) RGB.z);
    vector3.z = (float) ((0.29899999499320984 * (double) shift.z - 0.30000001192092896 * (double) num1 + 1.25 * (double) num2) * (double) RGB.x + (0.58700001239776611 * (double) shift.z - 0.58799999952316284 * (double) num1 - 1.0499999523162842 * (double) num2) * (double) RGB.y + (57.0 / 500.0 * (double) shift.z + 0.88599997758865356 * (double) num1 - 0.20299999415874481 * (double) num2) * (double) RGB.z);
    return vector3;
  }

  private void Update()
  {
    Vector3 vector3 = this.shift_col(new Vector3(this.baseColor.r, this.baseColor.g, this.baseColor.b), new Vector3((float) (1.5 * (double) UnityEngine.Time.time * 360.0), 1f, 1.5f));
    this.m_light.color = new Color(vector3.x, vector3.y, vector3.z);
  }
}
