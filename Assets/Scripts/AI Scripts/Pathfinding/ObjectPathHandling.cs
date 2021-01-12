using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPathHandling : MonoBehaviour
{

    Vector2 size;

    Vector2[] points = new Vector2[4];

    int chunkSize;



    List<Vector2> obstaclePoints = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {

        chunkSize = ObstacleController.instance.ChunkSize;

        size = GetComponent<BoxCollider2D>().size;

        if(transform.position.x % chunkSize != 0 || transform.position.y % chunkSize != 0)
        {
            Vector2 pos = transform.position;

         

            pos.x = Mathf.FloorToInt(pos.x);

          

            pos.x -= Mathf.FloorToInt(pos.x % chunkSize);

         

            pos.y = Mathf.FloorToInt(pos.y);





            pos.y -= Mathf.FloorToInt(pos.y % chunkSize);

       

            transform.position = pos;
        }


        SetObstacle(-1);

        
        //Bottom Left


    }

   

    void SetObstacle(int value, int resistenceValue = 1)
    {
        //find the bottom left position, then we will itterate from there
        Vector2 pos = transform.position;

        int x = Mathf.FloorToInt(size.x / chunkSize);
        int y = Mathf.FloorToInt(size.y / chunkSize);


        for (int i = -1; i < x + 1; i++)
        {
            for(int j = -1; j < y + 1; j++)
            {

                Vector2 p = new Vector2(pos.x + (i * chunkSize), pos.y + (j * chunkSize));

                if (i == -1 || i == x || j == -1 || j == y)
                {
                   

                    //TODO: maybe setting the resistance to 1 here isnt the best idea, will need to adapts this to suit whats going on
                    if (ObstacleController.instance.GetNativeResistence(p) != -1)
                    {
                        ObstacleController.instance.SetObstacle(p, resistenceValue);
                    }


                }
                else
                {

                    ObstacleController.instance.SetObstacle(p, value);
                }


                obstaclePoints.Add(p);
                

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Vector2 pos in obstaclePoints)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector2 p = new Vector2(pos.x + chunkSize/2, pos.y + chunkSize/2);
            
            Gizmos.DrawCube(p, new Vector3(chunkSize, chunkSize, 1));
            
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
