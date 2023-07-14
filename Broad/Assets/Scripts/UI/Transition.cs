using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mirror;

public class Transition : MonoBehaviour
{
    public static Transition instance { get; private set; }

    /// <summary>フェードの時間</summary>
    public const float basis_fade_time = 0.5f;

    /// <summary>フェード用のCanvasGroup</summary>
    public CanvasGroup fadeCanvasGroup { get; private set; }

    /// <summary>遷移中か</summary>
    public bool fading { get; private set; }    

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        fadeCanvasGroup = CreateFadeCanvas();

        fading = true;

        // フェードアウト開始
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.DOFade(0f, basis_fade_time).OnComplete(() => fading = false);
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

        // ドロップダウンリストが標準で30000なので、それを上回るsortingOrderを設定
        canvas.sortingOrder = 30001;

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

            panelObject.AddComponent<CanvasRenderer>();

            Image fadePanelImage = panelObject.AddComponent<Image>();
            fadePanelImage.color = Color.black;
            fadePanelImage.raycastTarget = false;
        }

        return panelObject.AddComponent<CanvasGroup>();
    }

    public void LoadScene(string sceneName, NetworkManager networkManager, float fadeInTime = basis_fade_time, float fadeOutTime = basis_fade_time)
        => DOLoadScene(sceneName, networkManager, fadeInTime, fadeOutTime);

    void DOLoadScene(string sceneName, NetworkManager networkManager, float fadeInTime = basis_fade_time, float fadeOutTime = basis_fade_time)
    {
        if (fading) return;

        fading = true;

        // フェードイン開始
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.DOFade(1f, fadeInTime).OnComplete(FadeOut);

        void FadeOut()
        {
            // シーン遷移
            if (NetworkClient.activeHost) networkManager.ServerChangeScene(sceneName);

            // 遷移前のシーンで再生していたDOTweenをリセットする
            DOTween.KillAll();

            fadeCanvasGroup.alpha = 1f;

            // フェードアウト開始
            fadeCanvasGroup.DOFade(0f, fadeOutTime).OnComplete(() => fading = false);
        }
    }
}
