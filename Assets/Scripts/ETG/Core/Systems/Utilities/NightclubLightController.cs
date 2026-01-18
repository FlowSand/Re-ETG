using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class NightclubLightController : MonoBehaviour
  {
    private static List<Vector2> m_registeredPositions;
    public float LightChangePeriod = 1f;
    public float MotionMaxSpeed = 5f;
    public Rect ValidMotionRect;
    private float m_elapsed;
    private Transform m_transform;
    private ShadowSystem m_light;
    private SceneLightManager m_colors;
    private RoomHandler m_parentRoom;
    private Material m_lightMaterial;
    private int m_colorID;
    private int m_positionIndex;
    private bool m_inMotion;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new NightclubLightController__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void Update()
    {
      if ((Object) this.m_light == (Object) null)
        return;
      this.m_elapsed += !GameManager.IsBossIntro ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME;
      if ((double) this.m_elapsed > (double) this.LightChangePeriod)
      {
        this.m_elapsed -= this.LightChangePeriod;
        this.m_lightMaterial.SetColor(this.m_colorID, this.m_colors.validColors[Random.Range(0, this.m_colors.validColors.Length)]);
      }
      if (this.m_inMotion)
        return;
      this.StartCoroutine(this.HandleMotion());
    }

    private bool CheckPositionValid(Vector2 targetPosition)
    {
      bool flag = true;
      for (int index = 0; index < NightclubLightController.m_registeredPositions.Count; ++index)
      {
        if ((double) Vector2.Distance(NightclubLightController.m_registeredPositions[index], targetPosition) < 3.0)
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    [DebuggerHidden]
    private IEnumerator HandleMotion()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new NightclubLightController__HandleMotionc__Iterator1()
      {
        _this = this
      };
    }
  }

