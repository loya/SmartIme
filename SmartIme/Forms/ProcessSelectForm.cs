using System.Diagnostics;

namespace SmartIme.Forms
{
    public class ProcessSelectForm : Form
    {
        private ListBox lstProcesses;
        private Button btnSelect;
        private readonly Process[] processes;
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


            lstProcesses = new ListBox
            {
                Left = 0,
                Top = 0,
                Width = this.ClientSize.Width,
                Height = this.ClientSize.Height - btnSelect.Height - 16, // 留出按钮和底部间距
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            lstProcesses.DoubleClick += (s, e) => btnSelect.PerformClick();

            this.Controls.Add(lstProcesses);
            this.Controls.Add(btnSelect);
            this.Controls.Add(btnCancel);

            processes = [.. Process.GetProcesses().DistinctBy(p => p.ProcessName)
                // .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .Where(p => existingApps == null || !existingApps.Contains(p.ProcessName))
                
                .OrderBy(p => p.ProcessName)];

            foreach (var process in processes)
            {
                try
                {
                    //lstProcesses.Items.Add($"{process.ProcessName} - {process.MainModule?.ModuleName}");
                    lstProcesses.Items.Add($"{process.ProcessName} - {process.MainWindowTitle}");

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

            // 添加窗体大小变化时调整按钮宽度和位置
            this.Resize += (s, e) =>
            {
                btnSelect.Width = (this.ClientSize.Width - 60) / 2;
                btnSelect.Left = 20;
                btnSelect.Top = this.ClientSize.Height - btnSelect.Height - 16;
                
                btnCancel.Width = (this.ClientSize.Width - 60) / 2;
                btnCancel.Left = btnSelect.Right + 20;
                btnCancel.Top = this.ClientSize.Height - btnCancel.Height - 16;
                
                lstProcesses.Width = this.ClientSize.Width;
                lstProcesses.Height = btnSelect.Top;
            };
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (lstProcesses.SelectedIndex >= 0)
            {
                SelectedProcess = processes[lstProcesses.SelectedIndex];
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
