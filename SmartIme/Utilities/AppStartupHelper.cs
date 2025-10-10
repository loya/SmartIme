using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;

namespace SmartIme.Utilities
{
    internal class AppStartupHelper
    {
        public static bool IsAppSetToStartup()
        {
            // 方法1: 使用当前用户注册表
            //try
            //{
            //    using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            //    {
            //        string appName = Assembly.GetExecutingAssembly().GetName().Name;
            //        return key?.GetValue(appName) != null;
            //    }
            //}
            //catch
            //{
            //    // 如果注册表方法失败，检查启动文件夹
            //    return IsStartupShortcutExists();
            //}
            return IsStartupShortcutExists();
        }

        public static void SetAppStartup(bool enable)
        {
            string appName = Application.ProductName;
            string appPath = Application.ExecutablePath;

            // 方法1: 尝试使用当前用户注册表（无需管理员权限）
            //bool registrySuccess = TrySetRegistryStartup(appName, appPath, enable);

            //if (!registrySuccess)
            //{
            //    // 方法2: 使用启动文件夹
            //    SetStartupFolderShortcut(appName, appPath, enable);
            //}
            SetStartupFolderShortcut(appName, appPath, enable);
        }

        private bool TrySetRegistryStartup(string appName, string appPath, bool enable)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key == null) return false;

                    if (enable)
                    {
                        key.SetValue(appName, $"\"{appPath}\" -minimized");
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"注册表方式设置开机自启动失败: {ex.Message}");
                return false;
            }
        }

        static void SetStartupFolderShortcut(string appName, string appPath, bool enable)
        {
            try
            {
                if (enable)
                {
                    // 使用任务计划程序实现开机自启动
                    CreateTaskScheduler(appName, appPath, "-minimized");
                }
                else
                {
                    // 删除任务计划
                    try
                    {
                        using (TaskService taskService = new TaskService())
                        {
                            if (taskService.RootFolder.Tasks.Exists(appName))
                            {
                                taskService.RootFolder.DeleteTask(appName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"删除任务计划失败: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置开机自启动失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static bool IsStartupShortcutExists()
        {
            try
            {
                string appName = Application.ProductName;
                using (TaskService taskService = new TaskService())
                {
                    return taskService.RootFolder.Tasks.Exists(appName);
                }
            }
            catch
            {
                return false;
            }
        }

        static void CreateTaskScheduler(string appName, string targetPath, string arguments = "")
        {
            try
            {
                // 创建任务服务实例
                using (TaskService taskService = new TaskService())
                {
                    // 检查任务是否已存在，如果存在则删除
                    if (taskService.RootFolder.Tasks.Exists(appName))
                    {
                        taskService.RootFolder.DeleteTask(appName);
                    }

                    // 创建任务定义
                    TaskDefinition taskDefinition = taskService.NewTask();
                    taskDefinition.RegistrationInfo.Description = "智能输入法切换助手开机自启动";
                    taskDefinition.RegistrationInfo.Author = "SmartIme";

                    // 设置任务以最高权限运行（管理员权限）
                    taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                    taskDefinition.Principal.LogonType = TaskLogonType.InteractiveToken;

                    // 设置任务在用户登录时触发
                    LogonTrigger logonTrigger = new LogonTrigger();
                    logonTrigger.Enabled = true;
                    // 设置延迟启动，避免系统启动时资源竞争
                    logonTrigger.Delay = TimeSpan.FromSeconds(2000);
                    taskDefinition.Triggers.Add(logonTrigger);

                    // 设置任务操作：启动程序
                    ExecAction action = new ExecAction(targetPath, arguments, Path.GetDirectoryName(targetPath));
                    taskDefinition.Actions.Add(action);

                    // 设置任务设置
                    taskDefinition.Settings.Enabled = true;
                    taskDefinition.Settings.DisallowStartIfOnBatteries = false; // 不在电池供电时禁用
                    taskDefinition.Settings.RunOnlyIfIdle = false; // 不仅在空闲时运行
                    taskDefinition.Settings.ExecutionTimeLimit = TimeSpan.Zero; // 不限制执行时间
                    taskDefinition.Settings.Hidden = false; // 不隐藏任务

                    // 注册任务
                    taskService.RootFolder.RegisterTaskDefinition(appName, taskDefinition);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"创建任务计划失败: {ex.Message}", ex);
            }
        }
    }
}

