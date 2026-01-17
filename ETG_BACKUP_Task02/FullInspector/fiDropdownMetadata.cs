// Decompiled with JetBrains decompiler
// Type: FullInspector.fiDropdownMetadata
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using System;
using UnityEngine;

#nullable disable
namespace FullInspector;

[Serializable]
public class fiDropdownMetadata : IGraphMetadataItemPersistent, ISerializationCallbackReceiver
{
  private fiAnimBool _isActive = new fiAnimBool(true);
  [SerializeField]
  private bool _showDropdown;
  private bool _invertedDefaultState;
  private bool _forceDisable;
  [SerializeField]
  private bool _serializedIsActive;

  public bool IsActive
  {
    get => this._isActive.value;
    set
    {
      if (value == this._isActive.target)
        return;
      if (fiSettings.EnableAnimation)
        this._isActive.target = value;
      else
        this._isActive = new fiAnimBool(value);
    }
  }

  public float AnimPercentage => this._isActive.faded;

  public bool IsAnimating => this._isActive.isAnimating;

  public bool ShouldDisplayDropdownArrow
  {
    get => !this._forceDisable && this._showDropdown;
    set
    {
      if (this._forceDisable && value)
        return;
      this._showDropdown = value;
    }
  }

  public void InvertDefaultState() => this._invertedDefaultState = true;

  public void ForceHideWithoutAnimation()
  {
    this._forceDisable = false;
    this._showDropdown = true;
    this._isActive = new fiAnimBool(false);
  }

  public void ForceDisable() => this._forceDisable = true;

  void ISerializationCallbackReceiver.OnBeforeSerialize()
  {
    this._serializedIsActive = this.IsActive;
  }

  void ISerializationCallbackReceiver.OnAfterDeserialize()
  {
    this._isActive = new fiAnimBool(this._serializedIsActive);
  }

  bool IGraphMetadataItemPersistent.ShouldSerialize()
  {
    return this._invertedDefaultState ? this.IsActive : !this.IsActive;
  }
}
