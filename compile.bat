@echo off
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set WPF_DIR=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF
if not exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
if not exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" set WPF_DIR=C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF

if not exist "%~dp0bin\Release" mkdir "%~dp0bin\Release"
if not exist "%~dp0bin\Release\Fonts" mkdir "%~dp0bin\Release\Fonts"

echo Compiling DesktopClockWidget.exe...
"%CSC%" /target:winexe /optimize+ /platform:anycpu /out:"%~dp0bin\Release\DesktopClockWidget.exe" /win32icon:"%~dp0app.ico" /r:"%WPF_DIR%\PresentationCore.dll" /r:"%WPF_DIR%\PresentationFramework.dll" /r:"%WPF_DIR%\WindowsBase.dll" /r:System.Xaml.dll /r:System.dll /r:System.Core.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Runtime.Serialization.dll "%~dp0src\DesktopClockWidget\DesktopClockWidget.cs" "%~dp0src\DesktopClockWidget\SettingsWindow.cs" "%~dp0src\DesktopClockWidget\Properties\AssemblyInfo.cs"

if errorlevel 1 (
    echo [ERROR] Compilation failed.
    exit /b 1
)

echo Copying Fonts payload...
xcopy /y /q "%~dp0Fonts\*.ttf" "%~dp0bin\Release\Fonts\" >nul 2>&1

echo [SUCCESS] Build complete: "%~dp0bin\Release\DesktopClockWidget.exe"