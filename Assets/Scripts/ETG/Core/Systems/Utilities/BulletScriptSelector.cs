// Decompiled with JetBrains decompiler
// Type: BulletScriptSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using FullInspector.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class BulletScriptSelector : fiInspectorOnly, tkCustomEditor
    {
      public string scriptTypeName;
      private static System.Type[] _types;
      private static string[] _typeNames;
      private static GUIContent[] _labels;

      public Bullet CreateInstance()
      {
        System.Type type = System.Type.GetType(this.scriptTypeName);
        if (type != null)
          return (Bullet) Activator.CreateInstance(type);
        Debug.LogError((object) ("Unknown type! " + this.scriptTypeName));
        return (Bullet) null;
      }

      public bool IsNull => string.IsNullOrEmpty(this.scriptTypeName) || this.scriptTypeName == "null";

      public BulletScriptSelector Clone()
      {
        return new BulletScriptSelector()
        {
          scriptTypeName = this.scriptTypeName
        };
      }

      tkControlEditor tkCustomEditor.GetEditor()
      {
        return new tkControlEditor((tkIControl) new tk<BulletScriptSelector, tkDefaultContext>.Popup(tk<BulletScriptSelector, tkDefaultContext>.Val<fiGUIContent>((tk<BulletScriptSelector, tkDefaultContext>.Value<fiGUIContent>.Generator) ((o, c) => new fiGUIContent(c.Label.text))), tk<BulletScriptSelector, tkDefaultContext>.Val<GUIContent[]>((tk<BulletScriptSelector, tkDefaultContext>.Value<GUIContent[]>.GeneratorNoContext) (o => o.GetLabels())), tk<BulletScriptSelector, tkDefaultContext>.Val<int>((tk<BulletScriptSelector, tkDefaultContext>.Value<int>.GeneratorNoContext) (o => string.IsNullOrEmpty(o.scriptTypeName) ? 0 : Math.Max(0, Array.FindIndex<string>(o.GetTypeNames(), (Predicate<string>) (gc => gc == o.scriptTypeName))))), (tk<BulletScriptSelector, tkDefaultContext>.Popup.OnSelectionChanged) ((o, c, v) =>
        {
          o.scriptTypeName = o.GetTypeNames()[v];
          if (o.scriptTypeName == "null")
            o.scriptTypeName = (string) null;
          return o;
        })));
      }

      public GUIContent[] GetLabels()
      {
        if (BulletScriptSelector._types == null)
          this.InitEditorCache();
        return BulletScriptSelector._labels;
      }

      public string[] GetTypeNames()
      {
        if (BulletScriptSelector._types == null)
          this.InitEditorCache();
        return BulletScriptSelector._typeNames;
      }

      private void InitEditorCache()
      {
        List<System.Type> typeList = new List<System.Type>();
        typeList.Add((System.Type) null);
        typeList.AddRange(fiRuntimeReflectionUtility.AllSimpleCreatableTypesDerivingFrom(typeof (Script)));
        typeList.Remove(typeof (Script));
        typeList.AddRange(fiRuntimeReflectionUtility.AllSimpleCreatableTypesDerivingFrom(typeof (ScriptLite)));
        typeList.Remove(typeof (ScriptLite));
        BulletScriptSelector._types = typeList.ToArray();
        BulletScriptSelector._typeNames = ((IEnumerable<System.Type>) BulletScriptSelector._types).Select<System.Type, string>((Func<System.Type, string>) (t => t == null ? "null" : t.FullName)).ToArray<string>();
        BulletScriptSelector._labels = ((IEnumerable<System.Type>) BulletScriptSelector._types).Select<System.Type, GUIContent>((Func<System.Type, GUIContent>) (t =>
        {
          if (t == null)
            return new GUIContent("null");
          return Attribute.GetCustomAttribute((MemberInfo) t, typeof (InspectorDropdownNameAttribute)) is InspectorDropdownNameAttribute customAttribute2 ? new GUIContent(customAttribute2.DisplayName) : new GUIContent(t.FullName);
        })).ToArray<GUIContent>();
      }
    }

}
