using UnityEngine;

#nullable disable

public interface IInputAdapter
  {
    bool GetKeyDown(KeyCode key);

    bool GetKeyUp(KeyCode key);

    float GetAxis(string axisName);

    Vector2 GetMousePosition();

    bool GetMouseButton(int button);

    bool GetMouseButtonDown(int button);

    bool GetMouseButtonUp(int button);
  }

