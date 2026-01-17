// Decompiled with JetBrains decompiler
// Type: TempControlsController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TempControlsController : MonoBehaviour
    {
      private float m_elapsed;
      private float m_lastTime;
      private const float CLOSE_THRESHOLD = 0.0f;

      public bool CanClose => (double) this.m_elapsed > 0.0;

      private void Awake()
      {
        this.m_elapsed = 0.0f;
        this.m_lastTime = UnityEngine.Time.realtimeSinceStartup;
      }

      private void Update()
      {
        this.m_elapsed += UnityEngine.Time.realtimeSinceStartup - this.m_lastTime;
        this.m_lastTime = UnityEngine.Time.realtimeSinceStartup;
        Debug.Log((object) this.m_elapsed);
        if ((double) this.m_elapsed <= 0.0 || !BraveInput.PrimaryPlayerInstance.ActiveActions.Device.AnyButton.WasPressed && !BraveInput.PrimaryPlayerInstance.ActiveActions.Device.GetControl(InputControlType.Start).WasPressed && !BraveInput.PrimaryPlayerInstance.ActiveActions.Device.GetControl(InputControlType.Select).WasPressed)
          return;
        GameManager.Instance.Unpause();
        Object.Destroy((Object) this.gameObject);
      }
    }

}
