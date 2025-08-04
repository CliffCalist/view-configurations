using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.SRPConfigurations
{
    public class ViewConfigRegistry : IViewConfigRegistry
    {
        private readonly Dictionary<ScriptableObject, ViewConfig> _map = new();



        public ViewConfigRegistry(IEnumerable<ViewConfig> viewConfigs)
        {
            foreach (var config in viewConfigs)
            {
                if (config == null || config.TargetAsBase == null)
                    continue;

                _map[config.TargetAsBase] = config;
            }
        }



        public TView GetViewFor<TView>(ScriptableObject config)
            where TView : ViewConfig
        {
            if (_map.TryGetValue(config, out var result))
                return result as TView;
            throw new Exception($"UI config not found for {config.name}");
        }

        public bool TryGetViewFor<TView>(ScriptableObject config, out TView uiConfig)
            where TView : ViewConfig
        {
            if (_map.TryGetValue(config, out var result) && result is TView typed)
            {
                uiConfig = typed;
                return true;
            }

            uiConfig = null;
            return false;
        }
    }
}