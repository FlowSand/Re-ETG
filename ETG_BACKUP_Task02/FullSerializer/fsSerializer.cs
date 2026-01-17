// Decompiled with JetBrains decompiler
// Type: FullSerializer.fsSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace FullSerializer;

public class fsSerializer
{
  private static HashSet<string> _reservedKeywords = new HashSet<string>()
  {
    "$ref",
    "$id",
    "$type",
    "$version",
    "$content"
  };
  private const string Key_ObjectReference = "$ref";
  private const string Key_ObjectDefinition = "$id";
  private const string Key_InstanceType = "$type";
  private const string Key_Version = "$version";
  private const string Key_Content = "$content";
  private Dictionary<Type, fsBaseConverter> _cachedConverters;
  private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;
  private readonly List<fsConverter> _availableConverters;
  private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;
  private readonly List<fsObjectProcessor> _processors;
  private readonly fsCyclicReferenceManager _references;
  private readonly fsSerializer.fsLazyCycleDefinitionWriter _lazyReferenceWriter;
  public fsContext Context;

  public fsSerializer()
  {
    this._cachedConverters = new Dictionary<Type, fsBaseConverter>();
    this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
    this._references = new fsCyclicReferenceManager();
    this._lazyReferenceWriter = new fsSerializer.fsLazyCycleDefinitionWriter();
    List<fsConverter> fsConverterList1 = new List<fsConverter>();
    List<fsConverter> fsConverterList2 = fsConverterList1;
    fsNullableConverter nullableConverter1 = new fsNullableConverter();
    nullableConverter1.Serializer = this;
    fsNullableConverter nullableConverter2 = nullableConverter1;
    fsConverterList2.Add((fsConverter) nullableConverter2);
    List<fsConverter> fsConverterList3 = fsConverterList1;
    fsGuidConverter fsGuidConverter1 = new fsGuidConverter();
    fsGuidConverter1.Serializer = this;
    fsGuidConverter fsGuidConverter2 = fsGuidConverter1;
    fsConverterList3.Add((fsConverter) fsGuidConverter2);
    List<fsConverter> fsConverterList4 = fsConverterList1;
    fsTypeConverter fsTypeConverter1 = new fsTypeConverter();
    fsTypeConverter1.Serializer = this;
    fsTypeConverter fsTypeConverter2 = fsTypeConverter1;
    fsConverterList4.Add((fsConverter) fsTypeConverter2);
    List<fsConverter> fsConverterList5 = fsConverterList1;
    fsDateConverter fsDateConverter1 = new fsDateConverter();
    fsDateConverter1.Serializer = this;
    fsDateConverter fsDateConverter2 = fsDateConverter1;
    fsConverterList5.Add((fsConverter) fsDateConverter2);
    List<fsConverter> fsConverterList6 = fsConverterList1;
    fsEnumConverter fsEnumConverter1 = new fsEnumConverter();
    fsEnumConverter1.Serializer = this;
    fsEnumConverter fsEnumConverter2 = fsEnumConverter1;
    fsConverterList6.Add((fsConverter) fsEnumConverter2);
    List<fsConverter> fsConverterList7 = fsConverterList1;
    fsPrimitiveConverter primitiveConverter1 = new fsPrimitiveConverter();
    primitiveConverter1.Serializer = this;
    fsPrimitiveConverter primitiveConverter2 = primitiveConverter1;
    fsConverterList7.Add((fsConverter) primitiveConverter2);
    List<fsConverter> fsConverterList8 = fsConverterList1;
    fsArrayConverter fsArrayConverter1 = new fsArrayConverter();
    fsArrayConverter1.Serializer = this;
    fsArrayConverter fsArrayConverter2 = fsArrayConverter1;
    fsConverterList8.Add((fsConverter) fsArrayConverter2);
    List<fsConverter> fsConverterList9 = fsConverterList1;
    fsDictionaryConverter dictionaryConverter1 = new fsDictionaryConverter();
    dictionaryConverter1.Serializer = this;
    fsDictionaryConverter dictionaryConverter2 = dictionaryConverter1;
    fsConverterList9.Add((fsConverter) dictionaryConverter2);
    List<fsConverter> fsConverterList10 = fsConverterList1;
    fsIEnumerableConverter ienumerableConverter1 = new fsIEnumerableConverter();
    ienumerableConverter1.Serializer = this;
    fsIEnumerableConverter ienumerableConverter2 = ienumerableConverter1;
    fsConverterList10.Add((fsConverter) ienumerableConverter2);
    List<fsConverter> fsConverterList11 = fsConverterList1;
    fsKeyValuePairConverter valuePairConverter1 = new fsKeyValuePairConverter();
    valuePairConverter1.Serializer = this;
    fsKeyValuePairConverter valuePairConverter2 = valuePairConverter1;
    fsConverterList11.Add((fsConverter) valuePairConverter2);
    List<fsConverter> fsConverterList12 = fsConverterList1;
    fsWeakReferenceConverter referenceConverter1 = new fsWeakReferenceConverter();
    referenceConverter1.Serializer = this;
    fsWeakReferenceConverter referenceConverter2 = referenceConverter1;
    fsConverterList12.Add((fsConverter) referenceConverter2);
    List<fsConverter> fsConverterList13 = fsConverterList1;
    fsReflectedConverter reflectedConverter1 = new fsReflectedConverter();
    reflectedConverter1.Serializer = this;
    fsReflectedConverter reflectedConverter2 = reflectedConverter1;
    fsConverterList13.Add((fsConverter) reflectedConverter2);
    this._availableConverters = fsConverterList1;
    this._availableDirectConverters = new Dictionary<Type, fsDirectConverter>();
    this._processors = new List<fsObjectProcessor>()
    {
      (fsObjectProcessor) new fsSerializationCallbackProcessor()
    };
    this.Context = new fsContext();
    foreach (Type converter in fsConverterRegistrar.Converters)
      this.AddConverter((fsBaseConverter) Activator.CreateInstance(converter));
  }

  public static bool IsReservedKeyword(string key) => fsSerializer._reservedKeywords.Contains(key);

  private static bool IsObjectReference(fsData data)
  {
    return data.IsDictionary && data.AsDictionary.ContainsKey("$ref");
  }

  private static bool IsObjectDefinition(fsData data)
  {
    return data.IsDictionary && data.AsDictionary.ContainsKey("$id");
  }

  private static bool IsVersioned(fsData data)
  {
    return data.IsDictionary && data.AsDictionary.ContainsKey("$version");
  }

  private static bool IsTypeSpecified(fsData data)
  {
    return data.IsDictionary && data.AsDictionary.ContainsKey("$type");
  }

  private static bool IsWrappedData(fsData data)
  {
    return data.IsDictionary && data.AsDictionary.ContainsKey("$content");
  }

  public static void StripDeserializationMetadata(ref fsData data)
  {
    if (data.IsDictionary && data.AsDictionary.ContainsKey("$content"))
      data = data.AsDictionary["$content"];
    if (!data.IsDictionary)
      return;
    Dictionary<string, fsData> asDictionary = data.AsDictionary;
    asDictionary.Remove("$ref");
    asDictionary.Remove("$id");
    asDictionary.Remove("$type");
    asDictionary.Remove("$version");
  }

  private static void ConvertLegacyData(ref fsData data)
  {
    if (!data.IsDictionary)
      return;
    Dictionary<string, fsData> asDictionary = data.AsDictionary;
    if (asDictionary.Count > 2)
      return;
    string key1 = "ReferenceId";
    string key2 = "SourceId";
    string key3 = "Data";
    string key4 = "Type";
    string key5 = "Data";
    if (asDictionary.Count == 2 && asDictionary.ContainsKey(key4) && asDictionary.ContainsKey(key5))
    {
      data = asDictionary[key5];
      fsSerializer.EnsureDictionary(data);
      fsSerializer.ConvertLegacyData(ref data);
      data.AsDictionary["$type"] = asDictionary[key4];
    }
    else if (asDictionary.Count == 2 && asDictionary.ContainsKey(key2) && asDictionary.ContainsKey(key3))
    {
      data = asDictionary[key3];
      fsSerializer.EnsureDictionary(data);
      fsSerializer.ConvertLegacyData(ref data);
      data.AsDictionary["$id"] = asDictionary[key2];
    }
    else
    {
      if (asDictionary.Count != 1 || !asDictionary.ContainsKey(key1))
        return;
      data = fsData.CreateDictionary();
      data.AsDictionary["$ref"] = asDictionary[key1];
    }
  }

  private static void Invoke_OnBeforeSerialize(
    List<fsObjectProcessor> processors,
    Type storageType,
    object instance)
  {
    for (int index = 0; index < processors.Count; ++index)
      processors[index].OnBeforeSerialize(storageType, instance);
  }

  private static void Invoke_OnAfterSerialize(
    List<fsObjectProcessor> processors,
    Type storageType,
    object instance,
    ref fsData data)
  {
    for (int index = processors.Count - 1; index >= 0; --index)
      processors[index].OnAfterSerialize(storageType, instance, ref data);
  }

  private static void Invoke_OnBeforeDeserialize(
    List<fsObjectProcessor> processors,
    Type storageType,
    ref fsData data)
  {
    for (int index = 0; index < processors.Count; ++index)
      processors[index].OnBeforeDeserialize(storageType, ref data);
  }

  private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(
    List<fsObjectProcessor> processors,
    Type storageType,
    object instance,
    ref fsData data)
  {
    for (int index = 0; index < processors.Count; ++index)
      processors[index].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
  }

  private static void Invoke_OnAfterDeserialize(
    List<fsObjectProcessor> processors,
    Type storageType,
    object instance)
  {
    for (int index = processors.Count - 1; index >= 0; --index)
      processors[index].OnAfterDeserialize(storageType, instance);
  }

  private static void EnsureDictionary(fsData data)
  {
    if (data.IsDictionary)
      return;
    fsData fsData = data.Clone();
    data.BecomeDictionary();
    data.AsDictionary["$content"] = fsData;
  }

  public void AddProcessor(fsObjectProcessor processor)
  {
    this._processors.Add(processor);
    this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
  }

  private List<fsObjectProcessor> GetProcessors(Type type)
  {
    fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>((MemberInfo) type);
    List<fsObjectProcessor> processors;
    if (attribute != null && attribute.Processor != null)
    {
      fsObjectProcessor instance = (fsObjectProcessor) Activator.CreateInstance(attribute.Processor);
      processors = new List<fsObjectProcessor>();
      processors.Add(instance);
      this._cachedProcessors[type] = processors;
    }
    else if (!this._cachedProcessors.TryGetValue(type, out processors))
    {
      processors = new List<fsObjectProcessor>();
      for (int index = 0; index < this._processors.Count; ++index)
      {
        fsObjectProcessor processor = this._processors[index];
        if (processor.CanProcess(type))
          processors.Add(processor);
      }
      this._cachedProcessors[type] = processors;
    }
    return processors;
  }

  public void AddConverter(fsBaseConverter converter)
  {
    if (converter.Serializer != null)
      throw new InvalidOperationException("Cannot add a single converter instance to multiple fsConverters -- please construct a new instance for " + (object) converter);
    switch (converter)
    {
      case fsDirectConverter _:
        fsDirectConverter fsDirectConverter = (fsDirectConverter) converter;
        this._availableDirectConverters[fsDirectConverter.ModelType] = fsDirectConverter;
        break;
      case fsConverter _:
        this._availableConverters.Insert(0, (fsConverter) converter);
        break;
      default:
        throw new InvalidOperationException($"Unable to add converter {(object) converter}; the type association strategy is unknown. Please use either fsDirectConverter or fsConverter as your base type.");
    }
    converter.Serializer = this;
    this._cachedConverters = new Dictionary<Type, fsBaseConverter>();
  }

  private fsBaseConverter GetConverter(Type type)
  {
    fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>((MemberInfo) type);
    fsBaseConverter fsBaseConverter;
    if (attribute != null && attribute.Converter != null)
    {
      fsBaseConverter = (fsBaseConverter) Activator.CreateInstance(attribute.Converter);
      fsBaseConverter.Serializer = this;
      this._cachedConverters[type] = fsBaseConverter;
    }
    else if (!this._cachedConverters.TryGetValue(type, out fsBaseConverter))
    {
      if (this._availableDirectConverters.ContainsKey(type))
      {
        fsBaseConverter = (fsBaseConverter) this._availableDirectConverters[type];
        this._cachedConverters[type] = fsBaseConverter;
      }
      else
      {
        for (int index = 0; index < this._availableConverters.Count; ++index)
        {
          if (this._availableConverters[index].CanProcess(type))
          {
            fsBaseConverter = (fsBaseConverter) this._availableConverters[index];
            this._cachedConverters[type] = fsBaseConverter;
            break;
          }
        }
      }
    }
    return fsBaseConverter != null ? fsBaseConverter : throw new InvalidOperationException("Internal error -- could not find a converter for " + (object) type);
  }

  public fsResult TrySerialize<T>(T instance, out fsData data)
  {
    return this.TrySerialize(typeof (T), (object) instance, out data);
  }

  public fsResult TryDeserialize<T>(fsData data, ref T instance)
  {
    object result = (object) instance;
    fsResult fsResult = this.TryDeserialize(data, typeof (T), ref result);
    if (fsResult.Succeeded)
      instance = (T) result;
    return fsResult;
  }

  public fsResult TrySerialize(Type storageType, object instance, out fsData data)
  {
    List<fsObjectProcessor> processors = this.GetProcessors(instance != null ? instance.GetType() : storageType);
    fsSerializer.Invoke_OnBeforeSerialize(processors, storageType, instance);
    if (object.ReferenceEquals(instance, (object) null))
    {
      data = new fsData();
      fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
      return fsResult.Success;
    }
    fsResult fsResult = this.InternalSerialize_1_ProcessCycles(storageType, instance, out data);
    fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
    return fsResult;
  }

  private fsResult InternalSerialize_1_ProcessCycles(
    Type storageType,
    object instance,
    out fsData data)
  {
    try
    {
      this._references.Enter();
      if (!this.GetConverter(instance.GetType()).RequestCycleSupport(instance.GetType()))
        return this.InternalSerialize_2_Inheritance(storageType, instance, out data);
      if (this._references.IsReference(instance))
      {
        data = fsData.CreateDictionary();
        this._lazyReferenceWriter.WriteReference(this._references.GetReferenceId(instance), data.AsDictionary);
        return fsResult.Success;
      }
      this._references.MarkSerialized(instance);
      fsResult fsResult = this.InternalSerialize_2_Inheritance(storageType, instance, out data);
      if (fsResult.Failed)
        return fsResult;
      this._lazyReferenceWriter.WriteDefinition(this._references.GetReferenceId(instance), data);
      return fsResult;
    }
    finally
    {
      if (this._references.Exit())
        this._lazyReferenceWriter.Clear();
    }
  }

  private fsResult InternalSerialize_2_Inheritance(
    Type storageType,
    object instance,
    out fsData data)
  {
    fsResult fsResult = this.InternalSerialize_3_ProcessVersioning(instance, out data);
    if (fsResult.Failed || storageType == instance.GetType() || !this.GetConverter(storageType).RequestInheritanceSupport(storageType))
      return fsResult;
    fsSerializer.EnsureDictionary(data);
    data.AsDictionary["$type"] = new fsData(instance.GetType().FullName);
    return fsResult;
  }

  private fsResult InternalSerialize_3_ProcessVersioning(object instance, out fsData data)
  {
    fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(instance.GetType());
    if (!versionedType.HasValue)
      return this.InternalSerialize_4_Converter(instance, out data);
    fsVersionedType fsVersionedType = versionedType.Value;
    fsResult fsResult = this.InternalSerialize_4_Converter(instance, out data);
    if (fsResult.Failed)
      return fsResult;
    fsSerializer.EnsureDictionary(data);
    data.AsDictionary["$version"] = new fsData(fsVersionedType.VersionString);
    return fsResult;
  }

  private fsResult InternalSerialize_4_Converter(object instance, out fsData data)
  {
    Type type = instance.GetType();
    return this.GetConverter(type).TrySerialize(instance, out data, type);
  }

  public fsResult TryDeserialize(fsData data, Type storageType, ref object result)
  {
    if (data.IsNull)
    {
      result = (object) null;
      List<fsObjectProcessor> processors = this.GetProcessors(storageType);
      fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
      fsSerializer.Invoke_OnAfterDeserialize(processors, storageType, (object) null);
      return fsResult.Success;
    }
    fsSerializer.ConvertLegacyData(ref data);
    try
    {
      this._references.Enter();
      List<fsObjectProcessor> processors;
      fsResult fsResult = this.InternalDeserialize_1_CycleReference(data, storageType, ref result, out processors);
      if (fsResult.Succeeded)
        fsSerializer.Invoke_OnAfterDeserialize(processors, storageType, result);
      return fsResult;
    }
    finally
    {
      this._references.Exit();
    }
  }

  private fsResult InternalDeserialize_1_CycleReference(
    fsData data,
    Type storageType,
    ref object result,
    out List<fsObjectProcessor> processors)
  {
    if (!fsSerializer.IsObjectReference(data))
      return this.InternalDeserialize_2_Version(data, storageType, ref result, out processors);
    int id = int.Parse(data.AsDictionary["$ref"].AsString);
    result = this._references.GetReferenceObject(id);
    processors = this.GetProcessors(result.GetType());
    return fsResult.Success;
  }

  private fsResult InternalDeserialize_2_Version(
    fsData data,
    Type storageType,
    ref object result,
    out List<fsObjectProcessor> processors)
  {
    if (fsSerializer.IsVersioned(data))
    {
      string asString = data.AsDictionary["$version"].AsString;
      fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(storageType);
      if (versionedType.HasValue && versionedType.Value.VersionString != asString)
      {
        List<fsVersionedType> path;
        fsResult fsResult1 = fsResult.Success + fsVersionManager.GetVersionImportPath(asString, versionedType.Value, out path);
        if (fsResult1.Failed)
        {
          processors = this.GetProcessors(storageType);
          return fsResult1;
        }
        fsResult fsResult2 = fsResult1 + this.InternalDeserialize_3_Inheritance(data, path[0].ModelType, ref result, out processors);
        if (fsResult2.Failed)
          return fsResult2;
        for (int index = 1; index < path.Count; ++index)
          result = path[index].Migrate(result);
        processors = this.GetProcessors(fsResult2.GetType());
        return fsResult2;
      }
    }
    return this.InternalDeserialize_3_Inheritance(data, storageType, ref result, out processors);
  }

  private fsResult InternalDeserialize_3_Inheritance(
    fsData data,
    Type storageType,
    ref object result,
    out List<fsObjectProcessor> processors)
  {
    fsResult success = fsResult.Success;
    processors = this.GetProcessors(storageType);
    fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
    Type type1 = storageType;
    if (fsSerializer.IsTypeSpecified(data))
    {
      fsData fsData = data.AsDictionary["$type"];
      if (!fsData.IsString)
      {
        success.AddMessage($"$type value must be a string (in {(object) data})");
      }
      else
      {
        string asString = fsData.AsString;
        Type type2 = fsTypeLookup.GetType(asString);
        if (type2 == null)
          success.AddMessage($"Unable to locate specified type \"{asString}\"");
        else if (!storageType.IsAssignableFrom(type2))
          success.AddMessage($"Ignoring type specifier; a field/property of type {(object) storageType} cannot hold an instance of {(object) type2}");
        else
          type1 = type2;
      }
    }
    if (object.ReferenceEquals(result, (object) null) || result.GetType() != type1)
      result = this.GetConverter(type1).CreateInstance(data, type1);
    fsSerializer.Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);
    fsResult fsResult;
    return fsResult = success + this.InternalDeserialize_4_Cycles(data, type1, ref result);
  }

  private fsResult InternalDeserialize_4_Cycles(fsData data, Type resultType, ref object result)
  {
    if (fsSerializer.IsObjectDefinition(data))
      this._references.AddReferenceWithId(int.Parse(data.AsDictionary["$id"].AsString), result);
    return this.InternalDeserialize_5_Converter(data, resultType, ref result);
  }

  private fsResult InternalDeserialize_5_Converter(fsData data, Type resultType, ref object result)
  {
    if (fsSerializer.IsWrappedData(data))
      data = data.AsDictionary["$content"];
    return this.GetConverter(resultType).TryDeserialize(data, ref result, resultType);
  }

  internal class fsLazyCycleDefinitionWriter
  {
    private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();
    private HashSet<int> _references = new HashSet<int>();

    public void WriteDefinition(int id, fsData data)
    {
      if (this._references.Contains(id))
      {
        fsSerializer.EnsureDictionary(data);
        data.AsDictionary["$id"] = new fsData(id.ToString());
      }
      else
        this._pendingDefinitions[id] = data;
    }

    public void WriteReference(int id, Dictionary<string, fsData> dict)
    {
      if (this._pendingDefinitions.ContainsKey(id))
      {
        fsData pendingDefinition = this._pendingDefinitions[id];
        fsSerializer.EnsureDictionary(pendingDefinition);
        pendingDefinition.AsDictionary["$id"] = new fsData(id.ToString());
        this._pendingDefinitions.Remove(id);
      }
      else
        this._references.Add(id);
      dict["$ref"] = new fsData(id.ToString());
    }

    public void Clear() => this._pendingDefinitions.Clear();
  }
}
