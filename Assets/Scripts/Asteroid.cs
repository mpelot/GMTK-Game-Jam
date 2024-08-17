using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > 10f || Mathf.Abs(transform.position.y) > 10f) {
            Destroy(gameObject);
        }
        /*float step = 0.2f * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, transform.right, step);*/
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Planet")) {
            rb.AddForce((collision.gameObject.transform.position - transform.position).normalized * 0.3f * (2.7f - (collision.gameObject.transform.position - transform.position).magnitude), ForceMode2D.Force);
        }
    }
}
