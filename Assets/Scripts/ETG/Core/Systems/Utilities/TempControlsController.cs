using InControl;
using UnityEngine;

#nullable disable

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

