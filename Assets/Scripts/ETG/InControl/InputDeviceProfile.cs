using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace InControl
{
  public abstract class InputDeviceProfile
  {
    private GameOptions.ControllerSymbology m_controllerSymbology = GameOptions.ControllerSymbology.Xbox;
    private static HashSet<System.Type> hideList = new HashSet<System.Type>();
    private float sensitivity = 1f;
    private float lowerDeadZone;
    private float upperDeadZone = 1f;

    public InputDeviceProfile()
    {
      this.Name = string.Empty;
      this.Meta = string.Empty;
      this.ControllerSymbology = GameOptions.ControllerSymbology.Xbox;
      this.AnalogMappings = new InputControlMapping[0];
      this.ButtonMappings = new InputControlMapping[0];
      this.IncludePlatforms = new string[0];
      this.ExcludePlatforms = new string[0];
      this.MinSystemBuildNumber = 0;
      this.MaxSystemBuildNumber = 0;
      this.DeviceClass = InputDeviceClass.Unknown;
      this.DeviceStyle = InputDeviceStyle.Unknown;
    }

    [SerializeField]
    public string Name { get; protected set; }

    [SerializeField]
    public string Meta { get; protected set; }

    [SerializeField]
    public InputControlMapping[] AnalogMappings { get; protected set; }

    [SerializeField]
    public InputControlMapping[] ButtonMappings { get; protected set; }

    [SerializeField]
    public string[] IncludePlatforms { get; protected set; }

    [SerializeField]
    public string[] ExcludePlatforms { get; protected set; }

    [SerializeField]
    public int MaxSystemBuildNumber { get; protected set; }

    [SerializeField]
    public int MinSystemBuildNumber { get; protected set; }

    [SerializeField]
    public InputDeviceClass DeviceClass { get; protected set; }

    [SerializeField]
    public InputDeviceStyle DeviceStyle { get; protected set; }

    [SerializeField]
    public GameOptions.ControllerSymbology ControllerSymbology
    {
      get => this.m_controllerSymbology;
      protected set => this.m_controllerSymbology = value;
    }

    [SerializeField]
    public float Sensitivity
    {
      get => this.sensitivity;
      protected set => this.sensitivity = Mathf.Clamp01(value);
    }

    [SerializeField]
    public float LowerDeadZone
    {
      get => this.lowerDeadZone;
      protected set => this.lowerDeadZone = Mathf.Clamp01(value);
    }

    [SerializeField]
    public float UpperDeadZone
    {
      get => this.upperDeadZone;
      protected set => this.upperDeadZone = Mathf.Clamp01(value);
    }

    [Obsolete("This property has been renamed to IncludePlatforms.", false)]
    public string[] SupportedPlatforms
    {
      get => this.IncludePlatforms;
      protected set => this.IncludePlatforms = value;
    }

    public virtual bool IsSupportedOnThisPlatform
    {
      get
      {
        int systemBuildNumber = Utility.GetSystemBuildNumber();
        if (this.MaxSystemBuildNumber > 0 && systemBuildNumber > this.MaxSystemBuildNumber || this.MinSystemBuildNumber > 0 && systemBuildNumber < this.MinSystemBuildNumber)
          return false;
        if (this.ExcludePlatforms != null)
        {
          int length = this.ExcludePlatforms.Length;
          for (int index = 0; index < length; ++index)
          {
            if (InputManager.Platform.Contains(this.ExcludePlatforms[index].ToUpper()))
              return false;
          }
        }
        if (this.IncludePlatforms == null || this.IncludePlatforms.Length == 0)
          return true;
        if (this.IncludePlatforms != null)
        {
          int length = this.IncludePlatforms.Length;
          for (int index = 0; index < length; ++index)
          {
            if (InputManager.Platform.Contains(this.IncludePlatforms[index].ToUpper()))
              return true;
          }
        }
        return false;
      }
    }

    internal static void Hide(System.Type type) => InputDeviceProfile.hideList.Add(type);

    internal bool IsHidden => InputDeviceProfile.hideList.Contains(this.GetType());

    public int AnalogCount => this.AnalogMappings.Length;

    public int ButtonCount => this.ButtonMappings.Length;
  }
}
