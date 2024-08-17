using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkRock : MonoBehaviour
{
    public float coreForce;

    private Rigidbody2D rb;
    private Core core;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        core = GameObject.Find("Core").GetComponent<Core>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > 10f || Mathf.Abs(transform.position.y) > 10f)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // Add gravitational force due to core
        Vector2 distanceToCore = core.transform.position - transform.position;
        rb.AddForce(distanceToCore.normalized * (coreForce / distanceToCore.sqrMagnitude), ForceMode2D.Force);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Gravity"))
        {
            rb.AddForce((collision.gameObject.transform.position - transform.position).normalized * 0.3f * (2.7f - (collision.gameObject.transform.position - transform.position).magnitude), ForceMode2D.Force);
        }
    }
}
