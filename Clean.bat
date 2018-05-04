@echo off

echo.
echo ----------------------------------------------------------------
echo.
echo Press any key to start cleaning the solution.
echo.
echo ----------------------------------------------------------------
echo.

pause

pushd %~dp0

del /f /s /q /ah /ar /aa *.suo
del /f /s /q /ah /ar /aa *.user

rd /s /q .\EasyPdfCloudSample\bin
rd /s /q .\EasyPdfCloudSample\obj
rd /s /q .\EasyPdfCloudSample\Package

rd /s /q .\.vs
rd /s /q .\packages

popd

echo.
echo ----------------------------------------------------------------
echo.
echo Done
echo.
echo ----------------------------------------------------------------
echo.

pause
