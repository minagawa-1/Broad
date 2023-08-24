using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>フェードアウト後の最大明度</summary>
    public float minAlpha => woskni.Range.O1.Lerp(1f - Config.data.brightness, 0.2f, 1f);

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        Config.Load();
        SaveSystem.Load();
        Localization.Setup();
        Localization.Correct();
    }

    private void Start()
    {
        fadeCanvasGroup = CreateFadeCanvas();

        fading = true;

        // フェードアウト開始
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.DOFade(minAlpha, basis_fade_time).OnComplete(() => fading = false);
    }

    private CanvasGroup CreateFadeCanvas()
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(transform);
        canvasObject.layer = (int)Layer.UI;

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
        fadeCanvasGroup.alpha = minAlpha;
        fadeCanvasGroup.DOFade(1f, fadeInTime).OnComplete(FadeOut);

        void FadeOut()
        {
            // シーン遷移
            if (NetworkClient.activeHost) networkManager.ServerChangeScene(sceneName);

            Localization.Setup();
            Localization.Correct();

            // 遷移前のシーンで再生していたDOTweenをリセットする
            DOTween.KillAll();

            fadeCanvasGroup.alpha = 1f;

            // フェードアウト開始
            fadeCanvasGroup.DOFade(minAlpha, fadeOutTime).OnComplete(() => fading = false);
        }
    }

    /// <summary>画面のフェードアウト後の最大明度を設定</summary>
    /// <param name="brightness">最大明度 (0.0 to 1.0)</param>
    public void SetMaxBrightness()
    {
        if (fading) return;

        fadeCanvasGroup.alpha = minAlpha;
    }
}
