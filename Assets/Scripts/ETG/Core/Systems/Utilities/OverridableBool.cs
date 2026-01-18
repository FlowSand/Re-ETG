using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class OverridableBool
    {
        public bool BaseValue;
        private List<OverridableBool.OverrideData> m_overrides = new List<OverridableBool.OverrideData>();

        public OverridableBool(bool defaultValue) => this.BaseValue = defaultValue;

        public void Debug()
        {
            for (int index = 0; index < this.m_overrides.Count; ++index)
            {
                string str = this.m_overrides[index].duration.HasValue ? this.m_overrides[index].duration.Value.ToString() : "null";
                UnityEngine.Debug.LogWarningFormat("override set: {0} (duration: {1})", (object) this.m_overrides[index], (object) str);
            }
        }

        public bool Value => this.m_overrides.Count > 0 ? !this.BaseValue : this.BaseValue;

        public bool HasOverride(string key)
        {
            for (int index = 0; index < this.m_overrides.Count; ++index)
            {
                if (this.m_overrides[index].key == key)
                    return true;
            }
            return false;
        }

        public void AddOverride(string key, float? duration = null)
        {
            for (int index = 0; index < this.m_overrides.Count; ++index)
            {
                if (this.m_overrides[index].key == key)
                {
                    if (!duration.HasValue || !this.m_overrides[index].duration.HasValue)
                    {
                        this.m_overrides[index].duration = new float?();
                        return;
                    }
                    this.m_overrides[index].duration = new float?(Mathf.Max(this.m_overrides[index].duration.Value, duration.Value));
                    return;
                }
            }
            this.m_overrides.Add(new OverridableBool.OverrideData()
            {
                key = key,
                duration = duration
            });
        }

        public void RemoveOverride(string key)
        {
            for (int index = 0; index < this.m_overrides.Count; ++index)
            {
                if (this.m_overrides[index].key == key)
                {
                    this.m_overrides.RemoveAt(index);
                    break;
                }
            }
        }

        public void SetOverride(string key, bool value, float? duration = null)
        {
            if (value != this.BaseValue)
            {
                this.AddOverride(key, duration);
            }
            else
            {
                if (duration.HasValue)
                    UnityEngine.Debug.LogWarningFormat("Trying to disable an override with a duration! {0} {1} {2}", (object) key, (object) value, (object) duration.Value);
                this.RemoveOverride(key);
            }
        }

        public void ClearOverrides() => this.m_overrides.Clear();

        public bool UpdateTimers(float deltaTime)
        {
            bool flag = false;
            for (int index = this.m_overrides.Count - 1; index >= 0; --index)
            {
                if (this.m_overrides[index].duration.HasValue)
                {
                    OverridableBool.OverrideData overrideData = this.m_overrides[index];
                    float? duration = overrideData.duration;
                    float? nullable1;
                    float? nullable2;
                    if (duration.HasValue)
                    {
                        nullable2 = new float?(duration.GetValueOrDefault() - deltaTime);
                    }
                    else
                    {
                        nullable1 = new float?();
                        nullable2 = nullable1;
                    }
                    overrideData.duration = nullable2;
                    nullable1 = this.m_overrides[index].duration;
                    if ((!nullable1.HasValue ? 0 : ((double) nullable1.GetValueOrDefault() <= 0.0 ? 1 : 0)) != 0)
                    {
                        this.m_overrides.RemoveAt(index);
                        flag = true;
                    }
                }
            }
            return flag;
        }

        private class OverrideData
        {
            public string key;
            public float? duration;
        }
    }

