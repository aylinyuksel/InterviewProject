#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Humelo
{
    [InitializeOnLoad]
    [CustomEditor(typeof(LavstarProsodyTTSGenerator))]
    public class LavstarProsodyTTSGeneratorEditor : Editor
    {
        private LavstarProsodyTTSGenerator _this; ///< As the name implies.

        private string ttsRequestString; ///< As the name implies.

        private string wavGenerationPath; ///< As the name implies.

        private string wavFileName; ///< As the name implies.

        private string apiKeyString; ///< As the name implies.

        private string currVoiceActor; ///< As the name implies.

        private bool showVoices = false; ///< As the name implies.

        private int voiceIndex; ///< As the name implies.

        static LavstarProsodyTTSGeneratorEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += hierarchyItemCB;
        }
        
        //Editor methods

        public void OnEnable()
        {
            _this = target as LavstarProsodyTTSGenerator;

            _this.WavGenerationPath = Application.persistentDataPath;

            _this.WavFileName = "test";
        }

        public void OnDisable()
        {
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override async void OnInspectorGUI()
        {
            GUILayout.Label("Prosody TTS Generation (by Humelo Inc.)", EditorStyles.boldLabel);

            apiKeyString = EditorGUILayout.PasswordField(new GUIContent("API Key String", "put API Key to use Humelo TTS."), _this.apiKeyString);
            if (!apiKeyString.Equals(_this.apiKeyString))
            {
                serializedObject.FindProperty("apiKeyString").stringValue = apiKeyString;
                serializedObject.ApplyModifiedProperties();
            }

            wavGenerationPath = EditorGUILayout.TextField(new GUIContent(".wav Path", "Type text string for requesting Humelo TTS(default: Application.persistentDataPath)."), _this.WavGenerationPath);
            if (!wavGenerationPath.Equals(_this.WavGenerationPath))
            {
                if (wavGenerationPath == string.Empty)
                    wavGenerationPath = Application.persistentDataPath;

                serializedObject.FindProperty("WavGenerationPath").stringValue = wavGenerationPath;
                serializedObject.ApplyModifiedProperties();
            }

            wavFileName = EditorGUILayout.TextField(new GUIContent(".wav Name", "Type text string for name of .wav file(default: test)."), _this.WavFileName);
            if (!wavFileName.Equals(_this.WavFileName))
            {
                serializedObject.FindProperty("WavFileName").stringValue = wavFileName;
                serializedObject.ApplyModifiedProperties();
            }

            currVoiceActor = EditorGUILayout.TextField(new GUIContent("Current Actor", "Select actor type for TTS (default: TTSClient.VoiceActorType.Paul)."), _this.currVoiceActor);
            if (_this.GetAllAvailableVoices().Count > 0 && currVoiceActor != _this.GetAllAvailableVoices()[voiceIndex].Name)
            {
                currVoiceActor = _this.GetAllAvailableVoices()[voiceIndex].Name;

                serializedObject.FindProperty("currVoiceActor").stringValue = currVoiceActor;
                serializedObject.ApplyModifiedProperties();
            }
            
            showVoices = EditorGUILayout.Foldout(showVoices, "Available Voice Actors (" + _this.GetAllAvailableVoices().Count + ")");
            if (showVoices)
            {
                EditorGUI.indentLevel++;

                foreach (var voice in _this.GetAllAvailableVoices())
                {
                    EditorGUILayout.SelectableLabel(voice.Name + "(" + voice.Gender+","+voice.Culture+")", GUILayout.Height(16), GUILayout.ExpandHeight(false));
                }

                EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent("Available Voice Actor List Refresh", "Refresh Actor List")))
            {
                _this.Refresh();
            }

            if(_this.GetAllAvailableVoices().Count > 0)
            {
                List<LavstarVoiceActor> actors = _this.GetAllAvailableVoices();

                string[] ActorName = new string[actors.Count];

                for(int i =0;i< actors.Count; i++)
                {
                    string actorInfo = actors[i].Name + "(" + actors[i].Gender + "," + actors[i].Culture + ")";
                    ActorName[i] = new string(actorInfo.ToCharArray());
                }

                voiceIndex = EditorGUILayout.Popup("Actor", voiceIndex, ActorName);
            }
            

            ttsRequestString = EditorGUILayout.TextField(new GUIContent("Text for TTS synthesize", "Type text string for requesting Humelo TTS(default: empty)."), _this.TTSRequestString);
            if (!ttsRequestString.Equals(_this.TTSRequestString))
            {
                serializedObject.FindProperty("TTSRequestString").stringValue = ttsRequestString;
                serializedObject.ApplyModifiedProperties();
            }
            
            if (GUILayout.Button(new GUIContent("Generate & Playback", "Generates the wav file from the input string and Playback.")))
            {
                _this.Login(_this.apiKeyString);

                string wavFilePath =  wavGenerationPath + "/" + wavFileName + ".wav";

                LavstarVoiceActor voiceActor = new LavstarVoiceActor(currVoiceActor, VoiceActorGender.MALE, "en");
                

                string result = await Task.Run(() => _this.Generate(ttsRequestString, voiceActor, wavFilePath));


                if (result == LavstarProsodyTTSClient.resultSuccess)
                {
                    Debug.Log(wavFilePath);

                    WWW www = new WWW("file:///" + wavFilePath);

                    AudioClip clip = www.GetAudioClip(false, true);

                    Debug.Log("Playback wavFilePath:" + wavFilePath);

                    PlayClip(clip);
                }

                return;
            }

            if (GUILayout.Button(new GUIContent("Playback Stop", "Stop TTS Playback")))
            {
                Debug.Log("Stop Playback");

                StopAllClips();

                return;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
                "PlayClip",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
                null,
                new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );
            method.Invoke(
                null,
                new object[] { clip, startSample, loop }
            );
        }
        
        public static void StopAllClips()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "StopAllClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] { },
                null
            );
            method.Invoke(
                null,
                new object[] { }
            );
        }
        
        private static void hierarchyItemCB(int instanceID, Rect selectionRect)
        {
        }
    }
}
#endif
// © 2019-2020 Humelo Inc.