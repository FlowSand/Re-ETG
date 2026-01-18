using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class Foyer : MonoBehaviour
    {
        public static bool DoIntroSequence = true;
        public static bool DoMainMenu = true;
        private static Foyer m_instance;
        public SpeculativeRigidbody TutorialBlocker;
        public FinalIntroSequenceManager IntroDoer;
        public Action<PlayerController> OnPlayerCharacterChanged;
        public System.Action OnCoopModeChanged;
        public Renderer PrimerSprite;
        public Renderer PowderSprite;
        public Renderer SlugSprite;
        public Renderer CasingSprite;
        public Renderer StatueSprite;
        public FoyerCharacterSelectFlag CurrentSelectedCharacterFlag;
        public static bool IsCurrentlyPlayingCharacterSelect;

        public static Foyer Instance
        {
            get
            {
                if (!(bool) (UnityEngine.Object) Foyer.m_instance)
                    Foyer.m_instance = UnityEngine.Object.FindObjectOfType<Foyer>();
                return Foyer.m_instance;
            }
        }

        public static void ClearInstance() => Foyer.m_instance = (Foyer) null;

        private void Awake()
        {
            DebugTime.Log("Foyer.Awake()");
            GameManager.EnsureExistence();
            GameManager.Instance.IsFoyer = true;
        }

        private void CheckHeroStatue()
        {
            this.PrimerSprite.enabled = GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT1);
            this.PowderSprite.enabled = GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT2);
            this.SlugSprite.enabled = GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT3);
            this.CasingSprite.enabled = GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_ELEMENT4);
            if (this.PowderSprite.enabled && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN))
                this.PowderSprite.GetComponent<tk2dBaseSprite>().SetSprite("statue_of_time_gunpowder_gold_001");
            if (this.PrimerSprite.enabled && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN))
                this.PrimerSprite.GetComponent<tk2dBaseSprite>().SetSprite("statue_of_time_shield_gold_001");
            if (this.CasingSprite.enabled && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN))
                this.CasingSprite.GetComponent<tk2dBaseSprite>().SetSprite("statue_of_time_shell_gold_001");
            if (!(bool) (UnityEngine.Object) this.StatueSprite || !GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN))
                return;
            this.StatueSprite.GetComponent<tk2dBaseSprite>().SetSprite("statue_of_time_dragun_001");
            Transform transform = this.StatueSprite.transform.Find("shadow");
            if (!(bool) (UnityEngine.Object) transform)
                return;
            transform.GetComponent<tk2dBaseSprite>().SetSprite("statue_of_time_dragun_shadow_001");
        }

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Foyer__Startc__Iterator0()
            {
                _this = this
            };
        }

        private void ToggleTutorialBlocker(PlayerController player)
        {
            bool flag = false;
            if ((UnityEngine.Object) player != (UnityEngine.Object) null)
                flag = player.characterIdentity == PlayableCharacters.Convict || player.characterIdentity == PlayableCharacters.Guide || player.characterIdentity == PlayableCharacters.Pilot || player.characterIdentity == PlayableCharacters.Soldier;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                flag = false;
            if (flag)
            {
                this.TutorialBlocker.gameObject.SetActive(false);
                this.TutorialBlocker.enabled = false;
            }
            else
            {
                this.TutorialBlocker.enabled = true;
                this.TutorialBlocker.gameObject.SetActive(true);
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleIntroSequence()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Foyer__HandleIntroSequencec__Iterator1()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleMainMenu()
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            Foyer__HandleMainMenuc__Iterator2 mainMenuCIterator2 = new Foyer__HandleMainMenuc__Iterator2();
            return (IEnumerator) mainMenuCIterator2;
        }

        private void DisableActiveCharacterSelectCharacter()
        {
            FoyerCharacterSelectFlag[] objectsOfType = UnityEngine.Object.FindObjectsOfType<FoyerCharacterSelectFlag>();
            List<FoyerCharacterSelectFlag> characterSelectFlagList = new List<FoyerCharacterSelectFlag>();
            while (characterSelectFlagList.Count < objectsOfType.Length)
            {
                FoyerCharacterSelectFlag characterSelectFlag = (FoyerCharacterSelectFlag) null;
                for (int index = 0; index < objectsOfType.Length; ++index)
                {
                    if (!characterSelectFlagList.Contains(objectsOfType[index]))
                        characterSelectFlag = !((UnityEngine.Object) characterSelectFlag == (UnityEngine.Object) null) ? ((double) characterSelectFlag.transform.position.x >= (double) objectsOfType[index].transform.position.x ? objectsOfType[index] : characterSelectFlag) : objectsOfType[index];
                }
                characterSelectFlagList.Add(characterSelectFlag);
            }
            for (int index = 0; index < characterSelectFlagList.Count; ++index)
            {
                if (characterSelectFlagList[index].IsCoopCharacter)
                {
                    characterSelectFlagList.RemoveAt(index);
                    --index;
                }
                else if (!characterSelectFlagList[index].PrerequisitesFulfilled())
                {
                    characterSelectFlagList.RemoveAt(index);
                    --index;
                }
            }
            for (int index = 0; index < characterSelectFlagList.Count; ++index)
                characterSelectFlagList[index].OnSelectedCharacterCallback(GameManager.Instance.PrimaryPlayer);
        }

        private List<FoyerCharacterSelectFlag> SetUpCharacterCallbacks()
        {
            FoyerCharacterSelectFlag[] objectsOfType = UnityEngine.Object.FindObjectsOfType<FoyerCharacterSelectFlag>();
            List<FoyerCharacterSelectFlag> characterSelectFlagList = new List<FoyerCharacterSelectFlag>();
            while (characterSelectFlagList.Count < objectsOfType.Length)
            {
                FoyerCharacterSelectFlag characterSelectFlag = (FoyerCharacterSelectFlag) null;
                for (int index = 0; index < objectsOfType.Length; ++index)
                {
                    if (!characterSelectFlagList.Contains(objectsOfType[index]))
                        characterSelectFlag = !((UnityEngine.Object) characterSelectFlag == (UnityEngine.Object) null) ? ((double) characterSelectFlag.transform.position.x >= (double) objectsOfType[index].transform.position.x ? objectsOfType[index] : characterSelectFlag) : objectsOfType[index];
                }
                characterSelectFlagList.Add(characterSelectFlag);
            }
            for (int index = 0; index < characterSelectFlagList.Count; ++index)
            {
                if (characterSelectFlagList[index].IsCoopCharacter)
                {
                    this.OnCoopModeChanged += new System.Action(characterSelectFlagList[index].OnCoopChangedCallback);
                    characterSelectFlagList.RemoveAt(index);
                    --index;
                }
                else if (!characterSelectFlagList[index].PrerequisitesFulfilled())
                {
                    UnityEngine.Object.Destroy((UnityEngine.Object) characterSelectFlagList[index].gameObject);
                    characterSelectFlagList.RemoveAt(index);
                    --index;
                }
            }
            for (int index = 0; index < characterSelectFlagList.Count; ++index)
            {
                this.OnPlayerCharacterChanged += new Action<PlayerController>(characterSelectFlagList[index].OnSelectedCharacterCallback);
                tk2dBaseSprite sprite = characterSelectFlagList[index].sprite;
                sprite.usesOverrideMaterial = true;
                Renderer renderer = sprite.renderer;
                if (!renderer.material.shader.name.Contains("PlayerPalettized"))
                    renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            }
            return characterSelectFlagList;
        }

        [DebuggerHidden]
        private IEnumerator HandleAmmonomiconLabel()
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            Foyer__HandleAmmonomiconLabelc__Iterator3 ammonomiconLabelCIterator3 = new Foyer__HandleAmmonomiconLabelc__Iterator3();
            return (IEnumerator) ammonomiconLabelCIterator3;
        }

        [DebuggerHidden]
        private IEnumerator HandleCharacterSelect()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Foyer__HandleCharacterSelectc__Iterator4()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        public IEnumerator OnSelectedCharacter(float delayTime, FoyerCharacterSelectFlag flag)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Foyer__OnSelectedCharacterc__Iterator5()
            {
                delayTime = delayTime,
                flag = flag,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleInputDelay(PlayerController p, float d)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Foyer__HandleInputDelayc__Iterator6()
            {
                p = p,
                d = d
            };
        }

        public void PlayerCharacterChanged(PlayerController newCharacter)
        {
            if (this.OnPlayerCharacterChanged == null)
                return;
            this.OnPlayerCharacterChanged(newCharacter);
        }

        public void ProcessPlayerEnteredFoyer(PlayerController p)
        {
            if (Dungeon.ShouldAttemptToLoadFromMidgameSave && GameManager.Instance.IsLoadingLevel || !(bool) (UnityEngine.Object) p)
                return;
            p.ForceStaticFaceDirection(Vector2.up);
            if (p.characterIdentity != PlayableCharacters.Eevee)
                p.SetOverrideShader(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout"));
            if ((UnityEngine.Object) p.CurrentGun != (UnityEngine.Object) null)
                p.CurrentGun.gameObject.SetActive(false);
            if (p.inventory != null)
                p.inventory.ForceNoGun = true;
            p.ProcessHandAttachment();
        }

        public void OnDepartedFoyer()
        {
            GameManager.Instance.IsFoyer = false;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                GameManager.Instance.AllPlayers[index].inventory.ForceNoGun = false;
                GameManager.Instance.AllPlayers[index].CurrentGun.gameObject.SetActive(true);
                GameManager.Instance.AllPlayers[index].ProcessHandAttachment();
                GameManager.Instance.AllPlayers[index].ClearOverrideShader();
                GameManager.Instance.AllPlayers[index].AlternateCostumeLibrary = (tk2dSpriteAnimation) null;
            }
        }

        private void PlacePlayerAtStart(PlayerController extantPlayer, Vector2 spot)
        {
            Vector3 vector3 = new Vector3(spot.x + 0.5f, spot.y + 0.5f, -0.1f);
            extantPlayer.transform.position = vector3;
            extantPlayer.Reinitialize();
        }

        private void FlagPitSRBsAsUnpathableCells()
        {
            RoomHandler entrance = GameManager.Instance.Dungeon.data.Entrance;
            for (int x = entrance.area.basePosition.x; x < entrance.area.basePosition.x + entrance.area.dimensions.x; ++x)
            {
                for (int y = entrance.area.basePosition.y; y < entrance.area.basePosition.y + entrance.area.dimensions.y; ++y)
                {
                    for (int index = 0; index < DebrisObject.SRB_Pits.Count; ++index)
                    {
                        Vector2 point = new Vector2((float) x + 0.5f, (float) y + 0.5f);
                        if (DebrisObject.SRB_Pits[index].ContainsPoint(point, collideWithTriggers: true))
                            GameManager.Instance.Dungeon.data[x, y].isOccupied = true;
                    }
                    for (int index = 0; index < DebrisObject.SRB_Walls.Count; ++index)
                    {
                        Vector2 point = new Vector2((float) x + 0.5f, (float) y + 0.5f);
                        if (DebrisObject.SRB_Walls[index].ContainsPoint(point, collideWithTriggers: true))
                            GameManager.Instance.Dungeon.data[x, y].isOccupied = true;
                    }
                }
            }
        }
    }

