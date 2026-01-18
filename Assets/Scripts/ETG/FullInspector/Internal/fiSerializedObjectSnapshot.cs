using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullInspector.Internal
{
  public class fiSerializedObjectSnapshot
  {
    private readonly List<string> _keys;
    private readonly List<string> _values;
    private readonly List<Object> _objectReferences;

    public fiSerializedObjectSnapshot(ISerializedObject obj)
    {
      this._keys = new List<string>((IEnumerable<string>) obj.SerializedStateKeys);
      this._values = new List<string>((IEnumerable<string>) obj.SerializedStateValues);
      this._objectReferences = new List<Object>((IEnumerable<Object>) obj.SerializedObjectReferences);
    }

    public void RestoreSnapshot(ISerializedObject target)
    {
      target.SerializedStateKeys = new List<string>((IEnumerable<string>) this._keys);
      target.SerializedStateValues = new List<string>((IEnumerable<string>) this._values);
      target.SerializedObjectReferences = new List<Object>((IEnumerable<Object>) this._objectReferences);
      target.RestoreState();
    }

    public bool IsEmpty => this._keys.Count == 0 || this._values.Count == 0;

    public override bool Equals(object obj)
    {
      fiSerializedObjectSnapshot objA = obj as fiSerializedObjectSnapshot;
      return !object.ReferenceEquals((object) objA, (object) null) && fiSerializedObjectSnapshot.AreEqual<string>(this._keys, objA._keys) && fiSerializedObjectSnapshot.AreEqual<string>(this._values, objA._values) && fiSerializedObjectSnapshot.AreEqual<Object>(this._objectReferences, objA._objectReferences);
    }

    public override int GetHashCode()
    {
      return ((13 * 7 + this._keys.GetHashCode()) * 7 + this._values.GetHashCode()) * 7 + this._objectReferences.GetHashCode();
    }

    public static bool operator ==(fiSerializedObjectSnapshot a, fiSerializedObjectSnapshot b)
    {
      return object.Equals((object) a, (object) b);
    }

    public static bool operator !=(fiSerializedObjectSnapshot a, fiSerializedObjectSnapshot b)
    {
      return !object.Equals((object) a, (object) b);
    }

    private static bool AreEqual<T>(List<T> a, List<T> b)
    {
      if (a.Count != b.Count)
        return false;
      for (int index = 0; index < a.Count; ++index)
      {
        if (!EqualityComparer<T>.Default.Equals(a[index], b[index]))
          return false;
      }
      return true;
    }
  }
}
