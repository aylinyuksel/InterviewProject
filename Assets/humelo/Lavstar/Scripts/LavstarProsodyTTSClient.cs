using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Humelo
{
    ///@brief for implementing to use Prosody TTS
    public class LavstarProsodyTTSClient
    {
        public const string resultFail = "fail"; ///< As the name implies.

        public const string resultSuccess = "success"; ///< As the name implies.

        public const string resultInvalidApikey = "invalidapikey"; ///< As the name implies.

        private string apikey = string.Empty; ///< As the name implies.

        private TTSClientCore ttsCore = null;

        public LavstarProsodyTTSClient()
        {
            ttsCore = GetTTSCore();
        }

        /*!
        * @brief A function for getting TTSClientCore object.
        * @return TTSClientCore
        */
        private TTSClientCore GetTTSCore()
        {
            if (ttsCore == null)
            {
                ttsCore = new TTSClientCore(TTSClientCore.ProsodyServer.Deploy);
            }
            return ttsCore;
        }

        /*!
        * @brief A function to login cloud tts service.
        * @param[in] apikey API key string for TTS Solution. 
        * @return Task<string>
        */
        public async Task<string> Login(string ApiKey)
        {
            if (ApiKey == "")
            {
                return "Please Enter Api Key String";
            }
            else
            {
                SetApiKey(ApiKey);

                return TTSClientCore.resultOK;
            }
        }

        /*!
        * @brief A function to set API Key.
        * @param[in] ApiKey API key string for TTS Solution. 
        * @return void
        */
        public void SetApiKey(string ApiKey)
        {
            apikey = ApiKey;
            GetTTSCore().SetApiKey(ApiKey);
        }

        /*!
        * @brief A function to request available voice actors.
        * @return Task<VoiceActorInfo[]>
        */
        public async Task<VoiceActorInfo[]> RequestVoiceAvailableList()
        {
            if(apikey == string.Empty)
            {
                VoiceActorInfo[] result = new VoiceActorInfo[1];
                result[0] = new VoiceActorInfo(TTSClientCore.resultUnauthorized, 0, TTSClientCore.resultNOK, TTSClientCore.resultNOK,0,null);

                return result;
            }

            GetTTSCore().SetApiKey(apikey);

            return await GetTTSCore().GetVoiceAvailableList();
        }

        /*!
        * @brief A function to synthesize voice file by text.
        * @return Task<string>
        */
        public async Task<string> GenerateWav(string text, LavstarVoiceActor voiceActor, string file_path)
        {
            string dirName = Path.GetDirectoryName(file_path);

            try
            {
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);
            }
            catch (IOException ex)
            {
                Debug.Log(ex.Message);
            }

            string result = await GetTTSCore().Synthesize(file_path, text, voiceActor.Name);

            if (result == Humelo.TTSClientCore.resultOK)
            {
                Debug.Log("Humelo TTS Synthesize Success");
                return resultSuccess;
            }
            else if (result == "unauthorized")
            {
                Debug.Log("Humelo TTS Apikey is not valid. check your apikey");
                return resultInvalidApikey;
            }
            else
            {
                Debug.Log("Humelo TTS Synthesize Fail");
                return resultFail;
            }
        }
    }
}
// © 2019-2020 Humelo Inc.