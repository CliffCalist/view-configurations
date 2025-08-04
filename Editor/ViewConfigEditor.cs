using UnityEditor;
using UnityEngine;
using WhiteArrow.SRPConfigurations;

namespace WhiteArrowEditor.SRPConfigurations
{
    [CustomEditor(typeof(ViewConfig), true)]
    public class ViewConfigEditor : Editor
    {
        private Editor _targetEditor;
        private bool _foldout;



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "_target");

            // Draw _target field manually (we want more control over it)
            var targetProp = serializedObject.FindProperty("_target");
            EditorGUILayout.PropertyField(targetProp);

            var targetObject = targetProp.objectReferenceValue as ScriptableObject;

            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("Target (business config) is not assigned.", MessageType.Error);

                if (GUILayout.Button("Create and assign new business config"))
                {
                    var viewConfigType = target.GetType();
                    var genericBase = viewConfigType.BaseType;

                    while (genericBase != null && (!genericBase.IsGenericType || genericBase.GetGenericTypeDefinition() != typeof(ViewConfig<>)))
                        genericBase = genericBase.BaseType;

                    if (genericBase == null)
                    {
                        Debug.LogError("Could not resolve generic type of ViewConfig<>.");
                        return;
                    }

                    var businessConfigType = genericBase.GetGenericArguments()[0];
                    var path = EditorUtility.SaveFilePanelInProject(
                        "Create Business Config",
                        $"New {businessConfigType.Name}",
                        "asset",
                        "Choose location to save the new business config."
                    );

                    if (!string.IsNullOrEmpty(path))
                    {
                        var newConfig = ScriptableObject.CreateInstance(businessConfigType);
                        AssetDatabase.CreateAsset(newConfig, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        targetProp.objectReferenceValue = (UnityEngine.Object)newConfig;
                    }
                }
            }
            else
            {
                _foldout = EditorGUILayout.Foldout(_foldout, "Business Config", true);
                if (_foldout)
                {
                    if (_targetEditor == null || _targetEditor.target != targetObject)
                        _targetEditor = CreateEditor(targetObject);

                    _targetEditor.OnInspectorGUI();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
