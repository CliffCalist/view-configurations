using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class ViewConfig : ScriptableObject
    {
        public abstract object TargetRaw { get; }

        public virtual string AssociatedName
        {
            get
            {
                if (TargetRaw == null)
                    return "Target is null";

                if (TargetRaw is Object unityObject)
                    return unityObject.name;

                return null;
            }
        }
    }

    public abstract class ViewConfig<T> : ViewConfig
    {
        public override sealed object TargetRaw => Target;
        public abstract T Target { get; }
    }
}