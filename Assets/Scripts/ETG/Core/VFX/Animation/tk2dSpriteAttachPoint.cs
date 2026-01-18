using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/Sprite/tk2dSpriteAttachPoint")]
[ExecuteInEditMode]
public class tk2dSpriteAttachPoint : MonoBehaviour
  {
    private tk2dBaseSprite sprite;
    public List<Transform> attachPoints = new List<Transform>();
    private static bool[] attachPointUpdated = new bool[32 /*0x20*/];
    public bool deactivateUnusedAttachPoints;
    public bool disableEmissionOnUnusedParticleSystems;
    public bool ignorePosition;
    public bool ignoreScale;
    public bool ignoreRotation;
    public bool centerUnusedAttachPoints;
    private List<string> attachPointNames = new List<string>();
    private static tk2dSpriteDefinition.AttachPoint[] emptyAttachPointArray = (tk2dSpriteDefinition.AttachPoint[]) null;

    public Transform GetAttachPointByName(string name)
    {
      if (this.attachPoints.Count != this.attachPointNames.Count)
        this.ReinitAttachPointNames();
      for (int index = 0; index < this.attachPoints.Count; ++index)
      {
        if (this.attachPoints[index].name.ToLowerInvariant() == name.ToLowerInvariant())
          return this.attachPoints[index];
      }
      return (Transform) null;
    }

    private void ReinitAttachPointNames()
    {
      this.attachPointNames.Clear();
      for (int index = 0; index < this.attachPoints.Count; ++index)
        this.attachPointNames.Add(!(bool) (UnityEngine.Object) this.attachPoints[index] ? (string) null : this.attachPoints[index].name);
    }

    private void Awake()
    {
      if (!((UnityEngine.Object) this.sprite == (UnityEngine.Object) null))
        return;
      this.sprite = this.GetComponent<tk2dBaseSprite>();
      if (!((UnityEngine.Object) this.sprite != (UnityEngine.Object) null))
        return;
      this.HandleSpriteChanged(this.sprite);
    }

    private void OnEnable()
    {
      if (!((UnityEngine.Object) this.sprite != (UnityEngine.Object) null))
        return;
      this.sprite.SpriteChanged += new Action<tk2dBaseSprite>(this.HandleSpriteChanged);
    }

    private void OnDisable()
    {
      if (!((UnityEngine.Object) this.sprite != (UnityEngine.Object) null))
        return;
      this.sprite.SpriteChanged -= new Action<tk2dBaseSprite>(this.HandleSpriteChanged);
    }

    private void UpdateAttachPointTransform(tk2dSpriteDefinition.AttachPoint attachPoint, Transform t)
    {
      if (!this.ignorePosition)
        t.localPosition = Vector3.Scale(attachPoint.position, this.sprite.scale);
      if (!this.ignoreScale)
        t.localScale = this.sprite.scale;
      if (!this.ignoreRotation)
      {
        float num = Mathf.Sign(this.sprite.scale.x) * Mathf.Sign(this.sprite.scale.y);
        t.localEulerAngles = new Vector3(0.0f, 0.0f, attachPoint.angle * num);
      }
      if (!this.disableEmissionOnUnusedParticleSystems)
        return;
      ParticleSystem component = t.GetComponent<ParticleSystem>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      BraveUtility.EnableEmission(component, true);
    }

    public void ForceAddAttachPoint(string apname)
    {
      GameObject gameObject = new GameObject(apname);
      Transform transform = gameObject.transform;
      transform.parent = this.transform;
      if (this.deactivateUnusedAttachPoints)
        gameObject.SetActive(false);
      this.attachPoints.Add(transform);
    }

    private void HandleSpriteChanged(tk2dBaseSprite spr)
    {
      tk2dSpriteDefinition.AttachPoint[] attachPointArray = spr.Collection.GetAttachPoints(spr.spriteId);
      if (tk2dSpriteAttachPoint.emptyAttachPointArray == null)
        tk2dSpriteAttachPoint.emptyAttachPointArray = new tk2dSpriteDefinition.AttachPoint[0];
      if (attachPointArray == null)
        attachPointArray = tk2dSpriteAttachPoint.emptyAttachPointArray;
      int length = Mathf.Max(attachPointArray.Length, this.attachPoints.Count);
      if (length > tk2dSpriteAttachPoint.attachPointUpdated.Length)
        tk2dSpriteAttachPoint.attachPointUpdated = new bool[length];
      if (this.attachPoints.Count != this.attachPointNames.Count)
        this.ReinitAttachPointNames();
      for (int index = 0; index < tk2dSpriteAttachPoint.attachPointUpdated.Length; ++index)
        tk2dSpriteAttachPoint.attachPointUpdated[index] = false;
      for (int index1 = 0; index1 < attachPointArray.Length; ++index1)
      {
        if (this.attachPoints.Count != this.attachPointNames.Count)
          this.ReinitAttachPointNames();
        tk2dSpriteDefinition.AttachPoint attachPoint1 = attachPointArray[index1];
        bool flag = false;
        for (int index2 = 0; index2 < this.attachPoints.Count; ++index2)
        {
          Transform attachPoint2 = this.attachPoints[index2];
          int index3 = this.attachPoints.IndexOf(attachPoint2);
          if ((UnityEngine.Object) attachPoint2 != (UnityEngine.Object) null && this.attachPointNames[index2] == attachPoint1.name)
          {
            if (this.deactivateUnusedAttachPoints || this.centerUnusedAttachPoints || this.disableEmissionOnUnusedParticleSystems)
              tk2dSpriteAttachPoint.attachPointUpdated[index3] = true;
            this.UpdateAttachPointTransform(attachPoint1, attachPoint2);
            flag = true;
          }
        }
        if (!flag)
        {
          Transform transform = new GameObject(attachPoint1.name).transform;
          transform.parent = this.transform;
          this.UpdateAttachPointTransform(attachPoint1, transform);
          this.attachPoints.Add(transform);
        }
      }
      if (this.centerUnusedAttachPoints)
      {
        for (int index = 0; index < tk2dSpriteAttachPoint.attachPointUpdated.Length; ++index)
        {
          if (index < this.attachPoints.Count && (UnityEngine.Object) this.attachPoints[index] != (UnityEngine.Object) null)
          {
            GameObject gameObject = this.attachPoints[index].gameObject;
            if (!tk2dSpriteAttachPoint.attachPointUpdated[index] && gameObject.activeSelf)
              gameObject.transform.position = spr.WorldCenter.ToVector3ZUp(gameObject.transform.position.z);
          }
        }
      }
      if (this.disableEmissionOnUnusedParticleSystems)
      {
        for (int index = 0; index < tk2dSpriteAttachPoint.attachPointUpdated.Length; ++index)
        {
          if (index < this.attachPoints.Count && !tk2dSpriteAttachPoint.attachPointUpdated[index] && (UnityEngine.Object) this.attachPoints[index] != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.attachPoints[index].gameObject)
          {
            ParticleSystem component = this.attachPoints[index].gameObject.GetComponent<ParticleSystem>();
            if ((bool) (UnityEngine.Object) component)
              BraveUtility.EnableEmission(component, false);
          }
        }
      }
      if (!this.deactivateUnusedAttachPoints)
        return;
      for (int index = 0; index < this.attachPoints.Count; ++index)
      {
        if ((UnityEngine.Object) this.attachPoints[index] != (UnityEngine.Object) null)
        {
          GameObject gameObject = this.attachPoints[index].gameObject;
          if (tk2dSpriteAttachPoint.attachPointUpdated[index] && !gameObject.activeSelf)
            gameObject.SetActive(true);
          else if (!tk2dSpriteAttachPoint.attachPointUpdated[index] && gameObject.activeSelf)
            gameObject.SetActive(false);
        }
        tk2dSpriteAttachPoint.attachPointUpdated[index] = false;
      }
    }
  }

