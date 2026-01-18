using InControl;
using UnityEngine;

#nullable disable
namespace BindingsExample
{
  public class BindingsExample : MonoBehaviour
  {
    private Renderer cachedRenderer;
    private PlayerActions playerActions;
    private string saveData;

    private void OnEnable()
    {
      this.playerActions = PlayerActions.CreateWithDefaultBindings();
      this.LoadBindings();
    }

    private void OnDisable() => this.playerActions.Destroy();

    private void Start() => this.cachedRenderer = this.GetComponent<Renderer>();

    private void Update()
    {
      this.transform.Rotate(Vector3.down, 500f * Time.deltaTime * this.playerActions.Move.X, Space.World);
      this.transform.Rotate(Vector3.right, 500f * Time.deltaTime * this.playerActions.Move.Y, Space.World);
      this.cachedRenderer.material.color = Color.Lerp(!this.playerActions.Fire.IsPressed ? Color.white : Color.red, !this.playerActions.Jump.IsPressed ? Color.white : Color.green, 0.5f);
    }

    private void SaveBindings()
    {
      this.saveData = this.playerActions.Save();
      Brave.PlayerPrefs.SetString("Bindings", this.saveData);
    }

    private void LoadBindings()
    {
      if (!Brave.PlayerPrefs.HasKey("Bindings"))
        return;
      this.saveData = Brave.PlayerPrefs.GetString("Bindings");
      this.playerActions.Load(this.saveData);
    }

    private void OnApplicationQuit() => Brave.PlayerPrefs.Save();

    private void OnGUI()
    {
      float y1 = 10f;
      GUI.Label(new Rect(10f, y1, 300f, y1 + 22f), "Last Input Type: " + (object) this.playerActions.LastInputType);
      float y2 = y1 + 22f;
      GUI.Label(new Rect(10f, y2, 300f, y2 + 22f), "Last Device Class: " + (object) this.playerActions.LastDeviceClass);
      float y3 = y2 + 22f;
      GUI.Label(new Rect(10f, y3, 300f, y3 + 22f), "Last Device Style: " + (object) this.playerActions.LastDeviceStyle);
      float y4 = y3 + 22f;
      int count1 = this.playerActions.Actions.Count;
      for (int index1 = 0; index1 < count1; ++index1)
      {
        PlayerAction action = this.playerActions.Actions[index1];
        string name = action.Name;
        if (action.IsListeningForBinding)
          name += " (Listening)";
        string text = $"{name} = {(object) action.Value}";
        GUI.Label(new Rect(10f, y4, 500f, y4 + 22f), text);
        float y5 = y4 + 22f;
        int count2 = action.Bindings.Count;
        for (int index2 = 0; index2 < count2; ++index2)
        {
          BindingSource binding = action.Bindings[index2];
          GUI.Label(new Rect(75f, y5, 300f, y5 + 22f), $"{binding.DeviceName}: {binding.Name}");
          if (GUI.Button(new Rect(20f, y5 + 3f, 20f, 17f), "-"))
            action.RemoveBinding(binding);
          if (GUI.Button(new Rect(45f, y5 + 3f, 20f, 17f), "+"))
            action.ListenForBindingReplacing(binding);
          y5 += 22f;
        }
        if (GUI.Button(new Rect(20f, y5 + 3f, 20f, 17f), "+"))
          action.ListenForBinding();
        if (GUI.Button(new Rect(50f, y5 + 3f, 50f, 17f), "Reset"))
          action.ResetBindings();
        y4 = y5 + 25f;
      }
      if (GUI.Button(new Rect(20f, y4 + 3f, 50f, 22f), "Load"))
        this.LoadBindings();
      if (GUI.Button(new Rect(80f, y4 + 3f, 50f, 22f), "Save"))
        this.SaveBindings();
      if (!GUI.Button(new Rect(140f, y4 + 3f, 50f, 22f), "Reset"))
        return;
      this.playerActions.Reset();
    }
  }
}
