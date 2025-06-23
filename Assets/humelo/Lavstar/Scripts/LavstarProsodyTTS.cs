using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Xml;
using System.Net;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Humelo
{
    ///@brief for implementing to use Prosody TTS
    public class LavstarProsodyTTS : LavstarTTSSolutionBase
    {
        private LavstarProsodyTTSClient ttsClient = null; ///< Prosody TTS Client instance

        private AudioSource tgtAudioSource = null; ///< As the name implies.

        public LavstarProsodyTTS()
        {
            ttsClient = GetTTSClient();
        }

        /*!
        * @brief A function to login cloud tts service.
        * @param[in] apikey API key string for TTS Solution (AWS Polly: Cognito Crendential, Google Cloud TTS: API Key, Prosody TTS: API Key) 
        * @return void
        */
        public override async void Login(string apikey)
        {
            await GetTTSClient().Login(apikey);
        }

        /*!
        * @brief A function for getting LavstarProsodyTTSClient object.
        * @return LavstarProsodyTTSClient
        */
        private LavstarProsodyTTSClient GetTTSClient()
        {
            if (ttsClient == null)
                ttsClient = new LavstarProsodyTTSClient();

            return ttsClient;
        }

        /*!
        * @brief A function to get available actor voices.
        * @return List all available voices
        */
        public override System.Collections.Generic.List<LavstarVoiceActor> GetAllAvailableVoices()
        {
            return allAvailableVoices;
        }

        /*!
        * @brief A function for RequestAvailableActorsFromServer.
        * @return void
        */
        public async override void RequestAvailableActorsFromServer()
        {
            VoiceActorInfo[] result = await GetTTSClient().RequestVoiceAvailableList();

            await OnRequestVoiceAvailableList(result);
        }

        /*!
        * @brief A function for synthesize TTS.
        * @param[in] text text string for TTS.
        * @param[in] actor voice actor.
        * @param[in] outputFile for local wav file save.
        * @param[in] audioSource for playback.
        * @return IEnumerator for coroutine execution.
        */
        public override IEnumerator Synthesize(string text, LavstarVoiceActor actor, string outputFile, AudioSource audioSource)
        {
            if (CheckinternetReachability() == true)
            {
                bool isWorking = true;
                
                System.Action<GenerateResponse> handler = null;
                handler = r =>
                {
                    isWorking = false;
                    
                    GenerateEvent -= handler;
                };

                GenerateEvent += handler;

                Generate(text, actor, outputFile, audioSource);

                do
                {
                    yield return null;
                } while (isWorking);
            }
            else
            {
                GenerateEventCall(TTSSTATUS.CHECKINTERNET);
            }
        }

        /*!
        * @brief A function for synthesize TTS.
        * @param[in] text text string for TTS.
        * @param[in] actor voice actor.
        * @param[in] file_path for local wav file save.
        * @param[in] playbackAudioTgt for playback.
        * @return Task<string>
        */
        public async Task<string> Generate(string text, LavstarVoiceActor voiceActor, string file_path, AudioSource playbackAudioTgt = null)
        {
            string result = await GetTTSClient().GenerateWav(text, voiceActor, file_path);

            if(result == LavstarProsodyTTSClient.resultSuccess)
            {
                if (playbackAudioTgt != null)
                {
                    tgtAudioSource = playbackAudioTgt;
                    
                    UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + file_path, AudioType.WAV);
                    UnityWebRequestAsyncOperation op = request.SendWebRequest();

                    op.completed += RequestCompleted;
                }
            }

            GenerateEventCall(new GenerateResponse(result));

            return result;
        }

        /*!
        * @brief A function for playback target audio source when the tts result is success.
        * @param[in] op AsyncOperation
        * @return void
        */
        void RequestCompleted(AsyncOperation op)
        {
            UnityWebRequestAsyncOperation uop = (UnityWebRequestAsyncOperation)op;
            UnityWebRequest request = uop.webRequest;

            if (request.isNetworkError)
            {
                Debug.LogWarning(request.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

                tgtAudioSource.Stop();
                tgtAudioSource.clip = clip;

                tgtAudioSource.Play();

            }
        }
    }
}
// © 2019-2020 Humelo Inc.