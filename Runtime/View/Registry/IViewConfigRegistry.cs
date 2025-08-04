namespace WhiteArrow.SRPConfigurations
{
    public interface IViewConfigRegistry
    {
        TView GetViewFor<TView>(CoreConfig config) where TView : ViewConfig;
        bool TryGetViewFor<TView>(CoreConfig config, out TView viewConfig) where TView : ViewConfig;
    }
}