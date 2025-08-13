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
                questionText.text = "Sorular y�klenemedi: " + request.error;
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
            responseText.text = "L�tfen bir cevap girin.";
            return;
        }

        allAnswers.Add(userAnswer);

        string prompt =
           "Sen bir teknik m�lakat de�erlendiricisisin. Yaz�l�m alan�nda deneyimli bir uzman olarak, aday�n yaln�zca teknik bilgisini de�erlendiriyorsun.\n\n" +
           $"A�a��da aday�n bir m�lakat sorusuna verdi�i cevap yer al�yor:\n" +
           $"Soru: {questions[currentQuestionIndex]}\n" +
           $"Cevap: {userAnswer}\n\n" +
           "L�tfen �u kriterlere g�re de�erlendir:\n" +
           "- Cevap do�ruysa: K�saca neden do�ru oldu�unu teknik olarak belirt.\n" +
           "- Cevap eksikse: Hangi y�nlerin eksik oldu�unu yaz.\n" +
           "- Cevap yanl��sa: Hatal� noktalar� belirt ve do�ru cevab� �zetle.\n" +
           "- E�er aday cevab�nda 'bilmiyorum', 'emin de�ilim', 'yard�mc� olur musun' gibi ifadeler kulland�ysa veya cevap konuyla ilgisizse, 0�2 aras�nda d���k bir puan ver.\n\n" +
           "Cevab�n� en fazla 2 k�sa c�mleyle a��kla. Sonuna yaln�zca �u formatta puan ver: [Puan:X] (�rnek: [Puan:1])\n" +
           "Puanlama 0 (�ok yetersiz) ile 10 (m�kemmel) aras�nda olmal� ve yaln�zca teknik i�erik dikkate al�narak adil yap�lmal�d�r.\n" +
           "**L�tfen sohbet etme veya motive edici mesaj yazma. Sadece teknik de�erlendirme ve puan ver.**";

        responseText.text = "De�erlendiriliyor...";
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
                "Sen teknik m�lakat yapan deneyimli bir de�erlendiricisin. Aday�n teknik bilgi seviyesi a�a��daki cevaplara ve ortalama puana g�re de�erlendirilecektir.\n\n" +
                $"Ortalama Puan: {average:F1} / 10\n" +
                "A�a��da aday�n sorulara verdi�i cevaplar yer al�yor:\n" +
                string.Join("\n", allAnswers.Select((a, i) => $"{i + 1}. {a}")) + "\n\n" +
                "Bu verilere dayanarak aday�n teknik yetkinli�i hakk�nda k�sa ve nesnel bir de�erlendirme yaz, ve hangi konulara �al��mas� gerekti�ini yaz. Maksimum 3 c�mle olmal�. Sohbet etme, sadece de�erlendirme yap.";

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
