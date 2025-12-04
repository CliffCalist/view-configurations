using System;
using System.Collections.Generic;

namespace WhiteArrow.Configurations
{
    public static class ConfigAssetService
    {
        private static readonly HashSet<ConfigAssetRegistry> s_registries = new();



        public static void AddRegistry(ConfigAssetRegistry registry)
        {
            s_registries.Add(registry);
        }

        public static void AddRegistryRange(IEnumerable<ConfigAssetRegistry> registries)
        {
            s_registries.UnionWith(registries);
        }


        public static void RemoveRegistry(ConfigAssetRegistry registry)
        {
            s_registries.Remove(registry);
        }

        public static void RemoveRegistryRange(IEnumerable<ConfigAssetRegistry> registries)
        {
            s_registries.ExceptWith(registries);
        }


        public static void ClearRegisters()
        {
            s_registries.Clear();
        }



        public static bool TryGetById<TConfigAsset>(string id, out TConfigAsset config)
            where TConfigAsset : ConfigAsset
        {
            try
            {
                config = GetById<TConfigAsset>(id);
                return true;
            }
            catch
            {
                config = default;
                return false;
            }
        }

        public static TConfigAsset GetById<TConfigAsset>(string id)
            where TConfigAsset : ConfigAsset
        {
            var targetType = typeof(TConfigAsset);

            foreach (var registry in s_registries)
            {
                if (!targetType.IsAssignableFrom(registry.ConfigType))
                    continue;

                var config = registry.GetBaseById(id);
                if (config != null && config is TConfigAsset typedConfig)
                    return typedConfig;
                else throw new InvalidOperationException($"Config with id '{id}' was found, but it is not assignable from {targetType}");
            }

            throw new InvalidOperationException($"Config with id '{id}' or type {targetType} was not found.");
        }
    }
}