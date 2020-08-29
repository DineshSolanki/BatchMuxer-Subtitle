using System.Runtime.InteropServices;

namespace BatchMuxer_SubEd_Console.Model
{
    public static class ThisOperatingSystem
    {
        public static OSPlatform CurrentOS = IsWindows() ? OSPlatform.Windows : IsLinux() ? OSPlatform.Linux : IsMacOS() ? OSPlatform.OSX : OSPlatform.Create("other");

        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}