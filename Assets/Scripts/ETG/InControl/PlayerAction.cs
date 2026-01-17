// Decompiled with JetBrains decompiler
// Type: InControl.PlayerAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

#nullable disable
namespace InControl
{
  public class PlayerAction : OneAxisInputControl
  {
    public BindingListenOptions ListenOptions;
    public BindingSourceType LastInputType;
    public ulong LastInputTypeChangedTick;
    public InputDeviceClass LastDeviceClass;
    public InputDeviceStyle LastDeviceStyle;
    private List<BindingSource> defaultBindings = new List<BindingSource>();
    private List<BindingSource> regularBindings = new List<BindingSource>();
    private List<BindingSource> visibleBindings = new List<BindingSource>();
    private readonly ReadOnlyCollection<BindingSource> bindings;
    private readonly ReadOnlyCollection<BindingSource> unfilteredBindings;
    private static readonly BindingSourceListener[] bindingSourceListeners = new BindingSourceListener[4]
    {
      (BindingSourceListener) new DeviceBindingSourceListener(),
      (BindingSourceListener) new UnknownDeviceBindingSourceListener(),
      (BindingSourceListener) new KeyBindingSourceListener(),
      (BindingSourceListener) new MouseBindingSourceListener()
    };
    private List<BindingSourceType> m_ignoredBindingSources = new List<BindingSourceType>();
    private InputDevice device;
    private InputDevice activeDevice;

    public PlayerAction(string name, PlayerActionSet owner)
    {
      this.Raw = true;
      this.Name = name;
      this.Owner = owner;
      this.bindings = new ReadOnlyCollection<BindingSource>((IList<BindingSource>) this.visibleBindings);
      this.unfilteredBindings = new ReadOnlyCollection<BindingSource>((IList<BindingSource>) this.regularBindings);
      owner.AddPlayerAction(this);
    }

    public string Name { get; private set; }

    public PlayerActionSet Owner { get; private set; }

    public event Action<BindingSourceType> OnLastInputTypeChanged;

    public object UserData { get; set; }

    public void AddDefaultBinding(BindingSource binding)
    {
      if (binding == (BindingSource) null)
        return;
      if (binding.BoundTo != null)
        throw new InControlException("Binding source is already bound to action " + binding.BoundTo.Name);
      if (!this.defaultBindings.Contains(binding))
      {
        this.defaultBindings.Add(binding);
        binding.BoundTo = this;
      }
      if (this.regularBindings.Contains(binding))
        return;
      this.regularBindings.Add(binding);
      binding.BoundTo = this;
      if (!binding.IsValid)
        return;
      this.visibleBindings.Add(binding);
    }

    public void AddDefaultBinding(params Key[] keys)
    {
      this.AddDefaultBinding((BindingSource) new KeyBindingSource(keys));
    }

    public void AddDefaultBinding(KeyCombo keyCombo)
    {
      this.AddDefaultBinding((BindingSource) new KeyBindingSource(keyCombo));
    }

    public void AddDefaultBinding(Mouse control)
    {
      this.AddDefaultBinding((BindingSource) new MouseBindingSource(control));
    }

    public void AddDefaultBinding(InputControlType control)
    {
      this.AddDefaultBinding((BindingSource) new DeviceBindingSource(control));
    }

    public void RemoveBindingOfType(InputControlType control)
    {
      for (int index = 0; index < this.bindings.Count; ++index)
      {
        if (this.bindings[index] != (BindingSource) null && this.bindings[index].BindingSourceType == BindingSourceType.DeviceBindingSource)
        {
          DeviceBindingSource binding = this.bindings[index] as DeviceBindingSource;
          if (binding.Control == control)
          {
            this.RemoveBinding((BindingSource) binding);
            break;
          }
        }
      }
      this.ForceUpdateVisibleBindings();
    }

    public bool AddBinding(BindingSource binding)
    {
      if (binding == (BindingSource) null)
        return false;
      if (binding.BoundTo != null)
      {
        Debug.LogWarning((object) ("Binding source is already bound to action " + binding.BoundTo.Name));
        return false;
      }
      if (this.regularBindings.Contains(binding))
        return false;
      this.regularBindings.Add(binding);
      binding.BoundTo = this;
      if (binding.IsValid)
        this.visibleBindings.Add(binding);
      return true;
    }

    public bool InsertBindingAt(int index, BindingSource binding)
    {
      if (index < 0 || index > this.visibleBindings.Count)
        throw new InControlException("Index is out of range for bindings on this action.");
      if (index == this.visibleBindings.Count)
        return this.AddBinding(binding);
      if (binding == (BindingSource) null)
        return false;
      if (binding.BoundTo != null)
      {
        Debug.LogWarning((object) ("Binding source is already bound to action " + binding.BoundTo.Name));
        return false;
      }
      if (this.regularBindings.Contains(binding))
        return false;
      this.regularBindings.Insert(index != 0 ? this.regularBindings.IndexOf(this.visibleBindings[index]) : 0, binding);
      binding.BoundTo = this;
      if (binding.IsValid)
        this.visibleBindings.Insert(index, binding);
      return true;
    }

    public bool ReplaceBinding(BindingSource findBinding, BindingSource withBinding)
    {
      if (findBinding == (BindingSource) null || withBinding == (BindingSource) null)
        return false;
      if (withBinding.BoundTo != null)
      {
        Debug.LogWarning((object) ("Binding source is already bound to action " + withBinding.BoundTo.Name));
        return false;
      }
      int index1 = this.regularBindings.IndexOf(findBinding);
      if (index1 < 0)
      {
        Debug.LogWarning((object) "Binding source to replace is not present in this action.");
        return false;
      }
      findBinding.BoundTo = (PlayerAction) null;
      this.regularBindings[index1] = withBinding;
      withBinding.BoundTo = this;
      int index2 = this.visibleBindings.IndexOf(findBinding);
      if (index2 >= 0)
        this.visibleBindings[index2] = withBinding;
      return true;
    }

    public bool HasBinding(BindingSource binding)
    {
      if (binding == (BindingSource) null)
        return false;
      BindingSource binding1 = this.FindBinding(binding);
      return !(binding1 == (BindingSource) null) && binding1.BoundTo == this;
    }

    public BindingSource FindBinding(BindingSource binding)
    {
      if (binding == (BindingSource) null)
        return (BindingSource) null;
      int index = this.regularBindings.IndexOf(binding);
      return index >= 0 ? this.regularBindings[index] : (BindingSource) null;
    }

    private void HardRemoveBinding(BindingSource binding)
    {
      if (binding == (BindingSource) null)
        return;
      int index = this.regularBindings.IndexOf(binding);
      if (index < 0)
        return;
      BindingSource regularBinding = this.regularBindings[index];
      if (regularBinding.BoundTo != this)
        return;
      regularBinding.BoundTo = (PlayerAction) null;
      this.regularBindings.RemoveAt(index);
      this.UpdateVisibleBindings();
    }

    public void RemoveBinding(BindingSource binding)
    {
      BindingSource binding1 = this.FindBinding(binding);
      if (!(binding1 != (BindingSource) null) || binding1.BoundTo != this)
        return;
      binding1.BoundTo = (PlayerAction) null;
    }

    public void ClearSpecificBindingByType(int index, params BindingSourceType[] types)
    {
      int num = 0;
      for (int index1 = 0; index1 < this.regularBindings.Count; ++index1)
      {
        if (Array.IndexOf<BindingSourceType>(types, this.regularBindings[index1].BindingSourceType) >= 0)
        {
          if (num == index)
          {
            this.regularBindings.RemoveAt(index1);
            break;
          }
          ++num;
        }
      }
      this.UpdateVisibleBindings();
    }

    public void SetBindingOfTypeByNumber(
      BindingSource binding,
      BindingSourceType sourceType,
      int index,
      Action<PlayerAction, BindingSource> OnBindingAdded = null)
    {
      List<BindingSource> bindingSourceList = new List<BindingSource>();
      List<BindingSourceType> bindingSourceTypeList = new List<BindingSourceType>();
      bindingSourceTypeList.Add(sourceType);
      if (sourceType == BindingSourceType.KeyBindingSource)
        bindingSourceTypeList.Add(BindingSourceType.MouseBindingSource);
      if (sourceType == BindingSourceType.MouseBindingSource)
        bindingSourceTypeList.Add(BindingSourceType.KeyBindingSource);
      if (sourceType == BindingSourceType.DeviceBindingSource)
        bindingSourceTypeList.Add(BindingSourceType.UnknownDeviceBindingSource);
      if (sourceType == BindingSourceType.UnknownDeviceBindingSource)
        bindingSourceTypeList.Add(BindingSourceType.DeviceBindingSource);
      for (int index1 = 0; index1 < this.regularBindings.Count; ++index1)
      {
        if (bindingSourceTypeList.Contains(this.regularBindings[index1].BindingSourceType))
        {
          bindingSourceList.Add(this.regularBindings[index1]);
          this.regularBindings.RemoveAt(index1);
          --index1;
        }
      }
      this.UpdateVisibleBindings();
      for (int index2 = 0; index2 < bindingSourceList.Count; ++index2)
        bindingSourceList[index2].BoundTo = (PlayerAction) null;
      if (bindingSourceList.Count <= index)
        bindingSourceList.Add(binding);
      else if (bindingSourceList.Count > index)
        bindingSourceList[index] = binding;
      for (int index3 = 0; index3 < bindingSourceList.Count; ++index3)
        this.AddBinding(bindingSourceList[index3]);
      this.UpdateVisibleBindings();
      if (OnBindingAdded == null)
        return;
      OnBindingAdded(this, binding);
    }

    public void IgnoreBindingsOfType(BindingSourceType sourceType)
    {
      this.m_ignoredBindingSources.Add(sourceType);
    }

    public void ClearBindingsOfType(BindingSourceType sourceType)
    {
      for (int index = 0; index < this.regularBindings.Count; ++index)
      {
        if (this.regularBindings[index].BindingSourceType == sourceType)
        {
          this.regularBindings.RemoveAt(index);
          --index;
        }
      }
      this.UpdateVisibleBindings();
    }

    public void RemoveBindingAt(int index)
    {
      if (index < 0 || index >= this.regularBindings.Count)
        throw new InControlException("Index is out of range for bindings on this action.");
      this.regularBindings[index].BoundTo = (PlayerAction) null;
    }

    private int CountBindingsOfType(BindingSourceType bindingSourceType)
    {
      int num = 0;
      int count = this.regularBindings.Count;
      for (int index = 0; index < count; ++index)
      {
        BindingSource regularBinding = this.regularBindings[index];
        if (regularBinding.BoundTo == this && regularBinding.BindingSourceType == bindingSourceType)
          ++num;
      }
      return num;
    }

    private void RemoveFirstBindingOfType(BindingSourceType bindingSourceType)
    {
      int count = this.regularBindings.Count;
      for (int index = 0; index < count; ++index)
      {
        BindingSource regularBinding = this.regularBindings[index];
        if (regularBinding.BoundTo == this && regularBinding.BindingSourceType == bindingSourceType)
        {
          regularBinding.BoundTo = (PlayerAction) null;
          this.regularBindings.RemoveAt(index);
          break;
        }
      }
    }

    private int IndexOfFirstInvalidBinding()
    {
      int count = this.regularBindings.Count;
      for (int index = 0; index < count; ++index)
      {
        if (!this.regularBindings[index].IsValid)
          return index;
      }
      return -1;
    }

    public void ClearBindings()
    {
      int count = this.regularBindings.Count;
      for (int index = 0; index < count; ++index)
        this.regularBindings[index].BoundTo = (PlayerAction) null;
      this.regularBindings.Clear();
      this.visibleBindings.Clear();
    }

    public void ResetBindings()
    {
      this.ClearBindings();
      this.regularBindings.AddRange((IEnumerable<BindingSource>) this.defaultBindings);
      int count = this.regularBindings.Count;
      for (int index = 0; index < count; ++index)
      {
        BindingSource regularBinding = this.regularBindings[index];
        regularBinding.BoundTo = this;
        if (regularBinding.IsValid)
          this.visibleBindings.Add(regularBinding);
      }
    }

    public void ListenForBinding() => this.ListenForBindingReplacing((BindingSource) null);

    public void ListenForBindingReplacing(BindingSource binding)
    {
      (this.ListenOptions ?? this.Owner.ListenOptions).ReplaceBinding = binding;
      this.Owner.listenWithAction = this;
      int length = PlayerAction.bindingSourceListeners.Length;
      for (int index = 0; index < length; ++index)
        PlayerAction.bindingSourceListeners[index].Reset();
    }

    public void StopListeningForBinding()
    {
      if (!this.IsListeningForBinding)
        return;
      this.Owner.listenWithAction = (PlayerAction) null;
    }

    public bool IsListeningForBinding => this.Owner.listenWithAction == this;

    public ReadOnlyCollection<BindingSource> Bindings => this.bindings;

    public ReadOnlyCollection<BindingSource> UnfilteredBindings => this.unfilteredBindings;

    private void RemoveOrphanedBindings()
    {
      for (int index = this.regularBindings.Count - 1; index >= 0; --index)
      {
        if (this.regularBindings[index].BoundTo != this)
          this.regularBindings.RemoveAt(index);
      }
    }

    internal void Update(ulong updateTick, float deltaTime, InputDevice device)
    {
      this.Device = device;
      this.UpdateBindings(updateTick, deltaTime);
      this.DetectBindings();
    }

    private void UpdateBindings(ulong updateTick, float deltaTime)
    {
      bool flag1 = this.IsListeningForBinding || this.Owner.IsListeningForBinding && this.Owner.PreventInputWhileListeningForBinding;
      BindingSourceType bindingSourceType = this.LastInputType;
      ulong num = this.LastInputTypeChangedTick;
      ulong updateTick1 = this.UpdateTick;
      InputDeviceClass inputDeviceClass = this.LastDeviceClass;
      InputDeviceStyle inputDeviceStyle = this.LastDeviceStyle;
      int count = this.regularBindings.Count;
      for (int index1 = count - 1; index1 >= 0; --index1)
      {
        BindingSource regularBinding = this.regularBindings[index1];
        bool flag2 = false;
        for (int index2 = 0; index2 < this.m_ignoredBindingSources.Count; ++index2)
        {
          if (this.m_ignoredBindingSources[index2] == regularBinding.BindingSourceType)
            flag2 = true;
        }
        if (!flag2)
        {
          if (regularBinding.BoundTo != this)
          {
            this.regularBindings.RemoveAt(index1);
            this.visibleBindings.Remove(regularBinding);
          }
          else if (!flag1 && this.UpdateWithValue(regularBinding.GetValue(this.Device), updateTick, deltaTime))
          {
            bindingSourceType = regularBinding.BindingSourceType;
            num = updateTick;
            inputDeviceClass = regularBinding.DeviceClass;
            inputDeviceStyle = regularBinding.DeviceStyle;
          }
        }
      }
      if (flag1 || count == 0)
        this.UpdateWithValue(0.0f, updateTick, deltaTime);
      this.Commit();
      this.Enabled = this.Owner.Enabled;
      if (num > this.LastInputTypeChangedTick && (bindingSourceType != BindingSourceType.MouseBindingSource || (double) Utility.Abs(this.LastValue - this.Value) >= (double) MouseBindingSource.JitterThreshold))
      {
        bool flag3 = bindingSourceType != this.LastInputType;
        this.LastInputType = bindingSourceType;
        this.LastInputTypeChangedTick = num;
        this.LastDeviceClass = inputDeviceClass;
        this.LastDeviceStyle = inputDeviceStyle;
        if (this.OnLastInputTypeChanged != null && flag3)
          this.OnLastInputTypeChanged(bindingSourceType);
      }
      if (this.UpdateTick <= updateTick1)
        return;
      this.activeDevice = !this.LastInputTypeIsDevice ? (InputDevice) null : this.Device;
    }

    private void DetectBindings()
    {
      if (!this.IsListeningForBinding)
        return;
      BindingSource bindingSource = (BindingSource) null;
      BindingListenOptions listenOptions = this.ListenOptions ?? this.Owner.ListenOptions;
      int length = PlayerAction.bindingSourceListeners.Length;
      for (int index = 0; index < length; ++index)
      {
        bindingSource = PlayerAction.bindingSourceListeners[index].Listen(listenOptions, this.device);
        if (bindingSource != (BindingSource) null)
          break;
      }
      if (bindingSource == (BindingSource) null || !listenOptions.CallOnBindingFound(this, bindingSource))
        return;
      if (this.HasBinding(bindingSource))
      {
        if (listenOptions.RejectRedundantBindings)
        {
          listenOptions.CallOnBindingRejected(this, bindingSource, BindingSourceRejectionType.DuplicateBindingOnActionSet);
        }
        else
        {
          this.StopListeningForBinding();
          listenOptions.CallOnBindingAdded(this, bindingSource);
        }
      }
      else
      {
        if (listenOptions.UnsetDuplicateBindingsOnSet)
        {
          int count = this.Owner.Actions.Count;
          for (int index = 0; index < count; ++index)
            this.Owner.Actions[index].HardRemoveBinding(bindingSource);
        }
        if (!listenOptions.AllowDuplicateBindingsPerSet && this.Owner.HasBinding(bindingSource))
        {
          listenOptions.CallOnBindingRejected(this, bindingSource, BindingSourceRejectionType.DuplicateBindingOnActionSet);
        }
        else
        {
          this.StopListeningForBinding();
          if (listenOptions.ReplaceBinding == (BindingSource) null)
          {
            if (listenOptions.MaxAllowedBindingsPerType > 0U)
            {
              while ((long) this.CountBindingsOfType(bindingSource.BindingSourceType) >= (long) listenOptions.MaxAllowedBindingsPerType)
                this.RemoveFirstBindingOfType(bindingSource.BindingSourceType);
            }
            else if (listenOptions.MaxAllowedBindings > 0U)
            {
              while ((long) this.regularBindings.Count >= (long) listenOptions.MaxAllowedBindings)
                this.regularBindings.RemoveAt(Mathf.Max(0, this.IndexOfFirstInvalidBinding()));
            }
            this.AddBinding(bindingSource);
          }
          else
            this.ReplaceBinding(listenOptions.ReplaceBinding, bindingSource);
          this.UpdateVisibleBindings();
          listenOptions.CallOnBindingAdded(this, bindingSource);
        }
      }
    }

    public void ForceUpdateVisibleBindings() => this.UpdateVisibleBindings();

    private void UpdateVisibleBindings()
    {
      this.visibleBindings.Clear();
      int count = this.regularBindings.Count;
      for (int index = 0; index < count; ++index)
      {
        BindingSource regularBinding = this.regularBindings[index];
        if (regularBinding.IsValid)
          this.visibleBindings.Add(regularBinding);
      }
    }

    internal InputDevice Device
    {
      get
      {
        if (this.device == null)
        {
          this.device = this.Owner.Device;
          this.UpdateVisibleBindings();
        }
        return this.device;
      }
      set
      {
        if (this.device == value)
          return;
        this.device = value;
        this.UpdateVisibleBindings();
      }
    }

    public InputDevice ActiveDevice
    {
      get => this.activeDevice == null ? InputDevice.Null : this.activeDevice;
    }

    private bool LastInputTypeIsDevice
    {
      get
      {
        return this.LastInputType == BindingSourceType.DeviceBindingSource || this.LastInputType == BindingSourceType.UnknownDeviceBindingSource;
      }
    }

    [Obsolete("Please set this property on device controls directly. It does nothing here.")]
    public new float LowerDeadZone
    {
      get => 0.0f;
      set
      {
      }
    }

    [Obsolete("Please set this property on device controls directly. It does nothing here.")]
    public new float UpperDeadZone
    {
      get => 0.0f;
      set
      {
      }
    }

    internal void Load(BinaryReader reader, ushort dataFormatVersion, bool upgrade)
    {
      this.ClearBindings();
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        BindingSourceType bindingSourceType = (BindingSourceType) reader.ReadInt32();
        BindingSource binding;
        switch (bindingSourceType)
        {
          case BindingSourceType.None:
            continue;
          case BindingSourceType.DeviceBindingSource:
            binding = (BindingSource) new DeviceBindingSource();
            break;
          case BindingSourceType.KeyBindingSource:
            binding = (BindingSource) new KeyBindingSource();
            break;
          case BindingSourceType.MouseBindingSource:
            binding = (BindingSource) new MouseBindingSource();
            break;
          case BindingSourceType.UnknownDeviceBindingSource:
            binding = (BindingSource) new UnknownDeviceBindingSource();
            break;
          default:
            throw new InControlException("Don't know how to load BindingSourceType: " + (object) bindingSourceType);
        }
        binding.Load(reader, dataFormatVersion, upgrade);
        this.AddBinding(binding);
      }
    }

    internal void Save(BinaryWriter writer)
    {
      this.RemoveOrphanedBindings();
      writer.Write(this.Name);
      int count = this.regularBindings.Count;
      writer.Write(count);
      for (int index = 0; index < count; ++index)
      {
        BindingSource regularBinding = this.regularBindings[index];
        writer.Write((int) regularBinding.BindingSourceType);
        regularBinding.Save(writer);
      }
    }
  }
}
