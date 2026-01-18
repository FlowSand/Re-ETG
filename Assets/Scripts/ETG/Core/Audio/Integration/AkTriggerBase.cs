// Decompiled with JetBrains decompiler
// Type: AkTriggerBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public abstract class AkTriggerBase : MonoBehaviour
  {
    public AkTriggerBase.Trigger triggerDelegate;

    public static Dictionary<uint, string> GetAllDerivedTypes()
    {
      Dictionary<uint, string> allDerivedTypes = new Dictionary<uint, string>();
      System.Type c = typeof (AkTriggerBase);
      System.Type[] types = c.Assembly.GetTypes();
      for (int index = 0; index < types.Length; ++index)
      {
        if (types[index].IsClass && (types[index].IsSubclassOf(c) || c.IsAssignableFrom(types[index]) && c != types[index]))
        {
          string name = types[index].Name;
          allDerivedTypes.Add(AkUtilities.ShortIDGenerator.Compute(name), name);
        }
      }
      allDerivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Awake"), "Awake");
      allDerivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Start"), "Start");
      allDerivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Destroy"), "Destroy");
      return allDerivedTypes;
    }

public delegate void Trigger(GameObject in_gameObject);
  }

