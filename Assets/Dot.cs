using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private Transform _transform;
    private Brain _brain;

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _transform.position = _position;
        }

    }

    private Vector2 _position;

    public Vector2 Velocity
    {
        get => _velocity;
        set
        {
            _velocity = Vector2.ClampMagnitude(value, MaxVelocity);
        }

    }

    private Vector2 _velocity;

    public Vector2 Acceleration { get => _acceleration; set => _acceleration = value; }
    private Vector2 _acceleration;

    private const float MaxVelocity = 20f;

    public bool Dead { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        _brain = GetComponent<Brain>();

        Acceleration = new Vector2(-5, 2);
    }

    // Update is called once per frame
    void Update()
    {
        // Only update alive dots
        if (Dead)
            return;



        Velocity += Acceleration * Time.deltaTime;
        Position += Velocity * Time.deltaTime;
    }

    public void Kill()
    {
        Dead = true;
    }
}
