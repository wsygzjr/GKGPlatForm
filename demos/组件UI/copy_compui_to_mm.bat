@echo off
setlocal enabledelayedexpansion

set "SRC=%~1"
set "DST=%~2"
set "LOG=%~3"

if "%SRC%"=="" (
  echo [copy_compui_to_mm] Missing SRC parameter
  exit /b 1
)
if "%DST%"=="" (
  echo [copy_compui_to_mm] Missing DST parameter
  exit /b 1
)
if "%LOG%"=="" (
  set "LOG=%TEMP%\copy_compui_to_mm_robocopy.log"
)

if not exist "%DST%" mkdir "%DST%" >nul 2>&1

robocopy "%SRC%" "%DST%" /E /XO /R:2 /W:1 /NFL /NDL /NP /LOG+:"%LOG%"
set "RC=%ERRORLEVEL%"

rem Robocopy return codes: 0-7 are success (including skipped/extra files)
if %RC% LEQ 7 exit /b 0

echo [copy_compui_to_mm] robocopy failed with code %RC% (see log: %LOG%)
exit /b %RC%
