using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    #region Node Struct
    [System.Serializable]
    public struct SpawnNode
    {
        //Make list array in inspector use output data name.

        [SerializeField] NodeBase prefab;
        [SerializeField] InputData[] inputData;
        [SerializeField] InputData outputData;

        public NodeBase Prefab
        {
            get => prefab;
        }
        public InputData InputData(int index)
        {
            return inputData[index];
        }
        public int Inputs
        {
            get => inputData.Length;
        }
        public InputData OutputData
        {
            get => outputData;
        }
    }
    #endregion

    #region Variables
    [Header("UI")]
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject stopButton;
    [SerializeField] Transform scrollRectContent;

    [Header("Nodes")]
    [SerializeField] protected SpawnNode startNodePrefab;
    [SerializeField] protected SpawnNode endNodePrefab;
    [SerializeField] protected SpawnNode[] nodesToSpawn;

    #region NonSerialized
    AudioSource source;

    protected Canvas[] canvases;
    GraphicRaycaster[] raycasters;
    CanvasScaler[] scalers;

    NodeBase _startNode;
    NodeBase _endNode;
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
        
        canvases = GetComponentsInChildren<Canvas>();
        raycasters = GetComponentsInChildren<GraphicRaycaster>();
        scalers = GetComponentsInChildren<CanvasScaler>();

        for (int i = 0; i < scalers.Length; i++)
        {
            scalers[i].referenceResolution = GameManager.ScreenResolution;
        }
    }

    protected virtual void Start()
    {
        RenderCamera = GameManager.RenderCamera;

        SpawnNodes();
    }

    protected virtual void OnCheckEnd(bool result)
    {
        if (result)
        {
            InteractCanvas = false;
            GameManager.NextLevel();
        }
    }

    protected void SpawnNodes()
    {
        StartCoroutine(SpawnNodesRoutine());
    }

    readonly WaitForSeconds waitTime = new WaitForSeconds(0.25f);

    protected virtual IEnumerator SpawnNodesRoutine()
    {
        Debug.LogError("Add Nodes Spawn Animation");

        InteractCanvas = false;

        _startNode = Instantiate(startNodePrefab.Prefab, scrollRectContent);
        _startNode.SetNewOutput(0, startNodePrefab.OutputData);
        _startNode.SetupNode();
        _startNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(-600, 0);

        yield return waitTime;

        int index = 0;
        for (int i = 0; i < nodesToSpawn.Length; i++)
        {
            NodeBase node = Instantiate(nodesToSpawn[i].Prefab, scrollRectContent);

            for (int y = 0; y < nodesToSpawn[i].Inputs; y++)
                node.SetNewInput(y, nodesToSpawn[i].InputData(y)); // Error
            node.SetNewOutput(0, nodesToSpawn[i].OutputData);

            node.SetupNode();

            node.GetComponent<RectTransform>().anchoredPosition = new Vector2(40 * index, -70 * index);
            index++;

            yield return waitTime;
        }

        _endNode = Instantiate(endNodePrefab.Prefab, scrollRectContent);
        _endNode.SetNewInput(0, endNodePrefab.InputData(0));
        _endNode.SetupNode();
        _endNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(600, 0);
        ((EndNode)_endNode).OnCheckEnd += OnCheckEnd;

        yield return waitTime;



        InteractCanvas = true;
    }

    #region UI Buttons
    public void StartSimulation()
    {
        raycasters[1].enabled = false;

        _startNode.Enact();
        startButton.SetActive(false);
        stopButton.SetActive(true);
    }

    public void StopSimulation()
    {
        raycasters[1].enabled = true;

        _startNode.Stop();
        startButton.SetActive(true);
        stopButton.SetActive(false);
    }

    public void Reload()
    {
        GameManager.ReloadLevel();
    }
    #endregion
}