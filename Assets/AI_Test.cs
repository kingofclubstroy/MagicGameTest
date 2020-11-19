using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Test : MonoBehaviour
{

    [SerializeField]
    GameObject targetObject;

    [SerializeField]
    int minDistance;

    [SerializeField]
    int speed;

    bool foundPath = false;

    List<Node> path;

    int countSinceLastPathUpdate = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        

        float speedLeft = speed * Time.deltaTime;
        if (path == null || countSinceLastPathUpdate >= 15)
        {

            path = ObstacleController.instance.A_Star(this.transform.position, targetObject.transform.position, 0);
            countSinceLastPathUpdate = 0;

           

        }
       

        if (path != null)
        {

            int pathsLookedAt = 0;
            Vector2 tempPosition = transform.position;

            foreach (Node p in path)
            { 

                float distance = Vector2.Distance(tempPosition, p.position);
               
                tempPosition = Vector2.MoveTowards(tempPosition, p.position, speedLeft);
                speedLeft -= distance;

                if (speedLeft <= 0)
                {
                    break;
                }

                pathsLookedAt++;

            }

            transform.position = tempPosition;

            path.RemoveRange(0, pathsLookedAt);

        }

        countSinceLastPathUpdate++;

       
    }
}
