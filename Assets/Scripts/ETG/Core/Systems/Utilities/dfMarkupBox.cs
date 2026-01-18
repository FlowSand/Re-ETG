using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class dfMarkupBox
    {
        public Vector2 Position = Vector2.zero;
        public Vector2 Size = Vector2.zero;
        public dfMarkupDisplayType Display;
        public dfMarkupBorders Margins = new dfMarkupBorders(0, 0, 0, 0);
        public dfMarkupBorders Padding = new dfMarkupBorders(0, 0, 0, 0);
        public dfMarkupStyle Style;
        public bool IsNewline;
        public int Baseline;
        private List<dfMarkupBox> children = new List<dfMarkupBox>();
        private dfMarkupBox currentLine;
        private int currentLinePos;

        private dfMarkupBox() => throw new NotImplementedException();

        public dfMarkupBox(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style)
        {
            this.Element = element;
            this.Display = display;
            this.Style = style;
            this.Baseline = style.FontSize;
        }

        public dfMarkupBox Parent { get; protected set; }

        public dfMarkupElement Element { get; protected set; }

        public List<dfMarkupBox> Children => this.children;

        public int Width
        {
            get => (int) this.Size.x;
            set => this.Size = new Vector2((float) value, this.Size.y);
        }

        public int Height
        {
            get => (int) this.Size.y;
            set => this.Size = new Vector2(this.Size.x, (float) value);
        }

        internal dfMarkupBox HitTest(Vector2 point)
        {
            Vector2 offset = this.GetOffset();
            Vector2 vector2 = offset + this.Size;
            if ((double) point.x < (double) offset.x || (double) point.x > (double) vector2.x || (double) point.y < (double) offset.y || (double) point.y > (double) vector2.y)
                return (dfMarkupBox) null;
            for (int index = 0; index < this.children.Count; ++index)
            {
                dfMarkupBox dfMarkupBox = this.children[index].HitTest(point);
                if (dfMarkupBox != null)
                    return dfMarkupBox;
            }
            return this;
        }

        internal dfRenderData Render()
        {
            try
            {
                this.endCurrentLine();
                return this.OnRebuildRenderData();
            }
            finally
            {
            }
        }

        public virtual Vector2 GetOffset()
        {
            Vector2 zero = Vector2.zero;
            for (dfMarkupBox dfMarkupBox = this; dfMarkupBox != null; dfMarkupBox = dfMarkupBox.Parent)
                zero += dfMarkupBox.Position;
            return zero;
        }

        internal void AddLineBreak()
        {
            if (this.currentLine != null)
                this.currentLine.IsNewline = true;
            int verticalPosition = this.getVerticalPosition(0);
            this.endCurrentLine();
            dfMarkupBox containingBlock = this.GetContainingBlock();
            this.currentLine = new dfMarkupBox(this.Element, dfMarkupDisplayType.block, this.Style)
            {
                Size = new Vector2(containingBlock.Size.x, (float) this.Style.FontSize),
                Position = new Vector2(0.0f, (float) verticalPosition),
                Parent = this
            };
            this.children.Add(this.currentLine);
        }

        public virtual void AddChild(dfMarkupBox box)
        {
            dfMarkupDisplayType display = box.Display;
            int num;
            switch (display)
            {
                case dfMarkupDisplayType.block:
                case dfMarkupDisplayType.listItem:
                case dfMarkupDisplayType.table:
                    num = 1;
                    break;
                default:
                    num = display == dfMarkupDisplayType.tableRow ? 1 : 0;
                    break;
            }
            if (num != 0)
                this.addBlock(box);
            else
                this.addInline(box);
        }

        public virtual void Release()
        {
            for (int index = 0; index < this.children.Count; ++index)
                this.children[index].Release();
            this.children.Clear();
            this.Element = (dfMarkupElement) null;
            this.Parent = (dfMarkupBox) null;
            this.Margins = new dfMarkupBorders();
        }

        protected virtual dfRenderData OnRebuildRenderData() => (dfRenderData) null;

        protected void renderDebugBox(dfRenderData renderData)
        {
            Vector3 zero = Vector3.zero;
            Vector3 vector3_1 = zero + Vector3.right * this.Size.x;
            Vector3 vector3_2 = vector3_1 + Vector3.down * this.Size.y;
            Vector3 vector3_3 = zero + Vector3.down * this.Size.y;
            renderData.Vertices.Add(zero);
            renderData.Vertices.Add(vector3_1);
            renderData.Vertices.Add(vector3_2);
            renderData.Vertices.Add(vector3_3);
            renderData.Triangles.AddRange(new int[6]
            {
                0,
                1,
                3,
                3,
                1,
                2
            });
            renderData.UV.Add(Vector2.zero);
            renderData.UV.Add(Vector2.zero);
            renderData.UV.Add(Vector2.zero);
            renderData.UV.Add(Vector2.zero);
            Color backgroundColor = this.Style.BackgroundColor;
            renderData.Colors.Add((Color32) backgroundColor);
            renderData.Colors.Add((Color32) backgroundColor);
            renderData.Colors.Add((Color32) backgroundColor);
            renderData.Colors.Add((Color32) backgroundColor);
        }

        public void FitToContents() => this.FitToContents(false);

        public void FitToContents(bool recursive)
        {
            if (this.children.Count == 0)
            {
                this.Size = new Vector2(this.Size.x, 0.0f);
            }
            else
            {
                this.endCurrentLine();
                Vector2 lhs = Vector2.zero;
                for (int index = 0; index < this.children.Count; ++index)
                {
                    dfMarkupBox child = this.children[index];
                    lhs = Vector2.Max(lhs, child.Position + child.Size);
                }
                this.Size = lhs;
            }
        }

        private dfMarkupBox GetContainingBlock()
        {
            for (dfMarkupBox containingBlock = this; containingBlock != null; containingBlock = containingBlock.Parent)
            {
                dfMarkupDisplayType display = containingBlock.Display;
                int num;
                switch (display)
                {
                    case dfMarkupDisplayType.block:
                    case dfMarkupDisplayType.listItem:
                    case dfMarkupDisplayType.inlineBlock:
                    case dfMarkupDisplayType.table:
                    case dfMarkupDisplayType.tableRow:
                        num = 1;
                        break;
                    default:
                        num = display == dfMarkupDisplayType.tableCell ? 1 : 0;
                        break;
                }
                if (num != 0)
                    return containingBlock;
            }
            return (dfMarkupBox) null;
        }

        private void addInline(dfMarkupBox box)
        {
            dfMarkupBorders margins = box.Margins;
            bool flag = !this.Style.Preformatted && this.currentLine != null && (double) this.currentLinePos + (double) box.Size.x > (double) this.currentLine.Size.x;
            if (this.currentLine == null || flag)
            {
                this.endCurrentLine();
                int verticalPosition = this.getVerticalPosition(margins.top);
                dfMarkupBox containingBlock = this.GetContainingBlock();
                if (containingBlock == null)
                {
                    Debug.LogError((object) "Containing block not found");
                    return;
                }
                dfDynamicFont dfDynamicFont = this.Style.Font ?? this.Style.Host.Font;
                float num1 = (float) dfDynamicFont.FontSize / (float) dfDynamicFont.FontSize;
                float num2 = (float) dfDynamicFont.Baseline * num1;
                this.currentLine = new dfMarkupBox(this.Element, dfMarkupDisplayType.block, this.Style)
                {
                    Size = new Vector2(containingBlock.Size.x, (float) this.Style.LineHeight),
                    Position = new Vector2(0.0f, (float) verticalPosition),
                    Parent = this,
                    Baseline = (int) num2
                };
                this.children.Add(this.currentLine);
            }
            if (this.currentLinePos == 0 && !box.Style.PreserveWhitespace && box is dfMarkupBoxText && (box as dfMarkupBoxText).IsWhitespace)
                return;
            Vector2 vector2 = new Vector2((float) (this.currentLinePos + margins.left), (float) margins.top);
            box.Position = vector2;
            box.Parent = this.currentLine;
            this.currentLine.children.Add(box);
            this.currentLinePos = (int) ((double) vector2.x + (double) box.Size.x + (double) box.Margins.right);
            this.currentLine.Size = new Vector2(Mathf.Max(this.currentLine.Size.x, vector2.x + box.Size.x), Mathf.Max(this.currentLine.Size.y, vector2.y + box.Size.y));
        }

        private int getVerticalPosition(int topMargin)
        {
            if (this.children.Count == 0)
                return topMargin;
            int num1 = 0;
            int index1 = 0;
            for (int index2 = 0; index2 < this.children.Count; ++index2)
            {
                dfMarkupBox child = this.children[index2];
                float num2 = child.Position.y + child.Size.y + (float) child.Margins.bottom;
                if ((double) num2 > (double) num1)
                {
                    num1 = (int) num2;
                    index1 = index2;
                }
            }
            dfMarkupBox child1 = this.children[index1];
            int num3 = Mathf.Max(child1.Margins.bottom, topMargin);
            return (int) ((double) child1.Position.y + (double) child1.Size.y + (double) num3);
        }

        private void addBlock(dfMarkupBox box)
        {
            if (this.currentLine != null)
            {
                this.currentLine.IsNewline = true;
                this.endCurrentLine(true);
            }
            dfMarkupBox containingBlock = this.GetContainingBlock();
            if ((double) box.Size.sqrMagnitude <= 1.4012984643248171E-45)
                box.Size = new Vector2(containingBlock.Size.x - (float) box.Margins.horizontal, (float) this.Style.FontSize);
            int verticalPosition = this.getVerticalPosition(box.Margins.top);
            box.Position = new Vector2((float) box.Margins.left, (float) verticalPosition);
            this.Size = new Vector2(this.Size.x, Mathf.Max(this.Size.y, box.Position.y + box.Size.y));
            box.Parent = this;
            this.children.Add(box);
        }

        private void endCurrentLine() => this.endCurrentLine(false);

        private void endCurrentLine(bool removeEmpty)
        {
            if (this.currentLine == null)
                return;
            if (this.currentLinePos == 0)
            {
                if (removeEmpty)
                    this.children.Remove(this.currentLine);
            }
            else
            {
                this.currentLine.doHorizontalAlignment();
                this.currentLine.doVerticalAlignment();
            }
            this.currentLine = (dfMarkupBox) null;
            this.currentLinePos = 0;
        }

        private void doVerticalAlignment()
        {
            if (this.children.Count == 0)
                return;
            float a1 = float.MinValue;
            float a2 = float.MaxValue;
            float a3 = float.MinValue;
            this.Baseline = (int) ((double) this.Size.y * 0.949999988079071);
            for (int index = 0; index < this.children.Count; ++index)
            {
                dfMarkupBox child = this.children[index];
                a3 = Mathf.Max(a3, child.Position.y + (float) child.Baseline);
            }
            for (int index = 0; index < this.children.Count; ++index)
            {
                dfMarkupBox child = this.children[index];
                dfMarkupVerticalAlign verticalAlign = child.Style.VerticalAlign;
                Vector2 position = child.Position;
                if (verticalAlign == dfMarkupVerticalAlign.Baseline)
                    position.y = a3 - (float) child.Baseline;
                child.Position = position;
            }
            for (int index = 0; index < this.children.Count; ++index)
            {
                dfMarkupBox child = this.children[index];
                Vector2 position = child.Position;
                Vector2 size = child.Size;
                a2 = Mathf.Min(a2, position.y);
                a1 = Mathf.Max(a1, position.y + size.y);
            }
            for (int index = 0; index < this.children.Count; ++index)
            {
                dfMarkupBox child = this.children[index];
                dfMarkupVerticalAlign verticalAlign = child.Style.VerticalAlign;
                Vector2 position = child.Position;
                Vector2 size = child.Size;
                switch (verticalAlign)
                {
                    case dfMarkupVerticalAlign.Top:
                        position.y = a2;
                        break;
                    case dfMarkupVerticalAlign.Middle:
                        position.y = (float) (((double) this.Size.y - (double) size.y) * 0.5);
                        break;
                    case dfMarkupVerticalAlign.Bottom:
                        position.y = a1 - size.y;
                        break;
                }
                child.Position = position;
            }
            int a4 = int.MaxValue;
            for (int index = 0; index < this.children.Count; ++index)
                a4 = Mathf.Min(a4, (int) this.children[index].Position.y);
            for (int index = 0; index < this.children.Count; ++index)
            {
                Vector2 position = this.children[index].Position;
                position.y -= (float) a4;
                this.children[index].Position = position;
            }
        }

        private void doHorizontalAlignment()
        {
            if (this.Style.Align == dfMarkupTextAlign.Left || this.children.Count == 0)
                return;
            int index1 = this.children.Count - 1;
            while (index1 > 0 && this.children[index1] is dfMarkupBoxText child1 && child1.IsWhitespace)
                --index1;
            if (this.Style.Align == dfMarkupTextAlign.Center)
            {
                float num1 = 0.0f;
                for (int index2 = 0; index2 <= index1; ++index2)
                    num1 += this.children[index2].Size.x;
                float num2 = (float) (((double) this.Size.x - (double) this.Padding.horizontal - (double) num1) * 0.5);
                for (int index3 = 0; index3 <= index1; ++index3)
                {
                    Vector2 position = this.children[index3].Position;
                    position.x += num2;
                    this.children[index3].Position = position;
                }
            }
            else if (this.Style.Align == dfMarkupTextAlign.Right)
            {
                float num = this.Size.x - (float) this.Padding.horizontal;
                for (int index4 = index1; index4 >= 0; --index4)
                {
                    Vector2 position = this.children[index4].Position with
                    {
                        x = num - this.children[index4].Size.x
                    };
                    this.children[index4].Position = position;
                    num -= this.children[index4].Size.x;
                }
            }
            else
            {
                if (this.Style.Align != dfMarkupTextAlign.Justify)
                    throw new NotImplementedException($"text-align: {(object) this.Style.Align} is not implemented");
                if (this.children.Count <= 1 || this.IsNewline || this.children[this.children.Count - 1].IsNewline)
                    return;
                float a = 0.0f;
                for (int index5 = 0; index5 <= index1; ++index5)
                {
                    dfMarkupBox child2 = this.children[index5];
                    a = Mathf.Max(a, child2.Position.x + child2.Size.x);
                }
                float num = (this.Size.x - (float) this.Padding.horizontal - a) / (float) this.children.Count;
                for (int index6 = 1; index6 <= index1; ++index6)
                    this.children[index6].Position += new Vector2((float) index6 * num, 0.0f);
                dfMarkupBox child3 = this.children[index1];
                Vector2 position = child3.Position with
                {
                    x = this.Size.x - (float) this.Padding.horizontal - child3.Size.x
                };
                child3.Position = position;
            }
        }
    }

