@echo off
echo Installing Entity Framework Core tools...
dotnet tool install --global dotnet-ef

echo Creating database migrations...
dotnet ef migrations add InitialCreate

echo Applying migrations to database...
dotnet ef database update

echo Building and running the application...
dotnet build
dotnet run

echo.
echo SecureChat is now running!
echo.
echo Note: Several issues have been fixed that might have prevented the application from starting:
echo - CORS configuration has been corrected to use specific origins instead of wildcard with credentials
echo - Background service has been updated to handle task cancellation properly
echo.
echo Open your browser and navigate to http://localhost:5000 to test the application.
echo.
echo Follow these steps to test:
echo 1. On the landing page, click "Create New Session"
echo 2. Note the session ID displayed in the welcome message
echo 3. Open a new browser window and go to http://localhost:5000
echo 4. Enter the session ID and click "Join"
echo 5. Test the dark/light mode toggle using the moon/sun icon
echo.
echo For more detailed instructions, see the README.md file.
echo.
pause