using System;

#nullable disable

public class RoomSealDisabler : BraveBehaviour
    {
        public bool MatchRoomState = true;

        private void Start()
        {
            this.transform.position.GetAbsoluteRoom().OnSealChanged += new Action<bool>(this.HandleSealStateChanged);
            this.HandleSealStateChanged(false);
        }

        private void HandleSealStateChanged(bool isSealed)
        {
            if (this.MatchRoomState)
            {
                if ((bool) (UnityEngine.Object) this.specRigidbody)
                    this.specRigidbody.enabled = isSealed;
                this.gameObject.SetActive(isSealed);
            }
            else
            {
                if ((bool) (UnityEngine.Object) this.specRigidbody)
                    this.specRigidbody.enabled = !isSealed;
                this.gameObject.SetActive(!isSealed);
            }
        }
    }

