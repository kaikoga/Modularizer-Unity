namespace Silksprite.Modularizer.Models
{
    public class ModuleDefinition
    {
        public string ModuleName { get; internal set; }
        public bool IsBaseModule { get; internal set; }
        public string[] Paths { get; internal set; }
    }
}