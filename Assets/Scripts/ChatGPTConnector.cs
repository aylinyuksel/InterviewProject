using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class ChatGPTConnector : MonoBehaviour
{
    [Header("UI Referanslarý")]
    [SerializeField] private TMP_InputField answerField;
    [SerializeField] private Button submitButton;

    public event Action<string, int> OnResponseReceived;

    private bool isRequestInProgress = false;
    private string apiKey;

    private void Start()
    {
        apiKey = APIKeyManager.GetOpenAIKey(); // API key’i dýþardan al
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSendButtonClick);
    }

    private void OnSendButtonClick()
    {
        if (isRequestInProgress) return;

        string userMessage = answerField?.text;
        if (!string.IsNullOrWhiteSpace(userMessage))
        {
            submitButton.interactable = false;
            SendMessageToChatGPT(userMessage);
        }
        else
        {
            Debug.LogWarning("Cevap alaný boþ!");
        }
    }

    public void SendMessageToChatGPT(string userMessage)
    {
        StartCoroutine(SendRequest(userMessage));
    }

    private IEnumerator SendRequest(string message)
    {
        isRequestInProgress = true;

        string url = "https://api.openai.com/v1/chat/completions";
        string escapedMessage = EscapeJson(message);

        string json = $"{{\"model\":\"gpt-3.5-turbo\",\"messages\":[{{\"role\":\"system\",\"content\":\"Sen bir teknik mülakat deðerlendiricisisin. Adayýn sadece teknik bilgisini deðerlendiriyorsun. Sohbet etme. Motive edici, kiþisel veya destekleyici cümleler yazma. Aþaðýdaki kullanýcý cevabýný teknik doðruluðu açýsýndan deðerlendir. 1 veya 2 kýsa cümleyle açýklama yap. Cevabýn sonunda tek bir [Puan:X] formatýnda, 0 (çok kötü) ile 10 (çok iyi) arasýnda bir puan ver. 'Puan:X' formatý dýþýnda hiçbir þey yazma. Eðer adayýn cevabý 'bilmiyorum', 'emin deðilim' gibi içerikler içeriyorsa puaný 0 ver. Örneðin: 'Cevap teknik olarak eksik ve yüzeyseldir. [Puan:2]'\"}},{{\"role\":\"user\",\"content\":\"{escapedMessage}\"}}]}}";

        byte[] postData = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            submitButton.interactable = true;
            isRequestInProgress = false;

            if (request.result == UnityWebRequest.Result.Success)
            {
                string result = request.downloadHandler.text;
                string content = ExtractContent(result);
                int score = ExtractScore(content);

                Debug.Log($"ChatGPT Cevap: {content} | Puan: {score}");
                OnResponseReceived?.Invoke(content, score);
            }
            else
            {
                Debug.LogError("ChatGPT Hata: " + request.error);
                OnResponseReceived?.Invoke("Bir hata oluþtu: " + request.error, -1);
            }
        }
    }

    private string ExtractContent(string json)
    {
        var match = Regex.Match(json, "\"content\":\\s*\"(.*?)\"", RegexOptions.Singleline);
        if (match.Success)
        {
            string content = match.Groups[1].Value;
            return content.Replace("\\n", "\n").Replace("\\\"", "\"");
        }
        return "Yanýt ayrýþtýrýlamadý.";
    }

    private int ExtractScore(string content)
    {
        var match = Regex.Match(content, @"\[Puan:\s*(\d+)\]");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int score))
            return Mathf.Clamp(score, 0, 10);
        return -1;
    }

    private string EscapeJson(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "")
            .Replace("\t", "\\t");
    }
}
