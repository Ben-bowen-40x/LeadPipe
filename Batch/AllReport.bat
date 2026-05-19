@echo off

set queryName=All Query
title %queryName%

set base=%USERPROFILE%\Repos

set database=%base%\LeadPipe\LeadPipe.Infrastructure\.info\leadpipe.test.db
set output=%base%\Automate\Automate.Infrastructure\.info\Reports\AllReport_Test.csv
set sql=%base%\LeadPipe\Batch\AllReport.sql

sqlite3 -header -csv %database% < %sql% > %output%

set error=%errorlevel%
echo.
echo %queryName% success: %error%
:: Error messages are placed in the output file
if not "%error%"="0" (
    type %output%
    goto :pauseExecution
)
echo.

:: Ending
echo %queryName% execution successful!
goto :end

:pauseExecution
echo %queryName% failed
pause 

echo.
:end