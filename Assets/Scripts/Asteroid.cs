using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float coreForce;
    public float planetForce;
    public float maxDistance;

    private Vector3 startingScale;
    public float scaleRate;
    public float _growthLevel = 1f;
    public float growthLevel
    {
        get
        {
            return _growthLevel;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _growthLevel = value;
            transform.localScale = new Vector3(startingScale.x + (_growthLevel * scaleRate), startingScale.y + (_growthLevel * scaleRate), 0f);
        }
    }

    public TrajectoryLine parentTrajectoryLine;
    public Spawner spawner;

    private Rigidbody2D rb;
    private Core core;
    private TrajectoryLine personalTrajectoryLine;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        core = GameObject.Find("Core").GetComponent<Core>();
        startingScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > maxDistance)
        {
            Destroy(gameObject);
        }
        if (personalTrajectoryLine == null && parentTrajectoryLine != null)
        {
            Vector2 closestPoint = parentTrajectoryLine.FindClosestTrajectoryPoint(transform.position);
            float distanceToClosestPoint = (closestPoint - (Vector2) transform.position).magnitude;
            if (distanceToClosestPoint > 0.4f)
            {
                personalTrajectoryLine = Instantiate(parentTrajectoryLine, this.transform);
                Color baseColor = personalTrajectoryLine.GetComponent<LineRenderer>().material.GetColor("_Color");
                baseColor.a = 0.05f;
                personalTrajectoryLine.GetComponent<LineRenderer>().material.SetColor("_Color", baseColor);
                spawner.RemoveAsteroidFromTrajectoryLine();
            }
        }
        if (personalTrajectoryLine != null)
        {
            personalTrajectoryLine.startingVelocity = rb.velocity;
        }
    }

    void FixedUpdate()
    {
        // Add gravitational force due to core
        Vector2 distanceToCore = core.transform.position - transform.position;
        rb.AddForce(distanceToCore.normalized * (coreForce / distanceToCore.sqrMagnitude), ForceMode2D.Force);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Gravity")) {
            rb.AddForce((collision.gameObject.transform.position - transform.position).normalized * planetForce * (2.7f - (collision.gameObject.transform.position - transform.position).magnitude), ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            if (gameObject.GetInstanceID() < collision.gameObject.GetInstanceID()) // Compare instanceIDs so that only one of the two objects get's destroyed.
            {
                Vector2 otherVelocity = collision.GetComponent<Rigidbody2D>().velocity;
                Destroy(collision.gameObject);
                growthLevel++;

                rb.velocity = (rb.velocity + otherVelocity) / 2;
            }
        }
        else if (collision.gameObject.CompareTag("ShrinkRock"))
        {
            Destroy(collision.gameObject);

            growthLevel--;
            if (growthLevel <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.RemoveAsteroidFromTrajectoryLine();
        }
    }
}
