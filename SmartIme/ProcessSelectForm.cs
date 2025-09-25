using System.Diagnostics;

namespace SmartIme
{
    public class ProcessSelectForm : Form
    {
        private ListBox lstProcesses;
        private Button btnSelect;
        private readonly Process[] processes;
        public Process SelectedProcess { get; private set; }

        public ProcessSelectForm()
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
            btnSelect = new Button
            {
                Text = "选择",
                DialogResult = DialogResult.OK,
                Height = 30,
                Width = this.ClientSize.Width - 40, // 两侧留20像素
                Left = 20,
                Top = this.ClientSize.Height - 30 - 12, // 距底部16像素
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            btnSelect.Click += BtnSelect_Click;

            this.Controls.Add(lstProcesses);
            this.Controls.Add(btnSelect);

            processes = Process.GetProcesses()
                .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
                .OrderBy(p => p.ProcessName)
                .ToArray();

            foreach (var process in processes)
            {
                try
                {
                    lstProcesses.Items.Add($"{process.ProcessName} - {process.MainModule?.ModuleName}");
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
            }
        }
    }
}
