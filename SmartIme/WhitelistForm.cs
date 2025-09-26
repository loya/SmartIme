using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace SmartIme
{
    public partial class WhitelistForm : Form
    {
        public class WhitelistApp
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Path { get; set; }

            public override string ToString()
            {
                return $"{Name} - {Title} ({Path})";
            }
        }

        private readonly BindingList<WhitelistApp> whitelistedApps = new BindingList<WhitelistApp>();
        private readonly MainForm mainForm;
        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public WhitelistForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;

            // 加载白名单
            LoadWhitelist();

            // 绑定列表
            listWhitelist.DataSource = whitelistedApps;
        }

        private void LoadWhitelist()
        {
            try
            {
                string jsonPath = GetWhitelistJsonPath();
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    var loadedApps = JsonSerializer.Deserialize<List<WhitelistApp>>(json);
                    if (loadedApps != null)
                    {
                        whitelistedApps.Clear();
                        foreach (var app in loadedApps)
                        {
                            whitelistedApps.Add(app);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载白名单失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveWhitelist()
        {
            try
            {
                string jsonPath = GetWhitelistJsonPath();
                string json = JsonSerializer.Serialize(whitelistedApps.ToList(), options);
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存白名单失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetWhitelistJsonPath()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string appDirectory = Path.GetDirectoryName(assemblyPath);
            return Path.Combine(appDirectory, "whitelist.json");
        }

        private void BtnAddApp_Click(object sender, EventArgs e)
        {
            // 只过滤已经在白名单中的应用
            var existingApps = whitelistedApps.Select(a => a.Name).ToList();
            using var processSelectForm = new ProcessSelectForm(existingApps);
            if (processSelectForm.ShowDialog(this) == DialogResult.OK && processSelectForm.SelectedProcess != null)
            {
                var selectedProcess = processSelectForm.SelectedProcess;
                string appName = selectedProcess.ProcessName;
                string appTitle = "";
                string appPath = "";
                try
                {
                    appTitle = selectedProcess.MainModule.ModuleName;
                    appPath = selectedProcess.MainModule?.FileName ?? "未知路径";
                }
                catch
                {
                    //MessageBox.Show("无法获取该进程的路径，可能是权限不足。请以管理员身份运行此程序后重试。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    appTitle = selectedProcess.MainWindowTitle;
                    appPath = "未知路径（可能是权限不足）";
                }

                if (!whitelistedApps.Any(a => a.Name == appName))
                {
                    // 检查是否已有规则
                    bool hasRule = mainForm.AppRuleGroups.Any(g => g.AppName == appName);
                    if (hasRule)
                    {
                        var result = MessageBox.Show($"应用 {appName} 已有输入法规则，是否仍要添加到白名单？",
                            "确认",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result != DialogResult.Yes)
                        {
                            return;
                        }
                    }

                    whitelistedApps.Add(new WhitelistApp
                    {
                        Name = appName,
                        Title = appTitle,
                        Path = appPath
                    });
                    SaveWhitelist();
                }
                else
                {
                    MessageBox.Show("该应用已在白名单中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnRemoveApp_Click(object sender, EventArgs e)
        {
            if (listWhitelist.SelectedItem != null)
            {
                var selectedApp = (WhitelistApp)listWhitelist.SelectedItem;
                whitelistedApps.Remove(selectedApp);
                SaveWhitelist();
            }
            else
            {
                MessageBox.Show("请先选择要移除的应用", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}