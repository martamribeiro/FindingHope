using System.IO;
using Convai.Scripts.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Convai.Scripts.Editor
{
    public class ConvaiNPCComponentSettingsWindow : EditorWindow
    {
        private ConvaiNPC _convaiNPC;

        private void OnGUI()
        {
            titleContent = new GUIContent("Convai NPC Components");
            Vector2 windowSize = new(300, 150);
            minSize = windowSize;
            maxSize = windowSize;
            if (_convaiNPC == null)
            {
                EditorGUILayout.LabelField("No ConvaiNPC selected");
                return;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUIUtility.labelWidth = 200f; // Set a custom label width

            _convaiNPC.IncludeActionsHandler = EditorGUILayout.Toggle(
                new GUIContent("NPC Actions", "Decides if Actions Handler is included"),
                _convaiNPC.IncludeActionsHandler);
            _convaiNPC.LipSync = EditorGUILayout.Toggle(new GUIContent("Lip Sync", "Decides if Lip Sync is enabled"),
                _convaiNPC.LipSync);
            _convaiNPC.HeadEyeTracking = EditorGUILayout.Toggle(
                new GUIContent("Head & Eye Tracking", "Decides if Head & Eye tracking is enabled"),
                _convaiNPC.HeadEyeTracking);
            _convaiNPC.EyeBlinking =
                EditorGUILayout.Toggle(new GUIContent("Eye Blinking", "Decides if Eye Blinking is enabled"),
                    _convaiNPC.EyeBlinking);

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            if (GUILayout.Button("Apply Changes", GUILayout.Height(40)))
            {
                ApplyChanges();
                EditorUtility.SetDirty(_convaiNPC);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Close();
            }
        }

        public static void Open(ConvaiNPC convaiNPC)
        {
            ConvaiNPCComponentSettingsWindow window = GetWindow<ConvaiNPCComponentSettingsWindow>();
            window.titleContent = new GUIContent("Convai NPC Component Settings");
            window._convaiNPC = convaiNPC;
            window.Show();
        }

        /// <summary>
        ///     Applies changes based on the user's selection in the inspector.
        /// </summary>
        private void ApplyChanges()
        {
            if (EditorUtility.DisplayDialog("Confirm Apply Changes",
                    "Do you want to apply the following changes?", "Yes", "No"))
            {
                ApplyComponent<ConvaiActionsHandler>(_convaiNPC.IncludeActionsHandler);
                ApplyComponent<ConvaiLipSync>(_convaiNPC.LipSync);
                ApplyComponent<ConvaiHeadTracking>(_convaiNPC.HeadEyeTracking);
                ApplyComponent<ConvaiBlinkingHandler>(_convaiNPC.EyeBlinking);
            }
        }

        /// <summary>
        ///     Applies or removes a component based on the specified condition.
        ///     If the component is to be removed, its state is saved. If it's added, its state is restored if previously saved.
        /// </summary>
        private void ApplyComponent<T>(bool includeComponent) where T : Component
        {
            T component = _convaiNPC.GetComponent<T>();

            // Define the path for saving/restoring component state
            string savedDataFileName = Path.Combine(StateSaver.ROOT_DIRECTORY, _convaiNPC.characterID,
                $"{SceneManager.GetActiveScene().name}_{_convaiNPC.characterID}_{typeof(T).FullName}_State.data");

            if (includeComponent)
            {
                if (component == null)
                {
                    component = _convaiNPC.gameObject.AddComponentSafe<T>();

                    // If saved data exists for this component, restore it
                    if (File.Exists(savedDataFileName))
                        component.RestoreStateFromFile(savedDataFileName);
                }
            }
            else if (component != null)
            {
                component.SaveStateToFile(savedDataFileName);
                DestroyImmediate(component);
            }
        }
    }
}