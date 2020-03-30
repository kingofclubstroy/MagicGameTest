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


    #endregion

    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

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
            } else
            {
                animator.SetBool("Walking_Up", false);
                animator.SetBool("Walking_Down", false);
                animator.SetBool("Walking_Left", false);
            }

            Vector3 tempVect = new Vector3(h, v, 0);
            tempVect = tempVect.normalized * speed * Time.deltaTime;

            this.transform.position += tempVect;
        }

        //TODO: remove
        if(Input.GetKeyDown("space"))
        {
            crawlController.CreateCrawl(transform.position);
            //crawlController.CreateCrawl(transform.position, 1000, 1000);
        }

        if(Input.GetKeyDown("r"))
        {

            crawlController.AddFire(transform.position);

        }

        if(Input.GetKeyDown("e"))
        {

            if(runningCrawl == null)
            {
                runningCrawl = crawlController.CreateCrawl(transform.position);
                runningStartPosition = transform.position;
                runningStartTime = Time.time;
            } 




        } else if(runningCrawl != null)
        {

            if (Time.time - runningStartTime >= 3)
            {
                runningCrawl = null;

            }
            else
            {
                int startingPixel = crawlController.GetWidth()/2;

                int startX = startingPixel + (int)transform.position.x;
                int startY = startingPixel + (int)transform.position.y;


                runningCrawl.GrowByPosition(new Vector2(startX, startY));
            }
            
        }





        

    }

    #endregion
}
