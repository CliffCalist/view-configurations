using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.Configurations
{
    public abstract class ConfigAssetRegistry : ScriptableObject
    {
        public abstract Type ConfigType { get; }
        public abstract IReadOnlyList<ConfigAsset> BaseConfigs { get; }



        internal abstract bool HasConfig(ConfigAsset asset);
        internal abstract void AddConfig(ConfigAsset asset);
        internal abstract void RemoveConfig(ConfigAsset asset);
        internal abstract bool RemoveAllNullConfigs();


        internal protected abstract IEnumerable<string> GetDeclaredConfigIds();


        public ConfigAsset GetBaseById(string id)
        {
            return BaseConfigs.FirstOrDefault(config => config.Id == id);
        }
    }

    public abstract class ConfigAssetRegistry<TConfigAsset> : ConfigAssetRegistry
        where TConfigAsset : ConfigAsset
    {
        [SerializeField] private List<TConfigAsset> _configs = new();


        public override sealed Type ConfigType => typeof(TConfigAsset);

        public IReadOnlyList<TConfigAsset> Configs => _configs;
        public override sealed IReadOnlyList<ConfigAsset> BaseConfigs => Configs;



        internal override sealed bool HasConfig(ConfigAsset asset)
        {
            return _configs.Contains(asset);
        }

        internal override sealed void AddConfig(ConfigAsset asset)
        {
            if (asset is TConfigAsset typedConfig)
            {
                if (_configs.Contains(typedConfig))
                    Debug.LogWarning($"Config with id {typedConfig.Id} already exists.", typedConfig);
                else
                    _configs.Add(typedConfig);
            }
            else throw new ArgumentException($"Type {asset.GetType()} is not assignable from {ConfigType}");
        }

        internal override sealed void RemoveConfig(ConfigAsset asset)
        {
            if (asset is TConfigAsset typedConfig)
                _configs.Remove(typedConfig);
            else throw new ArgumentException($"Type {asset.GetType()} is not assignable from {ConfigType}");
        }

        internal override sealed bool RemoveAllNullConfigs()
        {
            var hasRemovedConfigs = _configs.RemoveAll(config => config == null) > 0;
            return hasRemovedConfigs;
        }



        public TConfigAsset GetById(string id)
        {
            return Configs.FirstOrDefault(config => config.Id == id);
        }
    }
}