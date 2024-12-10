using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Brain
{

    private static readonly float MUTATION_RATE = 0.01f;
    private static readonly float ACCELERATION_MULTIPLIER = 5f;
    public Brain(int numberOfInstructions)
    {
        Debug.Log("Creating Random Brain");
        Directions = new(new Vector2[numberOfInstructions]);
        RandomizeInstructions();
    }

    public Brain(Brain parent)
    {
        Debug.Log("Creating Copied Brain");
        Directions = new(parent.Directions);
    }

    public List<Vector2> Directions;
    public int Step { get; set; } = 0;

    private void RandomizeInstructions()
    {
        var rnd = new System.Random();
        for (int i = 0; i < Directions.Count; i++)
        {
            float randomAngle = rnd.Next(0, 360);
            Directions[i] = randomAngle.Vector2FromDegrees() * ACCELERATION_MULTIPLIER;
        }
    }

    public void Mutate()
    {
        for (int i = 0; i < Directions.Count; i++)
        {
            if (Random.Range(0, 1f) < MUTATION_RATE)
            {
                // set this direction as a new random direction
                float randomAngle = Random.Range(0, 360);
                Directions[i] = randomAngle.Vector2FromDegrees() * ACCELERATION_MULTIPLIER;
            }
        }
    }
}

public static class FloatExtensions
{
    /// <summary>
    /// Create a Vector2 based on a degree angle
    /// </summary>
    /// <param name="degrees">Angle in degrees</param>
    /// <returns>A Vector2 object on the unit circle</returns>
    public static Vector2 Vector2FromDegrees(this float degrees)
    {
        return new Vector2(Mathf.Sin(degrees * Mathf.Deg2Rad), Mathf.Cos(degrees * Mathf.Deg2Rad));
    }

    /// <summary>
    /// Create a Vector2 based on an angle in radians
    /// </summary>
    /// <param name="radians">Angle in radians</param>
    /// <returns>A Vector2 object on the unit circle</returns>
    public static Vector2 Vector2FromRadians(this float radians)
    {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}
