@echo off
::  Script Name:        Archive_TimeZoneLab.CMD
::  Synopsis:           Make serial secured 7zip backups of the source code of a
::                      Visual Studio 2010 solution directory.
::
::  Author:             David A. Gray
::
::  ----------------------------------------------------------------------------
::  Revision History
::  ----------------------------------------------------------------------------
::
::  Date       By  Synopsis
::  ---------- --- -------------------------------------------------------------
::  2014/09/01 DAG Script developed by adapting a script from MyPlayPen.
::  ============================================================================

echo %~0 Begin
echo.

"%USERPROFILE%\My Documents\Visual Studio 2010\Projects\Tools_for_Programmers\WWSourceCodeArchiver\WWSourceCodeArchiver\bin\Release\WWSourceCodeArchiver.exe" "-p%USERPROFILE%\My Documents\Visual Studio 2010\Projects\_Laboratory\TimeZoneLab\TimeZoneLab\bin\Release\TimeZoneLab.exe" "-tC:\COMMON_DATA\Assorted_Zips\program_source\Visual Studio 2010"

wwpause

echo.
echo %~nx0 Done!