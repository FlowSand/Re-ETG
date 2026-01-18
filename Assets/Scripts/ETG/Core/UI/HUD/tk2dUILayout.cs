using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUILayout")]
public class tk2dUILayout : MonoBehaviour
  {
    public Vector3 bMin = new Vector3(0.0f, -1f, 0.0f);
    public Vector3 bMax = new Vector3(1f, 0.0f, 0.0f);
    public List<tk2dUILayoutItem> layoutItems = new List<tk2dUILayoutItem>();
    public bool autoResizeCollider;

    public int ItemCount => this.layoutItems.Count;

    public event Action<Vector3, Vector3> OnReshape;

    private void Reset()
    {
      if (!((UnityEngine.Object) this.GetComponent<Collider>() != (UnityEngine.Object) null))
        return;
      BoxCollider component = this.GetComponent<Collider>() as BoxCollider;
      if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
        return;
      Bounds bounds = component.bounds;
      Matrix4x4 worldToLocalMatrix = this.transform.worldToLocalMatrix;
      Vector3 position = this.transform.position;
      this.Reshape(worldToLocalMatrix.MultiplyPoint(bounds.min) - this.bMin, worldToLocalMatrix.MultiplyPoint(bounds.max) - this.bMax, true);
      Vector3 vector3 = worldToLocalMatrix.MultiplyVector(this.transform.position - position);
      Transform transform = this.transform;
      for (int index = 0; index < transform.childCount; ++index)
        transform.GetChild(index).localPosition -= vector3;
      component.center -= vector3;
      this.autoResizeCollider = true;
    }

    public virtual void Reshape(Vector3 dMin, Vector3 dMax, bool updateChildren)
    {
      foreach (tk2dUILayoutItem layoutItem in this.layoutItems)
        layoutItem.oldPos = layoutItem.gameObj.transform.position;
      this.bMin += dMin;
      this.bMax += dMax;
      Vector3 vector = new Vector3(this.bMin.x, this.bMax.y);
      this.transform.position += this.transform.localToWorldMatrix.MultiplyVector(vector);
      this.bMin -= vector;
      this.bMax -= vector;
      if (this.autoResizeCollider)
      {
        BoxCollider component = this.GetComponent<BoxCollider>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          component.center += (dMin + dMax) / 2f - vector;
          component.size += dMax - dMin;
        }
      }
      foreach (tk2dUILayoutItem layoutItem in this.layoutItems)
      {
        Vector3 vector3_1 = this.transform.worldToLocalMatrix.MultiplyVector(layoutItem.gameObj.transform.position - layoutItem.oldPos);
        Vector3 vector3_2 = -vector3_1;
        Vector3 vector3_3 = -vector3_1;
        if (updateChildren)
        {
          vector3_2.x += !layoutItem.snapLeft ? (!layoutItem.snapRight ? 0.0f : dMax.x) : dMin.x;
          vector3_2.y += !layoutItem.snapBottom ? (!layoutItem.snapTop ? 0.0f : dMax.y) : dMin.y;
          vector3_3.x += !layoutItem.snapRight ? (!layoutItem.snapLeft ? 0.0f : dMin.x) : dMax.x;
          vector3_3.y += !layoutItem.snapTop ? (!layoutItem.snapBottom ? 0.0f : dMin.y) : dMax.y;
        }
        if ((UnityEngine.Object) layoutItem.sprite != (UnityEngine.Object) null || (UnityEngine.Object) layoutItem.UIMask != (UnityEngine.Object) null || (UnityEngine.Object) layoutItem.layout != (UnityEngine.Object) null)
        {
          Matrix4x4 matrix4x4 = this.transform.localToWorldMatrix * layoutItem.gameObj.transform.worldToLocalMatrix;
          vector3_2 = matrix4x4.MultiplyVector(vector3_2);
          vector3_3 = matrix4x4.MultiplyVector(vector3_3);
        }
        if ((UnityEngine.Object) layoutItem.sprite != (UnityEngine.Object) null)
          layoutItem.sprite.ReshapeBounds(vector3_2, vector3_3);
        else if ((UnityEngine.Object) layoutItem.UIMask != (UnityEngine.Object) null)
          layoutItem.UIMask.ReshapeBounds(vector3_2, vector3_3);
        else if ((UnityEngine.Object) layoutItem.layout != (UnityEngine.Object) null)
        {
          layoutItem.layout.Reshape(vector3_2, vector3_3, true);
        }
        else
        {
          Vector3 vector3_4 = vector3_2;
          if (layoutItem.snapLeft && layoutItem.snapRight)
            vector3_4.x = (float) (0.5 * ((double) vector3_2.x + (double) vector3_3.x));
          if (layoutItem.snapTop && layoutItem.snapBottom)
            vector3_4.y = (float) (0.5 * ((double) vector3_2.y + (double) vector3_3.y));
          layoutItem.gameObj.transform.position += vector3_4;
        }
      }
      if (this.OnReshape == null)
        return;
      this.OnReshape(dMin, dMax);
    }

    public void SetBounds(Vector3 pMin, Vector3 pMax)
    {
      Matrix4x4 worldToLocalMatrix = this.transform.worldToLocalMatrix;
      this.Reshape(worldToLocalMatrix.MultiplyPoint(pMin) - this.bMin, worldToLocalMatrix.MultiplyPoint(pMax) - this.bMax, true);
    }

    public Vector3 GetMinBounds() => this.transform.localToWorldMatrix.MultiplyPoint(this.bMin);

    public Vector3 GetMaxBounds() => this.transform.localToWorldMatrix.MultiplyPoint(this.bMax);

    public void Refresh() => this.Reshape(Vector3.zero, Vector3.zero, true);
  }

