using UnityEngine;

namespace WhiteArrow.Configurations
{
    public abstract class ConfigAsset : ScriptableObject
    {
        [SerializeField] private string _id;



        public string Id => _id;
    }
}