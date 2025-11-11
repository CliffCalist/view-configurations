using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteArrow.ViewConfigurations;

namespace WhiteArrowEditor.ViewConfigurations
{
    [CustomEditor(typeof(ViewConfigRegistry), true)]
    public class ViewConfigRegistryEditor : Editor
    {
        protected ViewConfigRegistry _registry;
        protected FlexList _list;



        protected virtual void OnEnable()
        {
            _registry = (ViewConfigRegistry)target;
        }

        private void OnDisable()
        {
            EditorApplication.update -= _list.RefreshItemDisplayNames;
        }



        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            _list = new();
            _list.Label.text = "View Configs";

            _list.PreRefresh += () =>
            {
                var hasRemovedConfigs = _registry.RemoveAllNullConfigs();
                if (hasRemovedConfigs)
                    EditorUtility.SetDirty(_registry);
            };

            _list.SetItemsSource(
                _registry.Configs,
                CreateConfig,
                item => RemoveConfig(item as ViewConfig),
                item => RenderConfig(item as ViewConfig)
            );

            _list.GetItemName = item => GetConfigDisplayName(item as ViewConfig);

            _list.Refresh();
            EditorApplication.update += _list.RefreshItemDisplayNames;

            root.Add(_list);

            return root;
        }

        private void CreateConfig()
        {
            var instance = CreateInstance(_registry.ConfigType) as ViewConfig;
            if (instance == null)
                return;

            Undo.RecordObject(_registry, "Add element");

            instance.hideFlags = HideFlags.HideInHierarchy;
            _registry.AddConfig(instance);

            AssetDatabase.AddObjectToAsset(instance, _registry);
            EditorUtility.SetDirty(_registry);
        }

        private void RemoveConfig(ViewConfig config)
        {
            Undo.RecordObject(_registry, "Remove view config");

            _registry.RemoveConfig(config);
            AssetDatabase.RemoveObjectFromAsset(config);

            DestroyImmediate(config, true);
            EditorUtility.SetDirty(_registry);
        }

        private string GetConfigDisplayName(ViewConfig config)
        {
            return config?.AssociatedName;
        }

        private VisualElement RenderConfig(ViewConfig config)
        {
            var moduleEditor = CreateEditor(config);
            var editorElement = new IMGUIContainer(() => moduleEditor.OnInspectorGUI());
            return editorElement;
        }
    }
}