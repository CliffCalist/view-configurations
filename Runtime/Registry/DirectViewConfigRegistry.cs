using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class DirectViewConfigRegistry<TTarget, TViewConfig> : ViewConfigRegistry
        where TTarget : ScriptableObject
        where TViewConfig : DirectViewConfig<TTarget>
    {
        [SerializeField] private List<TViewConfig> _configs;



        public override sealed Type ConfigType => typeof(TViewConfig);
        public override sealed IReadOnlyList<ViewConfig> Configs => _configs;



        internal override sealed void AddConfig(ViewConfig instance)
        {
            if (instance is TViewConfig typedInstance)
                _configs.Add(typedInstance);
            else throw new ArgumentException($"Invalid type: {instance.GetType()}");
        }

        internal override sealed void RemoveConfig(ViewConfig instance)
        {
            if (instance is TViewConfig typedInstance)
                _configs.Remove(typedInstance);
            else throw new ArgumentException($"Invalid type: {instance.GetType()}");
        }

        internal override sealed bool RemoveAllNullConfigs()
        {
            return _configs.RemoveAll(m => m == null) > 0;
        }
    }
}