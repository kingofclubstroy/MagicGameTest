using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPathHandling : MonoBehaviour
{

    Vector2 size;

    Vector2[] points = new Vector2[4];

    // Start is called before the first frame update
    void Start()
    {
        size = GetComponent<BoxCollider2D>().size;

        if(transform.position.x % 16 != 0 || transform.position.y % 16 != 0)
        {
            Vector2 pos = transform.position;

            pos.x -= pos.x % 16;
            pos.y -= pos.y % 16;

            transform.position = pos;
        }


        SetObstacle(-1);

        
        //Bottom Left


    }

    void SetObstacle(int value)
    {
        //find the bottom left position, then we will itterate from there
        Vector2 pos = transform.position;

        int x = Mathf.FloorToInt(size.x / 16);
        int y = Mathf.FloorToInt(size.y / 16);

        Debug.Log("x = " + x);
        Debug.Log("y = " + y);

        for (int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {

                Vector2 p = new Vector2(pos.x + (i * 16), pos.y + (j * 16));
                Debug.Log(p);
                ObstacleController.instance.SetObstacle(p, value);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("entered wall collider");
    }

    private void OnDestroy()
    {
        SetObstacle(0);
    }
}
