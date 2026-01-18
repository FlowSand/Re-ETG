using UnityEngine;

#nullable disable

public class AkTriggerMouseDown : AkTriggerBase
    {
        private void OnMouseDown()
        {
            if (this.triggerDelegate == null)
                return;
            this.triggerDelegate((GameObject) null);
        }
    }

