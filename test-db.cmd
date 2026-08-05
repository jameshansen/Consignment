@echo off
rem Local MySQL for testing, loaded with the demo dataset.
rem   test-db          start the server
rem   test-db reload   start it, then rebuild consignment_db from the .sql files
rem   test-db stop     shut it down
rem
rem Set MYSQL_HOME to use a MySQL somewhere other than ..\demo-test\mysql51

setlocal enabledelayedexpansion

set "PORT=3399"
set "DBUSER=root"
set "ROOT=%~dp0"

set "SERVER="
if defined MYSQL_HOME if exist "%MYSQL_HOME%\bin\mysqld.exe" set "SERVER=%MYSQL_HOME%"
if not defined SERVER if exist "%ROOT%..\demo-test\mysql51\bin\mysqld.exe" set "SERVER=%ROOT%..\demo-test\mysql51"

if not defined SERVER (
    echo No MySQL server found.
    echo Unpack a MySQL 5.x into ..\demo-test\mysql51, or set MYSQL_HOME to where it is.
    exit /b 1
)

set "CONN=--protocol=tcp --host=127.0.0.1 --port=%PORT% -u %DBUSER%"

if /i "%~1"=="stop" goto stop
if /i "%~1"=="reload" goto reload
if "%~1"=="" goto start
if /i "%~1"=="start" goto start
echo Unknown command "%~1". Use start, reload or stop.
exit /b 1

:start
call :startserver || exit /b 1
echo.
echo   Server:   127.0.0.1 port %PORT%, user %DBUSER%, no password
echo   Database: consignment_db
echo.
echo   settings.ini wants:  server=127.0.0.1;Port=%PORT%
echo.
exit /b 0

:reload
call :startserver || exit /b 1
echo Rebuilding consignment_db. This drops whatever is in it now.
"%SERVER%\bin\mysql.exe" %CONN% -e "DROP DATABASE IF EXISTS consignment_db;" || exit /b 1
"%SERVER%\bin\mysql.exe" %CONN% < "%ROOT%consignment_db_structure.sql" || exit /b 1
"%SERVER%\bin\mysql.exe" %CONN% < "%ROOT%demo_database.sql" || exit /b 1
"%SERVER%\bin\mysql.exe" %CONN% -e "SELECT (SELECT COUNT(*) FROM CSTITEM) AS items, (SELECT COUNT(*) FROM CSTORDER) AS orders, (SELECT COUNT(*) FROM CSTTBLTAX) AS tax_codes;" consignment_db
echo Demo data loaded.
exit /b 0

:stop
"%SERVER%\bin\mysqladmin.exe" %CONN% ping >nul 2>&1
if errorlevel 1 (
    echo Not running.
    exit /b 0
)
"%SERVER%\bin\mysqladmin.exe" %CONN% shutdown
echo Stopped.
exit /b 0

:startserver
"%SERVER%\bin\mysqladmin.exe" %CONN% ping >nul 2>&1
if not errorlevel 1 (
    echo Already running on port %PORT%.
    exit /b 0
)

echo Starting MySQL from %SERVER%
start "MySQL (consignment test)" /min "%SERVER%\bin\mysqld.exe" --no-defaults --basedir="%SERVER%" --datadir="%SERVER%\data" --port=%PORT% --bind-address=127.0.0.1 --default-storage-engine=MyISAM --console

for /l %%i in (1,1,30) do (
    "%SERVER%\bin\mysqladmin.exe" %CONN% ping >nul 2>&1
    if not errorlevel 1 (
        echo Running on port %PORT%.
        exit /b 0
    )
    ping -n 2 127.0.0.1 >nul
)
echo MySQL did not start. Check the MySQL window for the error.
exit /b 1
