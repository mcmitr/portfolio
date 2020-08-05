using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * The PlantGrowth class is used to grow plant GameObjects in the scene when watered. It must be 
 * attached to the same object with the referenced particle system so that the listener will report 
 * collisions properly.
 * 
 */

public class PlantGrowth : MonoBehaviour
{
    // holds the particle system for the water effect
    public ParticleSystem wateringEffect;

    // holds a reference to the PlantSize script on plant gameobjects
    public PlantSize plantAttributes;

    //holds a list of particle collision events from the watering effect
    private List<ParticleCollisionEvent> waterCollisions = new List<ParticleCollisionEvent>();

    //looptracker keeps track of what iteration the growing loop is on, timesToGrow is the number of times 
    //the loop will iterate
    private int loopTracker;
    private int timesToGrow;


    /*
     * The OnParticleCollision method listens for the attached particle system to hit a gameobject
     * and makes a tagged plant object grown in size. Requires the plant object to have a collider,
     * rigidbody, and the PlantSize class attached to it. 
     * 
     */

    void OnParticleCollision(GameObject other)
    {
        //storing the number of times water particles have hit the plant
        timesToGrow = wateringEffect.GetCollisionEvents(other, waterCollisions);

        //getting script reference to use the plants scale and growth rate
        plantAttributes = other.GetComponent<PlantSize>();

        // the script needs to check if the particle effect is still playing, otherwise it will eventually
        // try to pull from a null value on dropletHits

        if (wateringEffect.isPlaying == true)
        {
            // setting loopTracker to 0 for the next while loop
            loopTracker = 0;

            // once the growth rate has been retrieved from the plant object, it can be used to make a new vector3
            // for easy addition
            float sizeAddition = plantAttributes.growthRate;
            Vector3 plantScaleChange = new Vector3(sizeAddition, sizeAddition, sizeAddition);


            while (loopTracker < timesToGrow)
            {

                if (other.gameObject.tag == "Plant" && other.transform.localScale.x <= plantAttributes.maxSize)
                {
                    other.transform.localScale += plantScaleChange;

                }// end if

                loopTracker++;

            }// end while

        }//end if

    } //end OnParticleCollision

}// end PlantGrowth
