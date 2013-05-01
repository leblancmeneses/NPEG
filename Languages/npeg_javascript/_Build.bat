@setlocal enableextensions
@cd /d "%~dp0"
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe tools\combine-compress.proj /fl /flp:v=diag;logfile=build.log
pause