using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class DirectViewConfig<TTarget> : ViewConfig<TTarget>
        where TTarget : ScriptableObject
    {
        [SerializeField] private TTarget _target;



        public override sealed TTarget Target => _target;
    }
}