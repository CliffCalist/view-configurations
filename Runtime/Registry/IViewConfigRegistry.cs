using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public interface IViewConfigRegistry
    {
        TView GetViewFor<TView>(ScriptableObject config) where TView : ViewConfig;
        bool TryGetViewFor<TView>(ScriptableObject config, out TView viewConfig) where TView : ViewConfig;
    }
}