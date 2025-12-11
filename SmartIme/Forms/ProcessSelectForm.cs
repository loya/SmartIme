using System.Diagnostics;

namespace SmartIme.Forms
{
    public class ProcessSelectForm : Form
    {
        private ListBox lstProcesses;
        private Button btnSelect;
        private TextBox txtFilter; // 过滤输入框
        private readonly Process[] processes;
        private Process[] filteredProcesses; // 过滤后的进程数组
        public Process SelectedProcess { get; private set; }
        public string SelectedProcessDisplayName { get; private set; }

        public ProcessSelectForm(IEnumerable<string> existingApps = null)
        {
            this.ShowInTaskbar = false;
            this.Text = "选择应用程序";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 创建过滤输入框
            txtFilter = new TextBox
            {
                Left = 20,
                Top = 10,
                Width = this.ClientSize.Width - 40,
                Height = 25,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtFilter.TextChanged += TxtFilter_TextChanged;

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
            this.Controls.Add(lstProcesses);
            this.Controls.Add(btnSelect);
            this.Controls.Add(btnCancel);

            processes = [.. Process.GetProcesses().DistinctBy(p => p.ProcessName)
                // .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .Where(p => existingApps == null || !existingApps.Contains(p.ProcessName))

                .OrderBy(p => p.ProcessName)];

            filteredProcesses = processes; // 初始时显示所有进程

            PopulateProcessList();

            // 添加窗体大小变化时调整按钮宽度和位置
            this.Resize += (s, e) =>
            {
                txtFilter.Width = this.ClientSize.Width - 40;
                
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
    }
}