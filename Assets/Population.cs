using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Population : MonoBehaviour
{
    // Start is called before the first frame update

    private int size = 100;
    [SerializeField]
    private GameObject[] _population;

    [SerializeField]
    private GameObject DotPrefab;

    [SerializeField]
    private GameObject Goal;

    private bool _preparingNextGeneration = false;

    private int _generationCount = 1;

    public TMP_Text GenerationText;

    public int GenerationCount
    {
        get => _generationCount;
        set
        {
            _generationCount = value;
            GenerationText.text = $"Current Generation: {_generationCount}";
        }
    }

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_preparingNextGeneration && GetAllPopulationDead())
        {
            StartCoroutine(StartNextGeneration());
        }
    }

    IEnumerator StartNextGeneration()
    {
        _preparingNextGeneration = true;
        while (_preparingNextGeneration)
        {
            yield return CaluclateFitness();

            // Kill the current dots
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            Init();



            _preparingNextGeneration = false;
        }

        ++GenerationCount;
    }

    void Init(int size = 100)
    {
        this.size = size;
        _population = new GameObject[size];
        for (int i = 0; i < size; i++)
        {
            _population[i] = Instantiate(DotPrefab, this.transform);
        }
    }

    IEnumerator CaluclateFitness()
    {
        List<Dot> dots = _population.Select(x => x.GetComponent<Dot>()).ToList();
        GameObject closestDot = _population[0];
        float highestFitness = 0;

        Dot.GoalPosition = Goal.transform.position;
        foreach (var dot in dots)
        {
            dot.CalculateFitness();
            if (dot.Fitness > highestFitness)
            {
                closestDot = dot.gameObject;
                highestFitness = dot.Fitness;
            }
            yield return null;
        }
        closestDot.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    bool GetAllPopulationDead()
    {
        for (int i = 0; i < _population.Length; i++)
        {
            var dot = _population[i].GetComponent<Dot>();
            if (!dot.Dead && !dot.ReachedGoal)
                return false;
        }

        return true;
    }

    void NaturalSelection()
    {

    }
}
