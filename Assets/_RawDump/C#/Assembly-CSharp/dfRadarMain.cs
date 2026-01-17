// Decompiled with JetBrains decompiler
// Type: dfRadarMain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Radar/Radar Main")]
public class dfRadarMain : MonoBehaviour
{
  public GameObject target;
  public float maxDetectDistance = 100f;
  public int radarRadius = 100;
  public List<dfControl> markerTypes;
  private List<dfRadarMarker> markers = new List<dfRadarMarker>();
  private dfControl control;

  public void Start()
  {
    this.ensureControlReference();
    for (int index = 0; index < this.markerTypes.Count; ++index)
      this.markerTypes[index].IsVisible = false;
  }

  public void LateUpdate() => this.updateMarkers();

  public void AddMarker(dfRadarMarker item)
  {
    if (string.IsNullOrEmpty(item.markerType))
      return;
    this.ensureControlReference();
    item.marker = this.instantiateMarker(item.markerType);
    if ((UnityEngine.Object) item.marker == (UnityEngine.Object) null)
      return;
    if (!string.IsNullOrEmpty(item.outOfRangeType))
      item.outOfRangeMarker = this.instantiateMarker(item.outOfRangeType);
    this.markers.Add(item);
  }

  private dfControl instantiateMarker(string markerName)
  {
    dfControl original = this.markerTypes.Find((Predicate<dfControl>) (x => x.name == markerName));
    if ((UnityEngine.Object) original == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("Marker type not found: " + markerName));
      return (dfControl) null;
    }
    dfControl child = UnityEngine.Object.Instantiate<dfControl>(original);
    child.hideFlags = HideFlags.DontSave;
    child.IsVisible = true;
    this.control.AddControl(child);
    return child;
  }

  public void RemoveMarker(dfRadarMarker item)
  {
    if (!this.markers.Remove(item))
      return;
    this.ensureControlReference();
    if ((UnityEngine.Object) item.marker != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) item.marker);
    if ((UnityEngine.Object) item.outOfRangeMarker != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) item.outOfRangeMarker);
    this.control.RemoveControl(item.marker);
  }

  private void ensureControlReference()
  {
    this.control = this.GetComponent<dfControl>();
    if ((UnityEngine.Object) this.control == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Host control not found");
      this.enabled = false;
    }
    else
      this.control.Pivot = dfPivotPoint.MiddleCenter;
  }

  private void updateMarkers()
  {
    for (int index = 0; index < this.markers.Count; ++index)
      this.updateMarker(this.markers[index]);
  }

  private void updateMarker(dfRadarMarker item)
  {
    Vector3 position1 = this.target.transform.position;
    Vector3 position2 = item.transform.position;
    float num1 = (float) ((double) Mathf.Atan2(position1.x - position2.x, -(position1.z - position2.z)) * 57.295780181884766 + 90.0) + this.target.transform.eulerAngles.y;
    float num2 = Vector3.Distance(position1, position2);
    if ((double) num2 > (double) this.maxDetectDistance)
    {
      item.marker.IsVisible = false;
      if (!((UnityEngine.Object) item.outOfRangeMarker != (UnityEngine.Object) null))
        return;
      dfControl outOfRangeMarker = item.outOfRangeMarker;
      outOfRangeMarker.IsVisible = true;
      outOfRangeMarker.transform.position = this.control.transform.position;
      outOfRangeMarker.transform.eulerAngles = new Vector3(0.0f, 0.0f, num1 - 90f);
    }
    else
    {
      if ((UnityEngine.Object) item.outOfRangeMarker != (UnityEngine.Object) null)
        item.outOfRangeMarker.IsVisible = false;
      float num3 = num2 * Mathf.Cos(num1 * ((float) Math.PI / 180f));
      float num4 = num2 * Mathf.Sin(num1 * ((float) Math.PI / 180f));
      float num5 = (float) this.radarRadius / this.maxDetectDistance * this.control.PixelsToUnits();
      float x = num3 * num5;
      float y = num4 * num5;
      item.marker.transform.localPosition = new Vector3(x, y, 0.0f);
      item.marker.IsVisible = true;
      item.marker.Pivot = dfPivotPoint.MiddleCenter;
    }
  }
}
