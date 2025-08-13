using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class InterviewManager : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_InputField answerField;
    [SerializeField] private TMP_Text responseText;
    [SerializeField] private ChatGPTConnector chatGPT;
    [SerializeField] private ElevenLabsTTS elevenLabs;
    [SerializeField] private GameObject nextButton;

    private List<string> questions = new List<string>();
    private List<int> scores = new List<int>();
    private List<string> allAnswers = new List<string>();
    private int currentQuestionIndex = 0;
    private string backendUrl;

    private void Start()
    {
        backendUrl = APIKeyManager.GetBackendUrl();
        StartCoroutine(FetchQuestions());

        if (chatGPT != null)
            chatGPT.OnResponseReceived += OnChatGPTResponse;

        if (nextButton != null)
            nextButton.SetActive(false);
    }

    private IEnumerator FetchQuestions()
    {
        string url = backendUrl.TrimEnd('/') + "/api/questions";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = request.downloadHandler.text;
                var questionList = JsonConvert.DeserializeObject<List<QuestionDto>>(json);

                var shuffled = questionList.OrderBy(q => UnityEngine.Random.value).Take(4).ToList();

                for (int i = 0; i < shuffled.Count; i++)
                {
                    questions.Add($"{i + 1}. {shuffled[i].questionText}");
                }

                ShowQuestion();
            }
            else
            {
                questionText.text = "Sorular yüklenemedi: " + request.error;
            }
        }
    }

    private void ShowQuestion()
    {
        questionText.text = questions[currentQuestionIndex];
        if (elevenLabs != null)
            elevenLabs.Speak(questions[currentQuestionIndex]);
    }

    public void SubmitAnswer()
    {
        string userAnswer = answerField.text.Trim();

        if (string.IsNullOrWhiteSpace(userAnswer))
        {
            responseText.text = "Lütfen bir cevap girin.";
            return;
        }

        allAnswers.Add(userAnswer);

        string prompt =
           "Sen bir teknik mülakat deðerlendiricisisin. Yazýlým alanýnda deneyimli bir uzman olarak, adayýn yalnýzca teknik bilgisini deðerlendiriyorsun.\n\n" +
           $"Aþaðýda adayýn bir mülakat sorusuna verdiði cevap yer alýyor:\n" +
           $"Soru: {questions[currentQuestionIndex]}\n" +
           $"Cevap: {userAnswer}\n\n" +
           "Lütfen þu kriterlere göre deðerlendir:\n" +
           "- Cevap doðruysa: Kýsaca neden doðru olduðunu teknik olarak belirt.\n" +
           "- Cevap eksikse: Hangi yönlerin eksik olduðunu yaz.\n" +
           "- Cevap yanlýþsa: Hatalý noktalarý belirt ve doðru cevabý özetle.\n" +
           "- Eðer aday cevabýnda 'bilmiyorum', 'emin deðilim', 'yardýmcý olur musun' gibi ifadeler kullandýysa veya cevap konuyla ilgisizse, 0–2 arasýnda düþük bir puan ver.\n\n" +
           "Cevabýný en fazla 2 kýsa cümleyle açýkla. Sonuna yalnýzca þu formatta puan ver: [Puan:X] (örnek: [Puan:1])\n" +
           "Puanlama 0 (çok yetersiz) ile 10 (mükemmel) arasýnda olmalý ve yalnýzca teknik içerik dikkate alýnarak adil yapýlmalýdýr.\n" +
           "**Lütfen sohbet etme veya motive edici mesaj yazma. Sadece teknik deðerlendirme ve puan ver.**";

        responseText.text = "Deðerlendiriliyor...";
        chatGPT.SendMessageToChatGPT(prompt);
    }

    private void OnChatGPTResponse(string reply, int score)
    {
        responseText.text = reply;
        if (score >= 0) scores.Add(score);
        nextButton.SetActive(true);
    }

    public void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            answerField.text = "";
            responseText.text = "";
            nextButton.SetActive(false);
            ShowQuestion();
        }
        else
        {
            float average = scores.Count > 0 ? (float)scores.Average() : 0f;
            InterviewResultData.averageScore = average;

            string summaryPrompt =
                "Sen teknik mülakat yapan deneyimli bir deðerlendiricisin. Adayýn teknik bilgi seviyesi aþaðýdaki cevaplara ve ortalama puana göre deðerlendirilecektir.\n\n" +
                $"Ortalama Puan: {average:F1} / 10\n" +
                "Aþaðýda adayýn sorulara verdiði cevaplar yer alýyor:\n" +
                string.Join("\n", allAnswers.Select((a, i) => $"{i + 1}. {a}")) + "\n\n" +
                "Bu verilere dayanarak adayýn teknik yetkinliði hakkýnda kýsa ve nesnel bir deðerlendirme yaz, ve hangi konulara çalýþmasý gerektiðini yaz. Maksimum 3 cümle olmalý. Sohbet etme, sadece deðerlendirme yap.";

            StartCoroutine(GetFinalFeedback(summaryPrompt));
        }
    }

    private IEnumerator GetFinalFeedback(string prompt)
    {
        bool isDone = false;
        string feedbackResult = "";

        void OnFinalResponse(string content, int score)
        {
            feedbackResult = content;
            isDone = true;
        }

        chatGPT.OnResponseReceived += OnFinalResponse;
        chatGPT.SendMessageToChatGPT(prompt);

        yield return new WaitUntil(() => isDone);
        chatGPT.OnResponseReceived -= OnFinalResponse;

        InterviewResultData.feedback = feedbackResult;
        UnityEngine.SceneManagement.SceneManager.LoadScene("ScoreScene");
    }

    private void OnDestroy()
    {
        if (chatGPT != null)
            chatGPT.OnResponseReceived -= OnChatGPTResponse;
    }

    [System.Serializable]
    public class QuestionDto
    {
        public int id;
        public string questionText;
    }
}
