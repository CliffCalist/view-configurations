using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class IndirectViewConfig<TTarget, TTargetProvider> : ViewConfig<TTarget>
        where TTargetProvider : ScriptableObject
    {
        [HideInInspector]
        [SerializeField] private TTargetProvider _targetProvider;



        public override sealed TTarget Target => _targetProvider == null ? default : GetTarget(_targetProvider);



        internal void SetTargetProvider(TTargetProvider provider)
        {
            _targetProvider = provider;
        }

        protected abstract TTarget GetTarget(TTargetProvider targetProvider);
    }
}