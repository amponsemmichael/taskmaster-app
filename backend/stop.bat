@echo off
REM TaskMaster Stop Script for Windows
REM This script stops the Docker containers for TaskMaster

echo Stopping TaskMaster application...

REM Stop Docker containers
docker-compose down

echo TaskMaster stopped successfully!
echo.
echo To remove all data (including database):
echo   docker-compose down -v

pause