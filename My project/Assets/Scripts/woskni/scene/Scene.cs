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
        public static GameObject    m_ScreenOut;           // ��ʓh���̉摜�I�u�W�F�N�g
        public static Timer         m_ChangeTimer;         // �V�[���J�ڃ^�C�}�[
        public static Scenes        m_CurrentScene;        // ���݂̃V�[��
        public static Scenes        m_LastScene;           // �O��̃V�[��

        // private
        private static float m_FadeInTime;              // �t�F�[�h�C���ɗv���鎞��
        private static float m_FadeOutTime;             // �t�F�[�h�A�E�g�ɗv���鎞��
        private static Color m_FadeColor;               // �t�F�[�h���̐F

        private static RectTransform m_RectTransform;
        private static Image m_Image;

        private static Canvas m_MainCanvas;

        // �V�[���J�ڂ̏��
        private enum SceneState
        {
              FadeIn
            , FadeOut
            , ChangeScene
            , Wait
        }
        private static SceneState m_SceneState;

        // ���̃V�[���ԍ�
        private static Scenes m_NextScene;

        private void Awake()
        {
            // �V�[�����܂����ł������Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);
            CreateMainCanvas();
            CreateScreenOut();

            // �t�F�[�h�A�E�g��Ԃɂ���
            m_SceneState = SceneState.FadeOut;
            m_Image.color = m_FadeColor = Color.black;

            m_ChangeTimer.Setup(1f);
        }

        /// <summary></summary>
        /// <param name="canvasName"></param>
        /// <param name="renderMode"></param>
        void CreateMainCanvas(string canvasName = "MainCanvas", RenderMode renderMode = RenderMode.ScreenSpaceOverlay, int sortingOrder = 1)
        {
            // ���ɑ��݂���ꍇ�̓G���[��f���ďI��
            if (m_MainCanvas != null) {
                Debug.LogError("���Ƀ��C���L�����o�X�����݂��Ă��܂��B");
                return;
            }

            // �L�����o�X�I�u�W�F�N�g�𐶐�
            GameObject obj = new GameObject(canvasName);

            // �V�[�����ʂɂ���
            DontDestroyOnLoad(obj);

            // Canvas�R���|�[�l���g���A�^�b�`���A�����_�[���[�h��ݒ�
            Canvas canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = renderMode;
            canvas.sortingOrder = sortingOrder;

            // �L�����o�X�ɕK�v�ȏ����A�^�b�`����
            obj.AddComponent<CanvasScaler>();
            obj.AddComponent<GraphicRaycaster>();

            m_MainCanvas = canvas;
        }

        private void Update()
        {
            // ��Ԃ��Ƃɏ�����ς���
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

            // �C�[�W���O�œ����x��ς���
            float rate = Easing.Linear(m_ChangeTimer.time, m_ChangeTimer.limit, startRate, 1f - startRate);
            m_Image.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, rate);

            if (m_ChangeTimer.IsFinished())
            {
                m_ChangeTimer.Setup(m_FadeOutTime);

                // �F���C�[�W���O�ŏI�F�ɕς���
                m_Image.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, System.Convert.ToInt32(isFadeIn));

                // �t�F�[�h�C���@����̑J�ڂȂ�ChangeScene
                // �t�F�[�h�A�E�g����̑J�ڂȂ�Wait        �ɏ�Ԃ�ς���
                m_SceneState = isFadeIn ? SceneState.ChangeScene : SceneState.Wait;
            }
        }

        private void SceneChange()
        {
            Time.timeScale = 1f;

            m_LastScene     = m_CurrentScene;
            m_CurrentScene  = m_NextScene;

            SceneManager.LoadScene((int)m_NextScene);

            // �t�F�[�h�A�E�g��Ԃɂ���
            m_Image.color = m_FadeColor;

            m_SceneState = SceneState.FadeOut;
        }

        /// <summary>
        /// �V�[���J��
        /// </summary>
        /// <param name="toSceneNum">�J�ڐ�̃V�[���ԍ�</param>
        /// <param name="fadeInTime">�t�F�[�h�C������</param>
        /// <param name="fadeOutTime">�t�F�[�h�A�E�g����</param>
        /// <param name="color">�t�F�[�h�F</param>
        public static void Change(Scenes toScene, float fadeInTime = 1f, float fadeOutTime = 1f, Color? color = null)
        {
            // �t�F�[�h���Ȃ�����
            if (IsFading()) return;

            m_FadeInTime = fadeInTime;
            m_FadeOutTime = fadeOutTime;
            m_FadeColor = color ?? Color.black;

            // �����h��Ԃ��I�u�W�F�N�g��null�Ȃ�Đ���
            if (m_ScreenOut == null) CreateScreenOut();

            // �h��Ԃ��I�u�W�F�N�g�̐F��ݒ�
            m_Image.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 0f);

            // �t�F�[�h�C����Ԃɂ���
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

            // RectTransform�̐ݒ�
            m_RectTransform = m_ScreenOut.AddComponent<RectTransform>();
            {
                m_RectTransform.position = new Vector3(Screen.width / 2f, Screen.height / 2f);
                m_RectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            }

            // Image�̐ݒ�
            m_Image = m_ScreenOut.AddComponent<Image>();
            m_Image.raycastTarget = false;
        }

        /// <summary>
        /// �t�F�[�h�����ۂ�
        /// </summary>
        public static bool IsFading() => m_SceneState != SceneState.Wait;
    }
}