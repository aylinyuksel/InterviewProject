using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreSummaryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        float score = InterviewResultData.averageScore;
        string feedback = InterviewResultData.feedback;

        scoreText.text = $"Ortalama Puan: {score:F1} / 10";
        feedbackText.text = feedback;

        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
