using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Stats
{
    public class Settings
    {
        public string ServerUrl { get; set; } = Environment.GetEnvironmentVariable("SERVER_URL");
        public string Email { get; set; } = Environment.GetEnvironmentVariable("EMAIL");
        public string Password { get; set; } = Environment.GetEnvironmentVariable("PASSWORD");
        public string Username { get; set; } = Environment.GetEnvironmentVariable("USERNAME");

        public string StatsDServer { get; set; } = Environment.GetEnvironmentVariable("STATSD_SERVER");
        public string AppName { get; set; } = Environment.GetEnvironmentVariable("APP_NAME");
        public string MemoryPath { get; set; } = Environment.GetEnvironmentVariable("PATH");
        public string Interval { get; set; } = Environment.GetEnvironmentVariable("INTERVAL");
    }
}
