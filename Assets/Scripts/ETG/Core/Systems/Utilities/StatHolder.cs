using System;

using UnityEngine;

#nullable disable

public class StatHolder : MonoBehaviour
    {
        public bool RequiresPlayerItemActive;
        public StatModifier[] modifiers;

        private void Start()
        {
            if (!this.RequiresPlayerItemActive)
                return;
            PlayerItem component = this.GetComponent<PlayerItem>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            component.OnActivationStatusChanged += (Action<PlayerItem>) (a =>
            {
                if (!(bool) (UnityEngine.Object) a.LastOwner)
                    return;
                a.LastOwner.stats.RecalculateStats(a.LastOwner);
            });
        }
    }

