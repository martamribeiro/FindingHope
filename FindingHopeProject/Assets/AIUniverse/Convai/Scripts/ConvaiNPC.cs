using System;
using System.Collections;

using Convai.gRPCAPI;

using UnityEngine;
using UnityEngine.SceneManagement;

using Grpc.Core;
using Service;

using TMPro;

using System.Collections.Generic;

// This script uses gRPC for streaming and is a work in progress
// Edit this script directly to customize your intelligent NPC character

public class ConvaiNPC : MonoBehaviour
{
    // do not edit
    [HideInInspector]
    public List<GetResponseResponse> getResponseResponses = new List<GetResponseResponse>();

    public string sessionID = "-1";

    [HideInInspector]
    public string stringCharacterText = "";

    List<ResponseAudio> ResponseAudios = new List<ResponseAudio>();

    public class ResponseAudio
    {
        public AudioClip audioClip;
        public string audioTranscript;
    };

    ActionConfig actionConfig = new ActionConfig();

    [SerializeField] public string CharacterID;

    private AudioSource audioSource;
    private Animator characterAnimator;
        
    [SerializeField] private TextMeshProUGUI CharacterText;

    bool animationPlaying = false;

    bool playingStopLoop = false;

    [Serializable]
    public class Character
    {
        [SerializeField] public string Name;
        [SerializeField] public string Bio;
    };

    [Serializable]
    public class Object
    {
        [SerializeField] public string Name;
        [SerializeField] public string Description;
    }

    [SerializeField] bool isActionActive;

    [Serializable]
    public class CharacterActionConfig
    {
        [SerializeField] public Character[] Characters;
        [SerializeField] public Object[] Objects;
        [SerializeField] public string[] stringActions;
    }

    [SerializeField] private CharacterActionConfig characterActionConfig;
    
    private Channel channel;
    private ConvaiService.ConvaiServiceClient client;

    private const int AUDIO_SAMPLE_RATE = 44100;
    private const string GRPC_API_ENDPOINT = "stream.convai.com";

    private int recordingFrequency = AUDIO_SAMPLE_RATE;
    private int recordingLength = 30;

    private ConvaiGRPCAPI grpcAPI;

    [SerializeField] public bool isCharacterActive;
    [SerializeField] bool enableTestMode;
    [SerializeField] string testUserQuery;

    private void Awake()
    {
        grpcAPI = FindObjectOfType<ConvaiGRPCAPI>();
        audioSource = GetComponent<AudioSource>();
        characterAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(playAudioInOrder());

        // do not edit
        #region GRPC_SETUP
        SslCredentials credentials = new SslCredentials();

        // The IP Address could be down
        channel = new Channel(GRPC_API_ENDPOINT, credentials);

        client = new ConvaiService.ConvaiServiceClient(channel);
        #endregion

        // do not edit
        #region ACTIONS_SETUP
        foreach (string action in characterActionConfig.stringActions)
        {
            actionConfig.Actions.Add(action);
        }

        foreach (Character character in characterActionConfig.Characters)
        {
            ActionConfig.Types.Character rpcCharacter = new ActionConfig.Types.Character
            {
                Name = character.Name,
                Bio = character.Bio
            };
        }

        foreach (Object eachObject in characterActionConfig.Objects)
        {
            ActionConfig.Types.Object rpcObject = new ActionConfig.Types.Object
            {
                Name = eachObject.Name,
                Description = eachObject.Description
            };
            actionConfig.Objects.Add(rpcObject);
        }
        #endregion

    }

    private void Update()
    {
        // this block starts and stops audio recording and processing
        if (isCharacterActive)
        {
            
            CharacterText.text = stringCharacterText;

            // Start recording when the left control is pressed
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                // Start recording audio
                grpcAPI.StartRecordAudio(client, isActionActive, recordingFrequency, recordingLength, CharacterID, actionConfig, enableTestMode, testUserQuery);
            }

            // Stop recording once left control is released and saves the audio file locally
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                grpcAPI.StopRecordAudio();
                // recordingStopLoop = true;
            }
        }

        if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Equals))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.Equals))
        {
            Application.Quit();
        }

        // if there is some audio in the queue, play it next
        // essentially make a playlist
        if (getResponseResponses.Count > 0)
        {
            ProcessResponseAudio(getResponseResponses[0]);
            getResponseResponses.Remove(getResponseResponses[0]);
        }

        // if any audio is playing, play the talking animation
        if (ResponseAudios.Count > 0)
        {
            if (animationPlaying == false)
            {
                // enable animation according to response
                // try talking first, then base it on the response
                animationPlaying = true;

                characterAnimator.SetBool("Talk", true);
            }
        }
        else
        {
            if (animationPlaying == true)
            {
                // deactivate animations to idle
                animationPlaying = false;

                characterAnimator.SetBool("Talk", false);
            }
        }
    }

    /// <summary>
    /// When the response list has more than one elements, then the audio will be added to a playlist. This function adds individual responses to the list.
    /// </summary>
    /// <param name="getResponseResponse">The getResponseResponse object that will be processed to add the audio and transcript to the playlist</param>
    void ProcessResponseAudio(GetResponseResponse getResponseResponse)
    {
        if (isCharacterActive)
        {
            string tempString = "";

            if (getResponseResponse.AudioResponse.TextData != null)
                tempString = getResponseResponse.AudioResponse.TextData;
            // stringCharacterText = getResponseResponse.AudioResponse.TextData;

            byte[] byteAudio = getResponseResponse.AudioResponse.AudioData.ToByteArray();

            AudioClip clip = grpcAPI.ProcessByteAudioDataToAudioClip(byteAudio, getResponseResponse.AudioResponse.AudioConfig.SampleRateHertz.ToString());

            ResponseAudios.Add(new ResponseAudio
            {
                audioClip = clip,
                audioTranscript = tempString
            });
        }
    }

    /// <summary>
    /// If the playlist is not empty, then it is played.
    /// </summary>
    IEnumerator playAudioInOrder()
    {
        // plays audio as soon as there is audio to play
        while (!playingStopLoop)
        {
            // Debug.Log("Response Audio Clip Count: " + ResponseAudioClips.Count);
            if (ResponseAudios.Count > 0)
            {
                // add animation logic here

                audioSource.PlayOneShot(ResponseAudios[0].audioClip);
                stringCharacterText = ResponseAudios[0].audioTranscript;
                yield return new WaitForSeconds(ResponseAudios[0].audioClip.length);
                ResponseAudios.Remove(ResponseAudios[0]);
            }
            else
                yield return null;
        }
    }

    void OnApplicationQuit()
    {
        playingStopLoop = true;
    }
}
