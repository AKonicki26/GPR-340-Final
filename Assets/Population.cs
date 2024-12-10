using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Population : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _population;

    private GameObject[] _nextGeneration;

    private int _bestInPreviousGenerationIndex;

    [SerializeField]
    private GameObject DotPrefab;

    [SerializeField]
    private GameObject Goal;

    private bool _preparingNextGeneration = false;

    private int _generationCount = 1;

    public TMP_Text GenerationText;

    private float fitnessSum;

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
            yield return NaturalSelection();
            yield return MutateChildren();

            // Kill the current dots
            foreach (var t in _population)
            {
                Destroy(t);
            }

            _population = _nextGeneration;

            // set each of them active
            for(int i = 0; i < _population.Length; i++)
            {
                _population[i].SetActive(true);
            }

            _preparingNextGeneration = false;
        }

        ++GenerationCount;
    }

    void Init(int size = 100)
    {
        _population = new GameObject[size];
        for (int i = 0; i < size; i++)
        {
            _population[i] = Instantiate(DotPrefab, this.transform);
        }
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

    IEnumerator CaluclateFitness()
    {
        List<Dot> dots = _population.Select(x => x.GetComponent<Dot>()).ToList();
        GameObject closestDot = _population[0];
        float highestFitness = 0;

        Dot.GoalPosition = Goal.transform.position;

        fitnessSum = 0;

        for (int i = 0; i < _population.Length; i++)
        {
            dots[i].CalculateFitness();
            if (dots[i].Fitness > highestFitness)
            {
                closestDot = dots[i].gameObject;
                highestFitness = dots[i].Fitness;
                _bestInPreviousGenerationIndex = i;
            }
            fitnessSum += dots[i].Fitness;
            yield return null;
        }
        closestDot.GetComponent<SpriteRenderer>().color = Color.green;
    }

    IEnumerator NaturalSelection()
    {
        _nextGeneration = new GameObject[_population.Length];

        for (int i = 0; i < _population.Length; i++)
        {
            Dot parent = selectParent();

            // Instantiate the next generation and set it inactive immediately
            // Now we can mess with it without any Update functions calling, but it will still run Start/Awake methods
            _nextGeneration[i] = Instantiate(DotPrefab, this.transform);

            var dot = _nextGeneration[i].GetComponent<Dot>();
            dot.BecomeChildOf(parent);

            _nextGeneration[i].SetActive(false);

            yield return null;
        }

        _nextGeneration[0].GetComponent<Dot>().BecomeChildOf(_population[_bestInPreviousGenerationIndex].GetComponent<Dot>());
        _nextGeneration[0].GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    Dot selectParent()
    {
        float rand = Random.Range(0, fitnessSum);

        float runningTotal = 0;

        foreach (var gameObject in _population)
        {
            var dot = gameObject.GetComponent<Dot>();
            runningTotal += dot.Fitness;
            if (runningTotal > rand)
            {
                return dot;
            }
        }
        // should never get here
        return null;
    }

    IEnumerator MutateChildren()
    {

        for (int i = 1; i < _nextGeneration.Length; i++)
        {
            _nextGeneration[i].GetComponent<Dot>().Brain.Mutate();


            yield return null;
        }
    }
}
