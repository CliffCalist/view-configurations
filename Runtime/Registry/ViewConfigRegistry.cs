using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class ViewConfigRegistry : ScriptableObject
    {
        public abstract Type ConfigType { get; }
        public abstract IReadOnlyList<ViewConfig> BaseConfigs { get; }



        internal abstract void AddConfig(ViewConfig instance);
        internal abstract void RemoveConfig(ViewConfig instance);
        internal abstract bool RemoveAllNullConfigs();



        public ViewConfig GetConfigFor(object target)
        {
            return BaseConfigs.FirstOrDefault(config => config.BaseTarget == target);
        }
    }

    public abstract class ViewConfigRegistry<TViewConfig> : ViewConfigRegistry
        where TViewConfig : ViewConfig
    {
        public abstract IReadOnlyList<TViewConfig> Configs { get; }

        public override sealed Type ConfigType => typeof(TViewConfig);
        public override sealed IReadOnlyList<ViewConfig> BaseConfigs => Configs;
    }
}