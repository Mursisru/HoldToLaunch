namespace HoldToLaunch_Engine
{
    public static class AppVersion
    {
        public const string ReleaseBase = "1.0.0";
        public const string VersionChannel = "DEV";
        public const int CycleBuildNumber = 1;
        public const string ChangeLetters = "H";
        public const int SubNumber = 9;

        public static string BuildToken => $"DEV{CycleBuildNumber}{ChangeLetters}{SubNumber}";
        public static string Display => $"{ReleaseBase} Build {BuildToken}";
    }
}
