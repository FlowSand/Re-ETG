// Decompiled with JetBrains decompiler
// Type: Iconographizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class Iconographizer : MonoBehaviour
    {
      public string SonySpriteName;
      public string XboxSpriteName;
      public string SwitchSpriteName;
      public string SwitchSingleJoyConSpriteName;
      public bool InteractOverride;
      public bool DoResize;
      [ShowInInspectorIf("DoResize", true)]
      public float ResizeScale = 3f;
      private dfSprite m_control;
      private tk2dBaseSprite m_sprite;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Iconographizer.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void HandleVisibilityChanged(dfControl control, bool value)
      {
        Vector2 vector2 = (Vector2) this.m_control.Position + new Vector2(this.m_control.Size.x / 2f, (float) (-(double) this.m_control.Size.y / 2.0));
        string str;
        switch (BraveInput.PlayerOneCurrentSymbology)
        {
          case GameOptions.ControllerSymbology.PS4:
            str = this.SonySpriteName;
            break;
          case GameOptions.ControllerSymbology.Switch:
            str = this.SwitchSpriteName;
            break;
          default:
            str = this.XboxSpriteName;
            break;
        }
        if (this.InteractOverride)
        {
          string controllerInteractKey = this.GetControllerInteractKey();
          if (controllerInteractKey != null)
            str = controllerInteractKey;
        }
        if (str == null)
          return;
        this.m_control.SpriteName = str;
        if (!this.DoResize)
          return;
        this.m_control.Size = this.m_control.SpriteInfo.sizeInPixels * this.ResizeScale;
        this.m_control.Position = (Vector3) (vector2 + new Vector2((float) (-(double) this.m_control.Size.x / 2.0), this.m_control.Size.y / 2f));
      }

      private string GetControllerInteractKey()
      {
        if (!Minimap.HasInstance)
          return (string) null;
        BraveInput primaryPlayerInstance = BraveInput.PrimaryPlayerInstance;
        if ((Object) primaryPlayerInstance == (Object) null || primaryPlayerInstance.IsKeyboardAndMouse())
          return (string) null;
        GungeonActions activeActions = primaryPlayerInstance.ActiveActions;
        if (activeActions != null && activeActions.InteractAction.Bindings.Count > 0)
        {
          ReadOnlyCollection<BindingSource> bindings = activeActions.InteractAction.Bindings;
          for (int index = 0; index < bindings.Count; ++index)
          {
            DeviceBindingSource deviceBindingSource = bindings[index] as DeviceBindingSource;
            if ((BindingSource) deviceBindingSource != (BindingSource) null && deviceBindingSource.Control != InputControlType.None)
              return UIControllerButtonHelper.GetControllerButtonSpriteName(deviceBindingSource.Control, BraveInput.PlayerOneCurrentSymbology);
          }
        }
        return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action1, BraveInput.PlayerOneCurrentSymbology);
      }
    }

}
