using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FullSerializer.Internal;

#nullable disable
namespace FullInspector.Internal
{
    public class fiAttributeProvider : MemberInfo
    {
        private readonly object[] _attributes;

        private fiAttributeProvider(object[] attributes) => this._attributes = attributes;

        public static MemberInfo Create(params object[] attributes)
        {
            return (MemberInfo) new fiAttributeProvider(attributes);
        }

        public override Type DeclaringType => throw new NotSupportedException();

        public override MemberTypes MemberType => throw new NotSupportedException();

        public override string Name => throw new NotSupportedException();

        public override Type ReflectedType => throw new NotSupportedException();

        public override object[] GetCustomAttributes(bool inherit) => this._attributes;

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return ((IEnumerable<object>) this._attributes).Where<object>((Func<object, bool>) (attr => attr.GetType().Resolve().IsAssignableFrom(attributeType.Resolve()))).ToArray<object>();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return ((IEnumerable<object>) this._attributes).Where<object>((Func<object, bool>) (attr => attr.GetType().Resolve().IsAssignableFrom(attributeType.Resolve()))).Any<object>();
        }
    }
}
