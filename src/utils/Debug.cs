namespace QuakeSounds.Utils
{
    public static class Debug
    {
        private static bool _enabled;
        private static Func<string, string>? _format;

        public static void Configure(bool enabled, Func<string, string> format)
        {
            _enabled = enabled;
            _format = format;
        }

        public static void Print(string message)
        {
            if (_enabled && _format != null)
            {
                Console.WriteLine(_format(message));
            }
        }
    }
}
