using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class ViewConfig : ScriptableObject
    {
        public abstract ScriptableObject TargetAsBase { get; }
    }

    public abstract class ViewConfig<T> : ViewConfig
        where T : ScriptableObject
    {
        [SerializeField] private T _target;


        public override sealed ScriptableObject TargetAsBase => _target;
        public T Target => _target;
    }
}