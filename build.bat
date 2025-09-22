@echo off
REM 检查Visual Studio开发人员命令提示符
set VSPATH="Z:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsDevCmd.bat"

if exist %VSPATH% (
    call %VSPATH%
    msbuild %~dp0SmartIme\SmartIme.csproj /property:GenerateFullPaths=true /t:build /consoleloggerparameters:NoSummary
    exit /b %errorlevel%
) else (
    echo Error: Visual Studio 2019 not found. Please install Visual Studio 2019 or later with .NET Framework support.
    exit /b 1
)