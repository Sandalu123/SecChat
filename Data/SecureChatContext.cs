using Microsoft.EntityFrameworkCore;
using SecureChat.Models;

namespace SecureChat.Data
{
    public class SecureChatContext : DbContext
    {
        public SecureChatContext(DbContextOptions<SecureChatContext> options)
            : base(options)
        {
        }
        
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<Participant> Participants { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and indexes
            modelBuilder.Entity<ChatSession>()
                .HasIndex(s => s.LastActivity);
                
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Session)
                .WithMany(s => s.Participants)
                .HasForeignKey(p => p.SessionId);
                
            modelBuilder.Entity<Participant>()
                .HasIndex(p => p.ConnectionId);
        }
    }
}