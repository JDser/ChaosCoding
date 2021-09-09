using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    #region Static Wrappers
    public static LevelManager Instance 
    {
        get;
        private set;
    }

    public static void PlaySound(AudioClip clip)
    {
        Instance.source.PlayOneShot(clip);
    }
    #endregion

    #region Variables
    [Header("Nodes")]
    [SerializeField] protected StartNode startNode;
    [SerializeField] protected EndNode endNode;

    [Header("Data")]
    [SerializeField] protected InputData startData;
    [SerializeField] protected InputData endData;

    #region NonSerialized
    AudioSource source;

    protected Canvas[] canvases;
    GraphicRaycaster[] raycasters;
    #endregion

    #region Properties
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

    #endregion

    protected virtual void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
    
        endNode.OnCheckEnd += OnCheckEnd;
    
        canvases = GetComponentsInChildren<Canvas>();
        raycasters = GetComponentsInChildren<GraphicRaycaster>();

    }

    protected virtual void Start()
    {
        RenderCamera = GameManager.RenderCamera;
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].GetComponent<CanvasScaler>().referenceResolution = GameManager.ScreenResolution;
        }

        startNode.SetNewOutput(0, startData);
        endNode.SetNewInput(0, endData);

        startNode.SetupNode();
        endNode.SetupNode();
    }


    protected virtual void OnCheckEnd(bool result)
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