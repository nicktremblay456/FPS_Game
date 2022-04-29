using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using TNT.Manager;

public class GameManager : Singleton<GameManager>
{
    #region Game Management Variables/Props
    private enum ECursorState
    {
        Visible, Invisible
    }
    private ECursorState m_CursorState;

    public Dictionary<int, Vector3> savedPositions = new Dictionary<int, Vector3>();

    private bool m_IsGamePaused = false;
    public static bool isGamePaused
    {
        get => Instance.m_IsGamePaused;
    }

    private bool m_IsInEditeMode = false;
    public static bool isInEditMode
    {
        get => Instance.m_IsInEditeMode;
    }

    private PlayerController  m_PlayerController;
    public static PlayerController playerController
    {
        get
        {
            if (Instance.m_PlayerController == null)
            {
                Instance.m_PlayerController = FindObjectOfType<PlayerController>();
            }

            return Instance.m_PlayerController;
        }
        set => Instance.m_PlayerController = value;
    }

    private PlayerInput m_PlayerInput;
    public static PlayerInput playerInput
    {
        get
        {
            if (Instance.m_PlayerInput == null)
            {
                Instance.m_PlayerInput = FindObjectOfType<PlayerInput>();
            }

            return Instance.m_PlayerInput;
        }
        set => Instance.m_PlayerInput = value;
    }
    #endregion

    #region Level/Loading/Transition Variables/Props
    public enum FadeType
    {
        Black,
        Loading,
        GameOver,
    }

    private bool m_IsFading;
    public static bool IsFading
    {
        get { return Instance.m_IsFading; }
    }
    private bool m_Transitioning;
    public static bool Transitioning
    {
        get { return Instance.m_Transitioning; }
    }
    [Space, Header("Level/Transition Management")]
    [SerializeField] private CanvasGroup m_FaderCanvasGroup;
    [SerializeField] private CanvasGroup m_LoadingCanvasGroup;
    [SerializeField] private CanvasGroup m_GameOverCanvasGroup;
    [SerializeField] private float m_FadeDuration = 1f;

    private Scene m_CurrentZoneScene;

    private const int MAX_SORTING_LAYER = 32767;
    #endregion

    private void Start()
    {
        SetCursorState(ECursorState.Invisible);
    }

    #region Game Management
    private void SetCursorState(ECursorState state)
    {
        m_CursorState = state;
        switch (m_CursorState)
        {
            case ECursorState.Visible:
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
            }
            case ECursorState.Invisible:
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            }
        }
    }

    public void Pause()
    {
        m_IsGamePaused = true;
        Time.timeScale = 0;

        playerInput.ReleaseControl();
        SetCursorState(ECursorState.Visible);
    }

    public void UnPause()
    {
        m_IsGamePaused = false;
        Time.timeScale = 1;

        playerInput.GainControl();
        SetCursorState(ECursorState.Invisible);
    }

    public void Exit()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    #endregion

    #region Level Management
    ///<summary>
    /// Return true if the the name entered in parameter is equal to the current scene name
    ///</summary>
    public static bool IsCurrentScene(string sceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return Instance.m_CurrentZoneScene.name == sceneName;
    }

    public void RestartZone()
    {
        m_CurrentZoneScene = SceneManager.GetActiveScene();
        StartCoroutine(Transition(m_CurrentZoneScene.name));
    }

    public void TransitionToScene(string sceneName)
    {
        if (!m_Transitioning)
        {
            StartCoroutine(Transition(sceneName));
        }
    }

    private void SetAlpha(float alpha)
    {
        m_FaderCanvasGroup.alpha = alpha;
    }

    private IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup)
    {
        m_IsFading = true;
        canvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(canvasGroup.alpha - finalAlpha) / m_FadeDuration;
        while (!Mathf.Approximately(canvasGroup.alpha, finalAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        canvasGroup.alpha = finalAlpha;
        m_IsFading = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeSceneIn()
    {
        CanvasGroup canvasGroup;
        if (m_FaderCanvasGroup.alpha > 0.1f)
        {
            canvasGroup = m_FaderCanvasGroup;
        }
        else if (m_GameOverCanvasGroup.alpha > 0.1f)
        {
            canvasGroup = m_GameOverCanvasGroup;
        }
        else
        {
            canvasGroup = m_LoadingCanvasGroup;
        }

        yield return StartCoroutine(Fade(0f, canvasGroup));
        //Debug.Log(canvasGroup.gameObject.name);
        canvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeSceneOut(FadeType fadeType = FadeType.Black)
    {
        CanvasGroup canvasGroup;
        switch (fadeType)
        {
            case FadeType.Black:
            {
                canvasGroup = m_FaderCanvasGroup;
                break;
            }
            case FadeType.GameOver:
            {
                canvasGroup = m_GameOverCanvasGroup;
                break;
            }
            default:
            {
                canvasGroup = m_LoadingCanvasGroup;
                break;
            }
        }

        canvasGroup.gameObject.SetActive(true);

        yield return Instance.StartCoroutine(Instance.Fade(1f, canvasGroup));
    }

    private IEnumerator Transition(string newSceneName)
    {
        m_Transitioning = true;

        if (playerInput)
        {
            playerInput.ReleaseControl();
        }
        
        yield return StartCoroutine(FadeSceneOut(FadeType.Loading));

        yield return SceneManager.LoadSceneAsync(newSceneName);

        if (playerInput)
        {
            playerInput.ReleaseControl();
        }

        yield return StartCoroutine(FadeSceneIn());

        if (playerInput)
        {
            playerInput.GainControl();
        }

        m_Transitioning = false;
    }
    #endregion

    #region Utilies
    public bool CheckCollision(LayerMask interactableLayer, GameObject other)
    {
        return 0 != (interactableLayer.value & 1 << other.gameObject.layer);
    }
    #endregion
}