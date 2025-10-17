using System.Diagnostics;

namespace SmartIme
{
    class CustomListBox : ListBox
    {
        /// <summary>
        /// 是否带图标
        /// </summary>
        private bool HasIcon { get; set; }
        /// <summary>
        /// 图标宽度（仅在HasIcon属性为true时有效）
        /// </summary>
        private int IconWidth { get; set; }
        /// <summary>
        /// 图标高度（仅在HasIcon属性为true时有效）
        /// </summary>
        private int IconHeight { get; set; }

        ToolTip tip = new ToolTip();

        public CustomListBox()
        {
            this.ScrollAlwaysVisible = false;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            // this.Font = new Font("Microsoft YaHei", 10);
            this.Font = new Font("微软雅黑", 10F, FontStyle.Regular);
            this.FormattingEnabled = true;
            this.ItemHeight = 19;
            tip.OwnerDraw = true;
            tip.Draw += (sender, e) =>
            {
                e.DrawBackground();
                e.DrawBorder();
                if (e.ToolTipText != null)
                {
                    using StringFormat sf = new()
                    {
                        // Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        // HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None,
                        FormatFlags = StringFormatFlags.NoWrap,
                    };
                    var fsize = e.Graphics.MeasureString(e.ToolTipText, this.Font);
                    RectangleF recf = new RectangleF(
                        e.Bounds.Left + 1, e.Bounds.Top + 1, fsize.Width, fsize.Height);
                    e.Graphics.DrawString(e.ToolTipText, e.Font, new SolidBrush(Color.Blue), recf, sf);
                }
            };
            // this.DrawMode = DrawMode.OwnerDrawVariable;
        }
        /// <summary>
        /// 设置图标大小（若不带图标就无需设置）
        /// </summary>
        /// <param name="w">图标宽度</param>
        /// <param name="h">图标高度</param>
        public void SetIconSize(int w, int h)
        {
            this.HasIcon = true;
            this.IconWidth = w;
            this.IconHeight = h;
            this.ItemHeight = h;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            Graphics g = e.Graphics;
            StringFormat sf = new StringFormat();
            sf.Trimming = StringTrimming.EllipsisCharacter; //超出指定矩形区域部分用"..."替代
            sf.LineAlignment = StringAlignment.Center;//垂直居中
            try
            {
                var item = Items[e.Index];

                SizeF size = g.MeasureString(item.ToString(), e.Font); //获取项文本尺寸

                // if (size.Width > e.Bounds.Width) //项文本宽度超过 项宽
                // {
                //     item.ShowTip = true; //显示tooltip
                // }

                //获取指定矩形区域 - 使用e.Bounds.Y而不是e.Index * ItemHeight，以正确处理滚动
                RectangleF rectF = new RectangleF(e.Bounds.Left,
                        e.Bounds.Top + (this.ItemHeight - size.Height) / 2.0f,
                        e.Bounds.Width - this.IconWidth, size.Height);
                //写 项文本
                g.DrawString(item.ToString(), e.Font, new SolidBrush(e.ForeColor), rectF, sf);
            }
            catch { throw; } //忽略异常

            base.OnDrawItem(e);
        }
        /// <summary>
        /// 重写鼠标移动事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int idx = IndexFromPoint(e.Location); //获取鼠标所在的项索引
            if (idx == CustomListBox.NoMatches) //鼠标所在位置没有 项
            {
                tip.SetToolTip(this, ""); //设置提示信息为空
            }
            else
            {
                var item = Items[idx];
                SizeF size = TextRenderer.MeasureText(item.ToString(), this.Font); //获取项文本尺寸
                Debug.WriteLine(size);
                if (size.Width > this.Bounds.Width) //项文本宽度超过 项宽
                {
                    string txt = this.Items[idx].ToString(); //获取项文本
                    tip.SetToolTip(this, txt); //设置提示信息
                }
                else
                {
                    tip.SetToolTip(this, ""); //设置提示信息为空
                }


            }
        }
    }

}