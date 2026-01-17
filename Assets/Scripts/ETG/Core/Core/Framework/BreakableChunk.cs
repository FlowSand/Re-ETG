// Decompiled with JetBrains decompiler
// Type: BreakableChunk
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class BreakableChunk : BraveBehaviour
    {
      public List<GameObject> subchunks;
      [Header("Puff Stuff")]
      public VFXPool puff;
      public int puffCount;
      public float puffAreaWidth;
      public float puffAreaHeight;
      public float puffSpawnDuration;
      [Header("Debris Stuff")]
      public float startingHeight;
      public float minForce;
      public float maxForce = 1f;
      public float upwardForce;
      public float angleVariance = 30f;
      public float angularVelocity = 90f;
      public float gravityOverride;
      [Header("Minutiae")]
      public float minDirectionalForce;
      public float maxDirectionalForce;
      public float directionalAngleVariance = 30f;
      public int randomDeletions;
      public bool slideMode;
      public bool useOverrideVelocityDir;
      [ShowInInspectorIf("useOverrideVelocityDir", false)]
      public Vector3 overrideVelocityDir;
      private Vector3 m_avgChunkPosition;

      public void Awake()
      {
        if (this.subchunks == null)
          this.subchunks = new List<GameObject>(this.transform.childCount);
        if (this.subchunks.Count != 0)
          return;
        for (int index = 0; index < this.transform.childCount; ++index)
          this.subchunks.Add(this.transform.GetChild(index).gameObject);
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void Trigger(bool destroyAfterTrigger = true, Vector3? directionalOrigin = null)
      {
        this.m_avgChunkPosition = Vector3.zero;
        foreach (GameObject subchunk in this.subchunks)
          this.m_avgChunkPosition += subchunk.transform.position;
        this.m_avgChunkPosition /= (float) this.subchunks.Count;
        for (int index = 0; index < this.randomDeletions && this.subchunks.Count > 1; ++index)
          this.subchunks.RemoveAt(UnityEngine.Random.Range(0, this.subchunks.Count));
        if (this.puffCount > 0)
        {
          for (int index = 0; index < this.puffCount; ++index)
          {
            if ((double) this.puffSpawnDuration == 0.0)
              this.SpawnRandomizedPuff();
            else
              this.Invoke("SpawnRandomizedPuff", UnityEngine.Random.Range(0.0f, this.puffSpawnDuration));
          }
        }
        foreach (GameObject subchunk in this.subchunks)
        {
          subchunk.transform.parent = SpawnManager.Instance.VFX;
          subchunk.SetActive(true);
          DebrisObject debrisObject = subchunk.AddComponent<DebrisObject>();
          debrisObject.bounceCount = 0;
          debrisObject.angularVelocity = this.angularVelocity;
          debrisObject.GravityOverride = this.gravityOverride;
          Vector3 vector3 = subchunk.transform.position - this.m_avgChunkPosition;
          if (this.useOverrideVelocityDir)
            vector3 = this.overrideVelocityDir;
          vector3 = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(-this.angleVariance, this.angleVariance)) * vector3;
          Vector3 startingForce = Vector3.zero;
          if (!this.slideMode)
          {
            startingForce = (vector3.normalized * UnityEngine.Random.Range(this.minForce, this.maxForce)).WithZ(this.upwardForce);
            if (directionalOrigin.HasValue)
            {
              vector3 = (subchunk.transform.position - directionalOrigin.Value).WithZ(0.0f);
              vector3 = Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(-this.directionalAngleVariance, this.directionalAngleVariance)) * vector3;
              startingForce += vector3.normalized * UnityEngine.Random.Range(this.minDirectionalForce, this.maxDirectionalForce);
            }
          }
          debrisObject.Trigger(startingForce, (double) this.startingHeight != 0.0 || this.slideMode ? this.startingHeight : 0.01f);
          if (this.slideMode)
            debrisObject.ApplyVelocity((Vector2) (vector3.normalized * UnityEngine.Random.Range(this.minForce, this.maxForce)));
          BreakableChunk chunkScript = subchunk.GetComponent<BreakableChunk>();
          if ((bool) (UnityEngine.Object) chunkScript)
            debrisObject.OnGrounded += (Action<DebrisObject>) (d => chunkScript.Trigger());
        }
        if (destroyAfterTrigger)
        {
          foreach (Renderer component in this.GetComponents<Renderer>())
            component.enabled = false;
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject, this.puffSpawnDuration + 0.5f);
        }
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this, this.puffSpawnDuration + 0.5f);
      }

      private void SpawnRandomizedPuff()
      {
        this.puff.SpawnAtPosition(this.m_avgChunkPosition + new Vector3(UnityEngine.Random.Range((float) (-(double) this.puffAreaWidth / 2.0), this.puffAreaWidth / 2f), UnityEngine.Random.Range((float) (-(double) this.puffAreaHeight / 2.0), this.puffAreaHeight / 2f)), sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero));
      }
    }

}
