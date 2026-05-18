@echo off

title Lab Query

set database="%USERPROFILE%\Repos\LeadPipe\LeadPipe.Infrastructure\.info\leadpipe.test.db"
set output="%USERPROFILE%\Repos\Automate\Automate.Infrastructure\.info\Reports\LabReport_Test.csv"
set sql="%USERPROFILE%\Repos\LeadPipe\Batch\LabReport.sql"

sqlite3 -header -csv %database% < %sql% > %output%

set error=%errorlevel%
echo.
echo Lab Query success: %error%
:: error messages are placed in the output file
if not "%error%"="0" (
    type %output%
    goto :pauseExecution
)
echo.

:: Ending
echo All Executions successful!
goto :end

:pauseExecution
echo Query failed
pause 

echo.
:end