// Decompiled with JetBrains decompiler
// Type: PlayerCommentInteractable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class PlayerCommentInteractable : BraveBehaviour, IPlayerInteractable
    {
      public CommentModule[] comments;
      public bool onlyTriggerOnce;
      public PlayerCommentInteractable[] linkedInteractables;
      public bool keyIsSequential;
      [Header("Interactable Region")]
      public bool usesOverrideInteractionRegion;
      [ShowInInspectorIf("usesOverrideInteractionRegion", false)]
      public Vector2 overrideRegionOffset = Vector2.zero;
      [ShowInInspectorIf("usesOverrideInteractionRegion", false)]
      public Vector2 overrideRegionDimensions = Vector2.zero;
      private bool m_isDoing;
      private bool m_hasBeenTriggered;
      public System.Action OnInteractionBegan;
      public System.Action OnInteractionFinished;
      private int m_seqIndex;

      private void Start()
      {
      }

      [DebuggerHidden]
      private IEnumerator Do()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PlayerCommentInteractable.<Do>c__Iterator0()
        {
          $this = this
        };
      }

      public void DoAmbientTalk(
        Transform baseTransform,
        Vector3 offset,
        string stringKey,
        float duration,
        string overrideAudioTag = "")
      {
        string stringSequential;
        if (this.keyIsSequential)
        {
          stringSequential = StringTableManager.GetStringSequential(stringKey, ref this.m_seqIndex);
          for (int index = 0; index < this.linkedInteractables.Length; ++index)
            ++this.linkedInteractables[index].m_seqIndex;
        }
        else
          stringSequential = StringTableManager.GetString(stringKey);
        TextBoxManager.ShowThoughtBubble(baseTransform.position + offset, baseTransform, duration, stringSequential, false, overrideAudioTag: overrideAudioTag);
      }

      public float GetDistanceToPoint(Vector2 point)
      {
        if (this.m_hasBeenTriggered && this.onlyTriggerOnce || this.m_isDoing)
          return 1000f;
        if (this.usesOverrideInteractionRegion)
          return BraveMathCollege.DistToRectangle(point, this.transform.position.XY() + this.overrideRegionOffset * (1f / 16f), this.overrideRegionDimensions * (1f / 16f));
        Bounds bounds = this.sprite.GetBounds();
        bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
        float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
        float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
        return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
      }

      public void OnEnteredRange(PlayerController interactor)
      {
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
      }

      public void OnExitRange(PlayerController interactor)
      {
        if (!(bool) (UnityEngine.Object) this.sprite)
          return;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
      }

      public void ForceDisable()
      {
        this.m_hasBeenTriggered = true;
        this.onlyTriggerOnce = true;
      }

      public void Interact(PlayerController interactor)
      {
        if (this.m_hasBeenTriggered && this.onlyTriggerOnce || this.m_isDoing)
          return;
        for (int index = 0; index < this.linkedInteractables.Length; ++index)
          this.linkedInteractables[index].m_hasBeenTriggered = true;
        this.m_hasBeenTriggered = true;
        this.StartCoroutine(this.Do());
      }

      public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
      {
        shouldBeFlipped = false;
        return string.Empty;
      }

      public float GetOverrideMaxDistance() => -1f;
    }

}
