using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureChat.Data;
using SecureChat.Models;

namespace SecureChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SecureChatContext _context;
        private readonly ILogger<ChatHub> _logger;
        
        public ChatHub(SecureChatContext context, ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task JoinSession(string sessionId)
        {
            var session = await _context.ChatSessions
                .Include(s => s.Participants)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
                
            if (session == null)
            {
                throw new HubException("Session not found or has expired");
            }
            
            // Update session activity
            session.LastActivity = DateTime.UtcNow;
            
            // Add to SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            
            // Notify others
            await Clients.OthersInGroup(sessionId).SendAsync("ParticipantJoined", Context.ConnectionId);
            
            await _context.SaveChangesAsync();
        }
        
        public async Task SendMessage(string sessionId, string encryptedMessage, string iv, string encryptedKeys)
        {
            // We only pass through encrypted content, never store or process it
            await Clients.OthersInGroup(sessionId).SendAsync("ReceiveMessage", Context.ConnectionId, encryptedMessage, iv, encryptedKeys);
            
            // Update activity timestamps
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session != null)
            {
                session.LastActivity = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task DestroySession(string sessionId)
        {
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session != null)
            {
                _context.ChatSessions.Remove(session);
                await _context.SaveChangesAsync();
            }
            
            await Clients.Group(sessionId).SendAsync("SessionDestroyed");
        }
        
        public async Task NotifyTyping(string sessionId)
        {
            await Clients.OthersInGroup(sessionId).SendAsync("UserTyping", Context.ConnectionId);
        }
        
        public async Task SharePublicKey(string sessionId, string publicKey)
        {
            await Clients.OthersInGroup(sessionId).SendAsync("PublicKeyShared", Context.ConnectionId, publicKey);
            await Clients.Caller.SendAsync("RequestExistingPublicKeys", sessionId);
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Find participant by connection ID
            var participant = await _context.Participants
                .Include(p => p.Session)
                .FirstOrDefaultAsync(p => p.ConnectionId == Context.ConnectionId);
                
            if (participant != null)
            {
                string sessionId = participant.SessionId;
                
                // Remove participant
                _context.Participants.Remove(participant);
                
                // If this was the last participant, remove the session too
                if (!await _context.Participants.AnyAsync(p => p.SessionId == sessionId))
                {
                    var session = await _context.ChatSessions.FindAsync(sessionId);
                    if (session != null)
                    {
                        _context.ChatSessions.Remove(session);
                    }
                }
                
                await _context.SaveChangesAsync();
                
                // Notify others
                await Clients.OthersInGroup(sessionId).SendAsync("ParticipantLeft", Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}