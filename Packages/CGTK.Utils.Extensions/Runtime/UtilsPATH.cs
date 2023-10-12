using System;
using System.IO;
using JetBrains.Annotations;

namespace CGTK.Utils.Extensions
{
    //public static class UtilsPATH
    //public static class EnvironmentExtensions
    public static class UtilsPATH
    {
        [PublicAPI]
        public static Boolean ExistsInPATH(in String fileName) => GetFromPATH(fileName: fileName).NotNullOrWhiteSpace();

        [PublicAPI]
        public static String GetFromPATH(in String fileName)
        {
            if (File.Exists(path: fileName)) return Path.GetFullPath(path: fileName);

            String __values = Environment.GetEnvironmentVariable(variable: "PATH");

            if (__values.IsNullOrWhiteSpace()) return null;

            String[] __paths = __values.Split(separator: Path.PathSeparator);
            foreach (String __path in __paths)
            {
                String __fullPath = Path.Combine(path1: __path, path2: fileName);
                
                if (File.Exists(path: __fullPath)) return __fullPath;
            }
            
            return null;
        }
    }
}
