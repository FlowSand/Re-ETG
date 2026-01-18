using UnityEngine;

#nullable disable

public class dfAnimatedInt : dfAnimatedValue<int>

    {

        public dfAnimatedInt(int StartValue, int EndValue, float Time) : base(StartValue, EndValue, Time)

        {

        }


    
        protected override int Lerp(int startValue, int endValue, float time)
        {
            return Mathf.RoundToInt(Mathf.Lerp((float) startValue, (float) endValue, time));
        }

        public static implicit operator dfAnimatedInt(int value) => new dfAnimatedInt(value, value, 0.0f);
    }

