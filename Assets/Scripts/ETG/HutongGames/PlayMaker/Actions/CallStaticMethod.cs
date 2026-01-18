using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [HutongGames.PlayMaker.Tooltip("Call a static method in a class.")]
    public class CallStaticMethod : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Full path to the class that contains the static method.")]
        public FsmString className;
        [HutongGames.PlayMaker.Tooltip("The static method to call.")]
        public FsmString methodName;
        [HutongGames.PlayMaker.Tooltip("Method paramters. NOTE: these must match the method's signature!")]
        public FsmVar[] parameters;
        [HutongGames.PlayMaker.Tooltip("Store the result of the method call.")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Store Result")]
        public FsmVar storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private System.Type cachedType;
        private string cachedClassName;
        private string cachedMethodName;
        private MethodInfo cachedMethodInfo;
        private ParameterInfo[] cachedParameterInfo;
        private object[] parametersArray;
        private string errorString;

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
            if (this.className == null || string.IsNullOrEmpty(this.className.Value))
            {
                this.Finish();
            }
            else
            {
                if (this.cachedClassName != this.className.Value || this.cachedMethodName != this.methodName.Value)
                {
                    this.errorString = string.Empty;
                    if (!this.DoCache())
                    {
                        Debug.LogError((object) this.errorString);
                        this.Finish();
                        return;
                    }
                }
                object obj;
                if (this.cachedParameterInfo.Length == 0)
                {
                    obj = this.cachedMethodInfo.Invoke((object) null, (object[]) null);
                }
                else
                {
                    for (int index = 0; index < this.parameters.Length; ++index)
                    {
                        FsmVar parameter = this.parameters[index];
                        parameter.UpdateValue();
                        this.parametersArray[index] = parameter.GetValue();
                    }
                    obj = this.cachedMethodInfo.Invoke((object) null, this.parametersArray);
                }
                this.storeResult.SetValue(obj);
            }
        }

        private bool DoCache()
        {
            this.cachedType = ReflectionUtils.GetGlobalType(this.className.Value);
            if (this.cachedType == null)
            {
                CallStaticMethod callStaticMethod = this;
                callStaticMethod.errorString = $"{callStaticMethod.errorString}Class is invalid: {this.className.Value}\n";
                this.Finish();
                return false;
            }
            this.cachedClassName = this.className.Value;
            List<System.Type> typeList = new List<System.Type>(this.parameters.Length);
            foreach (FsmVar parameter in this.parameters)
                typeList.Add(parameter.RealType);
            this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, typeList.ToArray());
            if (this.cachedMethodInfo == null)
            {
                CallStaticMethod callStaticMethod = this;
                callStaticMethod.errorString = $"{callStaticMethod.errorString}Invalid Method Name or Parameters: {this.methodName.Value}\n";
                this.Finish();
                return false;
            }
            this.cachedMethodName = this.methodName.Value;
            this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
            return true;
        }

        public override string ErrorCheck()
        {
            this.errorString = string.Empty;
            this.DoCache();
            if (!string.IsNullOrEmpty(this.errorString))
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
