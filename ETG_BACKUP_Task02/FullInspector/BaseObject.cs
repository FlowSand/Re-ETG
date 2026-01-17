// Decompiled with JetBrains decompiler
// Type: FullInspector.BaseObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector;

public abstract class BaseObject : 
  fiValueProxyEditor,
  fiIValueProxyAPI,
  ISerializedObject,
  ISerializationCallbackReceiver
{
  [SerializeField]
  [NotSerialized]
  [HideInInspector]
  private List<Object> _objectReferences;
  [SerializeField]
  [HideInInspector]
  [NotSerialized]
  private List<string> _serializedStateKeys;
  [SerializeField]
  [HideInInspector]
  [NotSerialized]
  private List<string> _serializedStateValues;

  List<Object> ISerializedObject.SerializedObjectReferences
  {
    get => this._objectReferences;
    set => this._objectReferences = value;
  }

  List<string> ISerializedObject.SerializedStateKeys
  {
    get => this._serializedStateKeys;
    set => this._serializedStateKeys = value;
  }

  List<string> ISerializedObject.SerializedStateValues
  {
    get => this._serializedStateValues;
    set => this._serializedStateValues = value;
  }

  bool ISerializedObject.IsRestored { get; set; }

  void ISerializedObject.RestoreState()
  {
    fiISerializedObjectUtility.RestoreState<FullSerializerSerializer>((ISerializedObject) this);
  }

  void ISerializedObject.SaveState()
  {
    fiISerializedObjectUtility.SaveState<FullSerializerSerializer>((ISerializedObject) this);
  }

  void ISerializationCallbackReceiver.OnAfterDeserialize()
  {
    fiISerializedObjectUtility.RestoreState<FullSerializerSerializer>((ISerializedObject) this);
  }

  void ISerializationCallbackReceiver.OnBeforeSerialize()
  {
    fiISerializedObjectUtility.SaveState<FullSerializerSerializer>((ISerializedObject) this);
  }

  object fiIValueProxyAPI.Value
  {
    get => (object) this;
    set
    {
    }
  }

  void fiIValueProxyAPI.SaveState()
  {
  }

  void fiIValueProxyAPI.LoadState()
  {
  }
}
