using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Dot : MonoBehaviour
{
    public Brain Brain;

    public Vector2 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector2 Velocity
    {
        get => velocity;
        set => velocity = Vector2.ClampMagnitude(value, MaxVelocity);
    }
    [SerializeField]
    private Vector2 velocity;
    private const float MaxVelocity = 20f;

    public Vector2 Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }
    [SerializeField]
    private Vector2 acceleration;

    private static Vector3 _screenBottomLeft;
    private static Vector3 _screentopRight;


    [SerializeField] private float fitness = 0;

    public float Fitness
    {
        get => fitness;
        set => fitness = value;
    }

    [SerializeField]
    private bool dead = false;
    public bool Dead
    {
        get => dead;
        private set => dead = value;
    }
    [SerializeField]
    private bool reachedGoal = false;
    public bool ReachedGoal {
        get => reachedGoal;
        private set => reachedGoal = value;
    }

    public static Vector2 GoalPosition;

    // Start is called before the first frame update
    void Start()
    {
        _screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
        _screentopRight = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        Brain = new(400);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!InScreenBounds())
            Kill();
        // Only update alive dots
        if (Dead || ReachedGoal)
            return;

        Move();
    }

    private void Move()
    {
        if (Brain.Directions.Count > Brain.Step)
        {
            Acceleration = Brain.Directions[Brain.Step];
            Brain.Step++;
        }
        else
        {
            Kill();
        }

        Velocity += Acceleration * Time.deltaTime;
        Position += Velocity * Time.deltaTime;
    }

    private bool InScreenBounds()
    {
        if (Position.x < _screenBottomLeft.x)
            return false;
        if (Position.x > _screentopRight.x)
            return false;
        if (Position.y < _screenBottomLeft.y)
            return false;
        if (Position.y > _screentopRight.y)
            return false;

        return true;
    }

    public void Kill()
    {
        Dead = true;
        Acceleration = Vector2.zero;
        Velocity = Vector2.zero;
    }

    public void CalculateFitness()
    {
        var distToGoal = Vector2.Distance(Position, GoalPosition);
        Fitness = 1.0f / ( distToGoal * distToGoal );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.GameObject().name);
        if (other.CompareTag("Goal"))
        {
            ReachedGoal = true;
        }
    }
}
