using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using UnityEngine;

#nullable disable

[Serializable]
public class DirectionalAnimation
    {
        public const float s_BACKFACING_ANGLE_MAX = 155f;
        public const float s_BACKFACING_ANGLE_MIN = 25f;
        public const float s_BACKWARDS_ANGLE_MAX = 120f;
        public const float s_BACKWARDS_ANGLE_MIN = 60f;
        public const float s_FORWARDS_ANGLE_MAX = -60f;
        public const float s_FORWARDS_ANGLE_MIN = -120f;
        public const float c_AngleBuffer = 2.5f;
        public static DirectionalAnimation.SingleAnimation[][] m_combined = new DirectionalAnimation.SingleAnimation[11][]
        {
            new DirectionalAnimation.SingleAnimation[0],
            new DirectionalAnimation.SingleAnimation[1]
            {
                new DirectionalAnimation.SingleAnimation(string.Empty, 0.0f, 360f, -90f)
            },
            new DirectionalAnimation.SingleAnimation[2]
            {
                new DirectionalAnimation.SingleAnimation("right", -90f, 90f, 0.0f, new int?(1)),
                new DirectionalAnimation.SingleAnimation("left", 90f, 270f, 180f, new int?(0))
            },
            new DirectionalAnimation.SingleAnimation[2]
            {
                new DirectionalAnimation.SingleAnimation("back", 0.0f, 180f, 90f),
                new DirectionalAnimation.SingleAnimation("front", 180f, 360f, -90f)
            },
            new DirectionalAnimation.SingleAnimation[4]
            {
                new DirectionalAnimation.SingleAnimation("back_right", 25f, 90f, 45f, new int?(3)),
                new DirectionalAnimation.SingleAnimation("front_right", -90f, 25f, -45f, new int?(2)),
                new DirectionalAnimation.SingleAnimation("front_left", 155f, 270f, -135f, new int?(1)),
                new DirectionalAnimation.SingleAnimation("back_left", 90f, 155f, 135f, new int?(0))
            },
            new DirectionalAnimation.SingleAnimation[6]
            {
                new DirectionalAnimation.SingleAnimation("back", 60f, 120f, 90f),
                new DirectionalAnimation.SingleAnimation("back_right", 25f, 60f, 45f, new int?(5)),
                new DirectionalAnimation.SingleAnimation("front_right", -60f, 25f, -45f, new int?(4)),
                new DirectionalAnimation.SingleAnimation("front", 240f, 300f, -90f),
                new DirectionalAnimation.SingleAnimation("front_left", 155f, 240f, -135f, new int?(2)),
                new DirectionalAnimation.SingleAnimation("back_left", 120f, 155f, 135f, new int?(1))
            },
            new DirectionalAnimation.SingleAnimation[8]
            {
                new DirectionalAnimation.SingleAnimation("back", 67.5f, 112.5f, 90f),
                new DirectionalAnimation.SingleAnimation("back_right", 22.5f, 67.5f, 45f, new int?(7)),
                new DirectionalAnimation.SingleAnimation("right", -22.5f, 22.5f, 0.0f, new int?(6)),
                new DirectionalAnimation.SingleAnimation("front_right", 292.5f, 337.5f, -45f, new int?(5)),
                new DirectionalAnimation.SingleAnimation("front", 247.5f, 292.5f, -90f),
                new DirectionalAnimation.SingleAnimation("front_left", 202.5f, 247.5f, -135f, new int?(3)),
                new DirectionalAnimation.SingleAnimation("left", 157.5f, 202.5f, 180f, new int?(2)),
                new DirectionalAnimation.SingleAnimation("back_left", 112.5f, 157.5f, 135f, new int?(1))
            },
            new DirectionalAnimation.SingleAnimation[16 /*0x10*/]
            {
                new DirectionalAnimation.SingleAnimation("north", 78.75f, 101.25f, 90f),
                new DirectionalAnimation.SingleAnimation("north_northeast", 56.25f, 78.75f, 67.5f, new int?(15)),
                new DirectionalAnimation.SingleAnimation("northeast", 33.75f, 56.25f, 45f, new int?(14)),
                new DirectionalAnimation.SingleAnimation("east_northeast", 11.25f, 33.75f, 22.5f, new int?(13)),
                new DirectionalAnimation.SingleAnimation("east", -11.25f, 11.25f, 0.0f, new int?(12)),
                new DirectionalAnimation.SingleAnimation("east_southeast", 326.25f, 348.75f, -22.5f, new int?(11)),
                new DirectionalAnimation.SingleAnimation("southeast", 303.75f, 326.25f, -45f, new int?(10)),
                new DirectionalAnimation.SingleAnimation("south_southeast", 281.25f, 303.75f, -67.5f, new int?(9)),
                new DirectionalAnimation.SingleAnimation("south", 258.75f, 281.25f, -90f),
                new DirectionalAnimation.SingleAnimation("south_southwest", 236.25f, 258.75f, -112.5f, new int?(7)),
                new DirectionalAnimation.SingleAnimation("southwest", 213.75f, 236.25f, -135f, new int?(6)),
                new DirectionalAnimation.SingleAnimation("west_southwest", 191.25f, 213.75f, -157.5f, new int?(5)),
                new DirectionalAnimation.SingleAnimation("west", 168.75f, 191.25f, 180f, new int?(4)),
                new DirectionalAnimation.SingleAnimation("west_northwest", 146.25f, 168.75f, 157.5f, new int?(3)),
                new DirectionalAnimation.SingleAnimation("northwest", 123.75f, 146.25f, 135f, new int?(2)),
                new DirectionalAnimation.SingleAnimation("north_northwest", 101.25f, 123.75f, 112.5f, new int?(1))
            },
            new DirectionalAnimation.SingleAnimation[16 /*0x10*/]
            {
                new DirectionalAnimation.SingleAnimation("north", 78.75f, 101.25f, 90f),
                new DirectionalAnimation.SingleAnimation("north_northeast", 56.25f, 78.75f, 67.5f, new int?(15)),
                new DirectionalAnimation.SingleAnimation("northeast", 33.75f, 56.25f, 45f, new int?(14)),
                new DirectionalAnimation.SingleAnimation("east_northeast", 11.25f, 33.75f, 22.5f, new int?(13)),
                new DirectionalAnimation.SingleAnimation("east", 348.75f, 11.25f, 0.0f, new int?(12)),
                new DirectionalAnimation.SingleAnimation("east_southeast", 326.25f, 348.75f, -22.5f, new int?(11)),
                new DirectionalAnimation.SingleAnimation("southeast", 303.75f, 326.25f, -45f, new int?(10)),
                new DirectionalAnimation.SingleAnimation("south_southeast", 281.25f, 303.75f, -67.5f, new int?(9)),
                new DirectionalAnimation.SingleAnimation("south", 258.75f, 281.25f, -90f),
                new DirectionalAnimation.SingleAnimation("south_southwest", 236.25f, 258.75f, -112.5f, new int?(7)),
                new DirectionalAnimation.SingleAnimation("southwest", 213.75f, 236.25f, -135f, new int?(6)),
                new DirectionalAnimation.SingleAnimation("west_southwest", 191.25f, 213.75f, -157.5f, new int?(5)),
                new DirectionalAnimation.SingleAnimation("west", 168.75f, 191.25f, 180f, new int?(4)),
                new DirectionalAnimation.SingleAnimation("west_northwest", 146.25f, 168.75f, 157.5f, new int?(3)),
                new DirectionalAnimation.SingleAnimation("northwest", 123.75f, 146.25f, 135f, new int?(2)),
                new DirectionalAnimation.SingleAnimation("north_northwest", 101.25f, 123.75f, 112.5f, new int?(1))
            },
            new DirectionalAnimation.SingleAnimation[4]
            {
                new DirectionalAnimation.SingleAnimation("north", 45f, 135f, 90f),
                new DirectionalAnimation.SingleAnimation("east", -45f, 45f, 0.0f, new int?(3)),
                new DirectionalAnimation.SingleAnimation("south", 225f, 315f, -90f),
                new DirectionalAnimation.SingleAnimation("west", 135f, 225f, 180f, new int?(1))
            },
            new DirectionalAnimation.SingleAnimation[8]
            {
                new DirectionalAnimation.SingleAnimation("north", 67.5f, 112.5f, 90f),
                new DirectionalAnimation.SingleAnimation("northeast", 22.5f, 67.5f, 45f, new int?(7)),
                new DirectionalAnimation.SingleAnimation("east", -22.5f, 22.5f, 0.0f, new int?(6)),
                new DirectionalAnimation.SingleAnimation("southeast", 292.5f, 337.5f, -45f, new int?(5)),
                new DirectionalAnimation.SingleAnimation("south", 247.5f, 292.5f, -90f),
                new DirectionalAnimation.SingleAnimation("southwest", 202.5f, 247.5f, -135f, new int?(3)),
                new DirectionalAnimation.SingleAnimation("west", 157.5f, 202.5f, 180f, new int?(2)),
                new DirectionalAnimation.SingleAnimation("northwest", 112.5f, 157.5f, 135f, new int?(1))
            }
        };
        public DirectionalAnimation.DirectionType Type;
        public string Prefix;
        public string[] AnimNames;
        public DirectionalAnimation.FlipType[] Flipped;
        private DirectionalAnimation.Info m_info = new DirectionalAnimation.Info();
        [NonSerialized]
        private int m_lastAnimIndex = -1;
        [NonSerialized]
        private int m_previousEighthIndex = -1;
        [NonSerialized]
        private float m_tempCooldown;
        [NonSerialized]
        private int m_tempIndex;

        public bool HasAnimation => this.Type != DirectionalAnimation.DirectionType.None;

        public DirectionalAnimation.Info GetInfo(Vector2 dir, bool frameUpdate = false)
        {
            return this.GetInfo(dir.ToAngle(), frameUpdate);
        }

        public DirectionalAnimation.Info GetInfo(float angleDegrees, bool frameUpdate = false)
        {
            if (float.IsNaN(angleDegrees))
            {
                Debug.LogWarning((object) "Warning: NaN Animation Angle!");
                angleDegrees = 0.0f;
            }
            if (this.Type == DirectionalAnimation.DirectionType.SixteenWayTemp)
                return this.GetInfoSixteenWayTemp(angleDegrees, frameUpdate);
            angleDegrees = BraveMathCollege.ClampAngle360(angleDegrees);
            DirectionalAnimation.SingleAnimation[] singleAnimationArray = DirectionalAnimation.m_combined[(int) this.Type];
            if (this.m_lastAnimIndex != -1 && (this.m_lastAnimIndex < 0 || this.m_lastAnimIndex >= singleAnimationArray.Length))
                this.m_lastAnimIndex = -1;
            if (this.m_lastAnimIndex != -1)
            {
                float minAngle = singleAnimationArray[this.m_lastAnimIndex].minAngle;
                if ((double) minAngle < 0.0 && (double) angleDegrees >= (double) minAngle + 360.0 - 2.5)
                    return this.GetInfo(this.m_lastAnimIndex);
                float maxAngle = singleAnimationArray[this.m_lastAnimIndex].maxAngle;
                if ((double) angleDegrees >= (double) minAngle - 2.5 && (double) angleDegrees <= (double) maxAngle + 2.5)
                    return this.GetInfo(this.m_lastAnimIndex);
            }
            for (int index = 0; index < singleAnimationArray.Length; ++index)
            {
                if (this.Type == DirectionalAnimation.DirectionType.Single || this.Flipped[index] != DirectionalAnimation.FlipType.Unused)
                {
                    float minAngle = singleAnimationArray[index].minAngle;
                    if ((double) minAngle < 0.0 && (double) angleDegrees >= (double) minAngle + 360.0)
                    {
                        if (frameUpdate)
                            this.m_lastAnimIndex = index;
                        return this.GetInfo(index);
                    }
                    float maxAngle = singleAnimationArray[index].maxAngle;
                    if ((double) angleDegrees >= (double) minAngle && (double) angleDegrees <= (double) maxAngle)
                    {
                        if (frameUpdate)
                            this.m_lastAnimIndex = index;
                        return this.GetInfo(index);
                    }
                }
            }
            int index1 = -1;
            float num1 = float.MaxValue;
            for (int index2 = 0; index2 < singleAnimationArray.Length; ++index2)
            {
                if (this.Type == DirectionalAnimation.DirectionType.Single || this.Flipped[index2] != DirectionalAnimation.FlipType.Unused)
                {
                    float num2 = Mathf.Min(BraveMathCollege.AbsAngleBetween(angleDegrees, singleAnimationArray[index2].minAngle), BraveMathCollege.AbsAngleBetween(angleDegrees, singleAnimationArray[index2].maxAngle));
                    if ((double) num2 < (double) num1)
                    {
                        index1 = index2;
                        num1 = num2;
                    }
                }
            }
            if (index1 < 0)
                return (DirectionalAnimation.Info) null;
            if (frameUpdate)
                this.m_lastAnimIndex = index1;
            return this.GetInfo(index1);
        }

        private DirectionalAnimation.Info GetInfoSixteenWayTemp(float angleDegrees, bool frameUpdate)
        {
            angleDegrees = BraveMathCollege.ClampAngle360(angleDegrees);
            if (frameUpdate)
                this.m_tempCooldown -= BraveTime.DeltaTime;
            int index;
            if ((double) this.m_tempCooldown > 0.0)
            {
                index = this.m_tempIndex;
            }
            else
            {
                int num = (int) ((double) BraveMathCollege.ClampAngle360((float) (-(double) angleDegrees + 90.0 + 22.5)) / 45.0);
                index = num * 2;
                if (num != -1 && this.m_previousEighthIndex != -1)
                {
                    if (num == 0 && this.m_previousEighthIndex == 7 || num == 7 && this.m_previousEighthIndex == 0)
                    {
                        index = 15;
                        this.m_tempIndex = index;
                        this.m_tempCooldown = 0.1f;
                    }
                    else if (num == this.m_previousEighthIndex + 1)
                    {
                        index = this.m_previousEighthIndex * 2 + 1;
                        this.m_tempIndex = index;
                        this.m_tempCooldown = 0.1f;
                    }
                    else if (num == this.m_previousEighthIndex - 1)
                    {
                        index = this.m_previousEighthIndex * 2 - 1;
                        this.m_tempIndex = index;
                        this.m_tempCooldown = 0.1f;
                    }
                }
                this.m_previousEighthIndex = num;
            }
            return this.Flipped[index] != DirectionalAnimation.FlipType.Unused ? this.GetInfo(index) : this.GetInfo((index + ((double) (index % 1) <= 0.5 ? -1 : 1) + ((IEnumerable<string>) this.AnimNames).Count<string>()) % ((IEnumerable<string>) this.AnimNames).Count<string>());
        }

        public DirectionalAnimation.Info GetInfo(int index)
        {
            if (this.Type == DirectionalAnimation.DirectionType.Single && index == 0)
            {
                this.m_info.SetAll(this.Prefix, false, -90f);
                return this.m_info;
            }
            if (index > this.Flipped.Length - 1)
                Debug.LogError((object) "shit");
            if (this.Flipped[index] == DirectionalAnimation.FlipType.Mirror)
            {
                index = DirectionalAnimation.GetMirrorIndex(this.Type, index);
                this.m_info.SetAll(this.GetName(index), true, this.GetArtAngle(index));
                return this.m_info;
            }
            this.m_info.SetAll(this.GetName(index), this.Flipped[index] == DirectionalAnimation.FlipType.Flip, this.GetArtAngle(index));
            return this.m_info;
        }

        public int GetNumAnimations() => DirectionalAnimation.GetNumAnimations(this.Type);

        private string GetName(int index)
        {
            if (string.IsNullOrEmpty(this.AnimNames[index]))
                this.AnimNames[index] = DirectionalAnimation.GetDefaultName(this.Prefix, this.Type, index);
            return this.AnimNames[index];
        }

        private float GetArtAngle(int index)
        {
            return DirectionalAnimation.m_combined[(int) this.Type][index].artAngle;
        }

        public static int GetNumAnimations(DirectionalAnimation.DirectionType type)
        {
            return DirectionalAnimation.m_combined[(int) type].Length;
        }

        public static string GetLabel(DirectionalAnimation.DirectionType type, int index)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DirectionalAnimation.m_combined[(int) type][index].suffix.Replace('_', ' '));
        }

        public static string GetSuffix(DirectionalAnimation.DirectionType type, int index)
        {
            return DirectionalAnimation.m_combined[(int) type][index].suffix;
        }

        public static string GetDefaultName(
            string prefix,
            DirectionalAnimation.DirectionType type,
            int index)
        {
            if (type == DirectionalAnimation.DirectionType.Single)
                return prefix;
            return prefix.Contains("{0}") ? string.Format(prefix, (object) DirectionalAnimation.GetSuffix(type, index)) : $"{prefix}_{DirectionalAnimation.GetSuffix(type, index)}";
        }

        public static bool HasMirror(DirectionalAnimation.DirectionType type, int index)
        {
            return DirectionalAnimation.m_combined[(int) type][index].mirrorIndex.HasValue;
        }

        public static int GetMirrorIndex(DirectionalAnimation.DirectionType type, int index)
        {
            return DirectionalAnimation.m_combined[(int) type][index].mirrorIndex.Value;
        }

        public enum DirectionType
        {
            None,
            Single,
            TwoWayHorizontal,
            TwoWayVertical,
            FourWay,
            SixWay,
            EightWay,
            SixteenWay,
            SixteenWayTemp,
            FourWayCardinal,
            EightWayOrdinal,
        }

        public class SingleAnimation
        {
            public string suffix;
            public float minAngle;
            public float maxAngle;
            public float artAngle;
            public int? mirrorIndex;

            public SingleAnimation(
                string suffix,
                float minAngle,
                float maxAngle,
                float artAngle,
                int? mirrorIndex = null)
            {
                this.suffix = suffix;
                this.minAngle = minAngle;
                this.maxAngle = maxAngle;
                this.artAngle = artAngle;
                this.mirrorIndex = mirrorIndex;
            }
        }

        public enum FlipType
        {
            None,
            Flip,
            Unused,
            Mirror,
        }

        public class Info
        {
            public string name;
            public bool flipped;
            public float artAngle;

            public void SetAll(string name, bool flipped, float artAngle)
            {
                this.name = name;
                this.flipped = flipped;
                this.artAngle = artAngle;
            }
        }
    }

