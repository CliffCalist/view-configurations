using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using WhiteArrow.ViewConfigurations;

namespace WhiteArrowEditor.ViewConfigurations
{
    [CustomEditor(typeof(IndirectViewConfigRegistry<,,>), true)]
    public class IndirectViewConfigRegistryEditor : ViewConfigRegistryEditor
    {
        private SerializedProperty _targetProviderProperty;



        protected override void OnEnable()
        {
            base.OnEnable();
            _targetProviderProperty = serializedObject.FindProperty("_targetProvider");
        }


        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            CreateTargetProviderField(root);
            RefreshTargetProviderFields();

            root.Add(base.CreateInspectorGUI());

            return root;
        }

        private void CreateTargetProviderField(VisualElement root)
        {
            var targetProviderField = new PropertyField(_targetProviderProperty);
            targetProviderField.RegisterValueChangeCallback(_ =>
            {
                Undo.RecordObject(target, "Change Target Provider");
                RefreshTargetProviderFields();
                EditorUtility.SetDirty(target);
            });
            root.Add(targetProviderField);
        }

        private void RefreshTargetProviderFields()
        {
            var registry = target as ViewConfigRegistry;
            var targetProvider = _targetProviderProperty.objectReferenceValue;

            foreach (var config in registry.Configs)
            {
                var configType = config.GetType();
                var method = configType.GetMethod("SetTargetProvider", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (method != null)
                {
                    method.Invoke(config, new[] { targetProvider });
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}