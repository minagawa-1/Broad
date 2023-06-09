using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mirror;

public class Transition : MonoBehaviour
{
    public static Transition Instance { get; private set; }

    public const float basis_fade_time = 0.5f; // �t�F�[�h�̎���

    public woskni.Timer fadeTimer;

    private CanvasGroup fadeCanvasGroup; // �t�F�[�h�p��CanvasGroup

    private void Awake()
    {
        fadeTimer.Setup(basis_fade_time);

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        fadeCanvasGroup = CreateFadeCanvas();

        fadeCanvasGroup.alpha = 0f;
    }

    private CanvasGroup CreateFadeCanvas()
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(transform);
        canvasObject.layer = LayerMask.NameToLayer(Layer.UI);

        RectTransform fadeCanvasRectTransform = canvasObject.AddComponent<RectTransform>();
        fadeCanvasRectTransform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        fadeCanvasRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 255;

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height); // ���z�̉�ʃT�C�Y

        canvasObject.AddComponent<GraphicRaycaster>();

        // �p�l��
        GameObject panelObject = new GameObject("FadePanel");
        {
            panelObject.transform.SetParent(canvasObject.transform);

            RectTransform fadePanelRectTransform = panelObject.AddComponent<RectTransform>();
            fadePanelRectTransform.position = new Vector2(Screen.width / 2, Screen.height / 2);
            fadePanelRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            CanvasRenderer fadePanelCanvasRenderer = panelObject.AddComponent<CanvasRenderer>();

            Image fadePanelImage = panelObject.AddComponent<Image>();
            fadePanelImage.color = Color.black;
            fadePanelImage.raycastTarget = false;
        }

        return panelObject.AddComponent<CanvasGroup>();
    }

    public async void LoadScene(string sceneName, NetworkManager networkManager, float fadeInTime = basis_fade_time, float fadeOutTime = basis_fade_time)
        => await DoLoadScene(sceneName, networkManager, fadeInTime, fadeOutTime);

    async UniTask DoLoadScene(string sceneName, NetworkManager networkManager, float fadeInTime = basis_fade_time, float fadeOutTime = basis_fade_time)
    {
        if (fadeTimer.IsStarted()) return;

        // �t�F�[�h�C��
        fadeTimer.Setup(fadeInTime);

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.DOFade(1f, fadeInTime); // �t�F�[�h�C���J�n

        await UniTask.WaitUntil(() => IsFadeFinished());

        // �t�F�[�h�A�E�g
        DOTween.Clear();

        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.DOFade(0f, fadeOutTime); // �t�F�[�h�A�E�g�J�n

        //// �z�X�g���V�[���J�ڂ�����������܂őҋ@
        //await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name != Scene.GameMainScene || NetworkClient.activeHost);

        // �V�[���J��
        if(NetworkClient.activeHost) networkManager.ServerChangeScene(sceneName);
    }

    bool IsFadeFinished()
    {
        fadeTimer.Update();
        return fadeTimer.IsFinished();
    }
}
