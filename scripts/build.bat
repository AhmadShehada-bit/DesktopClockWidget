@echo off
setlocal enabledelayedexpansion
echo ========================================================
echo  DesktopClock Widget - Build Script
echo ========================================================

set CSC_PATH=
if exist "%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe" set CSC_PATH=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not defined CSC_PATH if exist "%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe" set CSC_PATH=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not defined CSC_PATH (
    echo [ERROR] .NET Framework 4.0 C# compiler (csc.exe) not found.
    exit /b 1
)

if not exist "%~dp0bin\Release" mkdir "%~dp0bin\Release"
if not exist "%~dp0bin\Release\Fonts" mkdir "%~dp0bin\Release\Fonts"

echo Compiling DesktopClockWidget.exe...
"%CSC_PATH%" /target:winexe /optimize+ /platform:anycpu ^
    /out:"%~dp0bin\Release\DesktopClockWidget.exe" ^
    /win32icon:"%~dp0app.ico" ^
    /r:PresentationCore.dll ^
    /r:PresentationFramework.dll ^
    /r:WindowsBase.dll ^
    /r:System.Xaml.dll ^
    /r:System.dll ^
    /r:System.Core.dll ^
    /r:System.Drawing.dll ^
    /r:System.Windows.Forms.dll ^
    /r:System.Runtime.Serialization.dll ^
    "%~dp0src\DesktopClockWidget\DesktopClockWidget.cs" ^
    "%~dp0src\DesktopClockWidget\SettingsWindow.cs" ^
    "%~dp0src\DesktopClockWidget\Properties\AssemblyInfo.cs"

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Compilation failed.
    exit /b %ERRORLEVEL%
)

echo Copying Fonts payload...
xcopy /y /q "%~dp0Fonts\*.ttf" "%~dp0bin\Release\Fonts\" >nul 2>&1

echo [SUCCESS] Build complete: "%~dp0bin\Release\DesktopClockWidget.exe"
