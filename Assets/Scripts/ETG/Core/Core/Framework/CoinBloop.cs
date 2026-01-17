// Decompiled with JetBrains decompiler
// Type: CoinBloop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class CoinBloop : BraveBehaviour
    {
      private static int bloopCounter;
      public float bloopWait = 0.7f;
      private int m_bloopIndex = -1;
      private Vector3 m_cachedLocalPosition;
      private Vector3 m_cachedParentLocalPosition;
      private int m_cachedBloopCounter = -1;
      private float elapsed;
      private PlayerController m_player;
      private Bounds? m_sprBounds;

      private void Start()
      {
        if (this.m_bloopIndex != -1)
          return;
        this.gameObject.layer = LayerMask.NameToLayer("Unpixelated");
        this.transform.localPosition = BraveUtility.QuantizeVector(this.transform.localPosition, 16f);
        this.m_cachedLocalPosition = this.transform.localPosition;
        this.renderer.enabled = false;
      }

      private void Update()
      {
        if (this.m_bloopIndex <= -1)
          return;
        if (CoinBloop.bloopCounter > this.m_cachedBloopCounter)
        {
          this.m_cachedBloopCounter = CoinBloop.bloopCounter;
          if ((double) this.elapsed / (double) this.bloopWait > 0.75)
            this.elapsed = this.bloopWait;
        }
        if (!this.m_sprBounds.HasValue)
          this.m_sprBounds = new Bounds?(this.sprite.GetBounds());
        float num1 = Mathf.Max(0.625f, (float) ((double) this.m_sprBounds.Value.extents.y * 2.0 + 1.0 / 16.0));
        float num2 = (float) (CoinBloop.bloopCounter - this.m_bloopIndex) * num1;
        float num3 = 0.0f;
        if ((bool) (Object) GameUIRoot.Instance && (bool) (Object) this.m_player && (bool) (Object) GameUIRoot.Instance.GetReloadBarForPlayer(this.m_player) && GameUIRoot.Instance.GetReloadBarForPlayer(this.m_player).ReloadIsActive)
          num3 = 0.5f;
        this.transform.parent.localPosition = this.m_cachedParentLocalPosition + new Vector3(0.0f, num2 + num3, 0.0f);
      }

      protected void DoBloopInternal(
        tk2dBaseSprite targetSprite,
        string overrideSprite,
        Color tintColor,
        bool addOutline = false)
      {
        this.m_bloopIndex = CoinBloop.bloopCounter;
        this.m_cachedBloopCounter = CoinBloop.bloopCounter;
        if (string.IsNullOrEmpty(overrideSprite))
          this.sprite.SetSprite(targetSprite.Collection, targetSprite.spriteId);
        else
          this.sprite.SetSprite(targetSprite.Collection, overrideSprite);
        this.sprite.color = tintColor;
        if (addOutline)
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white);
        Bounds bounds = this.sprite.GetBounds();
        float num = bounds.min.x + bounds.extents.x;
        this.transform.parent.position = this.transform.parent.position.WithX(BraveMathCollege.QuantizeFloat(this.transform.parent.parent.GetComponent<PlayerController>().LockedApproximateSpriteCenter.x - num, 1f / 16f));
        this.transform.parent.localPosition = this.transform.parent.localPosition.WithZ(-5f);
        this.m_cachedParentLocalPosition = this.transform.parent.localPosition;
        this.StartCoroutine(this.Bloop());
      }

      public void DoBloop(
        tk2dBaseSprite targetSprite,
        string overrideSprite,
        Color tintColor,
        bool addOutline = false)
      {
        ++CoinBloop.bloopCounter;
        if ((Object) this.m_player == (Object) null)
          this.m_player = this.GetComponentInParent<PlayerController>();
        GameObject gameObject = Object.Instantiate<GameObject>(this.transform.parent.gameObject);
        gameObject.transform.parent = this.transform.parent.parent;
        gameObject.transform.position = this.transform.parent.position;
        CoinBloop componentInParent = gameObject.transform.GetChild(0).GetComponentInParent<CoinBloop>();
        componentInParent.m_player = this.m_player;
        componentInParent.DoBloopInternal(targetSprite, overrideSprite, tintColor, addOutline);
      }

      [DebuggerHidden]
      private IEnumerator Bloop()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CoinBloop.<Bloop>c__Iterator0()
        {
          $this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
