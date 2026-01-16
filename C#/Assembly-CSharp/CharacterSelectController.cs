// Decompiled with JetBrains decompiler
// Type: CharacterSelectController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class CharacterSelectController : MonoBehaviour
{
  public static bool HasSelected;
  public int startCharacter = 1;
  public GameObject[] playerArrows;
  public string[] playerPrefabPaths;
  public Camera uiCamera;
  public int currentSelected;
  public Transform pterodactylVFX;
  public dfSprite[] groundWinds;
  public dfSprite[] skyWinds;
  public Image FadeImage;
  protected int m_queuedChange;
  protected bool m_isTransitioning;
  protected bool m_isInitialized;
  protected Dictionary<GameObject, dfPanel> arrowToTextPanelMap = new Dictionary<GameObject, dfPanel>();
  protected GungeonActions activeActions;
  private Vector2 m_lastMousePosition = Vector2.zero;
  private int m_lastMouseSelected = -1;

  public static string GetCharacterPathFromIdentity(PlayableCharacters id)
  {
    switch (id)
    {
      case PlayableCharacters.Pilot:
        return "PlayerRogue";
      case PlayableCharacters.Convict:
        return "PlayerConvict";
      case PlayableCharacters.Robot:
        return "PlayerRobot";
      case PlayableCharacters.Soldier:
        return "PlayerMarine";
      case PlayableCharacters.Guide:
        return "PlayerGuide";
      case PlayableCharacters.CoopCultist:
        return "PlayerCoopCultist";
      case PlayableCharacters.Bullet:
        return "PlayerBullet";
      case PlayableCharacters.Eevee:
        return "PlayerEevee";
      case PlayableCharacters.Gunslinger:
        return "PlayerGunslinger";
      default:
        return "PlayerRogue";
    }
  }

  public static string GetCharacterPathFromQuickStart()
  {
    GameOptions.QuickstartCharacter quickstartCharacter = GameManager.Options.PreferredQuickstartCharacter;
    if (quickstartCharacter == GameOptions.QuickstartCharacter.LAST_USED)
    {
      switch (GameManager.Options.LastPlayedCharacter)
      {
        case PlayableCharacters.Pilot:
          quickstartCharacter = GameOptions.QuickstartCharacter.PILOT;
          break;
        case PlayableCharacters.Convict:
          quickstartCharacter = GameOptions.QuickstartCharacter.CONVICT;
          break;
        case PlayableCharacters.Robot:
          quickstartCharacter = GameOptions.QuickstartCharacter.ROBOT;
          break;
        case PlayableCharacters.Soldier:
          quickstartCharacter = GameOptions.QuickstartCharacter.SOLDIER;
          break;
        case PlayableCharacters.Guide:
          quickstartCharacter = GameOptions.QuickstartCharacter.GUIDE;
          break;
        case PlayableCharacters.Bullet:
          quickstartCharacter = GameOptions.QuickstartCharacter.BULLET;
          break;
        default:
          quickstartCharacter = GameOptions.QuickstartCharacter.PILOT;
          break;
      }
    }
    if (quickstartCharacter == GameOptions.QuickstartCharacter.BULLET && !GameStatsManager.Instance.GetFlag(GungeonFlags.SECRET_BULLETMAN_SEEN_05))
      quickstartCharacter = GameOptions.QuickstartCharacter.PILOT;
    if (quickstartCharacter == GameOptions.QuickstartCharacter.ROBOT && !GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_RECEIVED_BUSTED_TELEVISION))
      quickstartCharacter = GameOptions.QuickstartCharacter.PILOT;
    switch (quickstartCharacter)
    {
      case GameOptions.QuickstartCharacter.PILOT:
        return "PlayerRogue";
      case GameOptions.QuickstartCharacter.CONVICT:
        return "PlayerConvict";
      case GameOptions.QuickstartCharacter.SOLDIER:
        return "PlayerMarine";
      case GameOptions.QuickstartCharacter.GUIDE:
        return "PlayerGuide";
      case GameOptions.QuickstartCharacter.BULLET:
        return "PlayerBullet";
      case GameOptions.QuickstartCharacter.ROBOT:
        return "PlayerRobot";
      default:
        return "PlayerRogue";
    }
  }

  private void Start()
  {
    this.FadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1f);
    this.StartCoroutine(this.LerpFadeAlpha(1f, 0.0f, 0.3f));
    CharacterSelectController.HasSelected = false;
    this.currentSelected = this.startCharacter;
    this.m_lastMouseSelected = this.currentSelected;
    for (int index = 0; index < this.playerArrows.Length; ++index)
    {
      this.arrowToTextPanelMap.Add(this.playerArrows[index], this.playerArrows[index].transform.parent.parent.Find("TextPanel").GetComponent<dfPanel>());
      if (this.currentSelected != index)
      {
        this.playerArrows[index].SetActive(false);
      }
      else
      {
        this.playerArrows[index].GetComponent<tk2dSpriteAnimator>().Play();
        dfPanel arrowToTextPanel = this.arrowToTextPanelMap[this.playerArrows[index]];
        arrowToTextPanel.Width = 500f;
        dfPanel dfPanel = arrowToTextPanel;
        dfPanel.ResolutionChangedPostLayout = dfPanel.ResolutionChangedPostLayout + new Action<dfControl, Vector3, Vector3>(this.ResolutionChangedPanel);
        this.ResolutionChangedPanel((dfControl) arrowToTextPanel, Vector3.zero, Vector3.zero);
      }
    }
    this.StartCoroutine(this.HandleGroundWinds());
    this.StartCoroutine(this.HandleSkyWinds());
    this.StartCoroutine(this.HandlePterodactyl());
  }

  public void OnDestroy()
  {
    if (this.activeActions == null)
      return;
    this.activeActions.Destroy();
    this.activeActions = (GungeonActions) null;
  }

  [DebuggerHidden]
  private IEnumerator HandleSkyWinds()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CharacterSelectController.\u003CHandleSkyWinds\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleGroundWinds()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CharacterSelectController.\u003CHandleGroundWinds\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandlePterodactyl()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CharacterSelectController.\u003CHandlePterodactyl\u003Ec__Iterator2()
    {
      \u0024this = this
    };
  }

  private void Initialize()
  {
    this.m_isInitialized = true;
    uint out_bankID = 1;
    DebugTime.RecordStartTime();
    int num1 = (int) AkSoundEngine.LoadBank("SFX.bnk", -1, out out_bankID);
    DebugTime.Log("CharacterSelectController.Initialize.LoadBank()");
    int num2 = (int) AkSoundEngine.PostEvent("Play_AMB_night_loop_01", this.gameObject);
  }

  private void Do()
  {
    CharacterSelectController.HasSelected = true;
    CharacterSelectIdleDoer componentInParent = this.playerArrows[this.currentSelected].GetComponentInParent<CharacterSelectIdleDoer>();
    componentInParent.enabled = false;
    float delayTime = 0.25f;
    if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null && !string.IsNullOrEmpty(componentInParent.onSelectedAnimation))
    {
      tk2dSpriteAnimationClip clipByName = componentInParent.spriteAnimator.GetClipByName(componentInParent.onSelectedAnimation);
      delayTime = (float) clipByName.frames.Length / clipByName.fps;
      componentInParent.spriteAnimator.Play(clipByName);
    }
    this.StartCoroutine(this.OnSelectedCharacter(delayTime, this.playerPrefabPaths[this.currentSelected]));
  }

  [DebuggerHidden]
  private IEnumerator OnSelectedCharacter(float delayTime, string playerPrefabPath)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CharacterSelectController.\u003COnSelectedCharacter\u003Ec__Iterator3()
    {
      delayTime = delayTime,
      \u0024this = this
    };
  }

  [DebuggerHidden]
  private IEnumerator HandleTransition(GameObject arrowToSlide, GameObject targetArrow)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CharacterSelectController.\u003CHandleTransition\u003Ec__Iterator4()
    {
      arrowToSlide = arrowToSlide,
      targetArrow = targetArrow,
      \u0024this = this
    };
  }

  private void ResolutionChangedPanel(
    dfControl newTextPanel,
    Vector3 previousRelativePosition,
    Vector3 newRelativePosition)
  {
    dfLabel component1 = newTextPanel.transform.Find("NameLabel").GetComponent<dfLabel>();
    dfLabel component2 = newTextPanel.transform.Find("DescLabel").GetComponent<dfLabel>();
    dfLabel component3 = newTextPanel.transform.Find("GunLabel").GetComponent<dfLabel>();
    float num = (float) ((double) Screen.height * (double) component1.GetManager().RenderCamera.rect.height / 1080.0 * 4.0);
    int bottom = Mathf.FloorToInt(num);
    tk2dBaseSprite sprite = newTextPanel.Parent.GetComponentsInChildren<CharacterSelectFacecardIdleDoer>(true)[0].sprite;
    newTextPanel.transform.position = sprite.transform.position + new Vector3(18f * num * component1.PixelsToUnits(), 41f * num * component1.PixelsToUnits(), 0.0f);
    component1.TextScale = num;
    component2.TextScale = num;
    component3.TextScale = num;
    component1.Padding = new RectOffset(2 * bottom, 2 * bottom, -2 * bottom, bottom);
    component2.Padding = new RectOffset(2 * bottom, 2 * bottom, -2 * bottom, bottom);
    component3.Padding = new RectOffset(2 * bottom, 2 * bottom, -2 * bottom, bottom);
    component1.RelativePosition = new Vector3(num * 2f, num, 0.0f);
    component2.RelativePosition = new Vector3(0.0f, num + component1.Size.y, 0.0f) + component1.RelativePosition;
    component3.RelativePosition = new Vector3(0.0f, num + component2.Size.y, 0.0f) + component2.RelativePosition;
  }

  private void HandleShiftLeft()
  {
    if (this.m_isTransitioning)
    {
      this.m_queuedChange = -1;
    }
    else
    {
      this.currentSelected = (this.currentSelected - 1 + this.playerArrows.Length) % this.playerArrows.Length;
      int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
    }
  }

  private void HandleShiftRight()
  {
    if (this.m_isTransitioning)
    {
      this.m_queuedChange = 1;
    }
    else
    {
      this.currentSelected = (this.currentSelected + 1 + this.playerArrows.Length) % this.playerArrows.Length;
      int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
    }
  }

  private void ForceSelect(int index)
  {
    if (this.m_isTransitioning)
      return;
    this.currentSelected = index;
    int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
  }

  [DebuggerHidden]
  private IEnumerator LerpFadeAlpha(float startAlpha, float targetAlpha, float duration)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new CharacterSelectController.\u003CLerpFadeAlpha\u003Ec__Iterator5()
    {
      startAlpha = startAlpha,
      targetAlpha = targetAlpha,
      duration = duration,
      \u0024this = this
    };
  }

  public void HandleSelect()
  {
    if (GameManager.Instance.IsLoadingLevel)
      return;
    this.Do();
    int num1 = (int) AkSoundEngine.PostEvent("Play_UI_menu_characterselect_01", this.gameObject);
    int num2 = (int) AkSoundEngine.PostEvent("Stop_AMB_night_loop_01", this.gameObject);
  }

  private void Update()
  {
    if (!this.m_isInitialized)
      this.Initialize();
    if (CharacterSelectController.HasSelected)
      return;
    GameObject playerArrow1 = this.playerArrows[this.currentSelected];
    this.ResolutionChangedPanel((dfControl) this.arrowToTextPanelMap[this.playerArrows[this.currentSelected]], Vector3.zero, Vector3.zero);
    if ((double) (Input.mousePosition.XY() - this.m_lastMousePosition).magnitude > 2.0)
    {
      int index1 = -1;
      float num1 = float.MaxValue;
      Vector2 a = this.uiCamera.ScreenToWorldPoint(Input.mousePosition).XY();
      for (int index2 = 0; index2 < this.playerArrows.Length; ++index2)
      {
        tk2dBaseSprite component = this.playerArrows[index2].transform.parent.GetComponent<tk2dBaseSprite>();
        Vector2 b = component.transform.position.XY() + Vector2.Scale(component.transform.localScale.XY(), Vector2.Scale(component.scale.XY(), component.GetUntrimmedBounds().extents.XY()));
        float num2 = Vector2.Distance(a, b);
        if ((double) num2 < (double) num1 && (double) num2 < 0.10000000149011612)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      if (!this.m_isTransitioning)
      {
        if (index1 != -1 && index1 != this.currentSelected)
        {
          this.ForceSelect(index1);
          this.currentSelected = index1;
        }
        this.m_lastMouseSelected = index1;
      }
    }
    if (this.activeActions.SelectLeft.WasPressedAsDpadRepeating)
      this.HandleShiftLeft();
    if (this.activeActions.SelectRight.WasPressedAsDpadRepeating)
      this.HandleShiftRight();
    if (this.activeActions.MenuSelectAction.WasPressed || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
      this.HandleSelect();
    if (Input.GetMouseButtonDown(0) && this.m_lastMouseSelected != -1)
    {
      this.currentSelected = this.m_lastMouseSelected;
      this.HandleSelect();
    }
    if (this.m_queuedChange != 0 && !this.m_isTransitioning)
    {
      if ((UnityEngine.Object) playerArrow1 == (UnityEngine.Object) this.playerArrows[this.currentSelected])
      {
        this.currentSelected = (this.currentSelected + this.m_queuedChange + this.playerArrows.Length) % this.playerArrows.Length;
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
      }
      this.m_queuedChange = 0;
    }
    GameObject playerArrow2 = this.playerArrows[this.currentSelected];
    if ((UnityEngine.Object) playerArrow1 != (UnityEngine.Object) playerArrow2)
      this.StartCoroutine(this.HandleTransition(playerArrow1, playerArrow2));
    this.m_lastMousePosition = (Vector2) Input.mousePosition;
  }
}
