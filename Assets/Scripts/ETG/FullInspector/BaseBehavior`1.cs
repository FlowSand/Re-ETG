using Beebyte.Obfuscator;
using FullInspector.Internal;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector
{
  [Skip]
  public abstract class BaseBehavior<TSerializer> : 
    CommonBaseBehavior,
    ISerializedObject,
    ISerializationCallbackReceiver
    where TSerializer : BaseSerializer
  {
    [NotSerialized]
    [HideInInspector]
    [SerializeField]
    private List<Object> _objectReferences;
    [HideInInspector]
    [NotSerialized]
    [SerializeField]
    private List<string> _serializedStateKeys;
    [HideInInspector]
    [SerializeField]
    [NotSerialized]
    private List<string> _serializedStateValues;

    static BaseBehavior()
    {
      BehaviorTypeToSerializerTypeMap.Register(typeof (BaseBehavior<TSerializer>), typeof (TSerializer));
    }

    protected virtual void Awake()
    {
      fiSerializationManager.OnUnityObjectAwake<TSerializer>((ISerializedObject) this);
    }

    protected virtual void OnValidate()
    {
      if (Application.isPlaying || ((ISerializedObject) this).IsRestored)
        return;
      this.RestoreState();
    }

    [ContextMenu("Save Current State")]
    public void SaveState()
    {
      fiISerializedObjectUtility.SaveState<TSerializer>((ISerializedObject) this);
    }

    [ContextMenu("Restore Saved State")]
    public void RestoreState()
    {
      fiISerializedObjectUtility.RestoreState<TSerializer>((ISerializedObject) this);
    }

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

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      ((ISerializedObject) this).IsRestored = false;
      fiSerializationManager.OnUnityObjectDeserialize<TSerializer>((ISerializedObject) this);
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
      fiSerializationManager.OnUnityObjectSerialize<TSerializer>((ISerializedObject) this);
    }

    protected override void OnDestroy() => base.OnDestroy();
  }
}
