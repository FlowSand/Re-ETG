// Decompiled with JetBrains decompiler
// Type: FullInspector.BackupService.fiDeserializedObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector.Internal;
using FullSerializer.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.BackupService;

public class fiDeserializedObject
{
  public List<fiDeserializedMember> Members;

  public fiDeserializedObject(fiSerializedObject serializedState)
  {
    Type type = serializedState.Target.Target.GetType();
    fiSerializationOperator serializationOperator = new fiSerializationOperator()
    {
      SerializedObjects = serializedState.ObjectReferences
    };
    BaseSerializer baseSerializer = (BaseSerializer) fiSingletons.Get(BehaviorTypeToSerializerTypeMap.GetSerializerType(type));
    InspectedType inspectedType = InspectedType.Get(type);
    this.Members = new List<fiDeserializedMember>();
    foreach (fiSerializedMember member in serializedState.Members)
    {
      InspectedProperty propertyByName = inspectedType.GetPropertyByName(member.Name);
      if (propertyByName != null)
      {
        object obj = baseSerializer.Deserialize(fsPortableReflection.AsMemberInfo(propertyByName.StorageType), member.Value, (ISerializationOperator) serializationOperator);
        this.Members.Add(new fiDeserializedMember()
        {
          InspectedProperty = propertyByName,
          Value = obj,
          ShouldRestore = member.ShouldRestore
        });
      }
    }
  }
}
