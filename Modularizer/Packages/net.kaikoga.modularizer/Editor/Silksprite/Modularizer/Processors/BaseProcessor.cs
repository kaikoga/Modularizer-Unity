using System.IO;
using Silksprite.Modularizer.Models;

namespace Silksprite.Modularizer.Processors
{
    public abstract class BaseProcessor
    {
        public abstract void Process(ModularizeDefinition definition);

        protected static string BuildPrefabPath(string path, string moduleName)
        {
            return Path.Combine(path, $"{moduleName}.prefab");
        }
    }
}