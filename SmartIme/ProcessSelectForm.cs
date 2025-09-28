using System.Diagnostics;

namespace SmartIme
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
                Width = this.ClientSize.Width - 40, // 两侧留20像素
                Left = 20,
                Top = this.ClientSize.Height - 30 - 16, // 距底部16像素
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            btnSelect.Click += BtnSelect_Click;


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

            processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .Where(p => existingApps == null || !existingApps.Contains(p.ProcessName))
                .OrderBy(p => p.ProcessName)
                .ToArray();

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
                        lstProcesses.Items.Add($"{process.ProcessName} - {process.MainWindowTitle}");
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
                btnSelect.Width = this.ClientSize.Width - 40;
                btnSelect.Left = 20;
                btnSelect.Top = this.ClientSize.Height - btnSelect.Height - 16;
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
                using (var inputDialog = new Form())
                {
                    inputDialog.Text = "修改应用程序显示名称";
                    inputDialog.Size = new Size(400, 170);
                    inputDialog.StartPosition = FormStartPosition.CenterParent;
                    inputDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    inputDialog.MaximizeBox = false;
                    inputDialog.MinimizeBox = false;

                    var label = new Label
                    {
                        Text = "请输入应用程序显示名称:",
                        Left = 20,
                        Top = 20,
                        Width = 360
                    };

                    var textBox = new TextBox
                    {
                        Text = SelectedProcessDisplayName,
                        Left = 20,
                        Top = 50,
                        Width = 360
                    };

                    var okButton = new Button
                    {
                        Text = "确定",
                        DialogResult = DialogResult.OK,
                        Left = 190,
                        Top = 88,
                        Width = 80,
                        Height = 30

                    };

                    var cancelButton = new Button
                    {
                        Text = "取消",
                        DialogResult = DialogResult.Cancel,
                        Left = 285,
                        Top = 88,
                        Width = 80,
                        Height = 30,
                    };

                    inputDialog.Controls.Add(label);
                    inputDialog.Controls.Add(textBox);
                    inputDialog.Controls.Add(okButton);
                    inputDialog.Controls.Add(cancelButton);

                    inputDialog.AcceptButton = okButton;
                    inputDialog.CancelButton = cancelButton;

                    inputDialog.Load += (s, ev) =>
                    {
                        textBox.SelectionStart = textBox.Text.IndexOf("-") + 2;
                        textBox.SelectionLength = textBox.Text.Length;
                    };

                    if (inputDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        SelectedProcessDisplayName = textBox.Text.Trim();
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
