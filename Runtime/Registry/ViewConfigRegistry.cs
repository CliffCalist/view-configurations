using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public class ViewConfigRegistry : IViewConfigRegistry
    {
        private readonly Dictionary<ScriptableObject, List<ViewConfig>> _map = new();



        public ViewConfigRegistry(IEnumerable<ViewConfig> viewConfigs)
        {
            foreach (var config in viewConfigs)
            {
                if (config == null || config.TargetAsBase == null)
                    continue;

                if (_map.ContainsKey(config.TargetAsBase))
                    _map[config.TargetAsBase].Add(config);
                else
                    _map.Add(config.TargetAsBase, new() { config });
            }
        }



        public TView GetViewFor<TView>(ScriptableObject config)
            where TView : ViewConfig
        {
            if (_map.TryGetValue(config, out var viewConfigs))
            {
                if (TryFindViewConfigByType<TView>(viewConfigs, out var result))
                    return result;

                throw new InvalidOperationException($"{typeof(ViewConfig).Name} of type {typeof(TView).Name} not found for {config.name}");
            }
            else throw new InvalidOperationException($"{nameof(ViewConfig)} not found for {config.name}");
        }

        public bool TryGetViewFor<TView>(ScriptableObject config, out TView uiConfig)
            where TView : ViewConfig
        {
            if (_map.TryGetValue(config, out var viewConfigs))
                return TryFindViewConfigByType(viewConfigs, out uiConfig);

            uiConfig = default;
            return false;
        }

        private bool TryFindViewConfigByType<TView>(List<ViewConfig> viewConfigs, out TView viewConfig)
            where TView : ViewConfig
        {
            var result = viewConfigs.Find(c => c is TView);

            if (result != null)
                viewConfig = result as TView;
            else
                viewConfig = default;

            return result != null;
        }
    }
}