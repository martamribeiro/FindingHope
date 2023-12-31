using System.Collections.Generic;
using System.Reflection;
using Convai.Scripts.Utils;
using UnityEditor;
using UnityEngine;
using Logger = Convai.Scripts.Utils.Logger;

// This assumes your LoggerSettings is in this namespace

namespace Convai.Scripts.Editor
{
    public class LoggerSettingsWindow : EditorWindow
    {
        private const string SETTINGS_PATH = "Assets/Convai/Resources/Settings/LoggerSettings.asset";

        // Add this at the top of your editor window class
        private static readonly Dictionary<string, string> CategoryMapping = new()
        {
            { "Character", "characterResponse" },
            { "LipSync", "lipSync" },
            { "Actions", "actions" }
        };

        private LoggerSettings _settings;

        private void OnEnable()
        {
            // Load the LoggerSettings ScriptableObject

            _settings = AssetDatabase.LoadAssetAtPath<LoggerSettings>(SETTINGS_PATH);


            if (_settings == null)
            {
                CreateLoggerSettings();
                Logger.Warn("LoggerSettings ScriptableObject not found. Creating one...", Logger.LogCategory.Character);
            }
        }


        private void OnGUI()
        {
            // Setting window size
            minSize = new Vector2(850, 250);
            maxSize = minSize;

            if (_settings == null) return;

            EditorGUILayout.Space(20);

            // Create a custom GUIStyle based on EditorStyles.wordWrappedLabel
            GUIStyle customLabelStyle = new(EditorStyles.wordWrappedLabel)
            {
                fontSize = 15,
                normal = { textColor = Color.grey }
            };

            GUILayout.Label(
                "These settings only affect log settings related to the Convai plugin. Changes made here will not affect other parts of your project.",
                customLabelStyle);


            EditorGUILayout.Space(20);

            string[] headers =
                { "Select All", "Category", "Debug", "Info", "Error", "Exception", "Warning" };
            string[] rowNames = { "Character", "LipSync", "Actions" };

            // Header Style
            GUIStyle headerStyle = new(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            // Column headers
            EditorGUILayout.BeginHorizontal();
            foreach (string header in headers) GUILayout.Label(header, headerStyle, GUILayout.Width(95));
            EditorGUILayout.EndHorizontal();

            // Slightly increased spacing between rows
            EditorGUILayout.Space(5);

            // Rows
            foreach (string row in rowNames)
            {
                EditorGUILayout.BeginHorizontal();

                // 'Select All' checkbox for each row
                bool allSelectedForRow = GetAllFlagsForRow(row);
                bool newAllSelectedForRow = EditorGUILayout.Toggle(allSelectedForRow, GUILayout.Width(100));
                if (newAllSelectedForRow != allSelectedForRow) SetAllFlagsForRow(row, newAllSelectedForRow);

                GUILayout.Label(row, GUILayout.Width(100));

                // Individual checkboxes
                foreach (string logType in new[] { "Debug", "Info", "Error", "Exception", "Warning" })
                    RenderAndHandleCheckbox(row, logType);

                EditorGUILayout.EndHorizontal();
            }

            // Increased spacing before global actions
            EditorGUILayout.Space(20);

            // Global actions
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All", GUILayout.Width(150), GUILayout.Height(30))) // Slightly bigger button
                SetAllFlags(true);
            if (GUILayout.Button("Reset All", GUILayout.Width(150), GUILayout.Height(30))) // Slightly bigger button
                SetAllFlags(false);
            EditorGUILayout.EndHorizontal();

            // Additional space at the end for cleaner look
            EditorGUILayout.Space(20);

            if (GUI.changed) EditorUtility.SetDirty(_settings);
        }


        [MenuItem("Convai/Logger Settings")]
        public static void ShowWindow()
        {
            GetWindow<LoggerSettingsWindow>("Logger Settings");
        }


        private void CreateLoggerSettings()
        {
            _settings = CreateInstance<LoggerSettings>();

            // Set default values for Character
            _settings.characterResponseDebug = true;
            _settings.characterResponseInfo = true;
            _settings.characterResponseWarning = true;
            _settings.characterResponseError = true;
            _settings.characterResponseException = true;

            // Set default values for LipSync
            _settings.lipSyncDebug = true;
            _settings.lipSyncInfo = true;
            _settings.lipSyncWarning = true;
            _settings.lipSyncError = true;
            _settings.lipSyncException = true;

            // Set default values for Actions
            _settings.actionsDebug = true;
            _settings.actionsInfo = true;
            _settings.actionsWarning = true;
            _settings.actionsError = true;
            _settings.actionsException = true;

            // Check if the Convai folder exists and create if not
            if (!AssetDatabase.IsValidFolder("Assets/Convai/Resources"))
                AssetDatabase.CreateFolder("Assets/Convai", "Resources");

            // Check if the Settings folder exists and create if not
            if (!AssetDatabase.IsValidFolder("Assets/Convai/Resources/Settings"))
                AssetDatabase.CreateFolder("Assets/Convai/Resources", "Settings");

            AssetDatabase.CreateAsset(_settings, SETTINGS_PATH);
            AssetDatabase.SaveAssets();
        }

        private bool GetAllFlagsForRow(string rowName)
        {
            bool allSelected = true;

            foreach (string logType in new[] { "Debug", "Error", "Exception", "Info", "Warning" })
            {
                string baseFieldName = CategoryMapping.TryGetValue(rowName, out string value) ? value : string.Empty;
                if (string.IsNullOrEmpty(baseFieldName))
                {
                    Debug.LogError($"No mapping found for row {rowName}");
                    return false;
                }

                string fieldName = $"{baseFieldName}{logType}";
                FieldInfo field = _settings.GetType().GetField(fieldName);
                if (field != null)
                {
                    bool currentValue = (bool)field.GetValue(_settings);
                    allSelected &= currentValue;
                }
                else
                {
                    Debug.LogError($"Field {fieldName} does not exist in LoggerSettings");
                    return false;
                }
            }

            return allSelected;
        }


        private void RenderAndHandleCheckbox(string rowName, string logType)
        {
            // Using the mapping to get the base name for the fields
            string baseFieldName = CategoryMapping.TryGetValue(rowName, out string value) ? value : string.Empty;

            if (string.IsNullOrEmpty(baseFieldName))
            {
                Debug.LogError($"No mapping found for row {rowName}");
                return;
            }

            string fieldName = $"{baseFieldName}{logType}";

            FieldInfo field = _settings.GetType().GetField(fieldName);
            if (field != null)
            {
                bool currentValue = (bool)field.GetValue(_settings);
                bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(100));
                if (currentValue != newValue) field.SetValue(_settings, newValue);
            }
            else
            {
                Debug.LogError($"Field {fieldName} does not exist in LoggerSettings");
            }
        }


        private void SetAllFlagsForRow(string rowName, bool value)
        {
            foreach (string logType in new[] { "Debug", "Error", "Exception", "Info", "Warning" })
            {
                string baseFieldName = CategoryMapping.TryGetValue(rowName, out string value1) ? value1 : string.Empty;
                if (string.IsNullOrEmpty(baseFieldName))
                {
                    Debug.LogError($"No mapping found for row {rowName}");
                    return;
                }

                string fieldName = $"{baseFieldName}{logType}";
                FieldInfo field = _settings.GetType().GetField(fieldName);
                if (field != null)
                    field.SetValue(_settings, value);
                else
                    Debug.LogError($"Field {fieldName} does not exist in LoggerSettings");
            }
        }


        private void SetAllFlags(bool value)
        {
            string[] categories = { "characterResponse", "lipSync", "actions" };
            string[] logTypes = { "Debug", "Info", "Error", "Exception", "Warning" };

            foreach (string category in categories)
            foreach (string logType in logTypes)
            {
                string fieldName = $"{category}{logType}";
                FieldInfo field = _settings.GetType().GetField(fieldName);
                if (field != null && field.FieldType == typeof(bool))
                    field.SetValue(_settings, value);
                else
                    Debug.LogWarning($"Field {fieldName} not found or not boolean.");
            }
        }
    }
}