using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    #region Private Fields


    [Tooltip("Character's speed, used to move around the map")]
    [SerializeField]
    float speed;

    [SerializeField]
    CrawlController crawlController;

    Crawl runningCrawl;
    Vector2 runningStartPosition;
    float runningStartTime;

    [SerializeField]
    Animator animator;

    bool facingRight = true;

    bool casting = false;

    [SerializeField]
    GameObject CastingCircleProjectionPrefab;

    GameObject CastingCircleProjection;

    #endregion

    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        if(casting && (Input.GetKeyUp("space") || Input.GetKeyUp("r")))
        {
            animator.SetBool("Casting", false);
            casting = false;
        }

        //TODO: dont want to be doing this, but i must for now, means when holding space (casting) you cant move
        if(Input.GetKey(KeyCode.Space) == false)
        {
           
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if(v != 0 && h == 0)
            {
                if(v > 0)
                {
                    animator.SetBool("Walking_Up", true);
                    animator.SetBool("Walking_Down", false);
                    animator.SetBool("Walking_Left", false);
                } else
                {
                    animator.SetBool("Walking_Up", false);
                    animator.SetBool("Walking_Down", true);
                    animator.SetBool("Walking_Left", false);
                }
            } else if (h != 0)
            {
                animator.SetBool("Walking_Up", false);
                animator.SetBool("Walking_Down", false);
                animator.SetBool("Walking_Left", true);

                if (h > 0 && !facingRight)
                    Flip();
                else if (h < 0 && facingRight)
                    Flip();
            } else
            {
                animator.SetBool("Walking_Up", false);
                animator.SetBool("Walking_Down", false);
                animator.SetBool("Walking_Left", false);
            }

            Vector3 tempVect = new Vector3(h, v, 0);
            tempVect = tempVect.normalized * speed * Time.deltaTime;

            this.transform.position += tempVect;
        } else
        {
            // character is currently casting
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if(h != 0 || v != 0)
            {
                //We are wanting to move the casting projection, so lets see if we need to instantiate it
                if(CastingCircleProjection == null)
                {
                    CastingCircleProjection = Instantiate(CastingCircleProjectionPrefab, this.transform.position, Quaternion.identity);
                }

                CastingCircleProjection.transform.position += new Vector3(h, v);
            }

        }



        //TODO: remove
        if(Input.GetKeyDown("space"))
        {
           
            animator.SetBool("Casting", true);
            casting = true;
            //crawlController.CreateCrawl(transform.position, 1000, 1000);


        }

        if(Input.GetKeyDown("p"))
        {
            Vector2 bottom = transform.position;
            crawlController.CreateCrawl(bottom);
        }

        if(Input.GetKeyDown("r"))
        {
            Vector2 bottom = transform.position;
            crawlController.AddFire(bottom);
            animator.SetBool("Casting", true);
            casting = true;

        }

        if (Input.GetKeyDown("q"))
        {

            Vector2 bottom = transform.position;

            for(int x = -3; x <= 3; x++)
            {
                for(int y = -3; y <= 3; y++)
                {
                   
                        Vector2 pos = new Vector2(bottom.x + x, bottom.y + y);
                        WaterControllerScript.instance.AddWater(pos, 100);
                    
                }
            }

            WaterControllerScript.instance.applyTexture();

        }

        //if (Input.GetKeyDown("e"))
        //{

        //    if(runningCrawl == null)
        //    {
        //        runningCrawl = crawlController.CreateCrawl(transform.position);
        //        runningStartPosition = transform.position;
        //        runningStartTime = Time.time;
        //    } 

        //} else if(runningCrawl != null)
        //{

        //    if (Time.time - runningStartTime >= 3)
        //    {
        //        runningCrawl = null;

        //    }
        //    else
        //    {
        //        int startingPixel = crawlController.GetWidth()/2;

        //        int startX = startingPixel + (int)transform.position.x;
        //        int startY = startingPixel + (int)transform.position.y;


        //        runningCrawl.GrowByPosition(new Vector2(startX, startY));
        //    }
            
        //}


        void Flip()
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }




    }

    #endregion
}
