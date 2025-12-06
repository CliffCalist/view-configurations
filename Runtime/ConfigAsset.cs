using UnityEngine;
using UnityEngine.Serialization;

namespace WhiteArrow.Configurations
{
    public abstract class ConfigAsset : ScriptableObject
    {
        [HideInInspector]
        [SerializeField] private string _contextId;

        [FormerlySerializedAs("_id")]
        [SerializeField] private string _localId;



        public string ContextId
        {
            get => _contextId;
            internal set => _contextId = value;
        }

        public string LocalId => _localId;
        public string Id => $"{_contextId}/{_localId}";
    }
}