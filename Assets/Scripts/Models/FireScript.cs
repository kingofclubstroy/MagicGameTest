using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{

    private FireSprite fireSprite;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setFireSprite(FireSprite fireSprite)
    {
        this.fireSprite = fireSprite;

        spriteRenderer.sprite = this.fireSprite.getNextSprite();
    }



}
