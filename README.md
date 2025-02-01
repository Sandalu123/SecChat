# SecureChat

An end-to-end encrypted ephemeral messaging platform with real-time communication.

## Testing Instructions

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- SQL Server (or SQL Server LocalDB which comes with Visual Studio)
- A modern web browser (Chrome, Firefox, Edge, etc.)

### Setup and Run the Application

1. **Create and apply database migrations**

   Open a command prompt in the SecureChat project directory and run:

   ```
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

   This creates the necessary database tables based on our Entity Framework models.

2. **Build and run the application**

   In the SecureChat project directory, run:

   ```
   dotnet build
   dotnet run
   ```

   The application will start and display the URL where it's hosted (typically http://localhost:5000 or https://localhost:5001).

3. **Access the application**

   Open your web browser and navigate to the URL displayed in the console (e.g., http://localhost:5000).

### Testing Features

#### Landing Page
- You should see the landing page with two options: "Create a New Secure Chat" and "Join an Existing Chat"
- Test the dark/light mode toggle by clicking the moon/sun icon in the top right corner

#### Create a New Chat Session
1. Click the "Create New Session" button
2. You will be redirected to a chat room with a unique session ID
3. Note the session ID displayed in the welcome message (you'll need this to test joining)

#### Join an Existing Chat Session
1. Open a new browser window or incognito window to simulate another user
2. Navigate to the landing page
3. Enter the session ID from your first window in the "Enter Session ID" field
4. Click "Join"
5. You should be redirected to the same chat room with a different random name and avatar

#### Chat Room Features (Phase 1 Limitations)
In Phase 1, we've implemented the UI and foundation for the chat functionality. The complete real-time messaging will be added in Phase 2, but you can test the following features:

- Typing a message and clicking send (the message will appear in your window but won't be sent to other participants yet)
- Viewing the participant list (you should see yourself)
- Testing the theme toggle in the chat interface
- Clicking "Destroy Session" and confirming (this will show a placeholder message since the full functionality will be implemented in Phase 2)

### Expected Behavior in Phase 1

In Phase 1, the application provides:
- Creating and joining sessions (with database integration)
- Random name and avatar generation
- UI for the chat interface
- Dark/light mode theming
- Session cleanup via background service (sessions inactive for 30+ minutes will be automatically removed)

Full real-time messaging, encryption implementation, and other features will be completed in Phase 2.

### Testing the Session Cleanup

The session cleanup service will automatically remove sessions that have been inactive for 30 minutes or more. To test this:

1. Create a session and note the session ID
2. Wait 30+ minutes without any activity in the session
3. Try to join the session using the ID - you should receive an error message indicating the session doesn't exist

### Troubleshooting

If you encounter any issues:

- Ensure the database connection string in `appsettings.json` matches your SQL Server instance
- Check console logs in the browser developer tools (F12) for any JavaScript errors
- Verify that the .NET Core application is running without errors in the console
- If you're having database issues, confirm that the migrations were created and applied successfully
- If you see a "TaskCanceledException" in the console when shutting down the application, this is a normal part of the application lifecycle. The SessionCleanupService has been updated to handle this gracefully.

## Known Issues (Fixed)

1. **TaskCanceledException in Background Service**
   - **Symptom**: Error message `System.Threading.Tasks.TaskCanceledException: A task was canceled` when shutting down the application
   - **Cause**: The background service wasn't properly handling the cancellation token during application shutdown
   - **Fix**: The SessionCleanupService now properly catches TaskCanceledException and uses cancellation tokens in all async operations

2. **CORS Configuration Error**
   - **Symptom**: Error message `The CORS protocol does not allow specifying a wildcard (any) origin and credentials at the same time`
   - **Cause**: Using a wildcard origin (`*`) with `AllowCredentials()` in the CORS policy, which violates the CORS specification
   - **Fix**: Replaced the wildcard with specific localhost origins for development testing