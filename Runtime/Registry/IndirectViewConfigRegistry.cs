using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class IndirectViewConfigRegistry<TTarget, TTargetProvider, TViewConfig> : ViewConfigRegistry<TViewConfig>
        where TTargetProvider : ScriptableObject
        where TViewConfig : IndirectViewConfig<TTarget, TTargetProvider>
    {
        [SerializeField] private TTargetProvider _targetProvider;
        [SerializeField] private List<TViewConfig> _configs = new();



        public override sealed IReadOnlyList<TViewConfig> Configs => _configs;



        internal override sealed void AddConfig(ViewConfig instance)
        {
            if (instance is TViewConfig typedInstance)
            {
                _configs.Add(typedInstance);
                typedInstance.SetTargetProvider(_targetProvider);
            }
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