#if UNITY_EDITOR

using System;
using System.IO;
using Convai.Scripts.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Convai.Scripts.Utils.Logger;
using Object = UnityEngine.Object;

namespace Convai.Scripts.Editor
{
    /// <summary>
    ///     Custom editor for the ConvaiNPC component.
    ///     Provides functionalities to cache and restore states of all convai scripts whenever a scene is saved.
    /// </summary>
    [CustomEditor(typeof(ConvaiNPC))]
    [HelpURL("https://docs.convai.com/api-docs/plugins-and-integrations/unity-plugin/scripts-overview")]
    public class ConvaiNPCEditor : UnityEditor.Editor
    {
        private ConvaiChatUIHandler _chatUIHandler;
        private ConvaiNPC _convaiNPC;

        private bool _showSettings; // Flag to denote if settings pane should be shown

        private void OnEnable()
        {
            _convaiNPC = (ConvaiNPC)target;

            if (_convaiNPC == null)
            {
                Logger.Error("ConvaiNPC component is not attached to a GameObject", Logger.LogCategory.Character);
                return;
            }

            // Find the UIHandler script in the scene
            _chatUIHandler = FindObjectOfType<ConvaiChatUIHandler>();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // Begin a horizontal group
            EditorGUILayout.BeginHorizontal();

            // GuiLayout Button with tooltip for adding components settings pane
            GUIContent addComponentsContent =
                new("Add Components", "Open settings pane for additional component selection");
            if (GUILayout.Button(addComponentsContent, GUILayout.Width(120)))
                try
                {
                    ConvaiNPCComponentSettingsWindow.Open(_convaiNPC);

                    // If the UIHandler script is found, add the character to its character list
                    if (_chatUIHandler != null)
                    {
                        // Check if the character is already in the list to avoid duplicates
                        bool characterExists = false;
                        foreach (Character characterInfo in _chatUIHandler.characters)
                            if (characterInfo.characterName == _convaiNPC.characterName)
                            {
                                characterExists = true;
                                break;
                            }

                        // If the character is not in the list, add it
                        if (!characterExists)
                        {
                            Character newCharacter = new()
                            {
                                characterName = _convaiNPC.characterName,
                                CharacterTextColor = Color.red, // Set the default color
                                characterText = "" // Set the default text
                            };
                            _chatUIHandler.characters.Add(newCharacter);
                            EditorUtility.SetDirty(_chatUIHandler); // Mark the UIHandler script as dirty
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Unexpected error occurred when applying changes. Error: {ex}");
                }

            // Add Space
            GUILayout.Space(50);

            // // GuiLayout Button with tooltip
            // GUIContent loggerSettingsContent = new("Open Logger Settings", "Open the Logger settings window");
            // if (GUILayout.Button(loggerSettingsContent, GUILayout.Width(150)))
            //     LoggerSettingsWindow.ShowWindow(); // This opens the LoggerSettings

            // End the horizontal group for logger settings button
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    ///     Utility class to save the state of Convai scripts.
    /// </summary>
    public abstract class StateSaver
    {
        public const string ROOT_DIRECTORY = "Assets/Convai/Settings/Script State/";

        /// <summary>
        ///     Save the state of all Convai scripts in the current scene.
        /// </summary>
        [MenuItem("Convai/Save Script State", false, 4)]
        public static void SaveScriptState()
        {
            Scene scene = SceneManager.GetActiveScene();
            ConvaiNPC[] convaiObjects = Object.FindObjectsOfType<ConvaiNPC>();

            foreach (ConvaiNPC convaiNPC in convaiObjects)
            {
                Debug.Log($"Saving state for character: {convaiNPC.characterName}");
                MonoBehaviour[] scripts = convaiNPC.GetComponentsInChildren<MonoBehaviour>();

                string characterFolder = Path.Combine(ROOT_DIRECTORY, convaiNPC.characterID);
                if (!Directory.Exists(characterFolder)) Directory.CreateDirectory(characterFolder);

                foreach (MonoBehaviour script in scripts)
                {
                    string fullName = script.GetType().FullName;
                    if (fullName != null && !fullName.StartsWith("Convai.Scripts")) continue;

                    string assetPath = script.GetSavePath(characterFolder, scene.name, convaiNPC.characterID);
                    File.WriteAllText(assetPath, JsonUtility.ToJson(script));
                }
            }

            AssetDatabase.Refresh();
        }

        [InitializeOnLoad]
        public class SaveSceneHook
        {
            static SaveSceneHook()
            {
                EditorSceneManager.sceneSaved += SceneSaved;
            }

            private static void SceneSaved(Scene scene)
            {
                SaveScriptState();
            }
        }
    }

    // Extension methods to reduce repeated code and improve readability
    public static class EditorExtensions
    {
        public static void SaveStateToFile<T>(this T component, string path) where T : Component
        {
            string serializedComponentData = JsonUtility.ToJson(component);
            File.WriteAllText(path, serializedComponentData);
        }

        public static void RestoreStateFromFile<T>(this T component, string path) where T : Component
        {
            try
            {
                string savedData = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(savedData, component);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to restore component data for {typeof(T).Name}, Error: {ex}");
            }
        }

        public static T AddComponentSafe<T>(this GameObject go) where T : Component
        {
            try
            {
                return go.AddComponent<T>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add component of type {typeof(T).Name}, Error: {ex}");
                return null;
            }
        }

        public static string GetSavePath(this MonoBehaviour script, string characterFolder, string sceneName,
            string characterID)
        {
            return Path.Combine(characterFolder, $"{sceneName}_{characterID}_{script.GetType().FullName}_State.data");
        }
    }
}

#endif