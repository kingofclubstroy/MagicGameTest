using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
  
    // Used for organization
    private PolygonCollider2D[] colliders;

    // Collider on this game object
    private PolygonCollider2D localCollider;

    [SerializeField]
    AnimationHitboxes[] animationHitboxes;

    Dictionary<string, PolygonCollider2D[]> AnimationDictionary;

    int index = 0;


    void Start()
    {

        AnimationDictionary = new Dictionary<string, PolygonCollider2D[]>();

        foreach(AnimationHitboxes boxes in animationHitboxes)
        {
            AnimationDictionary[boxes.Name] = boxes.Hitboxes;
        }

        // Create a polygon collider
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true; // Set as a trigger so it doesn't collide with our environment
        localCollider.pathCount = 0; // Clear auto-generated polygons

        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("HitBoxController hit something!");
        Debug.Log(col.gameObject.name);
    }

    public void SetNewAnimation(string animation)
    {
        colliders = AnimationDictionary[animation];
        index = 0;
    }

    public void setHitBox()
    {
        if (index >= colliders.Length)
        {
            localCollider.pathCount = 0;
            
            return;
        }

        localCollider.SetPath(0, colliders[index].GetPath(0));
        index++;
       
        
    }

}
