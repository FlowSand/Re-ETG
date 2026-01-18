using FullInspector.Internal;
using FullSerializer;
using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FullInspector
{
  public class Facade<T>
  {
    public System.Type InstanceType;
    public Dictionary<string, string> FacadeMembers = new Dictionary<string, string>();
    public List<UnityEngine.Object> ObjectReferences = new List<UnityEngine.Object>();

    public void PopulateInstance(ref T instance)
    {
      if (instance.GetType() != this.InstanceType)
        Debug.LogWarning((object) $"PopulateInstance: Actual Facade type is different (instance.GetType() = {instance.GetType().CSharpName()}, InstanceType = {this.InstanceType.CSharpName()})");
      BaseSerializer baseSerializer = (BaseSerializer) fiSingletons.Get(fiInstalledSerializerManager.DefaultMetadata.SerializerType);
      ListSerializationOperator serializationOperator = new ListSerializationOperator()
      {
        SerializedObjects = this.ObjectReferences
      };
      InspectedType inspectedType = InspectedType.Get(instance.GetType());
      foreach (KeyValuePair<string, string> facadeMember in this.FacadeMembers)
      {
        string key = facadeMember.Key;
        InspectedProperty propertyByName = inspectedType.GetPropertyByName(key);
        if (propertyByName != null)
        {
          try
          {
            object obj = baseSerializer.Deserialize((MemberInfo) propertyByName.StorageType.Resolve(), facadeMember.Value, (ISerializationOperator) serializationOperator);
            propertyByName.Write((object) instance, obj);
          }
          catch (Exception ex)
          {
            Debug.LogError((object) $"Skipping property {key} in facade due to deserialization exception.\n{(object) ex}");
          }
        }
      }
    }

    public T ConstructInstance()
    {
      T instance = (T) Activator.CreateInstance(this.InstanceType);
      this.PopulateInstance(ref instance);
      return instance;
    }

    public T ConstructInstance(GameObject context)
    {
      T instance = !typeof (Component).IsAssignableFrom(this.InstanceType) ? (T) Activator.CreateInstance(this.InstanceType) : (T) context.AddComponent(this.InstanceType);
      this.PopulateInstance(ref instance);
      return instance;
    }
  }
}
