using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WhiteArrow.Configurations;

namespace WhiteArrowEditor.Configurations
{
    internal class ConfigAssetCreator : IFlexItemCreator
    {
        private readonly ConfigAssetRegistry _registry;



        public ConfigAssetCreator(ConfigAssetRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
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
                CreateAndSetConfig(configTypes[0]);
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
                        CreateAndSetConfig(type);
                        onComplete(true);
                    }
                );
            }

            menu.ShowAsContext();
        }

        private void CreateAndSetConfig(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!typeof(ConfigAsset).IsAssignableFrom(type))
                throw new ArgumentException($"Type {type} is not assignable from {nameof(ConfigAsset)}");

            var instance = ScriptableObject.CreateInstance(type) as ConfigAsset;
            if (instance == null)
                return;

            Undo.RecordObject(_registry, "Add element");

            instance.hideFlags = HideFlags.HideInHierarchy;
            _registry.AddConfig(instance);

            AssetDatabase.AddObjectToAsset(instance, _registry);
            EditorUtility.SetDirty(_registry);
        }

        private List<Type> GetDerivedConfigTypes()
        {
            var baseType = _registry.ConfigType;
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