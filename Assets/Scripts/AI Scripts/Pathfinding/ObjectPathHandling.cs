using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPathHandling : MonoBehaviour
{

    Vector2 size;

    Vector2[] points = new Vector2[4];

    List<Vector2> obstaclePoints = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        size = GetComponent<BoxCollider2D>().size;

        if(transform.position.x % 16 != 0 || transform.position.y % 16 != 0)
        {
            Vector2 pos = transform.position;

         

            pos.x = Mathf.FloorToInt(pos.x);

          

            pos.x -= Mathf.FloorToInt(pos.x % 16);

         

            pos.y = Mathf.FloorToInt(pos.y);





            pos.y -= Mathf.FloorToInt(pos.y % 16);

       

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


        for (int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {

                Vector2 p = new Vector2(pos.x + (i * 16), pos.y + (j * 16));

                //Set the area around the object to have a resistance
                //if (i == -1 || i == x || j == -1 || j == y)
                //{
                   

                //    if (ObstacleController.instance.GetNativeResistence(p) != -1)
                //    {

                //        ObstacleController.instance.SetObstacle(p, 1);
                //    }
                //}
                //else
                //{
                    
                ObstacleController.instance.SetObstacle(p, value);
                obstaclePoints.Add(p);
                //}

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Vector2 pos in obstaclePoints)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector2 p = new Vector2(pos.x + 8, pos.y + 8);
            
            Gizmos.DrawCube(p, new Vector3(16, 16, 1));
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
    }

    private void OnDestroy()
    {
        SetObstacle(0);
    }
}
