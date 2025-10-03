# SmartIme 项目概述

## 项目简介

SmartIme 是一个智能输入法切换助手，用于根据当前活动的应用程序自动切换输入法。该应用程序运行在系统托盘中，可以监控当前焦点窗口，并根据预定义的规则自动切换到相应的输入法。

## 技术栈

- **编程语言**: C#
- **框架**: .NET 8.0 Windows Forms
- **平台**: Windows
- **开发环境**: Visual Studio

## 项目结构

```
SmartIme/
├── Forms/                 # 窗体界面
├── Models/                # 数据模型
├── Utilities/             # 工具类
├── Properties/            # 应用程序属性和设置
├── app.config            # 应用程序配置文件
├── app.manifest          # 应用程序清单文件
├── SmartIme.csproj       # 项目文件
├── Program.cs            # 程序入口点
├── packages.config       # NuGet包配置
└── build.bat             # 构建脚本
```

## 核心功能

1. **自动输入法切换**: 根据当前活动的应用程序自动切换输入法
2. **规则配置**: 为不同的应用程序设置不同的输入法切换规则
3. **系统托盘运行**: 最小化到系统托盘，不占用任务栏空间
4. **开机自启动**: 支持设置开机自启动
5. **光标颜色提示**: 根据当前输入法改变光标颜色，提供视觉提示
6. **白名单功能**: 可以设置忽略某些应用程序的输入法切换

## 构建和运行

### 环境要求

- Windows 操作系统
- .NET 8.0 SDK
- Visual Studio 2022 或更高版本

### 构建步骤

1. 使用 Visual Studio 打开 `SmartIme.sln` 解决方案文件
2. 或者使用命令行运行 `build.bat` 脚本

### 运行方式

- 直接运行 SmartIme.exe
- 支持命令行参数 `-minimized` 以最小化模式启动

## 开发约定

### 代码结构

- 使用 Windows Forms 构建用户界面
- 遵循 MVVM 模式的思想，将数据模型与界面分离
- 使用 JSON 文件存储配置信息 (rules.json, whitelist.json)
- 使用应用程序设置存储用户偏好

### 命名约定

- 类名使用 PascalCase
- 私有字段使用下划线前缀
- 方法名使用 PascalCase
- 变量名使用 camelCase

### 配置文件

- `rules.json`: 存储应用程序的输入法切换规则
- `whitelist.json`: 存储白名单应用程序列表
- `app.config`: 存储应用程序的基本配置
- 用户设置存储在 `Properties.Settings` 中

## 权限要求

应用程序需要管理员权限运行，这在 `app.manifest` 文件中已配置：
```xml
<requestedExecutionLevel level="requireAdministrator" uiAccess="false"/>
```

## 许可证

本项目采用 Apache License 2.0 许可证。