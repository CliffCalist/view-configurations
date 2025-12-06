using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteArrow.Configurations;

namespace WhiteArrowEditor.Configurations
{
    [CustomEditor(typeof(ConfigAssetRegistry), true)]
    public class ConfigAssetRegistryEditor : Editor
    {
        private ConfigAssetRegistry _registry;
        private FlexList _list;



        public Type ConfigType => _registry.ConfigType;



        private void OnEnable()
        {
            _registry = (ConfigAssetRegistry)target;
        }

        private void OnDisable()
        {
            if (_list != null)
                EditorApplication.update -= _list.RefreshItemDisplayNames;
        }



        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var defaultInspector = new IMGUIContainer(() =>
            {
                DrawPropertiesExcluding(serializedObject, "m_Script", "_configs");
                var previousId = _registry.Id;
                serializedObject.ApplyModifiedProperties();

                if (_registry.Id != previousId)
                {
                    foreach (var config in _registry.BaseConfigs)
                    {
                        if (config != null && config.ContextId != _registry.Id)
                        {
                            config.ContextId = _registry.Id;
                            EditorUtility.SetDirty(config);
                        }
                    }
                }
            });
            root.Add(defaultInspector);


            if (_list != null)
                EditorApplication.update -= _list.RefreshItemDisplayNames;

            _list = new();
            _list.Label.text = "View Configs";

            _list.SetItemsSource(
                _registry.BaseConfigs,
                new ConfigAssetCreator(this),
                item => DestroyConfig(item as ConfigAsset),
                item => RenderConfig(item as ConfigAsset)
            );

            _list.PreRefresh += () =>
            {
                var hasRemovedConfigs = _registry.RemoveAllNullConfigs();
                if (hasRemovedConfigs)
                    EditorUtility.SetDirty(_registry);
            };

            _list.GetItemName = item => GetConfigDisplayName(item as ConfigAsset);

            _list.AddCustomHeaderElement(new ImportConfigsDropZone(this, _list));

            _list.Refresh();
            EditorApplication.update += _list.RefreshItemDisplayNames;

            root.Add(_list);
            return root;
        }



        private string GetConfigDisplayName(ConfigAsset config)
        {
            if (config == null)
                return "Null Config Reference";

            if (string.IsNullOrEmpty(config.LocalId))
                return "Id not set";

            return config.LocalId;
        }

        private VisualElement RenderConfig(ConfigAsset config)
        {
            var moduleEditor = CreateEditor(config);
            var editorElement = new IMGUIContainer(() => moduleEditor.OnInspectorGUI());
            return editorElement;
        }



        public void DestroyConfig(ConfigAsset config)
        {
            Undo.RecordObject(_registry, "Destroy view config");
            RemoveConfig(config);
            DestroyImmediate(config, true);
            EditorUtility.SetDirty(_registry);
        }

        public void RemoveConfig(ConfigAsset config)
        {
            Undo.RecordObject(_registry, "Remove view config");
            _registry.RemoveConfig(config);
            AssetDatabase.RemoveObjectFromAsset(config);
            EditorUtility.SetDirty(_registry);

            config.ContextId = string.Empty;
            config.hideFlags = HideFlags.None;
            EditorUtility.SetDirty(config);
        }

        public void CreateConfig(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!typeof(ConfigAsset).IsAssignableFrom(type))
                throw new ArgumentException($"Type {type} is not assignable from {nameof(ConfigAsset)}");

            var instance = CreateInstance(type) as ConfigAsset;
            if (instance == null)
                return;

            Undo.RecordObject(_registry, "Create config");
            AddConfig(instance);
        }

        public void AddConfig(ConfigAsset config)
        {
            Undo.RecordObject(_registry, "Add config");
            config.ContextId = _registry.Id;
            config.hideFlags = HideFlags.HideInHierarchy;
            _registry.AddConfig(config);
            AssetDatabase.AddObjectToAsset(config, _registry);
            EditorUtility.SetDirty(_registry);
        }
    }
}