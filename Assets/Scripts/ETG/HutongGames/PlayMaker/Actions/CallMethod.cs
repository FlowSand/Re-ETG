// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CallMethod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Call a method in a behaviour.")]
  [ActionCategory(ActionCategory.ScriptControl)]
  public class CallMethod : FsmStateAction
  {
    [ObjectType(typeof (MonoBehaviour))]
    [HutongGames.PlayMaker.Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
    public FsmObject behaviour;
    [HutongGames.PlayMaker.Tooltip("Name of the method to call on the component")]
    public FsmString methodName;
    [HutongGames.PlayMaker.Tooltip("Method paramters. NOTE: these must match the method's signature!")]
    public FsmVar[] parameters;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the result of the method call.")]
    [ActionSection("Store Result")]
    public FsmVar storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private FsmObject cachedBehaviour;
    private FsmString cachedMethodName;
    private System.Type cachedType;
    private MethodInfo cachedMethodInfo;
    private ParameterInfo[] cachedParameterInfo;
    private object[] parametersArray;
    private string errorString;

    public override void Reset()
    {
      this.behaviour = (FsmObject) null;
      this.methodName = (FsmString) null;
      this.parameters = (FsmVar[]) null;
      this.storeResult = (FsmVar) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.parametersArray = new object[this.parameters.Length];
      this.DoMethodCall();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoMethodCall();

    private void DoMethodCall()
    {
      if (this.behaviour.Value == (UnityEngine.Object) null)
        this.Finish();
      else if (this.NeedToUpdateCache() && !this.DoCache())
      {
        Debug.LogError((object) this.errorString);
        this.Finish();
      }
      else
      {
        object obj;
        if (this.cachedParameterInfo.Length == 0)
        {
          obj = this.cachedMethodInfo.Invoke((object) this.cachedBehaviour.Value, (object[]) null);
        }
        else
        {
          for (int index1 = 0; index1 < this.parameters.Length; ++index1)
          {
            FsmVar parameter = this.parameters[index1];
            parameter.UpdateValue();
            if (parameter.Type == VariableType.Array)
            {
              parameter.UpdateValue();
              object[] objArray = parameter.GetValue() as object[];
              Array instance = Array.CreateInstance(this.cachedParameterInfo[index1].ParameterType.GetElementType(), objArray.Length);
              for (int index2 = 0; index2 < objArray.Length; ++index2)
                instance.SetValue(objArray[index2], index2);
              this.parametersArray[index1] = (object) instance;
            }
            else
            {
              parameter.UpdateValue();
              this.parametersArray[index1] = parameter.GetValue();
            }
          }
          obj = this.cachedMethodInfo.Invoke((object) this.cachedBehaviour.Value, this.parametersArray);
        }
        if (this.storeResult == null || this.storeResult.IsNone || this.storeResult.Type == VariableType.Unknown)
          return;
        this.storeResult.SetValue(obj);
      }
    }

    private bool NeedToUpdateCache()
    {
      return this.cachedBehaviour == null || this.cachedMethodName == null || this.cachedBehaviour.Value != this.behaviour.Value || this.cachedBehaviour.Name != this.behaviour.Name || this.cachedMethodName.Value != this.methodName.Value || this.cachedMethodName.Name != this.methodName.Name;
    }

    private void ClearCache()
    {
      this.cachedBehaviour = (FsmObject) null;
      this.cachedMethodName = (FsmString) null;
      this.cachedType = (System.Type) null;
      this.cachedMethodInfo = (MethodInfo) null;
      this.cachedParameterInfo = (ParameterInfo[]) null;
    }

    private bool DoCache()
    {
      this.ClearCache();
      this.errorString = string.Empty;
      this.cachedBehaviour = new FsmObject(this.behaviour);
      this.cachedMethodName = new FsmString(this.methodName);
      if (this.cachedBehaviour.Value == (UnityEngine.Object) null)
      {
        if (!this.behaviour.UsesVariable || Application.isPlaying)
          this.errorString += "Behaviour is invalid!\n";
        this.Finish();
        return false;
      }
      this.cachedType = this.behaviour.Value.GetType();
      List<System.Type> typeList = new List<System.Type>(this.parameters.Length);
      foreach (FsmVar parameter in this.parameters)
        typeList.Add(parameter.RealType);
      this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, typeList.ToArray());
      if (this.cachedMethodInfo == null)
      {
        CallMethod callMethod = this;
        callMethod.errorString = $"{callMethod.errorString}Invalid Method Name or Parameters: {this.methodName.Value}\n";
        this.Finish();
        return false;
      }
      this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
      return true;
    }

    public override string ErrorCheck()
    {
      if (Application.isPlaying || !this.DoCache())
        return this.errorString;
      if (this.parameters.Length != this.cachedParameterInfo.Length)
        return $"Parameter count does not match method.\nMethod has {(object) this.cachedParameterInfo.Length} parameters.\nYou specified {(object) this.parameters.Length} paramaters.";
      for (int index = 0; index < this.parameters.Length; ++index)
      {
        System.Type realType = this.parameters[index].RealType;
        System.Type parameterType = this.cachedParameterInfo[index].ParameterType;
        if (!object.ReferenceEquals((object) realType, (object) parameterType))
          return $"Parameters do not match method signature.\nParameter {(object) (index + 1)} ({(object) realType}) should be of type: {(object) parameterType}";
      }
      if (object.ReferenceEquals((object) this.cachedMethodInfo.ReturnType, (object) typeof (void)))
      {
        if (!string.IsNullOrEmpty(this.storeResult.variableName))
          return "Method does not have return.\nSpecify 'none' in Store Result.";
      }
      else if (!object.ReferenceEquals((object) this.cachedMethodInfo.ReturnType, (object) this.storeResult.RealType))
        return "Store Result is of the wrong type.\nIt should be of type: " + (object) this.cachedMethodInfo.ReturnType;
      return string.Empty;
    }
  }
}
