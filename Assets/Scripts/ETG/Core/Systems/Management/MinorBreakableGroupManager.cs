// Decompiled with JetBrains decompiler
// Type: MinorBreakableGroupManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public class MinorBreakableGroupManager : BraveBehaviour
    {
      public MinorBreakableGroupManager.MinorBreakableGroupBehavior behavior;
      public bool autodetectDimensions = true;
      public IntVector2 overridePixelDimensions;
      private List<MinorBreakable> registeredBreakables = new List<MinorBreakable>();
      private List<DebrisObject> registeredDebris = new List<DebrisObject>();

      public void Initialize()
      {
        foreach (MinorBreakable componentsInChild in this.GetComponentsInChildren<MinorBreakable>(true))
          this.RegisterMinorBreakable(componentsInChild);
        DebrisObject[] componentsInChildren = this.GetComponentsInChildren<DebrisObject>(true);
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if ((Object) componentsInChildren[index].GetComponent<MinorBreakable>() == (Object) null)
            this.RegisterDebris(componentsInChildren[index]);
        }
      }

      protected override void OnDestroy() => base.OnDestroy();

      public Vector2 GetDimensions()
      {
        if (!this.autodetectDimensions)
          return PhysicsEngine.PixelToUnit(this.overridePixelDimensions);
        float a1 = float.MaxValue;
        float a2 = float.MaxValue;
        float a3 = this.transform.position.x;
        float a4 = this.transform.position.y;
        foreach (tk2dSprite componentsInChild in this.GetComponentsInChildren<tk2dSprite>(true))
        {
          Transform transform = componentsInChild.transform;
          Bounds bounds = componentsInChild.GetBounds();
          float x = bounds.size.x;
          float y = bounds.size.y;
          a1 = Mathf.Min(a1, transform.position.x);
          a2 = Mathf.Min(a2, transform.position.y);
          a3 = Mathf.Max(a3, transform.position.x + x);
          a4 = Mathf.Max(a4, transform.position.y + y);
        }
        return new Vector2(a3 - this.transform.position.x, a4 - this.transform.position.y);
      }

      public void Destabilize(Vector3 force, float height)
      {
        for (int index = 0; index < this.registeredBreakables.Count; ++index)
        {
          MinorBreakable registeredBreakable = this.registeredBreakables[index];
          if ((Object) registeredBreakable != (Object) null && (bool) (Object) registeredBreakable)
          {
            if ((bool) (Object) registeredBreakable.sprite && (Object) registeredBreakable.sprite.attachParent != (Object) null)
            {
              registeredBreakable.sprite.attachParent.DetachRenderer(registeredBreakable.sprite);
              registeredBreakable.sprite.attachParent = (tk2dBaseSprite) null;
            }
            DebrisObject component = registeredBreakable.GetComponent<DebrisObject>();
            if ((Object) component != (Object) null)
              component.Trigger(Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(-30f, 30f, Random.value)) * force, height);
            else
              registeredBreakable.Break(force.XY());
          }
        }
        for (int index = 0; index < this.registeredDebris.Count; ++index)
        {
          DebrisObject registeredDebri = this.registeredDebris[index];
          if ((bool) (Object) registeredDebri && (bool) (Object) registeredDebri.sprite)
          {
            if ((Object) registeredDebri.sprite.attachParent != (Object) null)
            {
              registeredDebri.sprite.attachParent.DetachRenderer(registeredDebri.sprite);
              registeredDebri.sprite.attachParent = (tk2dBaseSprite) null;
            }
            registeredDebri.Trigger(Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(-30f, 30f, Random.value)) * force, height);
            --index;
          }
        }
        this.registeredBreakables.Clear();
        this.registeredDebris.Clear();
      }

      public void InformBroken(MinorBreakable mb, Vector2 breakForce, float breakHeight)
      {
        this.DeregisterMinorBreakable(mb);
        for (int index = 0; index < this.registeredBreakables.Count; ++index)
        {
          MinorBreakable registeredBreakable = this.registeredBreakables[index];
          if ((bool) (Object) registeredBreakable)
          {
            switch (this.behavior)
            {
              case MinorBreakableGroupManager.MinorBreakableGroupBehavior.TRIGGERS_DEBRIS:
                DebrisObject component = registeredBreakable.GetComponent<DebrisObject>();
                Vector3 zero = Vector3.zero;
                Vector3 startingForce = !(breakForce == Vector2.zero) ? Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(-30f, 30f, Random.value)) * breakForce.ToVector3ZUp(0.5f) : Random.insideUnitCircle.normalized.ToVector3ZUp(0.5f);
                component.Trigger(startingForce, breakHeight);
                continue;
              case MinorBreakableGroupManager.MinorBreakableGroupBehavior.TRIGGERS_BREAK:
                if (breakForce == Vector2.zero)
                {
                  registeredBreakable.Break();
                  continue;
                }
                registeredBreakable.Break(breakForce);
                continue;
              default:
                continue;
            }
          }
        }
        for (int index = 0; index < this.registeredDebris.Count; ++index)
        {
          DebrisObject registeredDebri = this.registeredDebris[index];
          switch (this.behavior)
          {
            case MinorBreakableGroupManager.MinorBreakableGroupBehavior.TRIGGERS_DEBRIS:
              if (breakForce == Vector2.zero)
                breakForce = Random.insideUnitCircle.normalized;
              registeredDebri.Trigger(breakForce.ToVector3ZUp(0.5f), breakHeight);
              --index;
              break;
          }
        }
        if (this.behavior == MinorBreakableGroupManager.MinorBreakableGroupBehavior.NONE)
          return;
        this.registeredBreakables.Clear();
      }

      public void RegisterDebris(DebrisObject d)
      {
        if (!this.registeredDebris.Contains(d))
          this.registeredDebris.Add(d);
        d.groupManager = this;
      }

      public void DeregisterDebris(DebrisObject d)
      {
        d.groupManager = (MinorBreakableGroupManager) null;
        this.registeredDebris.Remove(d);
      }

      public void RegisterMinorBreakable(MinorBreakable mb)
      {
        if (!this.registeredBreakables.Contains(mb))
          this.registeredBreakables.Add(mb);
        mb.GroupManager = this;
      }

      public void DeregisterMinorBreakable(MinorBreakable mb)
      {
        mb.GroupManager = (MinorBreakableGroupManager) null;
        this.registeredBreakables.Remove(mb);
      }

      public enum MinorBreakableGroupBehavior
      {
        TRIGGERS_DEBRIS,
        TRIGGERS_BREAK,
        NONE,
      }
    }

}
