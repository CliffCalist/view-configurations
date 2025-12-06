using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WhiteArrow.Configurations;

namespace WhiteArrowEditor.Configurations
{
    internal class ConfigAssetCreator : IFlexItemCreator
    {
        private readonly ConfigAssetRegistryEditor _registryEditor;



        public ConfigAssetCreator(ConfigAssetRegistryEditor registry)
        {
            _registryEditor = registry ?? throw new ArgumentNullException(nameof(registry));
        }



        public void RequestCreate(Action<bool> onComplete)
        {
            var configTypes = GetDerivedConfigTypes();

            if (configTypes.Count == 0)
            {
                EditorUtility.DisplayDialog("No Configs", "All available configs are already added.", "OK");
                return;
            }

            if (configTypes.Count == 1)
            {
                _registryEditor.CreateConfig(configTypes[0]);
                onComplete(true);
            }
            else ShowGenericMenu(configTypes, onComplete);
        }

        private void ShowGenericMenu(List<Type> configTypes, Action<bool> onComplete)
        {
            var menu = new GenericMenu();
            foreach (var type in configTypes)
            {
                menu.AddItem(
                    new GUIContent(type.Name), false, () =>
                    {
                        _registryEditor.CreateConfig(type);
                        onComplete(true);
                    }
                );
            }

            menu.ShowAsContext();
        }

        private List<Type> GetDerivedConfigTypes()
        {
            var baseType = _registryEditor.ConfigType;
            var types = TypeCache.GetTypesDerivedFrom(baseType);
            var validTypes = new List<Type>();

            foreach (var type in types)
            {
                if (!type.IsAbstract && typeof(ConfigAsset).IsAssignableFrom(type))
                    validTypes.Add(type);
            }

            if (!baseType.IsAbstract && typeof(ConfigAsset).IsAssignableFrom(baseType))
                validTypes.Insert(0, baseType);

            return validTypes;
        }
    }
}