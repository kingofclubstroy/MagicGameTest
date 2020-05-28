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
        initializeCallbacks();
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

                Debug.Log("spell clicked");

                if(spellIcon != selectedSpell)
                {
                    if (selectedSpell != null)
                    {
                        //set the previously selected spell to unselected
                        selectedSpell.Selected = false;
                    }

                    SpellSelectedEvent spellSelected = new SpellSelectedEvent();

                    spellSelected.spell = spellIcon;

                    spellSelected.Description = "Spell selected callback firing";

                    spellSelected.FireEvent();

                    selectedSpell = spellIcon;

                    spellIcon.Selected = true;
                }

            }


        }

    }

    #region callback functions
    void initializeCallbacks()
    {
        //TODO: might not want to cast the spell from here, but will work for now
        SpellCastCall.RegisterListener(CastSpell);
    }

    void CastSpell(SpellCastCall e)
    {
        if (selectedSpell == null || !selectedSpell.Charged)
        {
            Debug.Log("selected spell = " + selectedSpell);
            return;
        }

        switch (selectedSpell.spell.getElement())
        {
            case CastingUIController.Element.NATURE:
                CrawlController.instance.ConsumeCrawl(transform.position, selectedSpell.spell.getCastingCost() * 10, selectedSpell.spell.getCastingCost());
                break;

            //TODO: make one for each element
            default:
                return;
        }

        StopCastingCall ev = new StopCastingCall();
        ev.FireEvent();
    }

    #endregion
}
