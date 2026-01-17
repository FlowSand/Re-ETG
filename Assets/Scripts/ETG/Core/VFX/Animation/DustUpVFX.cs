// Decompiled with JetBrains decompiler
// Type: DustUpVFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [Serializable]
    public class DustUpVFX
    {
      public GameObject runDustup;
      public GameObject waterDustup;
      public GameObject additionalWaterDustup;
      public GameObject rollNorthDustup;
      public GameObject rollNorthEastDustup;
      public GameObject rollEastDustup;
      public GameObject rollSouthEastDustup;
      public GameObject rollSouthDustup;
      public GameObject rollSouthWestDustup;
      public GameObject rollWestDustup;
      public GameObject rollNorthWestDustup;
      public GameObject rollLandDustup;

      public void InstantiateLandDustup(Vector3 worldPosition)
      {
        SpawnManager.SpawnVFX(this.rollLandDustup, worldPosition, Quaternion.identity);
      }

      public void InstantiateDodgeDustup(Vector2 velocity, Vector3 worldPosition)
      {
        switch (DungeonData.GetDirectionFromVector2(velocity))
        {
          case DungeonData.Direction.NORTH:
            if ((UnityEngine.Object) this.rollNorthDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject = SpawnManager.SpawnVFX(this.rollNorthDustup, worldPosition, Quaternion.identity);
              gameObject.GetComponent<tk2dSprite>().FlipX = false;
              gameObject.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject1 = SpawnManager.SpawnVFX(this.rollSouthDustup, worldPosition, Quaternion.identity);
            gameObject1.GetComponent<tk2dSprite>().FlipX = false;
            gameObject1.GetComponent<tk2dSprite>().FlipY = true;
            break;
          case DungeonData.Direction.NORTHEAST:
            if ((UnityEngine.Object) this.rollNorthEastDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject2 = SpawnManager.SpawnVFX(this.rollNorthEastDustup, worldPosition, Quaternion.identity);
              gameObject2.GetComponent<tk2dSprite>().FlipX = false;
              gameObject2.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject3 = SpawnManager.SpawnVFX(this.rollNorthWestDustup, worldPosition, Quaternion.identity);
            gameObject3.GetComponent<tk2dSprite>().FlipX = true;
            gameObject3.GetComponent<tk2dSprite>().FlipY = false;
            break;
          case DungeonData.Direction.EAST:
            if ((UnityEngine.Object) this.rollEastDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject4 = SpawnManager.SpawnVFX(this.rollEastDustup, worldPosition, Quaternion.identity);
              gameObject4.GetComponent<tk2dSprite>().FlipX = false;
              gameObject4.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject5 = SpawnManager.SpawnVFX(this.rollWestDustup, worldPosition, Quaternion.identity);
            gameObject5.GetComponent<tk2dSprite>().FlipX = true;
            gameObject5.GetComponent<tk2dSprite>().FlipY = false;
            break;
          case DungeonData.Direction.SOUTHEAST:
            if ((UnityEngine.Object) this.rollSouthEastDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject6 = SpawnManager.SpawnVFX(this.rollSouthEastDustup, worldPosition, Quaternion.identity);
              gameObject6.GetComponent<tk2dSprite>().FlipX = false;
              gameObject6.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject7 = SpawnManager.SpawnVFX(this.rollSouthWestDustup, worldPosition, Quaternion.identity);
            gameObject7.GetComponent<tk2dSprite>().FlipX = true;
            gameObject7.GetComponent<tk2dSprite>().FlipY = false;
            break;
          case DungeonData.Direction.SOUTH:
            if ((UnityEngine.Object) this.rollSouthDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject8 = SpawnManager.SpawnVFX(this.rollSouthDustup, worldPosition, Quaternion.identity);
              gameObject8.GetComponent<tk2dSprite>().FlipX = false;
              gameObject8.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject9 = SpawnManager.SpawnVFX(this.rollNorthDustup, worldPosition, Quaternion.identity);
            gameObject9.GetComponent<tk2dSprite>().FlipX = false;
            gameObject9.GetComponent<tk2dSprite>().FlipY = true;
            break;
          case DungeonData.Direction.SOUTHWEST:
            if ((UnityEngine.Object) this.rollSouthWestDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject10 = SpawnManager.SpawnVFX(this.rollSouthWestDustup, worldPosition, Quaternion.identity);
              gameObject10.GetComponent<tk2dSprite>().FlipX = false;
              gameObject10.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject11 = SpawnManager.SpawnVFX(this.rollSouthEastDustup, worldPosition, Quaternion.identity);
            gameObject11.GetComponent<tk2dSprite>().FlipX = true;
            gameObject11.GetComponent<tk2dSprite>().FlipY = false;
            break;
          case DungeonData.Direction.WEST:
            if ((UnityEngine.Object) this.rollWestDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject12 = SpawnManager.SpawnVFX(this.rollWestDustup, worldPosition, Quaternion.identity);
              gameObject12.GetComponent<tk2dSprite>().FlipX = false;
              gameObject12.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject13 = SpawnManager.SpawnVFX(this.rollEastDustup, worldPosition, Quaternion.identity);
            gameObject13.GetComponent<tk2dSprite>().FlipX = true;
            gameObject13.GetComponent<tk2dSprite>().FlipY = false;
            break;
          case DungeonData.Direction.NORTHWEST:
            if ((UnityEngine.Object) this.rollNorthWestDustup != (UnityEngine.Object) null)
            {
              GameObject gameObject14 = SpawnManager.SpawnVFX(this.rollNorthWestDustup, worldPosition, Quaternion.identity);
              gameObject14.GetComponent<tk2dSprite>().FlipX = false;
              gameObject14.GetComponent<tk2dSprite>().FlipY = false;
              break;
            }
            GameObject gameObject15 = SpawnManager.SpawnVFX(this.rollNorthEastDustup, worldPosition, Quaternion.identity);
            gameObject15.GetComponent<tk2dSprite>().FlipX = true;
            gameObject15.GetComponent<tk2dSprite>().FlipY = false;
            break;
        }
      }
    }

}
