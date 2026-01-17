// Decompiled with JetBrains decompiler
// Type: RobotPastController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class RobotPastController : MonoBehaviour
    {
      public bool InstantBossFight;
      public bool DoWavesOfEnemies;
      public TalkDoerLite WelcomeBot;
      public TalkDoerLite EmperorBot;
      public string[] validPrefixes;
      public string[] directionalAffixes;
      public GameObject RobotPrefab;
      public GameObject WarpVFX;
      public Vector2 outerRectMin;
      public Vector2 outerRectMax;
      public Vector2 innerRectMin;
      public Vector2 innerRectMax;
      public tk2dSprite EmperorSprite;
      public Rect excludedRect;
      [EnemyIdentifier]
      public string[] CritterIds;
      [NonSerialized]
      public List<Vector2> m_cachedPositions = new List<Vector2>();
      private List<List<Vector2>> m_points = new List<List<Vector2>>();
      private List<List<int>> m_ids = new List<List<int>>();
      private List<List<Vector2>> m_activePoints = new List<List<Vector2>>();
      private List<List<int>> m_activeIds = new List<List<int>>();
      private Dictionary<int, tk2dSpriteAnimator> m_extantRobots = new Dictionary<int, tk2dSpriteAnimator>();
      private List<tk2dSpriteAnimator> m_unusedRobots = new List<tk2dSpriteAnimator>();
      private List<List<string>> m_directionalAnimations = new List<List<string>>();
      private List<List<string>> m_directionalOffAnimations = new List<List<string>>();
      private List<Material> m_fadeMaterials = new List<Material>();
      [NonSerialized]
      private bool RobotsOff;
      private List<int> m_offPoints = new List<int>();
      private Vector2 m_centerPoint;

      private void Start()
      {
        RoomHandler entrance = GameManager.Instance.Dungeon.data.Entrance;
        this.innerRectMin = entrance.area.basePosition.ToVector2() + new Vector2(1f, 3f);
        this.innerRectMax = this.innerRectMin + entrance.area.dimensions.ToVector2() + new Vector2(-3f, -8.75f);
        this.outerRectMin = this.innerRectMin + new Vector2(-5f, -5f);
        this.outerRectMax = this.innerRectMax + new Vector2(5f, 5f);
        this.excludedRect = new Rect(this.EmperorSprite.WorldBottomLeft + new Vector2(-0.75f, -0.75f), this.EmperorSprite.WorldTopRight - this.EmperorSprite.WorldBottomLeft + new Vector2(1.25f, 1.25f));
        BraveUtility.DrawDebugSquare(this.innerRectMin, this.innerRectMax, Color.cyan, 1000f);
        BraveUtility.DrawDebugSquare(this.outerRectMin, this.outerRectMax, Color.cyan, 1000f);
        this.DistributePoints();
        float[] numArray = new float[5]
        {
          0.0f,
          0.5f,
          0.7f,
          0.9f,
          1f
        };
        for (int index = 0; index < 5; ++index)
        {
          Material material = new Material(this.RobotPrefab.GetComponent<Renderer>().sharedMaterial);
          material.SetColor("_OverrideColor", new Color(0.05f, 0.05f, 0.05f, numArray[index]));
          this.m_fadeMaterials.Add(material);
        }
        this.RobotPrefab.transform.position = new Vector3(1000f, -100f, -100f);
        if (this.InstantBossFight)
        {
          PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
          List<HealthHaver> allHealthHavers = StaticReferenceManager.AllHealthHavers;
          for (int index = 0; index < allHealthHavers.Count; ++index)
          {
            if (allHealthHavers[index].IsBoss)
            {
              allHealthHavers[index].GetComponent<ObjectVisibilityManager>().ChangeToVisibility(RoomHandler.VisibilityStatus.CURRENT);
              allHealthHavers[index].GetComponent<GenericIntroDoer>().TriggerSequence(primaryPlayer);
            }
          }
        }
        else
          this.StartCoroutine(this.HandlePastIntro());
      }

      private TerminatorPanelController HandleTerminatorUIOverlay()
      {
        dfControl dfControl = GameUIRoot.Instance.Manager.AddPrefab(ResourceCache.Acquire("Global Prefabs/TerminatorPanel") as GameObject);
        (dfControl as dfPanel).Size = GameUIRoot.Instance.Manager.GetScreenSize() * GameUIRoot.Instance.Manager.UIScale;
        TerminatorPanelController component = dfControl.GetComponent<TerminatorPanelController>();
        this.StartCoroutine(this.HandleTerminatorUIOverlay_CR(component));
        return component;
      }

      [DebuggerHidden]
      private IEnumerator HandleTerminatorUIOverlay_CR(TerminatorPanelController tpc)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RobotPastController.<HandleTerminatorUIOverlay_CR>c__Iterator0()
        {
          tpc = tpc
        };
      }

      [DebuggerHidden]
      private IEnumerator HandlePastIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RobotPastController.<HandlePastIntro>c__Iterator1()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator StartBossFight(HealthHaver boss, PlayerController m_robot)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RobotPastController.<StartBossFight>c__Iterator2()
        {
          boss = boss,
          m_robot = m_robot,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator LaunchRecenter(Vector2 targetPosition)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RobotPastController.<LaunchRecenter>c__Iterator3()
        {
          targetPosition = targetPosition
        };
      }

      public void OnBossKilled(Transform bossTransform)
      {
        this.StartCoroutine(this.OnBossKilled_CR(bossTransform));
      }

      [DebuggerHidden]
      private IEnumerator OnBossKilled_CR(Transform bossTransform)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RobotPastController.<OnBossKilled_CR>c__Iterator4()
        {
          _this = this
        };
      }

      private void DistributePoints()
      {
        Vector2 vector2 = (this.innerRectMin + this.innerRectMax) / 2f;
        for (int index1 = 0; index1 < this.validPrefixes.Length; ++index1)
        {
          this.m_points.Add(new List<Vector2>());
          this.m_ids.Add(new List<int>());
          this.m_activePoints.Add(new List<Vector2>());
          this.m_activeIds.Add(new List<int>());
          this.m_directionalAnimations.Add(new List<string>());
          this.m_directionalOffAnimations.Add(new List<string>());
          for (int index2 = 0; index2 < this.directionalAffixes.Length; ++index2)
          {
            this.m_directionalAnimations[index1].Add(this.validPrefixes[index1] + this.directionalAffixes[index2]);
            this.m_directionalOffAnimations[index1].Add($"{this.validPrefixes[index1]}_off{this.directionalAffixes[index2]}");
          }
        }
        List<Vector2> vector2List = new List<Vector2>();
        for (int index = 0; index < 1500; ++index)
        {
          Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
          Vector2 point = vector2;
          while (BraveMathCollege.AABBContains(this.innerRectMin, this.innerRectMax, point))
            point += normalized * UnityEngine.Random.Range(2f, 5f);
          if (!this.excludedRect.Contains(point))
            vector2List.Add(point);
        }
        for (int index3 = 0; index3 < vector2List.Count; ++index3)
        {
          for (int index4 = 0; index4 < vector2List.Count; ++index4)
          {
            if (index3 != index4 && (double) (vector2List[index3] - vector2List[index4]).sqrMagnitude < 0.25)
            {
              vector2List.RemoveAt(index4);
              --index4;
            }
          }
        }
        for (int index5 = 0; index5 < vector2List.Count; ++index5)
        {
          int index6 = UnityEngine.Random.Range(0, this.validPrefixes.Length);
          this.m_points[index6].Add(vector2List[index5]);
          this.m_ids[index6].Add(index5);
        }
      }

      private tk2dSpriteAnimator GetRobotAtPosition(Vector2 point)
      {
        if (this.m_unusedRobots.Count <= 0)
          return UnityEngine.Object.Instantiate<GameObject>(this.RobotPrefab, (Vector3) point, Quaternion.identity).GetComponent<tk2dSpriteAnimator>();
        tk2dSpriteAnimator unusedRobot = this.m_unusedRobots[0];
        this.m_unusedRobots.RemoveAt(0);
        unusedRobot.gameObject.SetActive(true);
        unusedRobot.transform.position = (Vector3) point;
        return unusedRobot;
      }

      public void TurnRobotsOff()
      {
        this.RobotsOff = true;
        this.StartCoroutine(this.TurnRobotsOffCR());
      }

      [DebuggerHidden]
      private IEnumerator TurnRobotsOffCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RobotPastController.<TurnRobotsOffCR>c__Iterator5()
        {
          _this = this
        };
      }

      private void LateUpdate()
      {
        if (!this.RobotsOff)
          this.m_centerPoint = GameManager.Instance.PrimaryPlayer.CenterPosition;
        Vector2 vector2_1 = GameManager.Instance.MainCameraController.MinVisiblePoint + new Vector2(-2f, -2f);
        Vector2 vector2_2 = GameManager.Instance.MainCameraController.MaxVisiblePoint + new Vector2(2f, 2f);
        for (int index1 = 0; index1 < this.m_points.Count; ++index1)
        {
          for (int index2 = 0; index2 < this.m_activePoints[index1].Count; ++index2)
          {
            int key = this.m_activeIds[index1][index2];
            Vector2 vector2_3 = this.m_activePoints[index1][index2];
            if ((double) vector2_1.x > (double) vector2_3.x || (double) vector2_1.y > (double) vector2_3.y || (double) vector2_2.x < (double) vector2_3.x || (double) vector2_2.y < (double) vector2_3.y)
            {
              tk2dSpriteAnimator extantRobot = this.m_extantRobots[key];
              extantRobot.gameObject.SetActive(false);
              this.m_unusedRobots.Add(extantRobot);
              this.m_extantRobots.Remove(key);
              this.m_activePoints[index1].RemoveAt(index2);
              this.m_activeIds[index1].RemoveAt(index2);
              --index2;
            }
            else if (Mathf.FloorToInt(UnityEngine.Time.realtimeSinceStartup) % this.m_points.Count == index1 && UnityEngine.Time.frameCount % this.m_activePoints[index1].Count == index2)
            {
              int sextant = BraveMathCollege.VectorToSextant(vector2_3 - this.m_centerPoint);
              string name = !this.RobotsOff ? this.m_directionalAnimations[index1][sextant] : this.m_directionalOffAnimations[index1][sextant];
              tk2dSpriteAnimator extantRobot = this.m_extantRobots[key];
              if (!extantRobot.IsPlaying(name) && !this.m_offPoints.Contains(key))
              {
                if (this.RobotsOff)
                  this.m_offPoints.Add(key);
                extantRobot.Play(name);
              }
            }
          }
          for (int index3 = 0; index3 < this.m_points[index1].Count; ++index3)
          {
            int key = this.m_ids[index1][index3];
            Vector2 point = this.m_points[index1][index3];
            if (!this.m_extantRobots.ContainsKey(key) && (double) vector2_1.x < (double) point.x && (double) vector2_1.y < (double) point.y && (double) vector2_2.x > (double) point.x && (double) vector2_2.y > (double) point.y)
            {
              tk2dSpriteAnimator robotAtPosition = this.GetRobotAtPosition(point);
              this.m_extantRobots.Add(key, robotAtPosition);
              this.m_activePoints[index1].Add(point);
              this.m_activeIds[index1].Add(key);
              int sextant = BraveMathCollege.VectorToSextant(point - this.m_centerPoint);
              if (!this.m_offPoints.Contains(key))
              {
                robotAtPosition.Play(!this.RobotsOff ? this.m_directionalAnimations[index1][sextant] : this.m_directionalOffAnimations[index1][sextant]);
                if (this.RobotsOff)
                  this.m_offPoints.Add(key);
              }
              else if (!robotAtPosition.IsPlaying(this.m_directionalOffAnimations[index1][sextant]))
              {
                robotAtPosition.Stop();
                robotAtPosition.sprite.SetSprite(robotAtPosition.GetClipByName(this.m_directionalOffAnimations[index1][sextant]).GetFrame(robotAtPosition.GetClipByName(this.m_directionalOffAnimations[index1][sextant]).frames.Length - 1).spriteId);
              }
              int index4 = Mathf.Max(Mathf.Min(Mathf.FloorToInt(BraveMathCollege.DistToRectangle(point, this.innerRectMin, this.innerRectMax - this.innerRectMin) * 1.5f - UnityEngine.Random.value), 4), 0);
              robotAtPosition.sprite.usesOverrideMaterial = true;
              robotAtPosition.renderer.material = this.m_fadeMaterials[index4];
              robotAtPosition.sprite.UpdateZDepth();
            }
          }
        }
      }
    }

}
