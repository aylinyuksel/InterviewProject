using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Humelo
{
    public class LavstarProsodyTTSGenerator : LavstarProsodyTTS
    {
        [Tooltip("User Apikey for Humelo Prosody TTS (default: empty).")]
        public string TTSRequestString = string.Empty; ///< As the name implies.

        public string WavGenerationPath = string.Empty; ///< As the name implies.

        public string WavFileName = string.Empty; ///< As the name implies.

        public string currVoiceActor; ///< current voice actor.

        void Start()
        {
            DontDestroyOnLoad(this);
            
            VoiceAvailable += RequestVoiceAvailableListHandler;

            Refresh();
        }

        /*!
        * @brief A function for refresh with API key.
        * @return void
        */
        public void Refresh()
        {
            Login(apiKeyString);

            allAvailableVoices.Clear();

            RequestAvailableActorsFromServer();
        }

        /*!
        * @brief A function for RequestVoiceAvailableList
        * @param[in] obj object 
        * @param[in] e EventArgs 
        * @return void
        */
        async Task RequestVoiceAvailableListHandler(object obj, EventArgs e)
        {
            Debug.Log("RequestVoiceAvailableListHandler called");

            if (allAvailableVoices.Count != 0)
                currVoiceActor = allAvailableVoices[0].Name;

        }
    }
}
// © 2019-2020 Humelo Inc.