﻿namespace PostsByMarko.Test.Shared.Helper
{
    public static class FileHelper
    {
        public static string? FindFileDirectory(string? startDirectory, string fileName)
        {
            while (true)
            {
                if (startDirectory == null) return null;

                if (File.Exists(Path.Combine(startDirectory, fileName))) return startDirectory;

                var info = Directory.GetParent(startDirectory);

                if (info != null)
                {
                    startDirectory = info.FullName;
                    continue;
                }

                return null;
            }
        }
    }
}
