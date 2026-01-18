using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using UnityEngine;

#nullable disable

public class TextBoxManager : MonoBehaviour
    {
        public static bool TIME_INVARIANT = false;
        private const float TEXT_REVEAL_SPEED_INSTANT = 3.40282347E+38f;
        private const float TEXT_REVEAL_SPEED_FAST = 100f;
        private const float TEXT_REVEAL_SPEED_SLOW = 27f;
        private const float SCALE_UP_TIME = 0.06f;
        private const float SCALE_DOWN_TIME = 0.06f;
        [SerializeField]
        private tk2dSlicedSprite boxSprite;
        [SerializeField]
        private tk2dTextMesh textMesh;
        [SerializeField]
        private tk2dTextMesh continueTextMesh;
        public float additionalPaddingLeft;
        public float additionalPaddingRight;
        public float additionalPaddingTop;
        public float additionalPaddingBottom;
        public float continuePaddingRight;
        public float continuePaddingBottom;
        public bool fitToScreen;
        public Color textColor = Color.black;
        private static float BOX_PADDING = 0.5f;
        private static float INFOBOX_PADDING = 0.25f;
        private bool m_isRevealingText;
        private bool skipTextReveal;
        private string audioTag = string.Empty;
        private float boxPadding;
        private Vector3 m_basePosition;
        private Transform boxSpriteTransform;
        private Transform textMeshTransform;
        private Transform continueTextMeshTransform;
        private static List<Transform> extantTextPointList = new List<Transform>();
        private static Dictionary<Transform, GameObject> extantTextBoxMap = new Dictionary<Transform, GameObject>();
        private static int UNPIXELATED_LAYER = -1;
        private static int PIXELATED_LAYER = -1;

        private float TEXT_REVEAL_SPEED
        {
            get
            {
                switch (GameManager.Options.TextSpeed)
                {
                    case GameOptions.GenericHighMedLowOption.LOW:
                        return 27f;
                    case GameOptions.GenericHighMedLowOption.MEDIUM:
                        return 100f;
                    case GameOptions.GenericHighMedLowOption.HIGH:
                        return float.MaxValue;
                    default:
                        return 100f;
                }
            }
        }

        public static float ZombieBoxMultiplier
        {
            get
            {
                switch (GameManager.Options.TextSpeed)
                {
                    case GameOptions.GenericHighMedLowOption.LOW:
                        return 2.5f;
                    case GameOptions.GenericHighMedLowOption.MEDIUM:
                        return 1.5f;
                    case GameOptions.GenericHighMedLowOption.HIGH:
                        return 1f;
                    default:
                        return 1f;
                }
            }
        }

        public bool IsRevealingText => this.m_isRevealingText;

        public bool IsScalingUp { get; set; }

        public bool IsScalingDown { get; set; }

        public static int ExtantTextBoxCount => TextBoxManager.extantTextBoxMap.Count;

        public static bool ExtantTextBoxVisible
        {
            get
            {
                if (TextBoxManager.extantTextBoxMap == null || TextBoxManager.extantTextBoxMap.Count == 0)
                    return false;
                for (int index = 0; index < TextBoxManager.extantTextPointList.Count; ++index)
                {
                    if (!(bool) (UnityEngine.Object) TextBoxManager.extantTextPointList[index])
                    {
                        TextBoxManager.extantTextPointList.RemoveAt(index);
                        --index;
                    }
                    else if (GameManager.Instance.MainCameraController.PointIsVisible(TextBoxManager.extantTextPointList[index].position.XY()))
                        return true;
                }
                return false;
            }
        }

        public static void ClearPerLevelData() => TextBoxManager.extantTextBoxMap.Clear();

        public static void ShowTextBox(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            string audioTag = "",
            bool instant = true,
            TextBoxManager.BoxSlideOrientation slideOrientation = TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT,
            bool showContinueText = false,
            bool useAlienLanguage = false)
        {
            TextBoxManager.ShowBoxInternal(worldPosition, parent, duration, text, "TextBox", TextBoxManager.BOX_PADDING, audioTag, instant, slideOrientation, showContinueText, useAlienLanguage);
        }

        public static void ShowInfoBox(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            bool instant = true,
            bool showContinueText = false)
        {
            TextBoxManager.ShowBoxInternal(worldPosition, parent, duration, text, "InfoBox", TextBoxManager.INFOBOX_PADDING, string.Empty, instant, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText);
        }

        public static void ShowLetterBox(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            bool instant = true,
            bool showContinueText = false)
        {
            TextBoxManager.ShowBoxInternal(worldPosition, parent, duration, text, "LetterBox", TextBoxManager.BOX_PADDING, string.Empty, instant, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText);
        }

        public static void ShowStoneTablet(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            bool instant = true,
            bool showContinueText = false)
        {
            TextBoxManager.ShowBoxInternal(worldPosition, parent, duration, text, "StoneTablet", TextBoxManager.BOX_PADDING, string.Empty, instant, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText);
        }

        public static void ShowWoodPanel(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            bool instant = true,
            bool showContinueText = false)
        {
            TextBoxManager.ShowBoxInternal(worldPosition, parent, duration, text, "WoodPanel", TextBoxManager.BOX_PADDING, string.Empty, instant, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText);
        }

        public static void ShowThoughtBubble(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            bool instant = true,
            bool showContinueText = false,
            string overrideAudioTag = "")
        {
            TextBoxManager.ShowBoxInternal(worldPosition, parent, duration, text, "ThoughtBubble", TextBoxManager.BOX_PADDING, overrideAudioTag, instant, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText);
        }

        public static void ShowNote(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            bool instant = true,
            bool showContinueText = false)
        {
            TextBoxManager.ShowBoxInternal(worldPosition, parent, duration, text, "Note", TextBoxManager.BOX_PADDING, string.Empty, instant, TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT, showContinueText);
        }

        public static bool TextBoxCanBeAdvanced(Transform parent)
        {
            return TextBoxManager.extantTextBoxMap.ContainsKey(parent) && TextBoxManager.extantTextBoxMap[parent].GetComponent<TextBoxManager>().IsRevealingText;
        }

        public static void AdvanceTextBox(Transform parent)
        {
            if (!TextBoxManager.extantTextBoxMap.ContainsKey(parent))
                return;
            TextBoxManager.extantTextBoxMap[parent].GetComponent<TextBoxManager>().SkipTextReveal();
        }

        protected static void ShowBoxInternal(
            Vector3 worldPosition,
            Transform parent,
            float duration,
            string text,
            string prefabName,
            float padding,
            string audioTag,
            bool instant,
            TextBoxManager.BoxSlideOrientation slideOrientation,
            bool showContinueText,
            bool UseAlienLanguage = false)
        {
            Vector2 prevBoxSize = new Vector2(-1f, -1f);
            if ((UnityEngine.Object) parent != (UnityEngine.Object) null && TextBoxManager.extantTextBoxMap.ContainsKey(parent))
            {
                prevBoxSize = TextBoxManager.extantTextBoxMap[parent].GetComponent<TextBoxManager>().boxSprite.dimensions;
                UnityEngine.Object.Destroy((UnityEngine.Object) TextBoxManager.extantTextBoxMap[parent]);
                TextBoxManager.extantTextPointList.Remove(parent);
                TextBoxManager.extantTextBoxMap.Remove(parent);
            }
            GameObject target = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load(prefabName));
            TextBoxManager component = target.GetComponent<TextBoxManager>();
            component.boxPadding = padding;
            component.IsScalingUp = true;
            component.audioTag = audioTag;
            component.SetText(text, worldPosition, instant, slideOrientation, UseAlienLanguage: UseAlienLanguage, clampThoughtBubble: prefabName == "ThoughtBubble");
            if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
            {
                component.transform.parent = parent;
                TextBoxManager.extantTextPointList.Add(parent);
                TextBoxManager.extantTextBoxMap.Add(parent, target);
            }
            if ((double) duration >= 0.0)
                component.HandleLifespan(target, parent, duration);
            if (showContinueText)
                component.ShowContinueText();
            component.StartCoroutine(component.HandleScaleUp(prevBoxSize));
        }

        private float ScaleFactor
        {
            get
            {
                return (float) Mathf.Max(1, Mathf.FloorToInt(1f / GameManager.Instance.MainCameraController.CurrentZoomScale));
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleScaleUp(Vector2 prevBoxSize)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TextBoxManager__HandleScaleUpc__Iterator0()
            {
                prevBoxSize = prevBoxSize,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleScaleDown()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TextBoxManager__HandleScaleDownc__Iterator1()
            {
                _this = this
            };
        }

        public static bool HasTextBox(Transform parent)
        {
            return TextBoxManager.extantTextBoxMap.ContainsKey(parent);
        }

        public static void ClearTextBoxImmediate(Transform parent)
        {
            if (TextBoxManager.extantTextBoxMap.ContainsKey(parent))
            {
                TextBoxManager.extantTextPointList.Remove(parent);
                TextBoxManager.extantTextBoxMap.Remove(parent);
            }
            TextBoxManager componentInChildren = parent.GetComponentInChildren<TextBoxManager>();
            if (!(bool) (UnityEngine.Object) componentInChildren)
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren.gameObject);
        }

        public static void ClearTextBox(Transform parent)
        {
            if (!TextBoxManager.extantTextBoxMap.ContainsKey(parent))
                return;
            TextBoxManager component = TextBoxManager.extantTextBoxMap[parent].GetComponent<TextBoxManager>();
            component.StartCoroutine(component.HandleScaleDown());
            TextBoxManager.extantTextPointList.Remove(parent);
            TextBoxManager.extantTextBoxMap.Remove(parent);
        }

        public void HandleLifespan(GameObject target, Transform parent, float lifespan)
        {
            this.StartCoroutine(this.TextBoxLifespanCR(target, parent, lifespan));
        }

        public void ShowContinueText()
        {
            if (!(bool) (UnityEngine.Object) this.continueTextMesh)
                return;
            this.StartCoroutine(this.ShowContinueTextCR());
        }

        public void SkipTextReveal() => this.skipTextReveal = true;

        [DebuggerHidden]
        private IEnumerator TextBoxLifespanCR(GameObject target, Transform parent, float lifespan)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TextBoxManager__TextBoxLifespanCRc__Iterator2()
            {
                lifespan = lifespan,
                parent = parent,
                target = target,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator ShowContinueTextCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TextBoxManager__ShowContinueTextCRc__Iterator3()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator RevealTextCharacters(string strippedString)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new TextBoxManager__RevealTextCharactersc__Iterator4()
            {
                strippedString = strippedString,
                _this = this
            };
        }

        private void LateUpdate()
        {
            TextBoxManager.UNPIXELATED_LAYER = !Pixelator.Instance.DoFinalNonFadedLayer ? LayerMask.NameToLayer("Unoccluded") : LayerMask.NameToLayer("Unfaded");
            if (TextBoxManager.PIXELATED_LAYER == -1)
                TextBoxManager.PIXELATED_LAYER = LayerMask.NameToLayer("FG_Critical");
            if (GameManager.Instance.IsPaused && this.gameObject.layer == TextBoxManager.UNPIXELATED_LAYER)
                this.gameObject.SetLayerRecursively(TextBoxManager.PIXELATED_LAYER);
            else if (!GameManager.Instance.IsPaused && this.gameObject.layer != TextBoxManager.UNPIXELATED_LAYER)
                this.gameObject.SetLayerRecursively(TextBoxManager.UNPIXELATED_LAYER);
            this.UpdateForCameraPosition();
        }

        public void UpdateForCameraPosition()
        {
            if (!this.fitToScreen)
                return;
            Vector3 vector1 = this.transform.position - this.m_basePosition;
            Vector2 vector2 = this.boxSprite.transform.position.XY() - vector1.XY();
            Vector2 vector3 = this.boxSprite.transform.position.XY() + this.boxSprite.dimensions / 16f - vector1.XY();
            Camera component = GameManager.Instance.MainCameraController.GetComponent<Camera>();
            Vector2 viewportPoint1 = (Vector2) component.WorldToViewportPoint(vector2.ToVector3ZUp(vector2.y));
            Vector2 viewportPoint2 = (Vector2) component.WorldToViewportPoint(vector3.ToVector3ZUp(vector3.y));
            this.transform.position = (this.m_basePosition + new Vector3(-(float) ((double) (Mathf.Min(viewportPoint1.x, 0.1f) + Mathf.Max(viewportPoint2.x - 1f, -0.1f)) * 480.0 / 16.0), -(float) ((double) (Mathf.Min(viewportPoint1.y, 0.1f) + Mathf.Max(viewportPoint2.y - 1f, -0.1f)) * 270.0 / 16.0), 0.0f)).Quantize(1f / 16f);
            if (this.IsScalingUp || this.IsScalingDown)
                return;
            this.transform.localScale = Vector3.one * this.ScaleFactor;
        }

        private string ToUpperExcludeSprites(string inputString)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = false;
            for (int index = 0; index < inputString.Length; ++index)
            {
                char upperInvariant = inputString[index];
                if (upperInvariant == '[' && !flag)
                    flag = true;
                else if (upperInvariant == ']' && flag)
                    flag = false;
                else if (!flag)
                    upperInvariant = char.ToUpperInvariant(upperInvariant);
                stringBuilder.Append(upperInvariant);
            }
            return stringBuilder.ToString();
        }

        public void SetText(
            string text,
            Vector3 worldPosition,
            bool instant = true,
            TextBoxManager.BoxSlideOrientation slideOrientation = TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT,
            bool showContinueText = true,
            bool UseAlienLanguage = false,
            bool clampThoughtBubble = false)
        {
            if ((UnityEngine.Object) this.boxSpriteTransform == (UnityEngine.Object) null)
                this.boxSpriteTransform = this.boxSprite.transform;
            if ((UnityEngine.Object) this.textMeshTransform == (UnityEngine.Object) null)
                this.textMeshTransform = this.textMesh.transform;
            if ((UnityEngine.Object) this.continueTextMeshTransform == (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.continueTextMesh)
                this.continueTextMeshTransform = this.continueTextMesh.transform;
            if (text == string.Empty)
                return;
            text = text.Replace("\\n", Environment.NewLine);
            float num1 = (float) (-(double) this.boxSpriteTransform.localPosition.x / ((double) this.boxSprite.dimensions.x / 16.0));
            string inputString = this.textMesh.GetStrippedWoobleString(text);
            this.textMesh.LineSpacing = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE ? 0.0f : 0.125f;
            if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE)
                this.textMesh.wordWrapWidth = 350;
            else if (inputString.Length < 25)
            {
                this.textMesh.wordWrapWidth = 250;
            }
            else
            {
                this.textMesh.wordWrapWidth = 200 + (inputString.Length - 25) / 4;
                if (!inputString.EndsWith(" "))
                    inputString += " ";
            }
            if (Application.isPlaying)
            {
                bool flag = false;
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                    flag |= GameManager.Instance.AllPlayers[index].UnderstandsGleepGlorp;
                if (UseAlienLanguage && !flag)
                {
                    inputString = this.ToUpperExcludeSprites(inputString);
                    this.textMesh.font = GameManager.Instance.DefaultAlienConversationFont;
                }
                else
                    this.textMesh.font = GameManager.Instance.DefaultNormalConversationFont;
            }
            this.textMesh.text = inputString;
            this.textMesh.CheckFontsForLanguage();
            this.textMesh.ForceBuild();
            Bounds trueBounds = this.textMesh.GetTrueBounds();
            float num2 = Mathf.Ceil((float) (((double) trueBounds.size.x + (double) this.boxPadding * 2.0 + (double) this.additionalPaddingLeft + (double) this.additionalPaddingRight) * 16.0)) / 16f;
            float num3 = Mathf.Ceil((float) (((double) trueBounds.size.y + (double) this.boxPadding * 2.0 + (double) this.additionalPaddingTop + (double) this.additionalPaddingBottom) * 16.0)) / 16f;
            if (showContinueText && (bool) (UnityEngine.Object) this.continueTextMesh)
                num2 += this.continueTextMesh.GetEstimatedMeshBoundsForString("...").extents.x * 2f;
            float num4 = num2 * 16f;
            float a = num3 * 16f;
            if (clampThoughtBubble)
            {
                float num5 = 47f + (Mathf.Max(47f, num4) - 47f).Quantize(23f, VectorConversions.Floor);
                float num6 = 57f + (Mathf.Max(57f, num4) - 57f).Quantize(23f, VectorConversions.Floor);
                if ((double) num5 < (double) num4)
                    num5 += 23f;
                if ((double) num6 < (double) num4)
                    num6 += 23f;
                num4 = (double) Mathf.Abs(num5 - num4) >= (double) Mathf.Abs(num6 - num4) ? num6 : num5;
            }
            Vector3 lhs = new Vector3(0.0f, 0.0f);
            tk2dSpriteDefinition currentSpriteDef = this.boxSprite.GetCurrentSpriteDef();
            Vector3 boundsDataExtents = currentSpriteDef.boundsDataExtents;
            if ((double) currentSpriteDef.texelSize.x != 0.0 && (double) currentSpriteDef.texelSize.y != 0.0 && (double) boundsDataExtents.x != 0.0 && (double) boundsDataExtents.y != 0.0)
                lhs = new Vector3(boundsDataExtents.x / currentSpriteDef.texelSize.x, boundsDataExtents.y / currentSpriteDef.texelSize.y, 1f);
            Vector3 vector3 = Vector3.Max(lhs, Vector3.one);
            this.boxSprite.dimensions = new Vector2(Mathf.Max(num4, (this.boxSprite.borderLeft + this.boxSprite.borderRight) * vector3.x), Mathf.Max(a, (this.boxSprite.borderTop + this.boxSprite.borderBottom) * vector3.y));
            this.boxSprite.BorderOnly = (double) this.boxSprite.dimensions.x < ((double) this.boxSprite.borderLeft + (double) this.boxSprite.borderRight) * (double) vector3.x || (double) this.boxSprite.dimensions.y < ((double) this.boxSprite.borderTop + (double) this.boxSprite.borderBottom) * (double) vector3.y;
            this.boxSprite.ForceBuild();
            this.textMesh.color = this.textColor;
            if (instant)
            {
                this.textMesh.text = this.textMesh.PreprocessWoobleSignifiers(text);
                if (UseAlienLanguage)
                    this.textMesh.text = this.ToUpperExcludeSprites(this.textMesh.text);
                this.textMesh.Commit();
            }
            else
            {
                this.textMesh.text = string.Empty;
                this.textMesh.Commit();
                string str = this.textMesh.PreprocessWoobleSignifiers(text);
                if (UseAlienLanguage)
                    str = this.ToUpperExcludeSprites(str);
                this.StartCoroutine(this.RevealTextCharacters(str));
            }
            float y = BraveMathCollege.QuantizeFloat(this.boxSprite.dimensions.y / 16f - this.boxPadding - this.additionalPaddingTop, 1f / 16f);
            if (this.textMesh.anchor == TextAnchor.UpperLeft)
                this.textMeshTransform.localPosition = new Vector3(this.boxPadding + this.additionalPaddingLeft, y, -0.1f);
            else if (this.textMesh.anchor == TextAnchor.UpperCenter)
                this.textMeshTransform.localPosition = new Vector3(num2 / 2f, y, -0.1f);
            this.textMeshTransform.localPosition += new Vector3(3f / 128f, 3f / 128f, 0.0f);
            if ((bool) (UnityEngine.Object) this.continueTextMesh)
            {
                if (showContinueText)
                {
                    Bounds meshBoundsForString = this.continueTextMesh.GetEstimatedMeshBoundsForString("...");
                    this.continueTextMeshTransform.localPosition = new Vector3((float) ((double) num2 - (double) this.continuePaddingRight - (double) meshBoundsForString.extents.x * 2.0), this.continuePaddingBottom, -0.1f);
                }
                else
                {
                    this.continueTextMesh.text = string.Empty;
                    this.continueTextMesh.Commit();
                }
            }
            switch (slideOrientation)
            {
                case TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT:
                    this.boxSpriteTransform.localPosition = this.boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat((float) (-1.0 * (double) num1 * ((double) this.boxSprite.dimensions.x / 16.0)), 1f / 16f));
                    break;
                case TextBoxManager.BoxSlideOrientation.FORCE_RIGHT:
                    num1 = 0.1f;
                    this.boxSpriteTransform.localPosition = this.boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat((float) (-1.0 * (double) num1 * ((double) this.boxSprite.dimensions.x / 16.0)), 1f / 16f));
                    break;
                case TextBoxManager.BoxSlideOrientation.FORCE_LEFT:
                    num1 = 0.85f;
                    this.boxSpriteTransform.localPosition = this.boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat((float) (-1.0 * (double) num1 * ((double) this.boxSprite.dimensions.x / 16.0)), 1f / 16f));
                    break;
                default:
                    this.boxSpriteTransform.localPosition = this.boxSpriteTransform.localPosition.WithX(BraveMathCollege.QuantizeFloat((float) (-1.0 * (double) num1 * ((double) this.boxSprite.dimensions.x / 16.0)), 1f / 16f));
                    break;
            }
            if (slideOrientation == TextBoxManager.BoxSlideOrientation.NO_ADJUSTMENT)
            {
                float num7 = !Application.isPlaying ? 0.0f : GameManager.Instance.MainCameraController.transform.position.x;
                if ((double) worldPosition.x > (double) num7)
                    this.boxSpriteTransform.localPosition = this.boxSpriteTransform.localPosition.WithX((float) (-1.0 * (1.0 - (double) num1) * ((double) this.boxSprite.dimensions.x / 16.0)));
            }
            this.transform.position = worldPosition;
            this.transform.localScale = Vector3.one * this.ScaleFactor;
            this.m_basePosition = worldPosition;
            this.UpdateForCameraPosition();
            this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unoccluded"));
        }

        public static float GetEstimatedReadingTime(string text)
        {
            int num1 = 0;
            bool flag = false;
            for (int index = 0; index < text.Length; ++index)
            {
                char c = text[index];
                switch (c)
                {
                    case '[':
                    case '{':
                        flag = true;
                        break;
                    case ']':
                    case '}':
                        flag = false;
                        break;
                }
                if (!flag && !char.IsWhiteSpace(c))
                    ++num1;
            }
            int num2 = 987;
            switch (GameManager.Options.CurrentLanguage)
            {
                case StringTableManager.GungeonSupportedLanguages.ENGLISH:
                    num2 = 987;
                    break;
                case StringTableManager.GungeonSupportedLanguages.RUBEL_TEST:
                    num2 = 1000;
                    break;
                case StringTableManager.GungeonSupportedLanguages.FRENCH:
                    num2 = 998;
                    break;
                case StringTableManager.GungeonSupportedLanguages.SPANISH:
                    num2 = 1025;
                    break;
                case StringTableManager.GungeonSupportedLanguages.GERMAN:
                    num2 = 920;
                    break;
                case StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE:
                    num2 = 913;
                    break;
                case StringTableManager.GungeonSupportedLanguages.JAPANESE:
                    num2 = 357;
                    break;
                case StringTableManager.GungeonSupportedLanguages.KOREAN:
                    num2 = 357;
                    break;
                case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
                    num2 = 986;
                    break;
                case StringTableManager.GungeonSupportedLanguages.POLISH:
                    num2 = 916;
                    break;
                case StringTableManager.GungeonSupportedLanguages.CHINESE:
                    num2 = 357;
                    break;
            }
            return (float) num1 / ((float) num2 / 60f);
        }

        public enum BoxSlideOrientation
        {
            NO_ADJUSTMENT,
            FORCE_RIGHT,
            FORCE_LEFT,
        }
    }

