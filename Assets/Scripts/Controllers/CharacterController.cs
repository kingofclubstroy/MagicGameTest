using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    #region Private Fields

    [SerializeField]
    private Character character;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        character = new Character(gameObject, 100f);
        

    }

    // Update is called once per frame
    void Update()
    {

        // Need to see if the character is on a tile that automatically interacts with the character
        //WorldController.instance.interactWithTile(character, transform.position);

    }
}
