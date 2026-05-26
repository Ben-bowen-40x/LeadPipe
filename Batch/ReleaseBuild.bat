@echo off
title Update Release Builds
setlocal enabledelayedexpansion

set REPOS=Automate LeadPipe

for %%R in (%REPOS%) do (
    set REPO=%%R
    title Update %%R Release Build
    echo.
    echo ========================================
    echo Building %%R
    echo ========================================

    cd %USERPROFILE%\Repos\%%R || (
        echo ERROR: Could not navigate to %%R repo.
        goto :failure
    )

    :: Capture and switch to main if needed
    for /f "delims=" %%B in ('git rev-parse --abbrev-ref HEAD') do set CURRENT_BRANCH=%%B
    if "!CURRENT_BRANCH!" neq "main" (
        echo Switching to main from !CURRENT_BRANCH!...
        git checkout main
        if errorlevel 1 goto :gitfailure
    ) else (
        echo Already on main.
    )

    dotnet build --configuration Release
    if errorlevel 1 goto :buildfailure

    echo Successfully built %%R Release!

    :: Return to dev
    for /f "delims=" %%B in ('git rev-parse --abbrev-ref HEAD') do set CURRENT_BRANCH=%%B
    if "!CURRENT_BRANCH!" neq "dev" (
        echo Switching back to dev...
        git checkout dev
        if errorlevel 1 goto :gitfailure
    ) else (
        echo Already on dev.
    )
)

goto :end

:buildfailure
echo Build failure in %REPO%. Check the output above.
pause
goto :end

:gitfailure
echo Git failed to switch branches in %REPO%. Please resolve manually.
pause
goto :end

:failure
echo Unexpected failure in %REPO%.
pause

:end
echo.
echo Done.