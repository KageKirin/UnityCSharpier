using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace kagekirin.csharpier
{
    public class CSharpierSettingsProvider : SettingsProvider
    {
        private SerializedObject m_CSharpierSettings;

        class Styles
        {
            /// explainer text
            public static GUIContent csharpierExplainer = new GUIContent(
                "Using CSharpier for Unity requires having both dotnet and the csharpier tool installed."
                    + "\nTo install csharpier, run `dotnet tool install -g csharpier` from the command line."
                    + "\nOnce installed, click the button below to automatically set the correct path to `dotnet-csharpier`."
            );

            /// formatting
            public static GUIStyle csharpierExplainerStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.UpperLeft,
            };

            public static GUIContent csharpierPathString = new GUIContent("CSharpier install path");
            public static GUIContent csharpierSetPathButton = new GUIContent(
                "Set from environment"
            );
            public static GUIContent csharpierIgnore = new GUIContent(".csharpierignore");
            public static GUIContent csharpierIgnoreButton = new GUIContent(
                "Create .csharpierignore"
            );

            public static GUILayoutOption[] csharpierIgnoreOptions = new GUILayoutOption[]
            {
                GUILayout.Height(400),
            };

            public static GUILayoutOption[] csharpierButtonSpaceOptions = new GUILayoutOption[]
            {
                GUILayout.Width(EditorGUIUtility.labelWidth),
                GUILayout.MaxWidth(EditorGUIUtility.labelWidth),
            };
        }

        public CSharpierSettingsProvider(
            string path,
            SettingsScope scopes,
            IEnumerable<string> keywords = null
        ) : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CSharpierSettings = CSharpierSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Box(Styles.csharpierExplainer, Styles.csharpierExplainerStyle);
            GUILayout.Space(20.0f);

            var pathRect = EditorGUILayout.BeginHorizontal();
            var csharpierPath = m_CSharpierSettings.FindProperty("m_CSharpierPath");
            EditorGUILayout.PropertyField(csharpierPath, Styles.csharpierPathString);
            if (GUILayout.Button(Styles.csharpierSetPathButton, Styles.csharpierButtonSpaceOptions))
            {
                var path = CSharpierSettings.LocateCSharpierTool();
                if (!string.IsNullOrEmpty(path))
                {
                    csharpierPath.stringValue = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20.0f);
            var ignoreRect = EditorGUILayout.BeginHorizontal();
            var csharpierignoreText =
                m_CSharpierSettings.FindProperty("m_CSharpierIgnoreContents").stringValue;

            EditorGUILayout.LabelField(Styles.csharpierIgnore, Styles.csharpierButtonSpaceOptions);
            csharpierignoreText = EditorGUILayout.TextArea(
                csharpierignoreText,
                Styles.csharpierIgnoreOptions
            );
            m_CSharpierSettings.FindProperty("m_CSharpierIgnoreContents").stringValue =
                csharpierignoreText;
            EditorGUILayout.EndHorizontal();

            var buttonRect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", Styles.csharpierButtonSpaceOptions);
            if (GUILayout.Button(Styles.csharpierIgnoreButton))
            {
                CSharpierSettings.CreateCSharpierIgnore();
                m_CSharpierSettings = CSharpierSettings.GetSerializedSettings();
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            m_CSharpierSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        [SettingsProvider]
        public static SettingsProvider CreateCSharpierSettingsProvider()
        {
            var provider = new CSharpierSettingsProvider(
                "Project/CSharpier",
                SettingsScope.Project
            );

            // Automatically extract all keywords from the Styles.
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }

        private IDisposable CreateSettingsWindowGUIScope()
        {
            var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var type = unityEditorAssembly.GetType("UnityEditor.SettingsWindow+GUIScope");
            return Activator.CreateInstance(type) as IDisposable;
        }
    }
} // namespace kagekirin.csharpier
