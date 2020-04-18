using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    #region Private Fields

    [SerializeField]
    private Character character;

    CastingUIController CastingUIController;

    Camera mainCam { get { return Camera.main.GetComponent<Camera>(); } }
    bool mainCamFound { get { return mainCam != null; } }

    Spell_Icon_Script selectedSpell;
   


    


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        character = new Character(gameObject, 100f);
        CastingUIController = GetComponentInChildren<CastingUIController>();
        

    }

    // Update is called once per frame
    void Update()
    {

        //TODO: move this to an interaction controller or something, it doesnt need to be here, but it will do for now
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit2D hi = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -1f));

            if (hi.collider != null)
            {
                Spell_Icon_Script spellIcon = hi.transform.GetComponent<Spell_Icon_Script>();

                if(spellIcon != selectedSpell)
                {
                    if (selectedSpell != null)
                    {
                        selectedSpell.unselect();
                    }

                    SpellSelectedEvent spellSelected = new SpellSelectedEvent();

                    //spellSelected.spell = spellIcon.spell;

                    spellSelected.FireEvent();

                    spellIcon.select();
                    selectedSpell = spellIcon;
                }

            }


        }

    }
}
