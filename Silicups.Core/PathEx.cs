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
            foreach (string filename in System.IO.Directory.GetFiles(baseDirectory, pattern, System.IO.SearchOption.AllDirectories))
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

                files.Add(filename);
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
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static String MakeRelativePathFromOtherPath(String absolutePath, String otherAbsolutePath)
        {
            return MakeRelativePath(System.IO.Path.GetDirectoryName(otherAbsolutePath) + System.IO.Path.DirectorySeparatorChar, absolutePath);
        }
    }
}
