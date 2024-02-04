namespace Silksprite.Modularizer.Extensions
{
    public static class ModularizerConfigExtensions
    {
        public static string ModuleName(this ModularizerConfig config, string moduleName) => $"{config.avatarRoot.gameObject.name}_{moduleName}"; 
    }
}