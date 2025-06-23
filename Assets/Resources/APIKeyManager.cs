using System.IO;
using UnityEngine;

[System.Serializable]
public class APIKeys
{
    public string openaiApiKey;
    public string elevenLabsApiKey;
    public string elevenLabsVoiceId;
    public string backendUrl;
}

public static class APIKeyManager
{
    private static APIKeys keys;

    static APIKeyManager()
    {
        LoadKeys();
    }

    private static void LoadKeys()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("apikeys");
        if (jsonFile != null)
        {
            keys = JsonUtility.FromJson<APIKeys>(jsonFile.text);
        }
        else
        {
            Debug.LogError("apikeys.json dosyasý 'Assets/Resources/' klasöründe bulunamadý!");
        }
    }

    public static string GetOpenAIKey()
    {
        if (keys == null) LoadKeys();
        return keys?.openaiApiKey ?? "";
    }

    public static string GetElevenLabsKey()
    {
        if (keys == null) LoadKeys();
        return keys?.elevenLabsApiKey ?? "";
    }

    public static string GetElevenLabsVoiceId()
    {
        if (keys == null) LoadKeys();
        return keys?.elevenLabsVoiceId ?? "";
    }

    public static string GetBackendUrl()
    {
        if (keys == null) LoadKeys();
        return keys?.backendUrl ?? "";
    }
}
