using System.Collections;
using System.Diagnostics;
using System.Text;

using UnityEngine;

#nullable disable

public class FoyerTimeHeroStatue : BraveBehaviour, IPlayerInteractable
    {
        public string targetDisplayKey;
        public Transform talkPoint;

        [DebuggerHidden]
        public IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new FoyerTimeHeroStatue__Startc__Iterator0()
            {
                _this = this
            };
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if ((Object) this.sprite == (Object) null)
                return 100f;
            Vector3 b = (Vector3) BraveMathCollege.ClosestPointOnRectangle(point, this.specRigidbody.UnitBottomLeft, this.specRigidbody.UnitDimensions);
            return Vector2.Distance(point, (Vector2) b) / 1.5f;
        }

        public float GetOverrideMaxDistance() => -1f;

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
            TextBoxManager.ClearTextBox(this.talkPoint);
        }

        public void Interact(PlayerController interactor)
        {
            if (TextBoxManager.HasTextBox(this.talkPoint))
                return;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(StringTableManager.GetLongString(this.targetDisplayKey));
            stringBuilder.Append("\n");
            stringBuilder.Append("\n");
            stringBuilder.Append(StringTableManager.EvaluateReplacementToken("%BTCKTP_PRIMER") + " ");
            stringBuilder.Append(StringTableManager.EvaluateReplacementToken("%BTCKTP_POWDER") + " ");
            stringBuilder.Append(StringTableManager.EvaluateReplacementToken("%BTCKTP_SLUG") + " ");
            stringBuilder.Append(StringTableManager.EvaluateReplacementToken("%BTCKTP_CASING"));
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, stringBuilder.ToString());
            tk2dTextMesh[] componentsInChildren1 = this.talkPoint.GetComponentsInChildren<tk2dTextMesh>();
            if (componentsInChildren1 != null && componentsInChildren1.Length > 0)
            {
                for (int index = 0; index < componentsInChildren1.Length; ++index)
                {
                    tk2dTextMesh tk2dTextMesh = componentsInChildren1[index];
                    tk2dTextMesh.LineSpacing = -0.25f;
                    tk2dTextMesh.transform.localPosition = tk2dTextMesh.transform.localPosition + new Vector3(0.0f, -0.375f, 0.0f);
                    tk2dTextMesh.ForceBuild();
                }
            }
            tk2dBaseSprite[] componentsInChildren2 = (tk2dBaseSprite[]) this.talkPoint.GetComponentsInChildren<tk2dSprite>();
            for (int index = 0; index < componentsInChildren2.Length; ++index)
            {
                if (componentsInChildren2[index].CurrentSprite.name.StartsWith("forged_bullet"))
                {
                    if (componentsInChildren2[index].CurrentSprite.name.Contains("primer"))
                        componentsInChildren2[index].renderer.material.SetFloat("_SaturationModifier", !GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT1) ? 0.0f : 1f);
                    if (componentsInChildren2[index].CurrentSprite.name.Contains("powder"))
                        componentsInChildren2[index].renderer.material.SetFloat("_SaturationModifier", !GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT2) ? 0.0f : 1f);
                    if (componentsInChildren2[index].CurrentSprite.name.Contains("slug"))
                        componentsInChildren2[index].renderer.material.SetFloat("_SaturationModifier", !GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT3) ? 0.0f : 1f);
                    if (componentsInChildren2[index].CurrentSprite.name.Contains("case"))
                        componentsInChildren2[index].renderer.material.SetFloat("_SaturationModifier", !GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT4) ? 0.0f : 1f);
                }
            }
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }
    }

