using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLevelManager : LevelManager
{
    [SerializeField, Range(1, 10)] int difficulty = 3;

    [Header("Nodes")]
    [SerializeField] MathNode addNode;
    [SerializeField] MathNode multiplyNode;
    [SerializeField] MathNode subtractNode;
    [SerializeField] MathNode divideNode;

    [SerializeField] NodeBase dataNode;

    [SerializeField] InputTypePrefab intPrefab;

    protected override void Start()
    {
        Setup();
        base.Start();
    }

    [Header("Test")]
    public float totalProcentDivision;
    public float procentLeft;
    public float[] procentDivision;

    void Setup()
    {
        int startNumber = ProgressManager.GetStartNumber();
        int endNumber = ProgressManager.GetEndNumber();

        startData.DefaultValue = startNumber.ToString();
        endData.DefaultValue = endNumber.ToString();

        int difference = endNumber - startNumber;



        
        procentDivision = new float[difficulty];

         procentLeft = 1f;

        for (int i = 0; i < procentDivision.Length; i++)
        {
            float _noise = (float)System.Math.Round((1f - Random.value) / difficulty, 2);
            float _a = (float)System.Math.Round(procentLeft / difficulty + _noise, 2);

            procentLeft -= _a;
            procentDivision[i] = _a;
            totalProcentDivision += Mathf.Abs(_a);
        }



        if (difference > 0)
        {
            //sequence go up



            for (int i = 0; i < difficulty; i++)
            {
                float random = Random.value;

                int number = (int)System.Math.Round(difference * random,0);

                difference -= number;
                if (difference < 0)
                    number -= Mathf.RoundToInt(difference / 2);


                CreateDataNode(number);
                CreateAddNode();

                if (random > .5f)
                {
                    //Adding
                    Debug.Log("Adding");
                }
                else
                {
                    //Multiply
                    Debug.Log("Multiply");
                }
            }
        }
        else
        {
            //sequence go down

            for (int i = 0; i < difficulty; i++)
            {
                float random = Random.value;
                if (random > .5f)
                {
                    //Subtract
                    Debug.Log("Subtract");
                }
                else
                {
                    //Divide
                    Debug.Log("Divide");
                }
            }
        }
    }

    protected override void OnCheckEnd(bool result)
    {
        if (result)
        {
            InteractCanvas = false;
            GameManager.NextLevel();

            ProgressManager.NextSequence();
        }
    }

    void CreateDataNode(int data)
    {
        NodeBase _data = Instantiate(dataNode, canvases[1].transform);
        
        _data.SetNewOutput(0,new InputData(data.ToString(),intPrefab));
        _data.SetupNode();
    }

    void CreateAddNode()
    {
        MathNode node = Instantiate(addNode, canvases[1].transform);
        
        node.OriginData = new InputData("", intPrefab);
        
        node.SetNewInput(0, new InputData("", intPrefab));
        node.SetNewInput(1, new InputData("", intPrefab));
        
        node.SetNewOutput(0, new InputData("", intPrefab));
        node.SetupNode();
    }   
    void CreateMultiplyNode()
    {
        MathNode node = Instantiate(multiplyNode, canvases[1].transform);
        
        node.OriginData = new InputData("", intPrefab);
        
        node.SetNewInput(0, new InputData("", intPrefab));
        node.SetNewInput(1, new InputData("", intPrefab));
        
        node.SetNewOutput(0, new InputData("", intPrefab));
        node.SetupNode();
    }  
    void CreateSubtractNode()
    {
        MathNode node = Instantiate(subtractNode, canvases[1].transform);
        
        node.OriginData = new InputData("", intPrefab);
        
        node.SetNewInput(0, new InputData("", intPrefab));
        node.SetNewInput(1, new InputData("", intPrefab));
        
        node.SetNewOutput(0, new InputData("", intPrefab));
        node.SetupNode();
    } 
    void CreateDivideNode()
    {
        MathNode node = Instantiate(divideNode, canvases[1].transform);
        
        node.OriginData = new InputData("", intPrefab);
        
        node.SetNewInput(0, new InputData("", intPrefab));
        node.SetNewInput(1, new InputData("", intPrefab));
        
        node.SetNewOutput(0, new InputData("", intPrefab));
        node.SetupNode();
    }

}