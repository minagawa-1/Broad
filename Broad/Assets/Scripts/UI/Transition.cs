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

    public const float basis_fade_time = 0.5f; // フェードの時間

    public woskni.Timer fadeTimer;

    private CanvasGroup fadeCanvasGroup; // フェード用のCanvasGroup

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

        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height); // 仮想の画面サイズ

        canvasObject.AddComponent<GraphicRaycaster>();

        // パネル
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

        // フェードイン
        fadeTimer.Setup(fadeInTime);

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.DOFade(1f, fadeInTime); // フェードイン開始

        await UniTask.WaitUntil(() => IsFadeFinished());

        // フェードアウト
        DOTween.Clear();

        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.DOFade(0f, fadeOutTime); // フェードアウト開始

        // シーン遷移
        if(NetworkClient.activeHost) networkManager.ServerChangeScene(sceneName);
    }

    bool IsFadeFinished()
    {
        fadeTimer.Update();
        return fadeTimer.IsFinished();
    }
}
