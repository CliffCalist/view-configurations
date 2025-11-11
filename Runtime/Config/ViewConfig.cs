using UnityEngine;

namespace WhiteArrow.ViewConfigurations
{
    public abstract class ViewConfig : ScriptableObject
    {
        public abstract object BaseTarget { get; }

        public virtual string AssociatedName
        {
            get
            {
                if (BaseTarget == null)
                    return "Target is null";

                if (BaseTarget is Object unityObject)
                    return unityObject.name;

                return null;
            }
        }
    }

    public abstract class ViewConfig<T> : ViewConfig
    {
        public override sealed object BaseTarget => Target;
        public abstract T Target { get; }
    }
}