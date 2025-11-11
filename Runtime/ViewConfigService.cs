using System.Collections.Generic;

namespace WhiteArrow.ViewConfigurations
{
    public static class ViewConfigService
    {
        private static readonly HashSet<ViewConfigRegistry> s_registries = new();



        public static void AddRegistry(ViewConfigRegistry registry)
        {
            s_registries.Add(registry);
        }

        public static void RemoveRegistry(ViewConfigRegistry registry)
        {
            s_registries.Remove(registry);
        }

        public static void ClearRegisters()
        {
            s_registries.Clear();
        }



        public static TViewConfig GetConfigFor<TViewConfig>(object target)
            where TViewConfig : ViewConfig
        {
            foreach (var registry in s_registries)
            {
                var config = registry.GetConfigFor(target);
                if (config != null && config is TViewConfig typedConfig)
                    return typedConfig;
            }

            return null;
        }
    }
}