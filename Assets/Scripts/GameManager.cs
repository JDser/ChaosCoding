using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Static vrappers
    public static GameManager Instance { get; private set; }

    public static void LoadLevel(int index)
    {
        Instance.Load(index);
    }  
    public static void NextLevel()
    {
        Instance.Load(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public static void ReloadLevel()
    {
        Instance.Load(SceneManager.GetActiveScene().buildIndex);
    }
    public static Camera RenderCamera
    {
        get => Instance.cam;
    }
    #endregion

    #region Variables
    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] RectTransform container_loadingScreen;

    [Header("Animation")]
    [SerializeField] float animationSpeed;
    [SerializeField] Vector3 rightPoint;
    [SerializeField] Vector3 leftPoint;

    [Header("Audio")]
    [SerializeField] AudioClip musicLoop;
    [SerializeField] AudioClip loadingAudio;

    [SerializeField] AudioSource musicLoopSource;
    [SerializeField] AudioSource source;

    AsyncOperation loadOperation;
    float _progress;
    WaitForSeconds waitTime = new WaitForSeconds(1.5f);

    bool _isLoading = false;

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        source.clip = loadingAudio;

        musicLoopSource.loop = true;
        musicLoopSource.clip = musicLoop;
        musicLoopSource.Play();
    }

    private void Load(int index)
    {
        if (_isLoading) return;
        StartCoroutine(LoadRoutine(index));
    }

    private IEnumerator LoadRoutine(int index)
    {
        if (index >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("No more scenes!");
            yield break;
        }

        _isLoading = true;

        yield return waitTime;

        loadOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        loadOperation.allowSceneActivation = false;

        container_loadingScreen.anchoredPosition = rightPoint;

        _progress = 0;

        source.loop = true;
        source.Play();

        while (_progress < 1f)
        {
            _progress += Time.deltaTime * animationSpeed;

            container_loadingScreen.anchoredPosition = Vector3.Lerp(rightPoint, leftPoint, _progress);

            if (_progress >= 0.5f && loadOperation.allowSceneActivation == false)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        source.loop = false;
        source.Stop();
        _isLoading = false;
    }
}