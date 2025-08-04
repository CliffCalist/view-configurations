using UnityEngine;

namespace WhiteArrow.SRPConfigurations
{
    public abstract class ViewConfig : ScriptableObject
    {
        public abstract CoreConfig TargetAsBase { get; }
    }

    public abstract class ViewConfig<T> : ViewConfig
        where T : CoreConfig
    {
        [SerializeField] private T _target;


        public override sealed CoreConfig TargetAsBase => _target;
        public T Target => _target;
    }
}