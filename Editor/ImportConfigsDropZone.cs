using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WhiteArrow.Configurations;

namespace WhiteArrowEditor.Configurations
{
    public class ImportConfigsDropZone : Box
    {
        private readonly ConfigAssetRegistryEditor _registryEditor;
        private readonly FlexList _list;



        public ImportConfigsDropZone(ConfigAssetRegistryEditor registryEditor, FlexList list)
        {
            _registryEditor = registryEditor;
            _list = list;

            CreateStyle();
            CreateLabel();
            RegisterCallbacks();
        }

        private void CreateStyle()
        {
            style.paddingLeft = 2;
            style.paddingRight = 2;

            style.borderLeftWidth = 1;
            style.borderRightWidth = 1;
            style.borderTopWidth = 1;
            style.borderBottomWidth = 1;

            var borderColor = new Color(0.1882353F, 0.1882353F, 0.1882353F, 1f);
            style.borderLeftColor = borderColor;
            style.borderRightColor = borderColor;
            style.borderTopColor = borderColor;
            style.borderBottomColor = borderColor;

            style.borderTopLeftRadius = 3;
            style.borderTopRightRadius = 3;
            style.borderBottomLeftRadius = 3;
            style.borderBottomRightRadius = 3;

            style.alignItems = Align.Center;
            style.justifyContent = Justify.Center;
            style.flexDirection = FlexDirection.Row;
        }

        private void CreateLabel()
        {
            var label = new Label("Import area");
            label.style.flexGrow = 1;
            Add(label);
        }

        private void RegisterCallbacks()
        {
            RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = IsValidDrop(DragAndDrop.objectReferences)
                    ? DragAndDropVisualMode.Copy
                    : DragAndDropVisualMode.Rejected;
            });

            RegisterCallback<DragPerformEvent>(evt =>
            {
                if (!IsValidDrop(DragAndDrop.objectReferences))
                    return;

                var changed = false;

                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (obj is not ConfigAsset config)
                        continue;

                    var path = AssetDatabase.GetAssetPath(config);
                    if (string.IsNullOrEmpty(path))
                        continue;

                    var copy = Object.Instantiate(config);
                    copy.name = config.name;
                    _registryEditor.AddConfig(copy);
                    AssetDatabase.MoveAssetToTrash(path);

                    changed = true;
                }

                if (changed)
                {
                    AssetDatabase.Refresh();
                    _list?.Refresh();
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