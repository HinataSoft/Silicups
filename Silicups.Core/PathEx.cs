using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Silicups.Core
{
    public static class PathEx
    {
        class FilterItem
        {
            public bool Include;
            public string Contains;
        }

        public static string[] FindFiles(string baseDirectory, string pattern, string filter)
        {
            var filters = new List<FilterItem>();
            if (!String.IsNullOrWhiteSpace(filter))
            {
                foreach (string s in filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string part = s;
                    bool include = true;
                    if (part.StartsWith("+"))
                    {
                        part = part.Substring(1);
                    }
                    else if (part.StartsWith("-"))
                    {
                        include = false;
                        part = part.Substring(1);
                    }
                    if (!String.IsNullOrWhiteSpace(part))
                    { filters.Add(new FilterItem() { Include = include, Contains = part }); }
                }
            }

            var files = new List<string>();
            foreach (string patternItem in pattern.Split('|'))
            {
                foreach (string filename in Directory.GetFiles(baseDirectory, patternItem, SearchOption.AllDirectories))
                {
                    bool filtered = false;
                    foreach (FilterItem filterItem in filters)
                    {
                        if (filterItem.Include && !filename.Contains(filterItem.Contains))
                        { filtered = true; break; }
                        if (!filterItem.Include && filename.Contains(filterItem.Contains))
                        { filtered = true; break; }
                    }
                    if (filtered)
                    { continue; }

                    files.Add(filename.Replace(@"\\", @"\"));
                }
            }
            return files.ToArray();
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// http://stackoverflow.com/a/340454
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static string MakeRelativePathFromOtherPath(string absolutePath, string otherAbsolutePath)
        {
            return MakeRelativePath(Path.GetDirectoryName(otherAbsolutePath) + Path.DirectorySeparatorChar, absolutePath);
        }

        public static string GetBestAbsolutePath(string workingDirectory, string absolutePath, string relativePath)
        {
            if (File.Exists(absolutePath) || Directory.Exists(absolutePath))
            { return absolutePath; }

            if (!String.IsNullOrEmpty(relativePath))
            {
                string otherAbsolutePath = Path.GetFullPath(Path.Combine(workingDirectory, relativePath));
                if (File.Exists(otherAbsolutePath) || Directory.Exists(otherAbsolutePath))
                { return otherAbsolutePath; }
            }

            return absolutePath;
        }

        public static string GetFullAbsolutePath(string path)
        {
            return System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)) + System.IO.Path.DirectorySeparatorChar;
        }
    }
}
