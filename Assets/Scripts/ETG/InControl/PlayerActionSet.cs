// Decompiled with JetBrains decompiler
// Type: InControl.PlayerActionSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using UnityEngine;

#nullable disable
namespace InControl
{
  public abstract class PlayerActionSet
  {
    private bool m_forceDisable;
    public BindingSourceType LastInputType;
    public ulong LastInputTypeChangedTick;
    public InputDeviceClass LastDeviceClass;
    public InputDeviceStyle LastDeviceStyle;
    private List<PlayerAction> actions = new List<PlayerAction>();
    private List<PlayerOneAxisAction> oneAxisActions = new List<PlayerOneAxisAction>();
    private List<PlayerTwoAxisAction> twoAxisActions = new List<PlayerTwoAxisAction>();
    private Dictionary<string, PlayerAction> actionsByName = new Dictionary<string, PlayerAction>();
    private BindingListenOptions listenOptions = new BindingListenOptions();
    internal PlayerAction listenWithAction;
    private InputDevice activeDevice;
    private const ushort currentDataFormatVersion = 2;

    protected PlayerActionSet()
    {
      this.Enabled = true;
      this.PreventInputWhileListeningForBinding = true;
      this.Device = (InputDevice) null;
      this.IncludeDevices = new List<InputDevice>();
      this.ExcludeDevices = new List<InputDevice>();
      this.Actions = new ReadOnlyCollection<PlayerAction>((IList<PlayerAction>) this.actions);
      InputManager.AttachPlayerActionSet(this);
    }

    public InputDevice Device { get; set; }

    public bool ForceDisable
    {
      get => this.m_forceDisable;
      set
      {
        this.m_forceDisable = value;
        if (!this.m_forceDisable)
          return;
        for (int index = 0; index < this.actions.Count; ++index)
          this.actions[index].CommitWithValue(0.0f, InputManager.GetCurrentTick(), 0.0f);
      }
    }

    public List<InputDevice> IncludeDevices { get; private set; }

    public List<InputDevice> ExcludeDevices { get; private set; }

    public ReadOnlyCollection<PlayerAction> Actions { get; private set; }

    public ulong UpdateTick { get; protected set; }

    public event Action<BindingSourceType> OnLastInputTypeChanged;

    public bool Enabled { get; set; }

    public bool PreventInputWhileListeningForBinding { get; set; }

    public object UserData { get; set; }

    public void Destroy()
    {
      this.OnLastInputTypeChanged = (Action<BindingSourceType>) null;
      InputManager.DetachPlayerActionSet(this);
    }

    protected PlayerAction CreatePlayerAction(string name) => new PlayerAction(name, this);

    internal void AddPlayerAction(PlayerAction action)
    {
      action.Device = this.FindActiveDevice();
      if (this.actionsByName.ContainsKey(action.Name))
        throw new InControlException($"Action '{action.Name}' already exists in this set.");
      this.actions.Add(action);
      this.actionsByName.Add(action.Name, action);
    }

    protected PlayerOneAxisAction CreateOneAxisPlayerAction(
      PlayerAction negativeAction,
      PlayerAction positiveAction)
    {
      PlayerOneAxisAction axisPlayerAction = new PlayerOneAxisAction(negativeAction, positiveAction);
      this.oneAxisActions.Add(axisPlayerAction);
      return axisPlayerAction;
    }

    protected PlayerTwoAxisAction CreateTwoAxisPlayerAction(
      PlayerAction negativeXAction,
      PlayerAction positiveXAction,
      PlayerAction negativeYAction,
      PlayerAction positiveYAction)
    {
      PlayerTwoAxisAction axisPlayerAction = new PlayerTwoAxisAction(negativeXAction, positiveXAction, negativeYAction, positiveYAction);
      this.twoAxisActions.Add(axisPlayerAction);
      return axisPlayerAction;
    }

    public PlayerAction this[string actionName]
    {
      get
      {
        PlayerAction playerAction;
        if (this.actionsByName.TryGetValue(actionName, out playerAction))
          return playerAction;
        throw new KeyNotFoundException($"Action '{actionName}' does not exist in this action set.");
      }
    }

    public PlayerAction GetPlayerActionByName(string actionName)
    {
      PlayerAction playerAction;
      return this.actionsByName.TryGetValue(actionName, out playerAction) ? playerAction : (PlayerAction) null;
    }

    internal void Update(ulong updateTick, float deltaTime)
    {
      if (this.ForceDisable)
        return;
      InputDevice device = this.Device ?? this.FindActiveDevice();
      BindingSourceType lastInputType = this.LastInputType;
      ulong inputTypeChangedTick = this.LastInputTypeChangedTick;
      InputDeviceClass lastDeviceClass = this.LastDeviceClass;
      InputDeviceStyle lastDeviceStyle = this.LastDeviceStyle;
      int count1 = this.actions.Count;
      for (int index = 0; index < count1; ++index)
      {
        PlayerAction action = this.actions[index];
        action.Update(updateTick, deltaTime, device);
        if (action.UpdateTick > this.UpdateTick)
        {
          this.UpdateTick = action.UpdateTick;
          this.activeDevice = action.ActiveDevice;
        }
        if (action.LastInputTypeChangedTick > inputTypeChangedTick)
        {
          lastInputType = action.LastInputType;
          inputTypeChangedTick = action.LastInputTypeChangedTick;
          lastDeviceClass = action.LastDeviceClass;
          lastDeviceStyle = action.LastDeviceStyle;
        }
      }
      int count2 = this.oneAxisActions.Count;
      for (int index = 0; index < count2; ++index)
        this.oneAxisActions[index].Update(updateTick, deltaTime);
      int count3 = this.twoAxisActions.Count;
      for (int index = 0; index < count3; ++index)
        this.twoAxisActions[index].Update(updateTick, deltaTime);
      if (inputTypeChangedTick <= this.LastInputTypeChangedTick)
        return;
      bool flag = lastInputType != this.LastInputType;
      this.LastInputType = lastInputType;
      this.LastInputTypeChangedTick = inputTypeChangedTick;
      this.LastDeviceClass = lastDeviceClass;
      this.LastDeviceStyle = lastDeviceStyle;
      if (this.OnLastInputTypeChanged == null || !flag)
        return;
      this.OnLastInputTypeChanged(lastInputType);
    }

    public void Reset()
    {
      int count = this.actions.Count;
      for (int index = 0; index < count; ++index)
        this.actions[index].ResetBindings();
    }

    private InputDevice FindActiveDevice()
    {
      bool flag1 = this.IncludeDevices.Count > 0;
      bool flag2 = this.ExcludeDevices.Count > 0;
      if (!flag1 && !flag2)
        return InputManager.ActiveDevice;
      InputDevice device1 = InputDevice.Null;
      int count = InputManager.Devices.Count;
      for (int index = 0; index < count; ++index)
      {
        InputDevice device2 = InputManager.Devices[index];
        if (device2 != device1 && device2.LastChangedAfter(device1) && (!flag2 || !this.ExcludeDevices.Contains(device2)) && (!flag1 || this.IncludeDevices.Contains(device2)))
          device1 = device2;
      }
      return device1;
    }

    public void ClearInputState()
    {
      int count1 = this.actions.Count;
      for (int index = 0; index < count1; ++index)
        this.actions[index].ClearInputState();
      int count2 = this.oneAxisActions.Count;
      for (int index = 0; index < count2; ++index)
        this.oneAxisActions[index].ClearInputState();
      int count3 = this.twoAxisActions.Count;
      for (int index = 0; index < count3; ++index)
        this.twoAxisActions[index].ClearInputState();
    }

    public bool HasBinding(BindingSource binding)
    {
      if (binding == (BindingSource) null)
        return false;
      int count = this.actions.Count;
      for (int index = 0; index < count; ++index)
      {
        if (this.actions[index].HasBinding(binding))
          return true;
      }
      return false;
    }

    public void RemoveBinding(BindingSource binding)
    {
      if (binding == (BindingSource) null)
        return;
      int count = this.actions.Count;
      for (int index = 0; index < count; ++index)
        this.actions[index].RemoveBinding(binding);
    }

    public bool IsListeningForBinding => this.listenWithAction != null;

    public BindingListenOptions ListenOptions
    {
      get => this.listenOptions;
      set => this.listenOptions = value ?? new BindingListenOptions();
    }

    public InputDevice ActiveDevice
    {
      get => this.activeDevice == null ? InputDevice.Null : this.activeDevice;
    }

    public string Save()
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output, Encoding.UTF8))
        {
          writer.Write((byte) 66);
          writer.Write((byte) 73);
          writer.Write((byte) 78);
          writer.Write((byte) 68);
          writer.Write((ushort) 2);
          int count = this.actions.Count;
          writer.Write(count);
          for (int index = 0; index < count; ++index)
            this.actions[index].Save(writer);
        }
        return Convert.ToBase64String(output.ToArray());
      }
    }

    public void Load(string data, bool upgrade = false)
    {
      if (data == null)
        return;
      try
      {
        using (MemoryStream input = new MemoryStream(Convert.FromBase64String(data)))
        {
          using (BinaryReader reader = new BinaryReader((Stream) input))
          {
            ushort dataFormatVersion = reader.ReadUInt32() == 1145981250U ? reader.ReadUInt16() : throw new Exception("Unknown data format.");
            if (dataFormatVersion < (ushort) 1 || dataFormatVersion > (ushort) 2)
              throw new Exception("Unknown data format version: " + (object) dataFormatVersion);
            int num = reader.ReadInt32();
            for (int index = 0; index < num; ++index)
            {
              PlayerAction playerAction;
              if (this.actionsByName.TryGetValue(reader.ReadString(), out playerAction))
                playerAction.Load(reader, dataFormatVersion, upgrade);
            }
            Debug.Log((object) "FINISHED LOADING SERIALIZED KEYBINDINGS.");
          }
        }
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ("Provided state could not be loaded:\n" + ex.Message));
        this.Reset();
      }
    }
  }
}
