@echo off

"c:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe" "%~dp0..\SoftDojo.Shihan.WinSvc.exe"

net start "Sample WCF Service"

pause
