@echo off

net stop "Sample WCF Service"
"c:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe" /u "%~dp0..\SoftDojo.Shihan.WinSvc.exe"

pause
