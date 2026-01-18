using System;

using UnityEngine;

#nullable disable
namespace InControl
{
    public class OneAxisInputControl : IInputControl
    {
        private float sensitivity = 1f;
        private float lowerDeadZone;
        private float upperDeadZone = 1f;
        private float stateThreshold;
        public float FirstRepeatDelay = 0.8f;
        public float RepeatDelay = 0.1f;
        public bool Raw;
        internal bool Enabled = true;
        internal bool Suppressed;
        private ulong pendingTick;
        private bool pendingCommit;
        private float nextRepeatTime;
        private float lastPressedTime;
        private float lastReleasedTime;
        private bool wasRepeated;
        private bool clearInputState;
        private InputControlState lastState;
        private InputControlState nextState;
        private InputControlState thisState;
        private float startRepeatTime;
        private float nextTimeForRepeat;

        public ulong UpdateTick { get; protected set; }

        private void PrepareForUpdate(ulong updateTick)
        {
            if (this.IsNull)
                return;
            if (updateTick < this.pendingTick)
                throw new InvalidOperationException("Cannot be updated with an earlier tick.");
            if (this.pendingCommit && (long) updateTick != (long) this.pendingTick)
                throw new InvalidOperationException("Cannot be updated for a new tick until pending tick is committed.");
            if (updateTick <= this.pendingTick)
                return;
            this.lastState = this.thisState;
            this.nextState.Reset();
            this.pendingTick = updateTick;
            this.pendingCommit = true;
        }

        public bool UpdateWithState(bool state, ulong updateTick, float deltaTime)
        {
            if (this.IsNull)
                return false;
            this.PrepareForUpdate(updateTick);
            this.nextState.Set(state || this.nextState.State);
            return state;
        }

        public bool UpdateWithValue(float value, ulong updateTick, float deltaTime)
        {
            if (this.IsNull)
                return false;
            this.PrepareForUpdate(updateTick);
            if ((double) Utility.Abs(value) <= (double) Utility.Abs(this.nextState.RawValue))
                return false;
            this.nextState.RawValue = value;
            if (!this.Raw)
                value = Utility.ApplyDeadZone(value, this.lowerDeadZone, this.upperDeadZone);
            this.nextState.Set(value, this.stateThreshold);
            return true;
        }

        internal bool UpdateWithRawValue(float value, ulong updateTick, float deltaTime)
        {
            if (this.IsNull)
                return false;
            this.Raw = true;
            this.PrepareForUpdate(updateTick);
            if ((double) Utility.Abs(value) <= (double) Utility.Abs(this.nextState.RawValue))
                return false;
            this.nextState.RawValue = value;
            this.nextState.Set(value, this.stateThreshold);
            return true;
        }

        internal void SetValue(float value, ulong updateTick)
        {
            if (this.IsNull)
                return;
            if (updateTick > this.pendingTick)
            {
                this.lastState = this.thisState;
                this.nextState.Reset();
                this.pendingTick = updateTick;
                this.pendingCommit = true;
            }
            this.nextState.RawValue = value;
            this.nextState.Set(value, this.StateThreshold);
        }

        public void ClearInputState()
        {
            this.lastState.Reset();
            this.thisState.Reset();
            this.nextState.Reset();
            this.wasRepeated = false;
            this.clearInputState = true;
        }

        public void Commit()
        {
            if (this.IsNull)
                return;
            this.pendingCommit = false;
            this.thisState = this.nextState;
            if (this.Suppressed && !this.thisState.State)
            {
                this.ClearInputState();
                this.Suppressed = false;
            }
            if (this.clearInputState)
            {
                this.lastState = this.nextState;
                this.UpdateTick = this.pendingTick;
                this.clearInputState = false;
            }
            else
            {
                bool state1 = this.lastState.State;
                bool state2 = this.thisState.State;
                this.wasRepeated = false;
                if (state1 && !state2)
                {
                    this.nextRepeatTime = 0.0f;
                    this.lastReleasedTime = Time.realtimeSinceStartup;
                }
                else if (state2)
                {
                    if (state1 != state2)
                    {
                        this.lastPressedTime = Time.realtimeSinceStartup;
                        this.nextRepeatTime = Time.realtimeSinceStartup + this.FirstRepeatDelay;
                    }
                    else if ((double) Time.realtimeSinceStartup >= (double) this.nextRepeatTime)
                    {
                        this.wasRepeated = true;
                        this.nextRepeatTime = Time.realtimeSinceStartup + this.RepeatDelay;
                    }
                }
                if (!(this.thisState != this.lastState))
                    return;
                this.UpdateTick = this.pendingTick;
            }
        }

        public void CommitWithState(bool state, ulong updateTick, float deltaTime)
        {
            this.UpdateWithState(state, updateTick, deltaTime);
            this.Commit();
        }

        public void CommitWithValue(float value, ulong updateTick, float deltaTime)
        {
            this.UpdateWithValue(value, updateTick, deltaTime);
            this.Commit();
        }

        internal void CommitWithSides(
            InputControl negativeSide,
            InputControl positiveSide,
            ulong updateTick,
            float deltaTime)
        {
            this.LowerDeadZone = Mathf.Max(negativeSide.LowerDeadZone, positiveSide.LowerDeadZone);
            this.UpperDeadZone = Mathf.Min(negativeSide.UpperDeadZone, positiveSide.UpperDeadZone);
            this.Raw = negativeSide.Raw || positiveSide.Raw;
            this.CommitWithValue(Utility.ValueFromSides(negativeSide.RawValue, positiveSide.RawValue), updateTick, deltaTime);
        }

        public bool State => this.Enabled && !this.Suppressed && this.thisState.State;

        public bool LastState
        {
            get => this.Enabled && !this.Suppressed && this.lastState.State;
            set => this.lastState.State = value;
        }

        public float Value => this.Enabled && !this.Suppressed ? this.thisState.Value : 0.0f;

        public float LastValue => this.Enabled && !this.Suppressed ? this.lastState.Value : 0.0f;

        public float RawValue => this.Enabled && !this.Suppressed ? this.thisState.RawValue : 0.0f;

        internal float NextRawValue => this.Enabled && !this.Suppressed ? this.nextState.RawValue : 0.0f;

        public bool HasChanged => this.Enabled && !this.Suppressed && this.thisState != this.lastState;

        public bool IsPressed => this.Enabled && !this.Suppressed && this.thisState.State;

        public bool WasPressed
        {
            get => this.Enabled && !this.Suppressed && (bool) this.thisState && !(bool) this.lastState;
        }

        public bool WasPressedRepeating
        {
            get
            {
                if (!(bool) this.thisState)
                    return false;
                if (!(bool) this.lastState)
                {
                    this.nextTimeForRepeat = Time.realtimeSinceStartup + 0.5f;
                    this.startRepeatTime = Time.realtimeSinceStartup;
                    return true;
                }
                if ((double) Time.realtimeSinceStartup > (double) this.startRepeatTime + 5.0 || (double) Time.realtimeSinceStartup < (double) this.nextTimeForRepeat)
                    return false;
                this.nextTimeForRepeat = Time.realtimeSinceStartup + 0.1f;
                return true;
            }
        }

        public void ResetRepeating() => this.nextTimeForRepeat = Time.realtimeSinceStartup + 0.5f;

        public bool WasPressedAsDpad
        {
            get
            {
                return this.Enabled && !this.Suppressed && (double) this.thisState.RawValue >= 0.5 && (double) this.lastState.RawValue < 0.5;
            }
        }

        public bool WasPressedAsDpadRepeating
        {
            get
            {
                if ((double) this.thisState.RawValue < 0.5)
                    return false;
                if ((double) this.lastState.RawValue < 0.5)
                {
                    this.nextTimeForRepeat = Time.realtimeSinceStartup + 0.5f;
                    this.startRepeatTime = Time.realtimeSinceStartup;
                    return true;
                }
                if ((double) Time.realtimeSinceStartup > (double) this.startRepeatTime + 5.0 || (double) Time.realtimeSinceStartup < (double) this.nextTimeForRepeat)
                    return false;
                this.nextTimeForRepeat = Time.realtimeSinceStartup + 0.1f;
                return true;
            }
        }

        public float PressedDuration
        {
            get
            {
                return (double) this.lastReleasedTime > (double) this.lastPressedTime ? this.lastReleasedTime - this.lastPressedTime : Time.realtimeSinceStartup - this.lastPressedTime;
            }
        }

        public bool WasReleased
        {
            get => this.Enabled && !this.Suppressed && !(bool) this.thisState && (bool) this.lastState;
        }

        public bool WasRepeated => this.Enabled && !this.Suppressed && this.wasRepeated;

        public void Suppress() => this.Suppressed = true;

        public float Sensitivity
        {
            get => this.sensitivity;
            set => this.sensitivity = Mathf.Clamp01(value);
        }

        public float LowerDeadZone
        {
            get => this.lowerDeadZone;
            set => this.lowerDeadZone = Mathf.Clamp01(value);
        }

        public float UpperDeadZone
        {
            get => this.upperDeadZone;
            set => this.upperDeadZone = Mathf.Clamp01(value);
        }

        public float StateThreshold
        {
            get => this.stateThreshold;
            set => this.stateThreshold = Mathf.Clamp01(value);
        }

        public bool IsNull => object.ReferenceEquals((object) this, (object) InputControl.Null);

        public static implicit operator bool(OneAxisInputControl instance) => instance.State;

        public static implicit operator float(OneAxisInputControl instance) => instance.Value;
    }
}
