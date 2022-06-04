using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;

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

            public static GUIContent csharpierPathString = new GUIContent("Csharpier install path");
            public static GUIContent csharpierIgnore = new GUIContent(".csharpierignore");

            public static GUILayoutOption[] csharpierIgnoreOptions = new GUILayoutOption[]
            {
                GUILayout.Height(400),
            };
        }

        public CSharpierSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            m_CSharpierSettings = CSharpierSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            GUILayout.Box(Styles.csharpierExplainer, Styles.csharpierExplainerStyle);
            GUILayout.Space(20.0f);

            EditorGUILayout.PropertyField(
                m_CSharpierSettings.FindProperty("m_CSharpierPath"),
                Styles.csharpierPathString
            );
            EditorGUILayout.PropertyField(
                m_CSharpierSettings.FindProperty("m_CSharpierIgnoreContents"),
                Styles.csharpierIgnore,
                Styles.csharpierIgnoreOptions
            );
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
    }
} // namespace kagekirin.csharpier
