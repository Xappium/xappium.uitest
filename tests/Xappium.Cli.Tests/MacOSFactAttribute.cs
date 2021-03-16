using Xappium;

namespace Xunit
{
    public class MacOSFactAttribute : FactAttribute
    {
        public MacOSFactAttribute()
        {
#if WINDOWS_NT
            Skip = "Test should only be run on MacOS";
#endif
        }
    }
}
