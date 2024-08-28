namespace SecureChat.Services
{
    public interface INameGenerator
    {
        string GenerateRandomName();
        string GenerateAvatarCode();
    }
}