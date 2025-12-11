# AGENTS.md

本文件为Qoder (qoder.com) 在此代码库中工作时提供指导。

## 项目概述

SmartIme是一个基于Windows的智能输入法切换助手，能够根据当前活动的应用程序自动切换输入法，并提供可视化提示。它在系统托盘中运行，为当前输入法提供视觉提示。

### 技术栈
- **语言**: C#
- **框架**: .NET 8.0 Windows Forms
- **平台**: Windows
- **关键依赖项**: 
  - Interop.UIAutomationClient (用于UI自动化)
  - TaskScheduler (用于启动计划)
  - FlaUI.UIA3 (用于UI自动化)

## 代码架构和结构

### 核心组件
1. **MainForm** (`SmartIme/Forms/MainForm.cs`) - 中央控制器，负责管理：
   - 系统监控和输入法切换逻辑
   - 托盘图标和菜单管理
   - 规则配置和管理
   - 应用程序白名单
   - 视觉提示显示（浮动窗口和光标颜色）

2. **数据模型** (`SmartIme/Models/`) - 核心数据结构：
   - `AppRuleGroup` - 按应用程序组织的规则
   - `Rule` - 单个规则定义，带有匹配模式
   - `AppSettings` - 存储在JSON中的应用配置

3. **工具类** (`SmartIme/Utilities/`) - 辅助类：
   - `CaretHelper` - 管理光标定位和颜色更改
   - `WinApi` - Windows API互操作函数
   - `TSFWapper` - 文本服务框架包装器，用于输入法控制
   - `AppHelper` - 应用程序检测和管理
   - `AppStartupHelper` - 启动计划集成

### 配置管理
- `settings/rules.json` - 应用程序特定的输入法切换规则
- `settings/whitelist.json` - 排除在自动切换之外的应用程序
- `settings/AppSettings.json` - 统一的用户首选项（窗口状态、颜色、字体等）

### 关键实现细节
1. **输入法监控**: 通过COM互操作使用TSF（文本服务框架）API
2. **窗口监控**: 利用Windows API函数（`GetForegroundWindow`、`GetWindowThreadProcessId`）
3. **视觉反馈**: 结合浮动提示窗口和光标颜色变化
4. **系统托盘**: 使用`NotifyIcon`组件实现
5. **单实例**: 使用`Mutex`强制执行

## 常用开发命令

### 构建项目
```bash
# 使用提供的构建脚本
build.bat

# 或直接使用MSBuild
msbuild SmartIme\SmartIme.csproj /property:GenerateFullPaths=true /t:build
```

### 运行应用程序
```bash
# 正常执行
SmartIme\bin\Debug\net8.0-windows\SmartIme.exe

# 最小化启动
SmartIme\bin\Debug\net8.0-windows\SmartIme.exe -minimized
```

### 项目结构
```
SmartIme/
├── Forms/           # UI表单（MainForm、AddRuleForm等）
├── Models/          # 数据模型（AppRuleGroup、Rule、AppSettings）
├── Utilities/       # 辅助类（WinApi、CaretHelper、TSFWapper）
├── settings/        # JSON配置文件
├── Program.cs       # 应用程序入口点
└── SmartIme.csproj  # 项目文件
```

### 入口点
应用程序入口在`SmartIme/Program.cs`中，其功能包括：
1. 使用互斥锁确保单实例执行
2. 验证管理员权限
3. 初始化带启动参数的主窗体

### 关键模式
- 配置存储在JSON文件中而不是传统的.NET设置中
- UI遵循Windows Forms约定，使用部分类分离设计器代码
- 大量使用Windows API互操作实现系统级功能
- 使用`CancellationTokenSource`进行异步监控以实现干净关闭