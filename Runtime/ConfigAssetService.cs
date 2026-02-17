using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.Configurations
{
    public static class ConfigAssetService
    {
        private static readonly HashSet<ScriptableObject> s_singletonInstances = new();
        private static readonly HashSet<ConfigAssetRegistry> s_registries = new();



        #region  Singleton
        public static void AddSingletonRange(IEnumerable<ScriptableObject> instances)
        {
            s_singletonInstances.UnionWith(instances);
        }

        public static void RemoveSingletonRange(IEnumerable<ScriptableObject> instances)
        {
            s_singletonInstances.ExceptWith(instances);
        }

        public static T GetSingleton<T>()
            where T : ScriptableObject
        {
            return s_singletonInstances.First(s => s is T) as T;
        }
        #endregion



        #region Registry
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



        public static TRegistry GetRegistry<TRegistry>()
             where TRegistry : ConfigAssetRegistry
        {
            if (TryGetRegistry<TRegistry>(out var registry))
                return registry;
            else
                throw new InvalidOperationException($"Registry of type {typeof(TRegistry)} not found.");
        }

        public static bool TryGetRegistry<TRegistry>(out TRegistry registry)
            where TRegistry : ConfigAssetRegistry
        {
            registry = s_registries.FirstOrDefault(r => r is TRegistry) as TRegistry;
            return registry != null;
        }

        public static TRegistry GetRegistry<TRegistry>(string registryId)
            where TRegistry : ConfigAssetRegistry
        {
            if (string.IsNullOrEmpty(registryId))
                throw new ArgumentNullException(nameof(registryId));

            if (TryGetRegistry<TRegistry>(registryId, out var registry))
                return registry;
            else throw new InvalidOperationException($"Registry of type {typeof(TRegistry)} with id '{registryId}' not found.");
        }

        public static bool TryGetRegistry<TRegistry>(string registryId, out TRegistry registry)
            where TRegistry : ConfigAssetRegistry
        {
            if (string.IsNullOrEmpty(registryId))
                throw new ArgumentNullException(nameof(registryId));

            registry = s_registries.FirstOrDefault(r => r is TRegistry && r.Id == registryId) as TRegistry;
            return registry != null;
        }



        public static bool TryGetConfigById<TConfigAsset>(string id, out TConfigAsset config)
            where TConfigAsset : ConfigAsset
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            var targetType = typeof(TConfigAsset);

            foreach (var registry in s_registries)
            {
                if (!registry.ConfigType.IsAssignableFrom(targetType))
                    continue;

                var configBase = registry.GetBaseById(id);
                if (configBase == null)
                    continue;

                if (configBase is TConfigAsset typedConfig)
                {
                    config = typedConfig;
                    return true;
                }

                config = default;
                return false;
            }

            config = default;
            return false;
        }

        public static TConfigAsset GetConfigById<TConfigAsset>(string id)
            where TConfigAsset : ConfigAsset
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (TryGetConfigById<TConfigAsset>(id, out var config))
                return config;
            else throw new InvalidOperationException($"Config with id '{id}' was not found or is not assignable from {typeof(TConfigAsset)}.");
        }
        #endregion
    }
}