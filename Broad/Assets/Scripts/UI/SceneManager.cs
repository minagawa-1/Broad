using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public float fadeDuration = 0.5f; // �t�F�[�h�̎���

    private CanvasGroup fadeCanvasGroup; // �t�F�[�h�p��CanvasGroup

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else                  Destroy(gameObject);

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

    public void LoadScene(string sceneName) => StartCoroutine(DoLoadScene(sceneName));

    private IEnumerator DoLoadScene(string sceneName)
    {
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.DOFade(1f, fadeDuration); // �t�F�[�h�C���J�n

        yield return new WaitForSeconds(fadeDuration);

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.DOFade(0f, fadeDuration); // �t�F�[�h�A�E�g�J�n

        yield return new WaitForSeconds(fadeDuration);
    }
}
