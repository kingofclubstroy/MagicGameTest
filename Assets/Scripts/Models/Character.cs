using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : ITakeDamage
{

    #region Private Fields

    [SerializeField]
    float health;

    GameObject gameObject;

    #endregion


    #region Constructor
    public Character(GameObject gameObject, float health)
    {
        this.gameObject = gameObject;
        this.health = health;
    }

    #endregion

    #region ITakeDamage Methods

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            
            // Unit is dead, so need to tell anything that cares about it before the object is destroyed

            UnitDeathEvent deathEvent = new UnitDeathEvent();
            deathEvent.UnitGO = gameObject;

            deathEvent.FireEvent();

            GameObject.Destroy(gameObject);

        }
             
    }

    #endregion
}
