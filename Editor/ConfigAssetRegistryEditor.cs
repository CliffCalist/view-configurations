using System.Linq;
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
            });
            root.Add(defaultInspector);


            if (_list != null)
                EditorApplication.update -= _list.RefreshItemDisplayNames;

            _list = new();
            _list.Label.text = "View Configs";

            _list.SetItemsSource(
                _registry.BaseConfigs,
                new ConfigAssetCreator(_registry),
                item => RemoveConfig(item as ConfigAsset),
                item => RenderConfig(item as ConfigAsset)
            );

            _list.PreRefresh += () =>
            {
                var hasRemovedConfigs = _registry.RemoveAllNullConfigs();
                if (hasRemovedConfigs)
                    EditorUtility.SetDirty(_registry);
            };

            _list.GetItemName = item => GetConfigDisplayName(item as ConfigAsset);

            _list.AddCustomHeaderElement(CreateDropZone());

            _list.Refresh();
            EditorApplication.update += _list.RefreshItemDisplayNames;

            root.Add(_list);
            return root;
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



        private void RemoveConfig(ConfigAsset config)
        {
            Undo.RecordObject(_registry, "Remove view config");

            _registry.RemoveConfig(config);
            AssetDatabase.RemoveObjectFromAsset(config);

            DestroyImmediate(config, true);
            EditorUtility.SetDirty(_registry);
        }



        private VisualElement CreateDropZone()
        {
            var dropZone = CreateStyledDropZoneBox();
            var label = CreateDropZoneLabel();
            dropZone.Add(label);
            RegisterDropZoneEvents(dropZone);
            return dropZone;
        }

        private Box CreateStyledDropZoneBox()
        {
            var dropZone = new Box();

            dropZone.style.paddingLeft = 2;
            dropZone.style.paddingRight = 2;

            dropZone.style.borderLeftWidth = 1;
            dropZone.style.borderRightWidth = 1;
            dropZone.style.borderTopWidth = 1;
            dropZone.style.borderBottomWidth = 1;

            var borderColor = new Color(0.1882353F, 0.1882353F, 0.1882353F, 0.1882353F);
            dropZone.style.borderLeftColor = borderColor;
            dropZone.style.borderRightColor = borderColor;
            dropZone.style.borderTopColor = borderColor;
            dropZone.style.borderBottomColor = new Color(0.1411765F, 0.1411765F, 0.1411765F, 0.1411765F);

            dropZone.style.borderTopLeftRadius = 3;
            dropZone.style.borderTopRightRadius = 3;
            dropZone.style.borderBottomLeftRadius = 3;
            dropZone.style.borderBottomRightRadius = 3;

            dropZone.style.alignItems = Align.Center;
            dropZone.style.justifyContent = Justify.Center;
            dropZone.style.flexDirection = FlexDirection.Row;

            return dropZone;
        }

        private Label CreateDropZoneLabel()
        {
            var label = new Label("Import area");
            label.style.flexGrow = 1;
            return label;
        }

        private void RegisterDropZoneEvents(Box dropZone)
        {
            dropZone.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = IsValidDrop(DragAndDrop.objectReferences)
                    ? DragAndDropVisualMode.Copy
                    : DragAndDropVisualMode.Rejected;
            });

            dropZone.RegisterCallback<DragPerformEvent>(evt =>
            {
                if (!IsValidDrop(DragAndDrop.objectReferences)) return;

                Undo.RecordObject(_registry, "Import ConfigAssets");
                bool changed = false;

                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (obj is not ConfigAsset config)
                        continue;

                    if (_registry.HasConfig(config))
                        continue;

                    var path = AssetDatabase.GetAssetPath(config);
                    if (string.IsNullOrEmpty(path))
                        continue;

                    var copy = Instantiate(config);
                    copy.name = config.name;
                    copy.hideFlags = HideFlags.HideInHierarchy;

                    _registry.AddConfig(copy);
                    AssetDatabase.AddObjectToAsset(copy, _registry);
                    AssetDatabase.MoveAssetToTrash(path);

                    changed = true;
                }

                if (changed)
                {
                    EditorUtility.SetDirty(_registry);
                    AssetDatabase.Refresh();
                    _list.Refresh();
                }

                DragAndDrop.AcceptDrag();
            });
        }

        private bool IsValidDrop(Object[] objects)
        {
            return objects.All(obj =>
                obj is ConfigAsset config &&
                !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(config)));
        }
    }
}