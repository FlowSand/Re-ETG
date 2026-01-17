// Decompiled with JetBrains decompiler
// Type: AkEnvironmentPortal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (BoxCollider))]
[AddComponentMenu("Wwise/AkEnvironmentPortal")]
[ExecuteInEditMode]
public class AkEnvironmentPortal : MonoBehaviour
{
  public const int MAX_ENVIRONMENTS_PER_PORTAL = 2;
  public Vector3 axis = new Vector3(1f, 0.0f, 0.0f);
  public AkEnvironment[] environments = new AkEnvironment[2];

  public float GetAuxSendValueForPosition(Vector3 in_position, int index)
  {
    float num1 = Vector3.Dot(Vector3.Scale(this.GetComponent<BoxCollider>().size, this.transform.lossyScale), this.axis);
    Vector3 rhs = Vector3.Normalize(this.transform.rotation * this.axis);
    float num2 = Vector3.Dot(in_position - (this.transform.position - num1 * 0.5f * rhs), rhs);
    return index == 0 ? (float) (((double) num1 - (double) num2) * ((double) num1 - (double) num2) / ((double) num1 * (double) num1)) : (float) ((double) num2 * (double) num2 / ((double) num1 * (double) num1));
  }
}
