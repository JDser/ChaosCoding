using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    #region Static Vrappers
    public static LevelManager Instance { get; private set; }
    public static void PlaySound(AudioClip clip)
    {
        Instance.source.PlayOneShot(clip);
    }
    #endregion

    #region Variables
    [Header("Nodes")]
    [SerializeField] StartNode startNode;
    [SerializeField] EndNode endNode;

    AudioSource source;

    Canvas[] canvases;
    GraphicRaycaster[] raycasters;

    public Camera RenderCamera
    {
        set
        {
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].worldCamera = value;
            }
        }
    }

    public bool InteractCanvas
    {
        set
        {
            for (int i = 0; i < raycasters.Length; i++)
            {
                raycasters[i].enabled = value;
            }
        }
    }
    #endregion

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();

        endNode.OnCheckEnd += OnCheckEnd;

        canvases = GetComponentsInChildren<Canvas>();
        raycasters = GetComponentsInChildren<GraphicRaycaster>();
    }
    private void Start()
    {
        RenderCamera = GameManager.RenderCamera;
    }


    private void OnCheckEnd(bool result)
    {
        if (result)
        {
            InteractCanvas = false;
            GameManager.NextLevel();
        }
    }

    #region UI Buttons
    public void StartSimulation()
    {
        startNode.Enact();
    }

    public void Reload()
    {
        GameManager.ReloadLevel();
    }
    #endregion
}