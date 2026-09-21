using System.Diagnostics;

namespace SmartIme.Forms
{
    public class ProcessSelectForm : Form
    {
        private ListBox lstProcesses;
        private Button btnSelect;
        private TextBox txtFilter; // 过滤输入框
        private CheckBox chkShowAllProcesses; // 显示所有进程复选框
        private Button btnRefresh; // 刷新按钮
        private Process[] processes;
        private Process[] filteredProcesses; // 过滤后的进程数组
        private IEnumerable<string> existingApps; // 保存已存在的应用程序列表
        public Process SelectedProcess { get; private set; }
        public string SelectedProcessDisplayName { get; private set; }

        public ProcessSelectForm(IEnumerable<string> existingApps = null)
        {
            this.ShowInTaskbar = false;
            this.Text = "选择应用程序";
            this.Size = new Size(400, 500); // 恢复窗体宽度为400
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 创建过滤输入框
            txtFilter = new TextBox
            {
                Left = 20,
                Top = 10,
                Width = 200, // 减小初始宽度以适应400px窗体
                Height = 25,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtFilter.TextChanged += TxtFilter_TextChanged;

            // 创建显示所有进程复选框
            chkShowAllProcesses = new CheckBox
            {
                Text = "所有进程",
                Left = 230, // 调整位置
                Top = 14,
                //Width = 70, // 减小宽度
                Height = 20,
				AutoSize = true,
                Checked = false // 默认不选择
            };
            chkShowAllProcesses.CheckedChanged += ChkShowAllProcesses_CheckedChanged;

            // 创建刷新按钮
            btnRefresh = new Button
            {
                Text = "刷 新",
                Left = 310, // 调整位置
                Top = 10,
                Width = 60, // 减小宽度
                Height = 25
            };
            btnRefresh.Click += BtnRefresh_Click;

            btnSelect = new Button
            {
                Text = "选择",
                DialogResult = DialogResult.OK,
                Height = 30,
                Width = (this.ClientSize.Width - 60) / 2, // 分成两个按钮
                Left = 20,
                Top = this.ClientSize.Height - 30 - 16, // 距底部16像素
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnSelect.Click += BtnSelect_Click;

            var btnCancel = new Button
            {
                Text = "退出",
                DialogResult = DialogResult.Cancel,
                Height = 30,
                Width = (this.ClientSize.Width - 60) / 2,
                Left = btnSelect.Right + 20,
                Top = this.ClientSize.Height - 30 - 16,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };


            lstProcesses = new CustomListBox
            {
                Left = 0,
                Top = txtFilter.Bottom + 10, // 调整位置以适应过滤框
                Width = this.ClientSize.Width,
                Height = this.ClientSize.Height - btnSelect.Height - txtFilter.Height - 30 - 16, // 调整高度
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            lstProcesses.DoubleClick += (s, e) => btnSelect.PerformClick();

            this.Controls.Add(txtFilter);
            this.Controls.Add(chkShowAllProcesses);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lstProcesses);
            this.Controls.Add(btnSelect);
            this.Controls.Add(btnCancel);

            // 保存现有的应用程序列表
            this.existingApps = existingApps;

            // 获取进程列表
            RefreshProcessList();

            // 添加窗体大小变化时调整按钮宽度和位置
            this.Resize += (s, e) =>
            {
                // 调整过滤框宽度，为复选框和按钮留出空间
                txtFilter.Width = Math.Max(80, this.ClientSize.Width - 140);
                
                // 调整复选框和按钮位置
                chkShowAllProcesses.Left = txtFilter.Right + 10;
                btnRefresh.Left = Math.Min(chkShowAllProcesses.Right + 10, this.ClientSize.Width - btnRefresh.Width - 10);

                btnSelect.Width = (this.ClientSize.Width - 60) / 2;
                btnSelect.Left = 20;
                btnSelect.Top = this.ClientSize.Height - btnSelect.Height - 16;

                btnCancel.Width = (this.ClientSize.Width - 60) / 2;
                btnCancel.Left = btnSelect.Right + 20;
                btnCancel.Top = this.ClientSize.Height - btnCancel.Height - 16;

                lstProcesses.Width = this.ClientSize.Width;
                lstProcesses.Top = txtFilter.Bottom + 10;
                lstProcesses.Height = btnSelect.Top - lstProcesses.Top;
            };
        }

        // 填充进程列表
        private void PopulateProcessList()
        {
            lstProcesses.Items.Clear();
            
            foreach (var process in filteredProcesses)
            {
                try
                {
                    // lstProcesses.Items.Add($"{process.ProcessName} - {process.MainModule?.ModuleName}");
                    lstProcesses.Items.Add($"{process.ProcessName} - {process.MainWindowTitle} ({process.MainModule?.FileName})");

                }
                catch
                {
                    try
                    {
                        lstProcesses.Items.Add($"{process.ProcessName} ");
                    }
                    catch
                    {
                        lstProcesses.Items.Add(process.ProcessName);
                    }
                }
            }
        }

        // 过滤输入框文本变化事件
        private void TxtFilter_TextChanged(object sender, EventArgs e)
        {
            string filterText = txtFilter.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(filterText))
            {
                filteredProcesses = processes;
            }
            else
            {
                filteredProcesses = processes.Where(p => 
                    p.ProcessName.ToLower().Contains(filterText) || 
                    (p.MainWindowTitle?.ToLower().Contains(filterText) ?? false)).ToArray();
            }
            
            PopulateProcessList();
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedIndex >= 0)
            {
                SelectedProcess = filteredProcesses[lstProcesses.SelectedIndex];
                SelectedProcessDisplayName = lstProcesses.SelectedItem.ToString();

                // 弹出对话框让用户修改显示名称
                using (var inputDialog = new PromptDialog(SelectedProcessDisplayName))
                {


                    if (inputDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        SelectedProcessDisplayName = inputDialog.ResultText;
                    }
                    else
                    {
                        DialogResult = DialogResult.None;
                    }
                }
            }
        }

        // 刷新进程列表
        private void RefreshProcessList()
        {
            try
            {
                var processList = Process.GetProcesses().DistinctBy(p => p.ProcessName);

                // 根据复选框状态决定是否过滤可见窗口进程
                if (!chkShowAllProcesses.Checked)
                {
                    processList = processList.Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)); // 只显示有可见窗口的进程
                }

                // 过滤已存在的应用程序
                if (existingApps != null)
                {
                    processList = processList.Where(p => !existingApps.Contains(p.ProcessName));
                }

                processes = [.. processList.OrderBy(p => p.ProcessName)];
                filteredProcesses = processes; // 重置过滤后的进程列表

                // 应用当前的过滤文本
                ApplyFilter();

                PopulateProcessList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新进程列表时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 应用过滤文本
        private void ApplyFilter()
        {
            string filterText = txtFilter.Text.ToLower();

            if (string.IsNullOrWhiteSpace(filterText))
            {
                filteredProcesses = processes;
            }
            else
            {
                filteredProcesses = processes.Where(p =>
                    p.ProcessName.ToLower().Contains(filterText) ||
                    (p.MainWindowTitle?.ToLower().Contains(filterText) ?? false)).ToArray();
            }
        }

        // 显示所有进程复选框状态改变事件
        private void ChkShowAllProcesses_CheckedChanged(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        // 刷新按钮点击事件
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }
    }
}