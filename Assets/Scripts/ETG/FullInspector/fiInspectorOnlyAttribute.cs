using System;

using UnityEngine;

#nullable disable
namespace FullInspector
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
    public sealed class fiInspectorOnlyAttribute : PropertyAttribute
    {
    }
}
