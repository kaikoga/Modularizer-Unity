using System;
using UnityEngine;

namespace Silksprite.Modularizer.DataObjects
{
    [Serializable]
    public class RendererSet
    {
        public bool enabled;
        public string moduleName;
        public Renderer[] renderers;
        public bool isBaseModule;

        public Command command;

        public enum Command
        {
            None,
            Up,
            Down,
            New
        }
    }
}