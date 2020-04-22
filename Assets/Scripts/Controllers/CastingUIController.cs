using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingUIController : MonoBehaviour
{

    [SerializeField]
    float decayRate = 10f;

    [SerializeField]
    float maxElement;

    [SerializeField]
    float maxCastRate;

    [SerializeField]
    Texture2D spellTexture;

    Vector2 screenCenter;

    [SerializeField]
    SpellTest[] spells;

    CastingCircleScript firstCircle;

    int numberSpells;
    bool firstDeserialization = true;

    bool reset = true;

    List<(Element, int)> circlesToInstantiate = new List<(Element, int)>();

    public enum Element
    {
        FIRE,
        NATURE,
        EARTH,
        WIND,
        WATER
    }

    [SerializeField]
    GameObject spellIconPrefab;

    [SerializeField]
    GameObject CastingCirclePrefab;

    Dictionary<Element, CastingCircleScript> castingElementDict = new Dictionary<Element, CastingCircleScript>();

    HashSet<Element> SpellTypes = new HashSet<Element>();

    public List<Spell_Icon_Script> spellIcons = new List<Spell_Icon_Script>();

    public Spell_Icon_Script selectedSpell;

    // Start is called before the first frame update
    void Start()
    {

        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        foreach(SpellTest spell in spells) {
            SpellTypes.Add(spell.getElement());
            spell.color = getColor(spell.getElement());
        }

    }

   

    // Update is called once per frame
    void Update()
    {
        updateCastingCircle();
    }

    void selectSpell()
    {

        //Get the quadrent of the circle the mouse is in
        Vector2 positionCentered = (Vector2) Input.mousePosition - screenCenter;
        Vector2 quadrent = new Vector2(Mathf.Sign(positionCentered.x), Mathf.Sign(positionCentered.y));

        Spell_Icon_Script currentSelectedSpell = null;

        //Now find out what this means and update the selected spell
        if(quadrent.x == -1)
        {
            if(quadrent.y == 1)
            {
                //TopLeft quadrent
                if(spellIcons.Count >= 1)
                {
                    currentSelectedSpell = spellIcons[0];
                }
            } else
            {
                //BottomLeft quadrent
                if(spellIcons.Count >= 4)
                {
                    currentSelectedSpell = spellIcons[3];
                }
            }
        } else
        {
            if(quadrent.y == 1)
            {
                //TopRight quadrent
                if(spellIcons.Count >= 2)
                {
                    currentSelectedSpell = spellIcons[1];
                }
            } else
            {
                //BottomRight quadrent
                if(spellIcons.Count >= 3)
                {
                    currentSelectedSpell = spellIcons[2];
                }
            }
        }

        if(currentSelectedSpell != null)
        {
            if(currentSelectedSpell == selectedSpell)
            {
                //we already have this spell selected, so we can return
                return;
            }else
            {
                if(selectedSpell != null)
                {
                    selectedSpell.unselect();
                }

                currentSelectedSpell.select();
                selectedSpell = currentSelectedSpell;
            }


        }

    }

    void updateCastingCircle()
    {

        float elementDifference = 0;

        //TODO: remove delta time dependence, may want to do a 30-60 fps limit, and may want to go with integers and not floats
        if (Input.GetKey(KeyCode.Space))
        {

            elementDifference = Time.deltaTime;

            GetSurroundingElementsPixels();

            updateElements();

            //selectSpell();

        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            reset = true;
            //TODO: need to group these actions together
            CrawlController.instance.clearCasting();
            FireControllerScript.instance.clearCasting();

            foreach(CastingCircleScript element in castingElementDict.Values)
            {
                element.initializeDestruction();
                
            }

            foreach (Spell_Icon_Script spell in spellIcons)
            {
                spell.initializeDestruction();
            }

            spellIcons.Clear();

            castingElementDict.Clear();

            firstCircle = null;

            SpellUnSelectedEvent unselected = new SpellUnSelectedEvent();
            unselected.FireEvent();

        }

       
    }

    void updateElements()
    {

        //TODO: remove delta time, make fixed?
        float elementDifference = Time.deltaTime;

        foreach (Element element in castingElementDict.Keys)
        {

            CastingCircleScript castingElement = castingElementDict[element];

            float tempElementDifference = 0;
          
            //TODO: may want to change these numbers, but they will work for now
            //Setting the amount casted to be related to the ratio amount of element present and the theiritical max amount that could be,
            // multiplied by the max cast speed im temporarly setting as 50 (maybe there are augments that improve this)
            tempElementDifference = ((castingElement.updateAmount / 900f) * 50f) * elementDifference;

            if (element == Element.FIRE)
            {
                tempElementDifference *= 3;
            }

            
            float elementCasted = Mathf.Clamp(castingElement.amount + tempElementDifference, 0, 100);

            if (elementCasted > castingElement.updateAmount && elementDifference > 0)
            {
                //The amount of the element casted is more than the amount of element surrounding the player, so lets set tot he amount around
                elementCasted -= Mathf.Clamp(elementCasted - castingElement.updateAmount, 0, 1);
            }

            foreach (Spell_Icon_Script icon in spellIcons)
            {
                if (element == icon.spell.getElement())
                {
                    icon.updateSpellIcon(elementCasted);
                }
            }

            castingElement.amount = elementCasted;

            float percent = (elementCasted / 100f);

            //updates the casting circle color
            castingElement.updateCastingCircleTexture(percent, getColor(element));

        }
        //if (allZero)
        //{

        //    foreach (Spell_Icon_Script icon in spellIcons)
        //    {
        //        icon.destroy();
        //    }
        //    //all of the casting elements have decayed to 0, so lets clear the list
        //    spellIcons.Clear();
        //    selectedSpell = null;

        //    //Need to tell everyone that the spell is unselected
        //    SpellSelectedEvent spellSelectedEvent = new SpellSelectedEvent();
        //    spellSelectedEvent.selected = false;

        //    spellSelectedEvent.FireEvent();

        //    bool keepUpdating = false;

        //    foreach(Casting)

        //    castingElementDict.Clear();



        //    running = false;
        //}
        
    }

    Color getColor(Element element)
    {

        switch(element)
        {
            case Element.FIRE:
                return new Color(0.94f, 0.38f,0.38f);

            case Element.NATURE:
              
                return new Color(0.2f, 0.85f, 0.5f);

            case Element.WATER:
                return new Color(0.28f, 0.53f, 1f);

            case Element.EARTH:
                //brown
                return new Color(204f/255f, 148f/255f, 115f/255f);

            case Element.WIND:
                return Color.white;
        }


        return Color.white;

    }

    void GetSurroundingElementsPixels()
    {

        if (SpellTypes.Contains(Element.FIRE))
        {
            DealWithElement(Element.FIRE, FireControllerScript.instance.GetNumberPixelsInCircle(this.transform.position, 25, reset));
        }
        if (SpellTypes.Contains(Element.NATURE))
        {
            DealWithElement(Element.NATURE, CrawlController.instance.GetNumberPixelsInCircle(this.transform.position, 25, reset));
        }

        circlesToInstantiate.Sort();
        circlesToInstantiate.Reverse();

        foreach((Element, int) c in circlesToInstantiate)
        {
            castingElementDict.Add(c.Item1, instantiateCastingCircleScript(c.Item2, castingElementDict.Count));
            foreach (SpellTest spell in spells)
            {
                if (spell.getElement() == c.Item1)
                {
                    makeSpellIcon(spell, 25f);
                }
            }

        }
        circlesToInstantiate.Clear();
        //DealWithElement(CastingElements.Element.EARTH, //TODO:::) 
        //DealWithElement(CastingElements.Element.WATER, //TODO:::)
        //DealWithElement(CastingElements.Element.WIND, //TODO:::)

        reset = false;

    }

    void DealWithElement(Element element, int pixels)
    {
        if(castingElementDict.ContainsKey(element))
        {
            castingElementDict[element].addTempAmount(pixels);
        } else if(pixels > 0)
        {

            circlesToInstantiate.Add((element, pixels));
            
        }

    }

    CastingCircleScript instantiateCastingCircleScript(int pixels, int sortingOrder)
    {

        Vector2 postion = new Vector2(transform.position.x, transform.position.y);

        postion.y += castingElementDict.Count * 3;

        GameObject castingCircle = Instantiate(CastingCirclePrefab, postion, Quaternion.identity);

        CastingCircleScript castingCircleScript = castingCircle.GetComponent<CastingCircleScript>();

        castingCircleScript.sortingOrder = sortingOrder;

        castingCircleScript.updateAmount = pixels;


        if(castingElementDict.Count == 0)
        {
            castingCircleScript.isFirst = true;
            firstCircle = castingCircleScript;
        }

        //castingCircleScript.spriteRenderer.sortingOrder = castingElementDict.Count;

        return castingCircleScript;

    }


    // void GetSurroundingElements()
    // {

    //    List<TileScript> neighbouringTiles = WorldController.instance.findNeighbours(WorldController.instance.GetTilePositionFromWorld(this.transform.parent.position), true);

    //    bool startOfCast = false;

    //    //Check to see if we have an empty element list, so we know we will have to sort the elements based on casting speed
    //    if (elementList.Count == 0)
    //    {
            
    //        startOfCast = true;
    //    }

    //    foreach (TileScript tile in neighbouringTiles)
    //    {

    //        if (tile != null)
    //        {

    //            if (tile.fire > 0)
    //            {
    //                CastingElements ele = null;

    //                foreach (CastingElements e in elementList)
    //                {
    //                    if (e.element == CastingElements.Element.FIRE)
    //                    {
    //                        ele = e;
    //                        break;
    //                    }
    //                }

    //                if (ele != null)
    //                {
    //                    ele.addTempAmount(tile.fire);
    //                }
    //                else
    //                {
    //                    CastingElements c = new CastingElements(CastingElements.Element.FIRE);
    //                    c.addTempAmount(tile.fire);
    //                    elementList.Add(c);
    //                    makeSpellIcon(CastingElements.Element.FIRE);

                        
    //                }
    //            }

    //            if (tile.fuel > 0)
    //            {
    //                CastingElements ele = null;

    //                foreach (CastingElements e in elementList)
    //                {
    //                    if (e.element == CastingElements.Element.NATURE)
    //                    {
    //                        ele = e;
    //                        break;
    //                    }
    //                }

    //                if (ele != null)
    //                {
    //                    ele.addTempAmount(tile.fuel);
    //                }
    //                else
    //                {

    //                    CastingElements c = new CastingElements(CastingElements.Element.NATURE, 0);
    //                    c.addTempAmount(tile.fuel);
    //                    elementList.Add(c);
    //                    makeSpellIcon(CastingElements.Element.NATURE);
    //                }
    //            }
    //        }

    //        //TODO: add looking for other elements

    //    }

    //    if(startOfCast && elementList.Count > 1)
    //    {
           
    //        //We didnt have any elements in the list before we searched and now we have multiple that we need to sort, so lets sort the list
    //        elementList.Sort((p1, p2) => p1.updateAmount.CompareTo(p2.updateAmount));
            
    //    }

    //    if(elementList.Count == 0)
    //    {
    //        Debug.Log("no elements around");
    //    }
    //}

    void makeSpellIcon(SpellTest spell, float distance)
    {

        //TODO: i am assuming an icon has a radius of 4

        Vector3 position;
        //TODO: fix this, broke so i could work on separating casing circles
        // finalTexture.width/2;

        switch (spellIcons.Count) {

            case 0:
                position = new Vector3(this.transform.position.x + (-1 * distance), this.transform.position.y + distance);
                break;

            case 1:
                position = new Vector3(this.transform.position.x + distance, this.transform.position.y + distance);
                break;

            case 2:
                position = new Vector3(this.transform.position.x + distance, this.transform.position.y + (-1 * distance));
                break;

            case 3:
                position = new Vector3(this.transform.position.x + (-1 * distance), this.transform.position.y + (-1 * distance));
                break;

            default:
                position = new Vector3(this.transform.position.x + (-1 * distance), this.transform.position.y + distance);
                break;


        }

        GameObject obj = Instantiate(spellIconPrefab, position, Quaternion.identity);

        //GameObject obj = Instantiate(spellIconPrefab, position, Quaternion.identity);

        Spell_Icon_Script icon = obj.GetComponent<Spell_Icon_Script>();

        icon.Initialize(spell, getColor(spell.getElement()));

        spellIcons.Add(icon);



    }

    void OnValidate()
    {
        if (firstDeserialization)
        {
            // This is the first time the editor properties have been deserialized in the object.
            // We take the actual array size.

            numberSpells = spells.Length;
            firstDeserialization = false;
        }
        else
        {
            // Something have changed in the object's properties. Verify whether the array size
            // has changed. If it has been expanded, initialize the new elements.
            //
            // Without this, new elements would be initialized to zero / null (first new element)
            // or to the value of the last element.

            if (spells.Length != numberSpells)
            {
                if (spells.Length > numberSpells)
                {
                    for (int i = numberSpells; i < spells.Length; i++)
                        spells[i] = new SpellTest();
                }

                numberSpells = spells.Length;
            }
        }
    }

    public List<Vector2> getPixelList()
    {
        if(firstCircle != null)
        {
            return firstCircle.getPixelList();
        }

        return null;
    }

}
