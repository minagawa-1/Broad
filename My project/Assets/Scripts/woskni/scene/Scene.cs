using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace woskni
{
    public enum Scenes
    {
          Trademark
        , Title
        , GameMain
        , Result
    }

    public class Scene : MonoBehaviour
    {
        // public
        public static GameObject    m_ScreenOut;           // 画面塗料の画像オブジェクト
        public static Timer         m_ChangeTimer;         // シーン遷移タイマー
        public static Scenes        m_CurrentScene;        // 現在のシーン
        public static Scenes        m_LastScene;           // 前回のシーン

        // private
        private static float m_FadeInTime;              // フェードインに要する時間
        private static float m_FadeOutTime;             // フェードアウトに要する時間
        private static Color m_FadeColor;               // フェード時の色

        private static RectTransform m_RectTransform;
        private static Image m_Image;

        private static Canvas m_MainCanvas;

        // シーン遷移の状態
        private enum SceneState
        {
              FadeIn
            , FadeOut
            , ChangeScene
            , Wait
        }
        private static SceneState m_SceneState;

        // 次のシーン番号
        private static Scenes m_NextScene;

        private void Awake()
        {
            // シーンをまたいでも消えないようにする
            DontDestroyOnLoad(gameObject);
            CreateMainCanvas();
            CreateScreenOut();

            // フェードアウト状態にする
            m_SceneState = SceneState.FadeOut;
            m_Image.color = m_FadeColor = Color.black;

            m_ChangeTimer.Setup(1f);
        }

        /// <summary></summary>
        /// <param name="canvasName"></param>
        /// <param name="renderMode"></param>
        void CreateMainCanvas(string canvasName = "MainCanvas", RenderMode renderMode = RenderMode.ScreenSpaceOverlay, int sortingOrder = 1)
        {
            // 既に存在する場合はエラーを吐いて終了
            if (m_MainCanvas != null) {
                Debug.LogError("既にメインキャンバスが存在しています。");
                return;
            }

            // キャンバスオブジェクトを生成
            GameObject obj = new GameObject(canvasName);

            // シーン共通にする
            DontDestroyOnLoad(obj);

            // Canvasコンポーネントをアタッチし、レンダーモードを設定
            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = renderMode;
            canvas.sortingOrder = sortingOrder;

            // キャンバスに必要な情報もアタッチする
            obj.AddComponent<CanvasScaler>();
            obj.AddComponent<GraphicRaycaster>();

            m_MainCanvas = canvas;
        }

        private void Update()
        {
            // 状態ごとに処理を変える
            switch (m_SceneState)
            {
                case SceneState.FadeIn: Fade(true); break;
                case SceneState.FadeOut: Fade(false); break;
                case SceneState.ChangeScene: SceneChange(); break;
                case SceneState.Wait: break;
            }
        }

        private void Fade(bool isFadeIn)
        {
            m_ChangeTimer.Update(false);

            float startRate = System.Convert.ToInt32(!isFadeIn);

            // イージングで透明度を変える
            float rate = Easing.Linear(m_ChangeTimer.time, m_ChangeTimer.limit, startRate, 1f - startRate);
            m_Image.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, rate);

            if (m_ChangeTimer.IsFinished())
            {
                m_ChangeTimer.Setup(m_FadeOutTime);

                // 色をイージング最終色に変える
                m_Image.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, System.Convert.ToInt32(isFadeIn));

                // フェードイン　からの遷移ならChangeScene
                // フェードアウトからの遷移ならWait        に状態を変える
                m_SceneState = isFadeIn ? SceneState.ChangeScene : SceneState.Wait;
            }
        }

        private void SceneChange()
        {
            Time.timeScale = 1f;

            m_LastScene     = m_CurrentScene;
            m_CurrentScene  = m_NextScene;

            SceneManager.LoadScene((int)m_NextScene);

            // フェードアウト状態にする
            m_Image.color = m_FadeColor;

            m_SceneState = SceneState.FadeOut;
        }

        /// <summary>
        /// シーン遷移
        /// </summary>
        /// <param name="toSceneNum">遷移先のシーン番号</param>
        /// <param name="fadeInTime">フェードイン時間</param>
        /// <param name="fadeOutTime">フェードアウト時間</param>
        /// <param name="color">フェード色</param>
        public static void Change(Scenes toScene, float fadeInTime = 1f, float fadeOutTime = 1f, Color? color = null)
        {
            // フェード中なら取りやめ
            if (IsFading()) return;

            m_FadeInTime = fadeInTime;
            m_FadeOutTime = fadeOutTime;
            m_FadeColor = color ?? Color.black;

            // もし塗りつぶしオブジェクトはnullなら再生成
            if (m_ScreenOut == null) CreateScreenOut();

            // 塗りつぶしオブジェクトの色を設定
            m_Image.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 0f);

            // フェードイン状態にする
            m_NextScene = toScene;
            m_SceneState = SceneState.FadeIn;
            m_ChangeTimer.Setup(m_FadeInTime);
        }

        public static void Push(Scenes toScene)
        {
            SceneManager.LoadScene(toScene.ToString(), LoadSceneMode.Additive);

            m_CurrentScene = toScene;

            m_ChangeTimer.Fin();
        }

        private static void CreateScreenOut()
        {
            m_ScreenOut = new GameObject();

            m_ScreenOut.name = "ScreenOut";
            m_ScreenOut.transform.SetParent(m_MainCanvas.transform);

            // RectTransformの設定
            m_RectTransform = m_ScreenOut.AddComponent<RectTransform>();
            {
                m_RectTransform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);
                m_RectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            }

            // Imageの設定
            m_Image = m_ScreenOut.AddComponent<Image>();
            m_Image.raycastTarget = false;
        }

        /// <summary>
        /// フェード中か否か
        /// </summary>
        public static bool IsFading() => m_SceneState != SceneState.Wait;
    }
}