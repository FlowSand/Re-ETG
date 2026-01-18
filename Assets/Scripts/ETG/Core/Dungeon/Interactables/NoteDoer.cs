using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class NoteDoer : DungeonPlaceableBehaviour, IPlayerInteractable
  {
    public NoteDoer.NoteBackgroundType noteBackgroundType;
    public string stringKey;
    public bool useAdditionalStrings;
    public string[] additionalStrings;
    public bool isNormalNote;
    public bool useItemsTable;
    [NonSerialized]
    public bool alreadyLocalized;
    public Transform textboxSpawnPoint;
    private bool m_boxIsExtant;
    public bool DestroyedOnFinish;
    private string m_selectedDisplayString;

    public void Start()
    {
      if (!((UnityEngine.Object) this.majorBreakable != (UnityEngine.Object) null))
        return;
      this.majorBreakable.OnBreak += new System.Action(this.OnBroken);
    }

    private void OnBroken()
    {
      GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor)).DeregisterInteractable((IPlayerInteractable) this);
    }

    private void OnDisable()
    {
      if (!this.m_boxIsExtant)
        return;
      GameUIRoot.Instance.ShowCoreUI(string.Empty);
      this.m_boxIsExtant = false;
      TextBoxManager.ClearTextBoxImmediate(this.textboxSpawnPoint);
    }

    public float GetDistanceToPoint(Vector2 point)
    {
      if (!(bool) (UnityEngine.Object) this.sprite)
      {
        if (this.m_boxIsExtant)
          this.ClearBox();
        return 1000f;
      }
      Bounds bounds = this.sprite.GetBounds();
      bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
      float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
      float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
      return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
    }

    public float GetOverrideMaxDistance() => -1f;

    public void OnEnteredRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
      this.sprite.UpdateZDepth();
    }

    public void OnExitRange(PlayerController interactor)
    {
      if (!(bool) (UnityEngine.Object) this)
        return;
      if (this.m_boxIsExtant)
        this.ClearBox();
      SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
      this.sprite.UpdateZDepth();
    }

    private void ClearBox()
    {
      GameUIRoot.Instance.ShowCoreUI(string.Empty);
      this.m_boxIsExtant = false;
      TextBoxManager.ClearTextBox(this.textboxSpawnPoint);
      if (!this.DestroyedOnFinish)
        return;
      this.GetAbsoluteParentRoom().DeregisterInteractable((IPlayerInteractable) this);
      RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
      LootEngine.DoDefaultItemPoof(this.sprite.WorldCenter);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    public void Interact(PlayerController interactor)
    {
      if (this.m_boxIsExtant)
      {
        this.ClearBox();
      }
      else
      {
        GameUIRoot.Instance.HideCoreUI(string.Empty);
        this.m_boxIsExtant = true;
        string text = this.m_selectedDisplayString;
        if (this.m_selectedDisplayString == null)
        {
          text = !this.alreadyLocalized ? (!this.useItemsTable ? StringTableManager.GetLongString(this.stringKey) : StringTableManager.GetItemsLongDescription(this.stringKey)) : this.stringKey;
          if (this.useAdditionalStrings)
          {
            if (this.isNormalNote)
              text = !this.alreadyLocalized ? (!this.useItemsTable ? StringTableManager.GetLongString(this.additionalStrings[UnityEngine.Random.Range(0, this.additionalStrings.Length)]) : StringTableManager.GetItemsLongDescription(this.additionalStrings[UnityEngine.Random.Range(0, this.additionalStrings.Length)])) : this.additionalStrings[UnityEngine.Random.Range(0, this.additionalStrings.Length)];
            else if (GameStatsManager.Instance.GetFlag(GungeonFlags.SECRET_NOTE_ENCOUNTERED))
              text = !this.alreadyLocalized ? (!this.useItemsTable ? StringTableManager.GetLongString(this.additionalStrings[UnityEngine.Random.Range(0, this.additionalStrings.Length)]) : StringTableManager.GetItemsLongDescription(this.additionalStrings[UnityEngine.Random.Range(0, this.additionalStrings.Length)])) : this.additionalStrings[UnityEngine.Random.Range(0, this.additionalStrings.Length)];
          }
          if (this.stringKey == "#IRONCOIN_SHORTDESC")
            text = $" \n{text}\n ";
          this.m_selectedDisplayString = text;
        }
        switch (this.noteBackgroundType)
        {
          case NoteDoer.NoteBackgroundType.LETTER:
            TextBoxManager.ShowLetterBox(this.textboxSpawnPoint.position, this.textboxSpawnPoint, -1f, text);
            break;
          case NoteDoer.NoteBackgroundType.STONE:
            TextBoxManager.ShowStoneTablet(this.textboxSpawnPoint.position, this.textboxSpawnPoint, -1f, text);
            break;
          case NoteDoer.NoteBackgroundType.WOOD:
            TextBoxManager.ShowWoodPanel(this.textboxSpawnPoint.position, this.textboxSpawnPoint, -1f, text);
            break;
          case NoteDoer.NoteBackgroundType.NOTE:
            TextBoxManager.ShowNote(this.textboxSpawnPoint.position, this.textboxSpawnPoint, -1f, text);
            break;
        }
        if (!this.useAdditionalStrings || this.isNormalNote)
          return;
        GameStatsManager.Instance.SetFlag(GungeonFlags.SECRET_NOTE_ENCOUNTERED, true);
      }
    }

    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
    {
      shouldBeFlipped = false;
      return string.Empty;
    }

    protected override void OnDestroy() => base.OnDestroy();

    public enum NoteBackgroundType
    {
      LETTER,
      STONE,
      WOOD,
      NOTE,
    }
  }

