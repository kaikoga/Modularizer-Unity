using System;
using UnityEngine;

namespace Silksprite.Modularizer.DataObjects
{
    [Serializable]
    public class RendererMapping
    {
        public Renderer renderer;
        public string moduleName;

        public RendererMapping(Renderer renderer, string moduleName)
        {
            this.renderer = renderer;
            this.moduleName = moduleName;
        }
    }
}