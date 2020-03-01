using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animator : Event_Controller
{ 
    
    public static Animator playerAnimator = new Animator();

    private bool isStillCrawling, isMotionCrawling;
    void Start()
    {
        playerAnimator = transform.GetComponent<Animator>();
        animatorConditions();
    }

    
    // state and change boolean to run animator condition logic
    private void animatorConditions()
    {
        // distinguish between idle crawling and movement crawling
         isStillCrawling =  crawling && ! walking && ! running ? true : false;
        isMotionCrawling =  crawling && 
         (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S)) ? true : false;

        // set boolean for animator logic.
         walkEvent += () =>
        {
            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isJumping",  jumping);
            playerAnimator.SetBool("isCrawling",  crawling);
        };
         runEvent += () =>
        {
            playerAnimator.SetBool("isRunning", true);
            playerAnimator.SetBool("isJumping",  jumping);
            playerAnimator.SetBool("isWalking", false);
        };
         jumpEvent += () => playerAnimator.SetBool("isJumping", true);
         crawlEvent += () =>
        {
            playerAnimator.SetBool("isCrawling", true);
            playerAnimator.SetBool("isWalking",  walking);
        };

         idleEvent += () =>
        {
            playerAnimator.SetBool("isWalking", false);
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isJumping", false);
            playerAnimator.SetBool("isCrawling", false);
            playerAnimator.SetBool("isStillCrawling", false);
        };
    }
}
