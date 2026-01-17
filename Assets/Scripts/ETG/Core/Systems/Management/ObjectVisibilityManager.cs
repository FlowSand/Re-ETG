// Decompiled with JetBrains decompiler
// Type: ObjectVisibilityManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public class ObjectVisibilityManager : BraveBehaviour
    {
      public RoomHandler parentRoom;
      private RoomHandler.VisibilityStatus currentVisibility;
      private List<Component> m_renderers;
      private bool m_initialized;
      private GameObject m_object;
      private List<Renderer> m_ignoredRenderers = new List<Renderer>();
      public bool SuppressPlayerEnteredRoom;
      public System.Action OnToggleRenderers;
      private bool m_activatingLight;

      private void Start()
      {
        if (this.m_initialized)
          return;
        this.Initialize(GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY()) ?? GameManager.Instance.Dungeon.data[this.transform.position.IntXY()].nearestRoom);
      }

      public void Initialize(RoomHandler room, bool allowEngagement = false)
      {
        if (room == null || !(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.gameObject)
        {
          UnityEngine.Debug.LogWarning((object) "Failing to initialize OVM!");
        }
        else
        {
          this.m_initialized = true;
          this.parentRoom = room;
          this.currentVisibility = room.visibility;
          this.parentRoom.BecameVisible += new RoomHandler.OnBecameVisibleEventHandler(this.HandleParentRoomEntered);
          this.parentRoom.BecameInvisible += new RoomHandler.OnBecameInvisibleEventHandler(this.HandleParentRoomExited);
          this.m_object = this.gameObject;
          this.ChangeToVisibility(this.currentVisibility, allowEngagement);
        }
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (this.parentRoom == null)
          return;
        this.parentRoom.BecameVisible -= new RoomHandler.OnBecameVisibleEventHandler(this.HandleParentRoomEntered);
        this.parentRoom.BecameInvisible -= new RoomHandler.OnBecameInvisibleEventHandler(this.HandleParentRoomExited);
      }

      public void ResetRenderersList() => this.m_renderers.Clear();

      private void AcquireRenderers()
      {
        this.m_renderers = new List<Component>();
        this.m_renderers.AddRange((IEnumerable<Component>) this.m_object.GetComponentsInChildren<ParticleSystem>());
        this.m_renderers.AddRange((IEnumerable<Component>) this.m_object.GetComponentsInChildren<AIActor>());
        this.m_renderers.AddRange((IEnumerable<Component>) this.m_object.GetComponentsInChildren<MeshRenderer>());
        this.m_renderers.AddRange((IEnumerable<Component>) this.m_object.GetComponentsInChildren<Light>());
      }

      private void ToggleRenderers(
        bool simpleEnabled,
        RoomHandler.VisibilityStatus visibilityStatus,
        bool allowEngagement)
      {
        for (int index = 0; index < this.m_renderers.Count; ++index)
        {
          bool flag1 = simpleEnabled;
          Component renderer1 = this.m_renderers[index];
          if ((bool) (UnityEngine.Object) renderer1)
          {
            switch (renderer1)
            {
              case Renderer _:
                Renderer renderer2 = renderer1 as Renderer;
                if (!this.m_ignoredRenderers.Contains(renderer2) && renderer2.enabled != flag1)
                {
                  renderer2.enabled = flag1;
                  continue;
                }
                continue;
              case Light _:
                bool flag2 = visibilityStatus == RoomHandler.VisibilityStatus.CURRENT;
                if (this.gameObject.activeSelf)
                {
                  Light l = renderer1 as Light;
                  if (l.enabled != flag2)
                  {
                    if (flag2)
                    {
                      this.StartCoroutine(this.ActivateLight(l));
                      continue;
                    }
                    this.StartCoroutine(this.DeactivateLight(l));
                    continue;
                  }
                  continue;
                }
                continue;
              case ParticleSystem _:
                (renderer1 as ParticleSystem).GetComponent<Renderer>().enabled = flag1;
                continue;
              case AIActor _:
                AIActor aiActor = renderer1 as AIActor;
                aiActor.enabled = flag1;
                if (allowEngagement && flag1 && this.gameObject.activeSelf && !aiActor.healthHaver.IsBoss)
                {
                  aiActor.HasBeenEngaged = true;
                  continue;
                }
                continue;
              case Behaviour _:
                Behaviour behaviour = renderer1 as Behaviour;
                if (!(bool) (UnityEngine.Object) behaviour || behaviour.enabled != flag1)
                {
                  behaviour.enabled = flag1;
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
        if ((bool) (UnityEngine.Object) this.aiShooter)
        {
          this.aiShooter.UpdateGunRenderers();
          this.aiShooter.UpdateHandRenderers();
        }
        if (this.OnToggleRenderers == null)
          return;
        this.OnToggleRenderers();
      }

      public void ChangeToVisibility(RoomHandler.VisibilityStatus status, bool allowEngagement = true)
      {
        if (!(bool) (UnityEngine.Object) this)
          return;
        if (this.m_renderers == null || this.m_renderers.Count == 0)
          this.AcquireRenderers();
        if (this.m_renderers == null || this.m_renderers.Count == 0)
        {
          BraveUtility.Log("Expensive visibility management on unmanaged object...", Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);
        }
        else
        {
          this.currentVisibility = status;
          bool simpleEnabled = false;
          switch (this.currentVisibility)
          {
            case RoomHandler.VisibilityStatus.OBSCURED:
              simpleEnabled = false;
              break;
            case RoomHandler.VisibilityStatus.VISITED:
              simpleEnabled = true;
              break;
            case RoomHandler.VisibilityStatus.CURRENT:
              simpleEnabled = true;
              break;
            case RoomHandler.VisibilityStatus.REOBSCURED:
              simpleEnabled = false;
              break;
          }
          this.ToggleRenderers(simpleEnabled, status, allowEngagement);
        }
      }

      [DebuggerHidden]
      private IEnumerator DeactivateLight(Light l)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ObjectVisibilityManager__DeactivateLightc__Iterator0()
        {
          l = l,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ActivateLight(Light l)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ObjectVisibilityManager__ActivateLightc__Iterator1()
        {
          l = l,
          _this = this
        };
      }

      private void HandleParentRoomEntered(float delay)
      {
        if (this.SuppressPlayerEnteredRoom)
          return;
        this.ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT);
      }

      [DebuggerHidden]
      private IEnumerator DelayedBecameVisible(float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ObjectVisibilityManager__DelayedBecameVisiblec__Iterator2()
        {
          delay = delay,
          _this = this
        };
      }

      private void HandleParentRoomExited()
      {
        this.ChangeToVisibility(RoomHandler.VisibilityStatus.VISITED);
      }

      public void AddIgnoredRenderer(Renderer renderer)
      {
        if (this.m_ignoredRenderers.Contains(renderer))
          return;
        this.m_ignoredRenderers.Add(renderer);
      }

      public void RemoveIgnoredRenderer(Renderer renderer) => this.m_ignoredRenderers.Remove(renderer);
    }

}
