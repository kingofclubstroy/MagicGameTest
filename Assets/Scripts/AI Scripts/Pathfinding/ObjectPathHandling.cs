using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPathHandling : MonoBehaviour
{

    Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        size = GetComponent<BoxCollider2D>().size;
        Debug.LogError("size = " + size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("entered wall collider");
    }
}
