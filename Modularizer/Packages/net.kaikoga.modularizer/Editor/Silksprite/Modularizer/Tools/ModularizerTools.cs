using System.IO;
using UnityEditor;
using UnityEngine;

namespace Silksprite.Modularizer.Tools
{
    public static class ModularizerTools
    {
        static string ProjectRoot => GetDirectoryName(Application.dataPath);

        static string GetDirectoryName(string path) => Path.GetDirectoryName(path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        static string AbsoluteToProjectPath(string absolutePath) => $"Assets{absolutePath.Substring(Application.dataPath.Length)}/";

        static string ProjectToAbsolutePath(string projectPath) => projectPath == null ? Application.dataPath : Path.Combine(ProjectRoot, projectPath);

        public static bool SelectFolder(string title, ref string projectPath)
        {
            var absolutePath = EditorUtility.OpenFolderPanel(title, ProjectToAbsolutePath(projectPath), "");
            if (absolutePath == null) return false;
            if (!absolutePath.StartsWith(ProjectRoot)) return false;
            projectPath = AbsoluteToProjectPath(absolutePath);
            return true;
        }

        public static void EnsureDirectory(string path)
        {
            Directory.CreateDirectory(GetDirectoryName(path));
        }

        public static string GetRelativePath(this Transform parent, Transform child)
        {
            var path = child.name;
            for (var current = child.parent; current != null && current != parent; current = current.parent)
            {
                path = $"{current.name}/{path}";
            }
            return path;
        }
    }
}