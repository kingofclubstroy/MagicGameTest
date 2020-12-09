using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
  
    // Used for organization
    private PolygonCollider2D[] colliders;

    // Collider on this game object
    private PolygonCollider2D localCollider;

    /// <summary>
    /// Set the hitboxes for each animation
    /// </summary>
    [SerializeField]
    AnimationHitboxes[] animationHitboxes;

    //Quality of life dictionary for grabbing appropriate colliders
    Dictionary<string, PolygonCollider2D[]> AnimationDictionary;

    //Indicates index for the next hitbox
    int hitboxIndex = 0;


    void Start()
    {

        //Convert serialized array of animation hitbox objects into a more easily accessable dictionary, indexed by name 
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

    /// <summary>
    /// Tell the controller which animation is running so we can prepare appropriate hitboxes
    /// </summary>
    /// <param name="animation"> Name of Animation that is about to exaqute</param>
    public void SetNewAnimation(string animation)
    {
        if (AnimationDictionary.ContainsKey(animation))
        {
            //Grab the appropriate list of coliders and resets the index of current hitbox
            colliders = AnimationDictionary[animation];
            hitboxIndex = 0;
        } else
        {
            Debug.LogError("No hitboxes set for this attack");
        }
    }

    /// <summary>
    /// Called by animation triggers to set the next appropriate hitbox or clear the last one
    /// </summary>
    public void setHitBox()
    {
        if (hitboxIndex >= colliders.Length)
        {
            //index if over hitbox count for this animation, so we will clear the hitbox
            localCollider.pathCount = 0;
            
            return;
        }

        //Sets the new points for the hitbox, and increment the index for when we set the next hitbox
        localCollider.SetPath(0, colliders[hitboxIndex].GetPath(0));
        hitboxIndex++;
       
        
    }

}
