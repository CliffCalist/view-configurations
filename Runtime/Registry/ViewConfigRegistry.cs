using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class ViewConfigRegistry : ScriptableObject
    {
        public abstract Type ConfigType { get; }
        public abstract IReadOnlyList<ViewConfig> Configs { get; }



        internal abstract void AddConfig(ViewConfig instance);
        internal abstract void RemoveConfig(ViewConfig instance);
        internal abstract bool RemoveAllNullConfigs();



        public ViewConfig GetConfigFor(object target)
        {
            return Configs.FirstOrDefault(config => config.TargetRaw == target);
        }
    }
}