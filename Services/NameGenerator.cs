using System;
using System.Collections.Generic;

namespace SecureChat.Services
{
    public class NameGenerator : INameGenerator
    {
        private static readonly Random _random = new Random();
        
        private static readonly string[] _adjectives = new string[]
        {
            "Brave", "Calm", "Eager", "Fancy", "Gentle", "Happy", "Jolly", "Kind", 
            "Lively", "Mighty", "Noble", "Polite", "Quiet", "Rapid", "Silly", "Witty",
            "Zealous", "Bright", "Clever", "Daring", "Elated", "Fierce", "Graceful", "Honest"
        };
        
        private static readonly string[] _nouns = new string[]
        {
            "Bear", "Cat", "Dolphin", "Eagle", "Fox", "Giraffe", "Horse", "Ibex",
            "Jaguar", "Koala", "Lion", "Monkey", "Narwhal", "Owl", "Panda", "Rabbit",
            "Squirrel", "Tiger", "Unicorn", "Vulture", "Wolf", "Zebra", "Falcon", "Hedgehog"
        };
        
        private static readonly string[] _colors = new string[]
        {
            "Red", "Blue", "Green", "Yellow", "Purple", "Orange", "Teal", "Pink",
            "Cyan", "Magenta", "Lime", "Indigo", "Violet", "Gold", "Silver", "Maroon"
        };
        
        public string GenerateRandomName()
        {
            string adjective = _adjectives[_random.Next(_adjectives.Length)];
            string noun = _nouns[_random.Next(_nouns.Length)];
            
            return $"{adjective}{noun}";
        }
        
        public string GenerateAvatarCode()
        {
            // Generate a code that can be used to create a consistent but anonymous avatar
            // Format: color-shape-backgroundcolor (e.g., "red-circle-blue")
            string color = _colors[_random.Next(_colors.Length)].ToLower();
            int shape = _random.Next(1, 7); // 1=circle, 2=square, 3=triangle, 4=star, 5=diamond, 6=hexagon
            string bgColor = _colors[_random.Next(_colors.Length)].ToLower();
            
            // Ensure foreground and background colors are different
            while (bgColor == color)
            {
                bgColor = _colors[_random.Next(_colors.Length)].ToLower();
            }
            
            return $"{color}-{shape}-{bgColor}";
        }
    }
}