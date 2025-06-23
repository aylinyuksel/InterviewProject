using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class ElevenLabsTTS : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    private string apiKey;
    private string voiceId;

    private void Start()
    {
        // API key ve voiceId artık dışarıdan alınır
        apiKey = APIKeyManager.GetElevenLabsKey();
        voiceId = APIKeyManager.GetElevenLabsVoiceId();
    }

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(voiceId))
        {
            Debug.LogError("ElevenLabs API key veya Voice ID eksik.");
            return;
        }

        StartCoroutine(SendTTSRequest(text));
    }

    IEnumerator SendTTSRequest(string text)
    {
        string url = $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}?optimize_streaming_latency=0";

        string json = "{\"text\": \"" + EscapeJson(text) + "\", \"voice_settings\": {\"stability\": 0.5, \"similarity_boost\": 0.75}}";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        byte[] postData = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("TTS Hata: " + request.error);
        }
    }

    private string EscapeJson(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
