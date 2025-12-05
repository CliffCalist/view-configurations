using System;
using System.Collections.Generic;
using System.Linq;

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



        public static bool TryGetRegistry<TRegistry>(out TRegistry registry)
            where TRegistry : ConfigAssetRegistry
        {
            try
            {
                registry = GetRegistry<TRegistry>();
                return true;
            }
            catch
            {
                registry = default;
                return false;
            }
        }

        public static TRegistry GetRegistry<TRegistry>()
            where TRegistry : ConfigAssetRegistry
        {
            return s_registries.First(r => r is TRegistry) as TRegistry;
        }

        public static bool TryGetRegistry<TRegistry>(string registryId, out TRegistry registry)
            where TRegistry : ConfigAssetRegistry
        {
            try
            {
                registry = GetRegistry<TRegistry>(registryId);
                return true;
            }
            catch
            {
                registry = default;
                return false;
            }
        }

        public static TRegistry GetRegistry<TRegistry>(string registryId)
            where TRegistry : ConfigAssetRegistry
        {
            foreach (var r in s_registries)
            {
                if (r is TRegistry typed && r.Id == registryId)
                    return typed;
            }

            throw new InvalidOperationException($"Registry of type {typeof(TRegistry)} with id '{registryId}' not found.");
        }



        public static bool TryGetConfigById<TConfigAsset>(string id, out TConfigAsset config)
            where TConfigAsset : ConfigAsset
        {
            try
            {
                config = GetConfigById<TConfigAsset>(id);
                return true;
            }
            catch
            {
                config = default;
                return false;
            }
        }

        public static TConfigAsset GetConfigById<TConfigAsset>(string id)
            where TConfigAsset : ConfigAsset
        {
            var targetType = typeof(TConfigAsset);

            foreach (var registry in s_registries)
            {
                if (!targetType.IsAssignableFrom(registry.ConfigType))
                    continue;

                var config = registry.GetBaseById(id);
                if (config == null)
                    continue;

                if (config is TConfigAsset typedConfig)
                    return typedConfig;

                throw new InvalidOperationException($"Config with id '{id}' was found, but it is not assignable from {targetType}");
            }

            throw new InvalidOperationException($"Config with id '{id}' or type {targetType} was not found.");
        }

        public static bool TryGetConfigById<TConfigAsset>(string registryId, string id, out TConfigAsset config)
            where TConfigAsset : ConfigAsset
        {
            try
            {
                config = GetConfigById<TConfigAsset>(registryId, id);
                return true;
            }
            catch
            {
                config = default;
                return false;
            }
        }

        public static TConfigAsset GetConfigById<TConfigAsset>(string registryId, string id)
            where TConfigAsset : ConfigAsset
        {
            var targetType = typeof(TConfigAsset);

            foreach (var registry in s_registries)
            {
                if (!targetType.IsAssignableFrom(registry.ConfigType))
                    continue;

                if (registry.Id != registryId)
                    continue;

                var config = registry.GetBaseById(id);
                if (config == null)
                    continue;

                if (config is TConfigAsset typedConfig)
                    return typedConfig;

                throw new InvalidOperationException($"Config with id '{id}' was found in registry '{registryId}', but it is not assignable from {targetType}");
            }

            throw new InvalidOperationException($"Config with id '{id}' or type {targetType} was not found in registry '{registryId}'.");
        }
    }
}