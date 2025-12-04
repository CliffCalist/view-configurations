using UnityEditor;
using UnityEngine.UIElements;
using WhiteArrow.Configurations;

namespace WhiteArrowEditor.Configurations
{
    [CustomEditor(typeof(ConfigAssetRegistry), true)]
    public class ConfigAssetRegistryEditor : Editor
    {
        private ConfigAssetRegistry _registry;
        private FlexList _list;



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

            if (_list != null)
                EditorApplication.update -= _list.RefreshItemDisplayNames;

            _list = new();
            _list.Label.text = "View Configs";

            _list.PreRefresh += () =>
            {
                var hasRemovedConfigs = _registry.RemoveAllNullConfigs();
                if (hasRemovedConfigs)
                    EditorUtility.SetDirty(_registry);
            };

            _list.SetItemsSource(
                _registry.BaseConfigs,
                new ConfigAssetCreator(_registry),
                item => RemoveConfig(item as ConfigAsset),
                item => RenderConfig(item as ConfigAsset)
            );

            _list.GetItemName = item => GetConfigDisplayName(item as ConfigAsset);

            _list.Refresh();
            EditorApplication.update += _list.RefreshItemDisplayNames;

            root.Add(_list);
            return root;
        }

        private void RemoveConfig(ConfigAsset config)
        {
            Undo.RecordObject(_registry, "Remove view config");

            _registry.RemoveConfig(config);
            AssetDatabase.RemoveObjectFromAsset(config);

            DestroyImmediate(config, true);
            EditorUtility.SetDirty(_registry);
        }

        private string GetConfigDisplayName(ConfigAsset config)
        {
            if (config == null)
                return "Null Config Reference";

            if (string.IsNullOrEmpty(config.Id))
                return "Id not set";

            return config.Id;
        }

        private VisualElement RenderConfig(ConfigAsset config)
        {
            var moduleEditor = CreateEditor(config);
            var editorElement = new IMGUIContainer(() => moduleEditor.OnInspectorGUI());
            return editorElement;
        }
    }
}