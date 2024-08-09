using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureChat.Data;
using SecureChat.Models;
using SecureChat.Services;

namespace SecureChat.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SecureChatContext _context;
    private readonly INameGenerator _nameGenerator;

    public HomeController(
        ILogger<HomeController> logger,
        SecureChatContext context,
        INameGenerator nameGenerator)
    {
        _logger = logger;
        _context = context;
        _nameGenerator = nameGenerator;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        try
        {
            var session = new ChatSession();
            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created new session with ID: {SessionId}", session.Id);
            
            return RedirectToAction("Room", new { id = session.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new session");
            ModelState.AddModelError("", "An error occurred while creating the session. Please try again.");
            return View("Index");
        }
    }
    
    [HttpGet("join")]
    public async Task<IActionResult> Join(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            ModelState.AddModelError("", "Session ID is required");
            return View("Index");
        }
        
        var session = await _context.ChatSessions.FindAsync(id);
        if (session == null)
        {
            ModelState.AddModelError("", "Session not found or has expired");
            return View("Index");
        }
        
        return RedirectToAction("Room", new { id });
    }
    
    [HttpGet("room/{id}")]
    public async Task<IActionResult> Room(string id)
    {
        var session = await _context.ChatSessions.FindAsync(id);
        if (session == null)
        {
            return NotFound();
        }
        
        // Generate random name and avatar for participant
        var randomName = _nameGenerator.GenerateRandomName();
        var avatarCode = _nameGenerator.GenerateAvatarCode();
        
        // Update session activity
        session.LastActivity = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        ViewBag.SessionId = id;
        ViewBag.RandomName = randomName;
        ViewBag.AvatarCode = avatarCode;
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}