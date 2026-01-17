// Decompiled with JetBrains decompiler
// Type: FullInspector.fiValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FullInspector;

public abstract class fiValue<T> : 
  fiValueProxyEditor,
  fiIValueProxyAPI,
  ISerializationCallbackReceiver
{
  public T Value;
  [HideInInspector]
  [SerializeField]
  private string SerializedState;
  [HideInInspector]
  [SerializeField]
  private List<UnityEngine.Object> SerializedObjectReferences;

  void ISerializationCallbackReceiver.OnBeforeSerialize() => this.Serialize();

  void ISerializationCallbackReceiver.OnAfterDeserialize() => this.Deserialize();

  object fiIValueProxyAPI.Value
  {
    get => (object) this.Value;
    set => this.Value = (T) value;
  }

  void fiIValueProxyAPI.SaveState() => this.Serialize();

  void fiIValueProxyAPI.LoadState() => this.Deserialize();

  private void Serialize()
  {
    FullSerializerSerializer serializerSerializer = fiSingletons.Get<FullSerializerSerializer>();
    ListSerializationOperator serializationOperator = fiSingletons.Get<ListSerializationOperator>();
    serializationOperator.SerializedObjects = new List<UnityEngine.Object>();
    try
    {
      this.SerializedState = serializerSerializer.Serialize((MemberInfo) typeof (T).Resolve(), (object) this.Value, (ISerializationOperator) serializationOperator);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Exception caught when serializing {(object) this} (with type {(object) this.GetType()})\n{(object) ex}");
    }
    this.SerializedObjectReferences = serializationOperator.SerializedObjects;
  }

  private void Deserialize()
  {
    if (this.SerializedObjectReferences == null)
      this.SerializedObjectReferences = new List<UnityEngine.Object>();
    FullSerializerSerializer serializerSerializer = fiSingletons.Get<FullSerializerSerializer>();
    ListSerializationOperator serializationOperator = fiSingletons.Get<ListSerializationOperator>();
    serializationOperator.SerializedObjects = this.SerializedObjectReferences;
    if (string.IsNullOrEmpty(this.SerializedState))
      return;
    try
    {
      this.Value = (T) serializerSerializer.Deserialize((MemberInfo) typeof (T).Resolve(), this.SerializedState, (ISerializationOperator) serializationOperator);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Exception caught when deserializing {(object) this} (with type {(object) this.GetType()});\n{(object) ex}");
    }
  }
}
