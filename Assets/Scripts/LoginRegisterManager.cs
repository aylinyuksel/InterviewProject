using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LoginRegisterManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Login Alanları")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text loginMessage;

    [Header("Register Alanları")]
    public TMP_InputField nameInput;
    public TMP_InputField surnameInput;
    public TMP_InputField emailInputR;
    public TMP_InputField passwordInputR;
    public TMP_Text registerMessage;

    private string baseUrl;

    private void Start()
    {
        baseUrl = APIKeyManager.GetBackendUrl() + "/api/Users";

        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void OpenRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void OnLoginClick()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginMessage.text = "Lütfen tüm alanları doldurun.";
            return;
        }

        StartCoroutine(LoginRequest(email, password));
    }

    public void OnRegisterClick()
    {
        string name = nameInput.text.Trim();
        string surname = surnameInput.text.Trim();
        string email = emailInputR.text.Trim();
        string password = passwordInputR.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            registerMessage.text = "Lütfen tüm alanları doldurun.";
            return;
        }

        StartCoroutine(RegisterRequest(name, surname, email, password));
    }

    IEnumerator LoginRequest(string email, string password)
    {
        string json = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/login", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                loginMessage.text = "Giriş başarılı.";
                SceneManager.LoadScene("tasarim");
            }
            else
            {
                loginMessage.text = "Giriş başarısız: " + request.downloadHandler.text;
            }
        }
    }

    IEnumerator RegisterRequest(string name, string surname, string email, string password)
    {
        string json = $"{{\"userName\":\"{name}\",\"userSurname\":\"{surname}\",\"email\":\"{email}\",\"password\":\"{password}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/register", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                registerMessage.text = "Kayıt başarılı. Giriş ekranına dönülüyor...";
                Invoke(nameof(OpenLoginPanel), 1.5f);
            }
            else
            {
                registerMessage.text = "Kayıt başarısız: " + request.downloadHandler.text;
            }
        }
    }
}
