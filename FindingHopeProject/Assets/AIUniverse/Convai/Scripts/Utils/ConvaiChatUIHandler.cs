using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Convai.Scripts.Utils
{
    // Enum to configure the type of UI to use
    public enum UIType
    {
        Subtitle,
        QuestionAnswer,
        ChatBox
    }

    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Disable editing
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true; // Enable editing back
        }
    }
#endif

    // Helper class to group properties of a Message
    internal class Message
    {
        public TMP_Text SpeakerName;
        public string Text;
        public TMP_Text TextObject;
    }

    [Serializable]
    public class Character
    {
        [Header("Character settings")] [Tooltip("Display name of the NPC")]
        public string characterName = "Character";

        public string characterText;

        [ColorUsage(true)] [Tooltip("Color of the NPC text. Alpha value will be ignored.")] [SerializeField]
        private Color characterTextColor = Color.white;

        [NonSerialized] public string CharacterTextColorHtml; // new field to store the HTML color

        public Color CharacterTextColor
        {
            get => characterTextColor;
            set
            {
                characterTextColor = value;
                CharacterTextColorHtml =
                    ColorUtility.ToHtmlStringRGB(characterTextColor); // convert once when color is set
            }
        }
    }

    [AddComponentMenu("Convai/Chat UI Controller")]
    [DisallowMultipleComponent]
    [HelpURL(
        "https://docs.convai.com/api-docs/plugins-and-integrations/unity-plugin/scripts-overview/convaichatuihandler.cs")]
    public class ConvaiChatUIHandler : MonoBehaviour
    {
        private const int MAX_MESSAGE = 25; // limit on the number of messages
        private const float FADE_IN_DURATION = 0.5f;
        private const float FADE_OUT_DURATION = 1.5f;

        [Header("Character List")] [Tooltip("List of characters")]
        public List<Character> characters = new();

        [Header("Player settings")] [Tooltip("Display name of the player")]
        public string playerName = "Player";

        [Tooltip("Default text of the player")]
        public string playerText;

        [ColorUsage(true)] [Tooltip("Color of the player's text. Alpha value will be ignored.")]
        public Color playerTextColor = Color.white;

        [Header("UI Components")] [SerializeField] [Tooltip("GameObject which is active when the player is talking")]
        public GameObject playerTalkingMarker;

        [SerializeField] [Tooltip("TextMeshProUGUI component for showing the player's text")]
        public TextMeshProUGUI playerTextField;

        [SerializeField] [Tooltip("TextMeshProUGUI component for showing the character's text")]
        public TextMeshProUGUI characterTextField;

        [Header("UI Settings")] [Tooltip("Type of UI to use")]
        public UIType uIType;

        [Tooltip("Is the chat UI currently visible")] [ReadOnly]
        public bool chatUIActive;

        [SerializeField] [Tooltip("Is the character currently talking")] [ReadOnly]
        private bool isCharacterTalking;

        [SerializeField] [Tooltip("Is the player currently talking")] [ReadOnly]
        private bool isPlayerTalking;

        private readonly List<Message> _messageList = new(); // list to hold messages

        private int _activeCharacterIndex; // Index of the active character
        private GameObject _chatPanel;
        private ScrollRect _chatScrollRect;
        private CanvasGroup _chatUICanvasGroup;
        private GameObject _currentCharacterTextBox;
        private GameObject _currentPlayerTextBox;
        private Speaker _currentSpeaker = Speaker.None;
        private Coroutine _fadeChatUICoroutine;
        private bool _isFirstMessage = true;
        private GameObject _textObject;

        public bool IsCharacterTalking
        {
            get => isCharacterTalking;
            set
            {
                if (value != isCharacterTalking && value)
                {
                    // _currentCharacterTextBox = Instantiate(_textObject, _chatPanel.transform);
                }

                isCharacterTalking = value;
            }
        }

        public bool IsPlayerTalking
        {
            get => isPlayerTalking;
            set
            {
                if (value != isPlayerTalking && value)
                {
                    // _currentPlayerTextBox = Instantiate(_textObject, _chatPanel.transform);
                }

                isPlayerTalking = value;
            }
        }


        // On Awake, find necessary Game Objects based on UI type
        private void Awake()
        {
            switch (uIType)
            {
                case UIType.ChatBox:
                    _chatPanel = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
                    _textObject = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
                    _chatScrollRect = transform.GetChild(0).GetChild(0).GetComponent<ScrollRect>();
                    break;
                case UIType.Subtitle:
                case UIType.QuestionAnswer:
                    // No additional setup needed
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _chatUICanvasGroup = gameObject.GetComponent<CanvasGroup>();
        }

        // On Start, set default values for player and character names if not already set
        private void Start()
        {
            // Checking for characters in the list
            if (characters.Count == 0)
            {
                Character playerCharacter = new()
                {
                    characterName = playerName,
                    CharacterTextColor = playerTextColor // use new property, not private field
                };
                characters.Add(playerCharacter);
            }

            // Ensuring that HTML color is set for each character
            foreach (Character character in characters) character.CharacterTextColor = character.CharacterTextColor;

            ConvaiNPCManager.Instance.OnActiveNPCChanged += HandleActiveNPCChanged;
        }

        private void Update()
        {
            playerTalkingMarker.SetActive(IsPlayerTalking);

            string playerTextHtmlColor = ColorUtility.ToHtmlStringRGB(playerTextColor);
            string characterTextHtmlColor = characters[_activeCharacterIndex].CharacterTextColorHtml;

            switch (uIType)
            {
                case UIType.Subtitle:
                    if (IsCharacterTalking)
                        playerTextField.text =
                            !string.IsNullOrEmpty(characters[_activeCharacterIndex].characterText.Trim())
                                ? $"<b><color=#{characterTextHtmlColor}>{characters[_activeCharacterIndex].characterName}</color></b> : {characters[_activeCharacterIndex].characterText.Trim()}"
                                : "";
                    else
                        playerTextField.text = !string.IsNullOrEmpty(playerText)
                            ? $"<b><color=#{playerTextHtmlColor}>{playerName}</color></b> : {playerText}"
                            : "";
                    break;
                case UIType.QuestionAnswer:
                    playerTextField.text =
                        $"<b><color=#{playerTextHtmlColor}>{playerName}</color></b> : {playerText.Trim()}";
                    characterTextField.text =
                        $"<b><color=#{characterTextHtmlColor}>{characters[_activeCharacterIndex].characterName}</color></b> : {characters[_activeCharacterIndex].characterText.Trim()}";
                    break;
                case UIType.ChatBox:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Additional Exception Handling

        /// <summary>
        ///     Handles the active NPC change event.
        /// </summary>
        private void HandleActiveNPCChanged(ConvaiNPC newActiveNPC)
        {
            if (_fadeChatUICoroutine != null) StopCoroutine(_fadeChatUICoroutine);

            bool fadeIn = newActiveNPC != null;
            float fadeDuration = fadeIn ? FADE_IN_DURATION : FADE_OUT_DURATION;
            _fadeChatUICoroutine = StartCoroutine(FadeChatUI(fadeIn, fadeDuration));

            if (newActiveNPC != null)
            {
                _activeCharacterIndex = characters.FindIndex(c => c.characterName == newActiveNPC.characterName);
                if (_activeCharacterIndex == -1)
                    throw new Exception($"No character found with name {newActiveNPC.characterName}");
            }
        }

        private IEnumerator FadeChatUI(bool fadeIn, float duration)
        {
            if (_chatUICanvasGroup == null)
            {
                Debug.LogError("CanvasGroup is not initialized.");
                yield break;
            }

            float startAlpha = _chatUICanvasGroup.alpha;
            float endAlpha = fadeIn ? 1 : 0;

            float startTime = Time.time;
            while (Time.time <= startTime + duration)
            {
                float normalizedTime = Mathf.Clamp01((Time.time - startTime) / duration);
                _chatUICanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
                yield return null;
            }

            _chatUICanvasGroup.alpha = endAlpha;
            _fadeChatUICoroutine = null;
        }

        /// <summary>
        ///     Process text coming from the character based on the UI type.
        /// </summary>
        public void SendCharacterText(string charName, string text)
        {
            // Get character index
            int characterIndex = characters.FindIndex(c => c.characterName == charName);
            if (characterIndex < 0) throw new Exception($"No character found named {charName}");

            switch (uIType)
            {
                case UIType.Subtitle:
                case UIType.QuestionAnswer:
                    characters[characterIndex].characterText = text;
                    break;
                case UIType.ChatBox:
                    BroadcastCharacterDialogue(charName, text);
                    Canvas.ForceUpdateCanvases();
                    _chatScrollRect.verticalNormalizedPosition = 0.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Process text coming from the player based on the UI type.
        /// </summary>
        public void SendPlayerText(string text)
        {
            switch (uIType)
            {
                case UIType.Subtitle:
                case UIType.QuestionAnswer:
                    playerText = text;
                    break;
                case UIType.ChatBox:
                    BroadcastPlayerDialogue(text);
                    Canvas.ForceUpdateCanvases();
                    _chatScrollRect.verticalNormalizedPosition = 0.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void BroadcastCharacterDialogue(string currentCharacterName, string text)
        {
            string trimmedText = text.Trim();

            if (_currentSpeaker != Speaker.Character || _isFirstMessage)
            {
                _isFirstMessage = false;
                HandleNewCharacterMessage(currentCharacterName, trimmedText);
            }
            else
            {
                AppendToExistingCharacterMessage(trimmedText);
            }

            _currentSpeaker = Speaker.Character;
        }

        private void HandleNewCharacterMessage(string characterName, string text)
        {
            if (_messageList.Count >= MAX_MESSAGE)
            {
                // Remove first message if list size exceeds max count
                Destroy(_messageList[0].TextObject.gameObject);
                _messageList.RemoveAt(0);
            }

            Message newMessage = new()
            {
                Text = text,
                SpeakerName = _textObject.GetComponent<TMP_Text>()
            };

            GameObject newText = _currentCharacterTextBox
                ? _currentCharacterTextBox
                : Instantiate(_textObject, _chatPanel.transform);

            newMessage.TextObject = newText.GetComponent<TMP_Text>();
            newMessage.TextObject.text = FormatCharacterText(characterName, text);

            _messageList.Add(newMessage);
        }

        private void AppendToExistingCharacterMessage(string text)
        {
            // Append new text to existing message
            _messageList[^1].Text += " " + text;
            _messageList[^1].TextObject.text += " " + text;
        }

        private string FormatCharacterText(string characterName, string text)
        {
            return
                $"<b><color=#{characters[_activeCharacterIndex].CharacterTextColorHtml}>{characterName}</color></b>: {text}";
        }

        private void BroadcastPlayerDialogue(string text)
        {
            string trimmedText = text.Trim();

            if (chatUIActive) chatUIActive = false;

            // separate logic for handling new and existing messages
            if (_currentSpeaker != Speaker.Player || _messageList.Count == 0)
                HandleNewPlayerMessage(trimmedText);
            else
                AppendToExistingPlayerMessage(trimmedText);

            // Always update last message if chat UI is not active
            if (!chatUIActive && _messageList.Count > 0) UpdateLastPlayerMessage(trimmedText);

            _currentSpeaker = Speaker.Player;
        } // ReSharper disable Unity.PerformanceAnalysis
        private void HandleNewPlayerMessage(string text)
        {
            if (_messageList.Count >= MAX_MESSAGE)
            {
                // remove first message if list exceeds max count
                Destroy(_messageList[0].TextObject.gameObject);
                _messageList.RemoveAt(0);
            }

            Message newMessage = new()
            {
                Text = text,
                SpeakerName = _textObject.GetComponent<TMP_Text>()
            };

            GameObject newText = Instantiate(_textObject, _chatPanel.transform);
            newMessage.TextObject = newText.GetComponent<TMP_Text>();
            newMessage.TextObject.text = FormatPlayerText(text);

            _messageList.Add(newMessage);
        }

        private void AppendToExistingPlayerMessage(string text)
        {
            // append new text to existing message
            _messageList[^1].Text += " " + text;
            _messageList[^1].TextObject.text += " " + text;
        }

        private void UpdateLastPlayerMessage(string text)
        {
            // update last message text
            _messageList[^1].TextObject.text = FormatPlayerText(text);
        }

        private string FormatPlayerText(string text)
        {
            // format player text with color and player name
            return $"<b><color=#{ColorUtility.ToHtmlStringRGB(playerTextColor)}>{playerName}</color></b>: {text}";
        }

        // Defines who is currently speaking
        private enum Speaker
        {
            None,
            Player,
            Character
        }
    }
}