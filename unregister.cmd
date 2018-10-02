@echo off
SET PASSIEDLL=Release\PassIE.dll
IF NOT [%1]==[] SET MONITOREDLL=%1\vMonitor.dll

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe /unregister bin\x64\%MONITOREDLL%
C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe /unregister bin\x86\%MONITOREDLL%
