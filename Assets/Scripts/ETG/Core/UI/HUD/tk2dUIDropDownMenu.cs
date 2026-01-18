using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

[AddComponentMenu("2D Toolkit/UI/tk2dUIDropDownMenu")]
public class tk2dUIDropDownMenu : MonoBehaviour
    {
        public tk2dUIItem dropDownButton;
        public tk2dTextMesh selectedTextMesh;
        [HideInInspector]
        public float height;
        public tk2dUIDropDownItem dropDownItemTemplate;
        [SerializeField]
        private string[] startingItemList;
        [SerializeField]
        private int startingIndex;
        private List<string> itemList = new List<string>();
        public string SendMessageOnSelectedItemChangeMethodName = string.Empty;
        private int index;
        private List<tk2dUIDropDownItem> dropDownItems = new List<tk2dUIDropDownItem>();
        private bool isExpanded;
        [HideInInspector]
        [SerializeField]
        private tk2dUILayout menuLayoutItem;
        [HideInInspector]
        [SerializeField]
        private tk2dUILayout templateLayoutItem;

        public List<string> ItemList
        {
            get => this.itemList;
            set => this.itemList = value;
        }

        public event System.Action OnSelectedItemChange;

        public int Index
        {
            get => this.index;
            set
            {
                this.index = Mathf.Clamp(value, 0, this.ItemList.Count - 1);
                this.SetSelectedItem();
            }
        }

        public string SelectedItem
        {
            get
            {
                return this.index >= 0 && this.index < this.itemList.Count ? this.itemList[this.index] : string.Empty;
            }
        }

        public GameObject SendMessageTarget
        {
            get
            {
                return (UnityEngine.Object) this.dropDownButton != (UnityEngine.Object) null ? this.dropDownButton.sendMessageTarget : (GameObject) null;
            }
            set
            {
                if (!((UnityEngine.Object) this.dropDownButton != (UnityEngine.Object) null) || !((UnityEngine.Object) this.dropDownButton.sendMessageTarget != (UnityEngine.Object) value))
                    return;
                this.dropDownButton.sendMessageTarget = value;
            }
        }

        public tk2dUILayout MenuLayoutItem
        {
            get => this.menuLayoutItem;
            set => this.menuLayoutItem = value;
        }

        public tk2dUILayout TemplateLayoutItem
        {
            get => this.templateLayoutItem;
            set => this.templateLayoutItem = value;
        }

        private void Awake()
        {
            foreach (string startingItem in this.startingItemList)
                this.itemList.Add(startingItem);
            this.index = this.startingIndex;
            this.dropDownItemTemplate.gameObject.SetActive(false);
            this.UpdateList();
        }

        private void OnEnable() => this.dropDownButton.OnDown += new System.Action(this.ExpandButtonPressed);

        private void OnDisable() => this.dropDownButton.OnDown -= new System.Action(this.ExpandButtonPressed);

        public void UpdateList()
        {
            if (this.dropDownItems.Count > this.ItemList.Count)
            {
                for (int count = this.ItemList.Count; count < this.dropDownItems.Count; ++count)
                    this.dropDownItems[count].gameObject.SetActive(false);
            }
            while (this.dropDownItems.Count < this.ItemList.Count)
                this.dropDownItems.Add(this.CreateAnotherDropDownItem());
            for (int index = 0; index < this.ItemList.Count; ++index)
            {
                tk2dUIDropDownItem dropDownItem = this.dropDownItems[index];
                Vector3 localPosition = dropDownItem.transform.localPosition with
                {
                    y = !((UnityEngine.Object) this.menuLayoutItem != (UnityEngine.Object) null) || !((UnityEngine.Object) this.templateLayoutItem != (UnityEngine.Object) null) ? (float) (-(double) this.height - (double) index * (double) dropDownItem.height) : this.menuLayoutItem.bMin.y - (float) index * (this.templateLayoutItem.bMax.y - this.templateLayoutItem.bMin.y)
                };
                dropDownItem.transform.localPosition = localPosition;
                if ((UnityEngine.Object) dropDownItem.label != (UnityEngine.Object) null)
                    dropDownItem.LabelText = this.itemList[index];
                dropDownItem.Index = index;
            }
            this.SetSelectedItem();
        }

        public void SetSelectedItem()
        {
            if (this.index < 0 || this.index >= this.ItemList.Count)
                this.index = 0;
            if (this.index >= 0 && this.index < this.ItemList.Count)
            {
                this.selectedTextMesh.text = this.ItemList[this.index];
                this.selectedTextMesh.Commit();
            }
            else
            {
                this.selectedTextMesh.text = string.Empty;
                this.selectedTextMesh.Commit();
            }
            if (this.OnSelectedItemChange != null)
                this.OnSelectedItemChange();
            if (!((UnityEngine.Object) this.SendMessageTarget != (UnityEngine.Object) null) || this.SendMessageOnSelectedItemChangeMethodName.Length <= 0)
                return;
            this.SendMessageTarget.SendMessage(this.SendMessageOnSelectedItemChangeMethodName, (object) this, SendMessageOptions.RequireReceiver);
        }

        private tk2dUIDropDownItem CreateAnotherDropDownItem()
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.dropDownItemTemplate.gameObject);
            gameObject.name = "DropDownItem";
            gameObject.transform.parent = this.transform;
            gameObject.transform.localPosition = this.dropDownItemTemplate.transform.localPosition;
            gameObject.transform.localRotation = this.dropDownItemTemplate.transform.localRotation;
            gameObject.transform.localScale = this.dropDownItemTemplate.transform.localScale;
            tk2dUIDropDownItem component1 = gameObject.GetComponent<tk2dUIDropDownItem>();
            component1.OnItemSelected += new Action<tk2dUIDropDownItem>(this.ItemSelected);
            tk2dUIUpDownHoverButton component2 = gameObject.GetComponent<tk2dUIUpDownHoverButton>();
            component1.upDownHoverBtn = component2;
            component2.OnToggleOver += new Action<tk2dUIUpDownHoverButton>(this.DropDownItemHoverBtnToggle);
            return component1;
        }

        private void ItemSelected(tk2dUIDropDownItem item)
        {
            if (this.isExpanded)
                this.CollapseList();
            this.Index = item.Index;
        }

        private void ExpandButtonPressed()
        {
            if (this.isExpanded)
                this.CollapseList();
            else
                this.ExpandList();
        }

        private void ExpandList()
        {
            this.isExpanded = true;
            int num = Mathf.Min(this.ItemList.Count, this.dropDownItems.Count);
            for (int index = 0; index < num; ++index)
                this.dropDownItems[index].gameObject.SetActive(true);
            tk2dUIDropDownItem dropDownItem = this.dropDownItems[this.index];
            if (!((UnityEngine.Object) dropDownItem.upDownHoverBtn != (UnityEngine.Object) null))
                return;
            dropDownItem.upDownHoverBtn.IsOver = true;
        }

        private void CollapseList()
        {
            this.isExpanded = false;
            foreach (Component dropDownItem in this.dropDownItems)
                dropDownItem.gameObject.SetActive(false);
        }

        private void DropDownItemHoverBtnToggle(tk2dUIUpDownHoverButton upDownHoverButton)
        {
            if (!upDownHoverButton.IsOver)
                return;
            foreach (tk2dUIDropDownItem dropDownItem in this.dropDownItems)
            {
                if ((UnityEngine.Object) dropDownItem.upDownHoverBtn != (UnityEngine.Object) upDownHoverButton && (UnityEngine.Object) dropDownItem.upDownHoverBtn != (UnityEngine.Object) null)
                    dropDownItem.upDownHoverBtn.IsOver = false;
            }
        }

        private void OnDestroy()
        {
            foreach (tk2dUIDropDownItem dropDownItem in this.dropDownItems)
            {
                dropDownItem.OnItemSelected -= new Action<tk2dUIDropDownItem>(this.ItemSelected);
                if ((UnityEngine.Object) dropDownItem.upDownHoverBtn != (UnityEngine.Object) null)
                    dropDownItem.upDownHoverBtn.OnToggleOver -= new Action<tk2dUIUpDownHoverButton>(this.DropDownItemHoverBtnToggle);
            }
        }
    }

