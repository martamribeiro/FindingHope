#if UNITY_ANDROID
using UnityEngine.Android;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Convai.Scripts.Utils;
using Grpc.Core;
using Service;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Convai.Scripts.Utils.Logger;

// This script uses gRPC for streaming and is a work in progress
// Edit this script directly to customize your intelligent NPC character

namespace Convai.Scripts
{
    [RequireComponent(typeof(Animator), typeof(AudioSource))]
    [AddComponentMenu("Convai/ConvaiNPC")]
    [HelpURL(
        "https://docs.convai.com/api-docs/plugins-and-integrations/unity-plugin/overview-of-the-convainpc.cs-script")]
    public class ConvaiNPC : MonoBehaviour
    {
        private const int AUDIO_SAMPLE_RATE = 44100;
        private const string GRPC_API_ENDPOINT = "stream.convai.com";
        private const int RECORDING_FREQUENCY = AUDIO_SAMPLE_RATE;
        private const int RECORDING_LENGTH = 30;
        private static readonly int Talk = Animator.StringToHash("Talk");

        [Header("Character Information")]
        [Tooltip("Enter the character name for this NPC.")]
        [SerializeField]
        public string characterName;

        [Tooltip("Enter the character ID for this NPC.")]
        [SerializeField]
        public string characterID;

        [Tooltip("The current session ID for the chat with this NPC.")]
        [SerializeField]
        public string sessionID = "-1";

        [Tooltip("Is this character active?")] public bool isCharacterActive;
        [HideInInspector] public ConvaiActionsHandler actionsHandler;
        [HideInInspector] public ConvaiLipSync lipSyncHandler;
        private readonly List<ResponseAudio> _responseAudios = new();
        public readonly Queue<GetResponseResponse> GetResponseResponses = new();
        private ActionConfig _actionConfig;
        private bool _animationPlaying;
        private AudioSource _audioSource;
        private Channel _channel;
        private Animator _characterAnimator;
        private ConvaiChatUIHandler _chatUIHandler;
        private ConvaiService.ConvaiServiceClient _client;
        private ConvaiCrosshairHandler _convaiCrosshairHandler;
        private TMP_InputField _currentInputField;
        private FaceModel _faceModel;
        private ConvaiGRPCAPI _grpcAPI;
        private bool _isActionActive;
        private bool _isCharacterTalking;
        private bool _isLipSyncActive;
        private bool _playingStopLoop;
        [NonSerialized] public bool IncludeActionsHandler, LipSync, HeadEyeTracking, EyeBlinking;

        private void Awake()
        {
            // Find and assign references to various components and handlers using FindObjectOfType

            Logger.Info("Initializing ConvaiNPC component for character: " + characterName,
                Logger.LogCategory.Character);

            // Find and assign the ConvaiChatUIHandler component in the scene
            _chatUIHandler = FindObjectOfType<ConvaiChatUIHandler>();

            // Find and assign the ConvaiCrosshairHandler component in the scene
            _convaiCrosshairHandler = FindObjectOfType<ConvaiCrosshairHandler>();

            // Get the AudioSource component attached to the same GameObject
            _audioSource = GetComponent<AudioSource>();

            // Get the Animator component attached to the same GameObject
            _characterAnimator = GetComponent<Animator>();

            // Check if a ConvaiActionsHandler component is attached to this GameObject
            if (GetComponent<ConvaiActionsHandler>())
            {
                // If present, set the action handling flag to true
                _isActionActive = true;

                // Get the ConvaiActionsHandler component and its action configuration
                actionsHandler = GetComponent<ConvaiActionsHandler>();
                _actionConfig = actionsHandler.ActionConfig;
            }

            // Check if a ConvaiLipSync component is attached to this GameObject
            if (GetComponent<ConvaiLipSync>())
            {
                // If present, set the lip-sync handling flag to true
                _isLipSyncActive = true;

                // If present, get the ConvaiLipSync component
                lipSyncHandler = GetComponent<ConvaiLipSync>();


                _faceModel = lipSyncHandler.faceModelType;
            }

            Logger.Info("ConvaiNPC component initialized", Logger.LogCategory.Character);
        }

        private async void Start()
        {
            // Assign the ConvaiGRPCAPI component in the scene
            _grpcAPI = ConvaiGRPCAPI.Instance;

            // Start the coroutine that plays audio clips in order
            StartCoroutine(PlayAudioInOrder());
            InvokeRepeating(nameof(ProcessResponse), 0f, 1 / 100f);
            StartCoroutine(WatchForInputSubmission());

            // Check if the platform is Android
#if UNITY_ANDROID
            // Check if the user has not authorized microphone permission
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                // Request microphone permission from the user
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif

            // DO NOT EDIT
            // gRPC setup configuration

            #region GRPC_SETUP

            // Create SSL credentials for secure communication
            SslCredentials credentials = new();

            // Initialize a gRPC channel with the specified endpoint and credentials
            _channel = new Channel(GRPC_API_ENDPOINT, credentials);

            // Initialize the gRPC client for the ConvaiService using the channel
            _client = new ConvaiService.ConvaiServiceClient(_channel);

            #endregion

            sessionID = await ConvaiGRPCAPI.InitializeSessionIDAsync(characterName, _client, characterID, sessionID);
        }


        private void Update()
        {
            // Handle text input focus and submission
            if (isCharacterActive)
                HandleTextInput();

            // Handle character-specific actions if the character is active and the input field is not focused
            if (isCharacterActive && (_currentInputField == null || !_currentInputField.isFocused))
                HandleCharacterActions();

            // Handle scene management actions
            HandleSceneManagement();

            // Handle character talking animation
            HandleCharacterTalkingAnimation();
        }

        /// <summary>
        ///     Unity callback that is invoked when the application is quitting.
        ///     Stops the loop that plays audio in order.
        /// </summary>
        private void OnApplicationQuit()
        {
            // Set the flag to stop the loop that plays audio
            _playingStopLoop = true;
        }

        /// <summary>
        ///     Handles user text input and submission.
        /// </summary>
        private void HandleTextInput()
        {
            if (_currentInputField != null && _currentInputField.isFocused)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    HandleInputSubmission(_currentInputField.text);
                    _currentInputField.text = "";
                    _currentInputField.DeactivateInputField();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _currentInputField.text = "";
                    _currentInputField.DeactivateInputField();
                }
                // Prevent further input handling when the input field is focused
            }
        }

        /// <summary>
        ///     Handles basic character-specific actions such as starting and stopping audio recording.
        /// </summary>
        private void HandleCharacterActions()
        {
            // Start recording audio when the T key is pressed
            if (Input.GetKeyDown(KeyCode.T))
            {
                UpdateActionConfig();
                StartListening();
            }
            // Stop recording audio when the T key is released
            else if (Input.GetKeyUp(KeyCode.T))
            {
                StopListening();
            }
        }

        /// <summary>
        ///     Updates the action configuration with the current attention object, ie the object currently being pointed by the
        ///     crosshair
        /// </summary>
        private void UpdateActionConfig()
        {
            if (_actionConfig != null && _convaiCrosshairHandler != null)
                _actionConfig.CurrentAttentionObject = _convaiCrosshairHandler.FindPlayerReferenceObject();
        }


        /// <summary>
        ///     Handles scene management actions such as reloading the scene or quitting the application.
        /// </summary>
        private static void HandleSceneManagement()
        {
            // Reload the scene when 'R' key and '=' key are pressed simultaneously
            if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Equals))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // Quit the application when 'Escape' key and '=' key are pressed simultaneously
            else if (Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.Equals)) Application.Quit();
        }


        /// <summary>
        ///     Handles the character's talking animation based on whether the character is currently talking.
        /// </summary>
        private void HandleCharacterTalkingAnimation()
        {
            if (_isCharacterTalking)
            {
                if (!_animationPlaying)
                {
                    _animationPlaying = true;
                    _characterAnimator.SetBool(Talk, true);
                }
            }
            else if (_animationPlaying)
            {
                _animationPlaying = false;
                _characterAnimator.SetBool(Talk, false);
            }
        }

        /// <summary>
        ///     Watches for input submission in the scene and updates the current input field.
        ///     This coroutine runs indefinitely and should be started only once.
        /// </summary>
        private IEnumerator WatchForInputSubmission()
        {
            while (true)
            {
                TMP_InputField inputFieldInScene = FindActiveInputField();

                if (inputFieldInScene != null && _currentInputField != inputFieldInScene)
                {
                    if (_currentInputField != null) _currentInputField.onSubmit.RemoveAllListeners();

                    _currentInputField = inputFieldInScene;
                    _currentInputField.onSubmit.AddListener(HandleInputSubmission);
                }

                // Wait until the next frame before checking again
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        ///     Finds the active input field in the scene.
        /// </summary>
        /// <returns>The active TMP_InputField if found, otherwise null.</returns>
        private static TMP_InputField FindActiveInputField()
        {
            // Find all TMP_InputField components in the scene
            TMP_InputField[] inputFields = FindObjectsOfType<TMP_InputField>();

            // Return the first active and interactable input field (if any)
            return inputFields.FirstOrDefault(inputField =>
                inputField.gameObject.activeInHierarchy && inputField.interactable);

            // If no active and interactable input field is found, return null
        }

        private void HandleInputSubmission(string input)
        {
            Logger.DebugLog("Sending user text to the server...", Logger.LogCategory.Character);
            _chatUIHandler.SendPlayerText(input);

            // Run SendTextData without awaiting it to avoid blocking the UI thread.
            // Capture the context to return to the main thread after the call.
            SendTextDataAsync(input);

            // Clear the input field text
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            if (FindActiveInputField() != null) FindActiveInputField().text = "";
        }

        /// <summary>
        ///     Sends text data to the server asynchronously.
        /// </summary>
        /// <param name="text">The text to send.</param>
        private async void SendTextDataAsync(string text)
        {
            try
            {
                await ConvaiGRPCAPI.Instance.SendTextData(_client, text, characterID,
                    _isActionActive, _isLipSyncActive, _actionConfig, _faceModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, Logger.LogCategory.Character);
                // Handle the exception, e.g., show a message to the user.
            }
        }

        /// <summary>
        ///     Initializes the session in an asynchronous manner and handles the receiving of results from the server.
        ///     Initiates the audio recording process using the gRPC API.
        /// </summary>
        public async void StartListening()
        {
            // Check if the character is active and should start listening
            // Start recording audio using the ConvaiGRPCAPI  StartRecordAudio method
            // Pass necessary parameters such as client, action activity status, recording details, and character information

            await _grpcAPI.StartRecordAudio(_client, _isActionActive, _isLipSyncActive, RECORDING_FREQUENCY,
                RECORDING_LENGTH,
                characterID, _actionConfig, _faceModel);
        }

        /// <summary>
        ///     Stops the ongoing audio recording process.
        /// </summary>
        public void StopListening()
        {
            // Stop the audio recording process using the ConvaiGRPCAPI StopRecordAudio method
            _grpcAPI.StopRecordAudio();
        }

        /// <summary>
        ///     Processes a response fetched from a character.
        /// </summary>
        /// <remarks>
        ///     1. Processes audio/text/face data from the response and adds it to _responseAudios.
        ///     2. Identifies actions from the response and parses them for execution.
        /// </remarks>
        private void ProcessResponse()
        {
            // Check if the character is active and should process the response
            if (isCharacterActive)
                if (GetResponseResponses.Count > 0)
                {
                    GetResponseResponse getResponseResponse = GetResponseResponses.Dequeue();

                    if (getResponseResponse?.AudioResponse != null)
                    {
                        // Check if text data exists in the response
                        if (getResponseResponse.AudioResponse.AudioData.ToByteArray().Length > 46)
                        {
                            // Initialize empty string for text
                            string textDataString = getResponseResponse.AudioResponse.TextData;

                            byte[] byteAudio = getResponseResponse.AudioResponse.AudioData.ToByteArray();

                            Logger.Info("Clip Size: " + byteAudio.Length, Logger.LogCategory.LipSync);

                            AudioClip clip = _grpcAPI.ProcessByteAudioDataToAudioClip(byteAudio,
                                getResponseResponse.AudioResponse.AudioConfig.SampleRateHertz.ToString());

                            // Add the response audio along with associated data to the list
                            _responseAudios.Add(new ResponseAudio
                            {
                                AudioClip = clip,
                                AudioTranscript = textDataString,
                                isFinal = false
                            });
                        }
                    }
                    else if (getResponseResponse?.DebugLog != null)
                    {
                        _responseAudios.Add(new ResponseAudio
                        {
                            AudioClip = null,
                            AudioTranscript = null,
                            isFinal = true
                        });
                    }
                }
                else
                {
                    _isCharacterTalking = false;
                }
        }

        /// <summary>
        ///     Plays audio clips attached to characters in the order of responses.
        /// </summary>
        /// <returns>
        ///     A IEnumerator that can facilitate coroutine functionality
        /// </returns>
        /// <remarks>
        ///     1. Starts a loop that plays audio from response, and performs corresponding actions and animations.
        ///     2. Loop continues until the application quits.
        /// </remarks>
        private IEnumerator PlayAudioInOrder()
        {
            while (!_playingStopLoop)
                // Check if there are audio clips in the playlist
                if (_responseAudios.Count > 0)
                {
                    ResponseAudio currentResponseAudio = _responseAudios[0];

                    // Set the current audio clip to play

                    if (!currentResponseAudio.isFinal)
                    {
                        _audioSource.clip = currentResponseAudio.AudioClip;

                        _audioSource.Play();
                        _isCharacterTalking = true;

                        if (lipSyncHandler != null)
                            lipSyncHandler.isCharacterTalking = true;

                        if (_chatUIHandler != null) _chatUIHandler.IsCharacterTalking = true;

                        if (!string.IsNullOrEmpty(currentResponseAudio.AudioTranscript))
                            _chatUIHandler.SendCharacterText(characterName, currentResponseAudio.AudioTranscript.Trim());

                        yield return new WaitForSeconds(currentResponseAudio.AudioClip.length);

                        _audioSource.Stop();
                        _audioSource.clip = null;

                        if (_chatUIHandler != null) _chatUIHandler.IsCharacterTalking = false;
                    }
                    else
                    {
                    }

                    _responseAudios.RemoveAt(0);
                }
                else
                {
                    yield return null;
                }
        }

        private class ResponseAudio
        {
            public AudioClip AudioClip;
            public string AudioTranscript;
            public bool isFinal;
        }
    }
}