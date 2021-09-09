using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class Progress
{
    int[][] _sequences;
    int _currentSequence = 0;
    int _currentNumber = 0;


    public int CurrentNumber
    {
        get => _sequences[_currentSequence][_currentNumber];
    }
    public int NextNumber
    {
        get => _sequences[_currentSequence][_currentNumber + 1];
    }

    public void IncreaseNumber()
    {
        _currentNumber++;
        if (_currentNumber > _sequences[_currentSequence].Length)
        {
            _currentSequence++;
            _currentNumber = 0;
        }
    }

    public Progress(int NumberOfSequences, int minRange, int maxRange)
    {
        _sequences = new int[NumberOfSequences][];

        for (int i = 0; i < NumberOfSequences; i++)
        {
            _sequences[i] = GenerateSequence(minRange, maxRange).ToArray();
        }
    }

    List<int> GenerateSequence(int minRange, int maxRange)
    {
        List<int> _list = new List<int>();

        int currentNumber = Random.Range(minRange, maxRange);
        if (currentNumber % 2 == 0)
            currentNumber++;

        int _break = 100;
        int _currentBreak = 0;

        while (true)
        {
            _list.Add(currentNumber);
            _currentBreak++;
            if (_currentBreak > _break)
                break;

            if (currentNumber == 1)
            {
                break;
            }

            if (currentNumber % 2 == 0)
            {
                currentNumber /= 2;
            }
            else
            {
                currentNumber = 3 * currentNumber + 1;
            }
        }

        return _list;
    }
}