using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionCircleHandler : MonoBehaviour
{

    AIVariables AiVariables;

    // Start is called before the first frame update
    void Start()
    {
        AiVariables = GetComponentInParent<AIVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Trigger Handling

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger enter");
       
        //TODO: Need to add a tag to things ai are interested about, is currently just adding anything other than itself
        if (collision.gameObject != transform.gameObject && collision.gameObject.tag != "Obstacle" && collision.GetType() != typeof(PolygonCollider2D))
        {

            AiVariables.AddNearbyEnemy(collision.gameObject);

        }

        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("trigger exit");
        if (collision.gameObject != transform.gameObject)
        {
            AiVariables.RemoveNearbyEnemy(collision.gameObject);

        }

        
    }

    #endregion
}
