using System;
using System.Collections.Generic;

using FullSerializer.Internal;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
    public static class fiISerializedObjectUtility
    {
        private static bool SaveStateForProperty(
            ISerializedObject obj,
            InspectedProperty property,
            BaseSerializer serializer,
            ISerializationOperator serializationOperator,
            out string serializedValue,
            ref bool success)
        {
            object obj1 = property.Read((object) obj);
            try
            {
                serializedValue = obj1 != null ? serializer.Serialize(property.MemberInfo, obj1, serializationOperator) : (string) null;
                return true;
            }
            catch (Exception ex)
            {
                success = false;
                serializedValue = (string) null;
                Debug.LogError((object) $"Exception caught when serializing property <{property.Name}> in <{(object) obj}> with value {obj1}\n{(object) ex}");
                return false;
            }
        }

        public static bool SaveState<TSerializer>(ISerializedObject obj) where TSerializer : BaseSerializer
        {
            bool success = true;
            if (obj is ISerializationCallbacks serializationCallbacks)
                serializationCallbacks.OnBeforeSerialize();
            TSerializer serializer = fiSingletons.Get<TSerializer>();
            ListSerializationOperator serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = new List<UnityEngine.Object>();
            List<string> b1 = new List<string>();
            List<string> b2 = new List<string>();
            if (fiUtility.IsEditor || obj.SerializedStateKeys == null || obj.SerializedStateKeys.Count == 0)
            {
                List<InspectedProperty> properties = InspectedType.Get(obj.GetType()).GetProperties(InspectedMemberFilters.FullInspectorSerializedProperties);
                for (int index = 0; index < properties.Count; ++index)
                {
                    InspectedProperty property = properties[index];
                    string serializedValue;
                    if (fiISerializedObjectUtility.SaveStateForProperty(obj, property, (BaseSerializer) serializer, (ISerializationOperator) serializationOperator, out serializedValue, ref success))
                    {
                        b1.Add(property.Name);
                        b2.Add(serializedValue);
                    }
                }
            }
            else
            {
                InspectedType inspectedType = InspectedType.Get(obj.GetType());
                for (int index = 0; index < obj.SerializedStateKeys.Count; ++index)
                {
                    InspectedProperty property = inspectedType.GetPropertyByName(obj.SerializedStateKeys[index]) ?? inspectedType.GetPropertyByFormerlySerializedName(obj.SerializedStateKeys[index]);
                    string serializedValue;
                    if (property != null && fiISerializedObjectUtility.SaveStateForProperty(obj, property, (BaseSerializer) serializer, (ISerializationOperator) serializationOperator, out serializedValue, ref success))
                    {
                        b1.Add(property.Name);
                        b2.Add(serializedValue);
                    }
                }
            }
            if (fiISerializedObjectUtility.AreListsDifferent((IList<string>) obj.SerializedStateKeys, (IList<string>) b1))
                obj.SerializedStateKeys = b1;
            if (fiISerializedObjectUtility.AreListsDifferent((IList<string>) obj.SerializedStateValues, (IList<string>) b2))
                obj.SerializedStateValues = b2;
            if (fiISerializedObjectUtility.AreListsDifferent((IList<UnityEngine.Object>) obj.SerializedObjectReferences, (IList<UnityEngine.Object>) serializationOperator.SerializedObjects))
                obj.SerializedObjectReferences = serializationOperator.SerializedObjects;
            if (obj is ScriptableObject)
                fiLateBindings.EditorUtility.SetDirty((UnityEngine.Object) obj);
            serializationCallbacks?.OnAfterSerialize();
            return success;
        }

        private static bool AreListsDifferent(IList<string> a, IList<string> b)
        {
            if (a == null || a.Count != b.Count)
                return true;
            int count = a.Count;
            for (int index = 0; index < count; ++index)
            {
                if (a[index] != b[index])
                    return true;
            }
            return false;
        }

        private static bool AreListsDifferent(IList<UnityEngine.Object> a, IList<UnityEngine.Object> b)
        {
            if (a == null || a.Count != b.Count)
                return true;
            int count = a.Count;
            for (int index = 0; index < count; ++index)
            {
                if (!object.ReferenceEquals((object) a[index], (object) b[index]))
                    return true;
            }
            return false;
        }

        public static bool RestoreState<TSerializer>(ISerializedObject obj) where TSerializer : BaseSerializer
        {
            bool flag = true;
            if (obj is ISerializationCallbacks serializationCallbacks)
                serializationCallbacks.OnBeforeDeserialize();
            if (obj.SerializedStateKeys == null)
                obj.SerializedStateKeys = new List<string>();
            if (obj.SerializedStateValues == null)
                obj.SerializedStateValues = new List<string>();
            if (obj.SerializedObjectReferences == null)
                obj.SerializedObjectReferences = new List<UnityEngine.Object>();
            if (obj.SerializedStateKeys.Count != obj.SerializedStateValues.Count && fiSettings.EmitWarnings)
                Debug.LogWarning((object) "Serialized key count does not equal value count; possible data corruption / bad manual edit?", obj as UnityEngine.Object);
            if (obj.SerializedStateKeys.Count == 0)
            {
                if (fiSettings.AutomaticReferenceInstantation)
                    fiISerializedObjectUtility.InstantiateReferences((object) obj, (InspectedType) null);
                return flag;
            }
            TSerializer serializer = fiSingletons.Get<TSerializer>();
            ListSerializationOperator serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = obj.SerializedObjectReferences;
            InspectedType inspectedType = InspectedType.Get(obj.GetType());
            for (int index = 0; index < obj.SerializedStateKeys.Count; ++index)
            {
                string serializedStateKey = obj.SerializedStateKeys[index];
                string serializedStateValue = obj.SerializedStateValues[index];
                InspectedProperty inspectedProperty = inspectedType.GetPropertyByName(serializedStateKey) ?? inspectedType.GetPropertyByFormerlySerializedName(serializedStateKey);
                if (inspectedProperty == null)
                {
                    if (fiSettings.EmitWarnings)
                        Debug.LogWarning((object) $"Unable to find serialized property with name={serializedStateKey} on type {(object) obj.GetType()}", obj as UnityEngine.Object);
                }
                else
                {
                    object obj1 = (object) null;
                    if (!string.IsNullOrEmpty(serializedStateValue))
                    {
                        try
                        {
                            obj1 = serializer.Deserialize(inspectedProperty.MemberInfo, serializedStateValue, (ISerializationOperator) serializationOperator);
                        }
                        catch (Exception ex)
                        {
                            flag = false;
                            Debug.LogError((object) $"Exception caught when deserializing property <{serializedStateKey}> in <{(object) obj}>\n{(object) ex}", obj as UnityEngine.Object);
                        }
                    }
                    try
                    {
                        inspectedProperty.Write((object) obj, obj1);
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        if (fiSettings.EmitWarnings)
                        {
                            Debug.LogWarning((object) "Caught exception when updating property value; see next message for the exception", obj as UnityEngine.Object);
                            Debug.LogError((object) ex);
                        }
                    }
                }
            }
            serializationCallbacks?.OnAfterDeserialize();
            obj.IsRestored = true;
            return flag;
        }

        private static void InstantiateReferences(object obj, InspectedType metadata)
        {
            if (metadata == null)
                metadata = InspectedType.Get(obj.GetType());
            if (metadata.IsCollection)
                return;
            List<InspectedProperty> properties = metadata.GetProperties(InspectedMemberFilters.InspectableMembers);
            for (int index = 0; index < properties.Count; ++index)
            {
                InspectedProperty inspectedProperty = properties[index];
                if (inspectedProperty.StorageType.Resolve().IsClass && !inspectedProperty.StorageType.Resolve().IsAbstract && inspectedProperty.Read(obj) == null)
                {
                    InspectedType metadata1 = InspectedType.Get(inspectedProperty.StorageType);
                    if (metadata1.HasDefaultConstructor)
                    {
                        object instance = metadata1.CreateInstance();
                        inspectedProperty.Write(obj, instance);
                        fiISerializedObjectUtility.InstantiateReferences(instance, metadata1);
                    }
                }
            }
        }
    }
}
