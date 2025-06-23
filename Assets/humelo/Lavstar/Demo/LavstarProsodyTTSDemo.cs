using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Humelo;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Humelo
{
    ///@brief a class for implementing Prosody TTS demo scene.
    public class LavstarProsodyTTSDemo : LavstarTTSDemoBase
    {
        private LavstarVoiceActor currVoiceActor; ///< for saving current voice actor.

        void Start()
        {
            ttsSolution.Login(ttsSolution.apiKeyString);
            
            ttsSolution.VoiceAvailable += RequestVoiceAvailableListHandler;

            ttsSolution.RequestAvailableActorsFromServer();

            base.Start();
        }

        /*!
        * @brief A function for handling RequestVoiceAvailableList.
        * @param[in] obj object.
        * @param[in] e EventArgs.
        * @return void
        */
        async Task RequestVoiceAvailableListHandler(object obj, EventArgs e)
        {
            Debug.Log("RequestVoiceAvailableListHandler called");
            FillVoicesList();
        }

        /*!
        * @brief A function to save voice list.
        * @return void
        */
        private void FillVoicesList()
        {
            List<string> elements = new List<string>();

            List<LavstarVoiceActor> voiceActors = ttsSolution.GetAllAvailableVoices();

            for (int i = 0; i < voiceActors.Count; i++)
                elements.Add(voiceActors[i].Name);
            
            FillVoicesList(elements);
        }

        /*!
        * @brief A function for handling StopButtonOnClick.
        * @return void
        */
        protected override void StopButtonOnClickHandler()
        {
           
        }

        /*!
        * @brief A function for handling PlaybackButtonOnClick.
        * @return void
        */
        protected override void PlaybackButtonOnClickHandler()
        {
            string content = contentInputField.text;

            string filePath = string.Empty;

            filePath = Application.persistentDataPath;

            if (ttsSolution.outputFileName == string.Empty)
                ttsSolution.outputFileName = "test.wav";
            
            string file_full_path = filePath + "/" + ttsSolution.outputFileName + ".wav";

            System.Action<GenerateResponse> handler = null;
            handler = r =>
            {
                statusText.text = "Status: " + r.response;

                ttsSolution.GenerateEvent -= handler;
            };

            ttsSolution.GenerateEvent += handler;

            StartCoroutine(ttsSolution.Synthesize(content, currVoiceActor, file_full_path, audioSource));
        }
        
        /*!
        * @brief A function to handle VoiceGenderSelected dropdown list.
        * @param[in] index index of dropdown item.
        * @return void
        */
        protected override void VoiceGenderSelectedDropdownOnChangedHandler(int index)
        {
            throw new NotImplementedException();
        }

        /*!
        * @brief A function to handle VoiceActorsSelected dropdown list.
        * @param[in] index index of dropdown item.
        * @return void
        */
        protected override void VoiceActorsSelectedDropdownOnChangedHandler(int index)
        {
            currVoiceActor = ttsSolution.GetAllAvailableVoices()[index];
        }

    }
}