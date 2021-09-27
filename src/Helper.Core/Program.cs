using Helper.Core.Modules;
using System;

namespace Helper.Core
{
    class Program
    {
        static void Main(bool package = false)
        {
            if (package)
            {
                var copy = new CopyPackages();
                copy.Run();
            }
        }
    }
}
