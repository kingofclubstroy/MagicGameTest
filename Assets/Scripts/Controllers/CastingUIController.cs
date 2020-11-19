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

    [SerializeField]
    SpriteRenderer spriteRenderer;

    Texture2D texture;

    Vector2 screenCenter;

    [SerializeField]
    string[] SpellNames;

    Spell[] spells;

    CastingCircleScript firstCircle;

    int numberSpells;
    bool firstDeserialization = true;

    bool reset = true;

    List<(Element, int)> circlesToInstantiate = new List<(Element, int)>();

    [SerializeField]
    GameObject spellIconPrefab;

    [SerializeField]
    GameObject CastingCirclePrefab;

    Dictionary<Element, CastingCircleScript> castingElementDict = new Dictionary<Element, CastingCircleScript>();

    HashSet<Element> SpellTypes = new HashSet<Element>();

    bool textureInitialized = false;

    public List<Spell_Icon_Script> spellIcons = new List<Spell_Icon_Script>();

    public Spell_Icon_Script selectedSpell;

    public Vector2 positionToCast;

    bool casting = false;

    public CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        initializeCallbacks();
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        character = FindObjectOfType<CharacterController>();
        //character = GetComponent<CharacterController>();

        SpellHandler spellHandler = FindObjectOfType<SpellHandler>();

        spells = new Spell[SpellNames.Length];

        int index = 0;
        foreach(string s in SpellNames)
        {
            spells[index] = spellHandler.GetSpell(s);
            index++;
        }

        foreach(Spell spell in spells) {
            SpellTypes.Add(spell.spellParams.element);
            spell.color = getColor(spell.spellParams.element);
        }

    }

   

    // Update is called once per frame
    void Update()
    {
        updateCastingCircle();
    }

    void updateCastingCircle()
    {

        //TODO: remove delta time dependence, may want to do a 30-60 fps limit, and may want to go with integers and not floats
        if (casting)
        {

            UpdateCasting();

            //selectSpell();

        }
        

       
    }

    void updateElements()
    {

        //TODO: remove delta time, make fixed?
        float elementDifference = Time.deltaTime;

        foreach (Element element in castingElementDict.Keys)
        {

            CastingCircleScript castingElement = castingElementDict[element];

            float elementCasted = Mathf.Clamp(castingElement.amount, 0, 100);

            foreach (Spell_Icon_Script icon in spellIcons)
            {
                if (element == icon.spell.spellParams.element)
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

    void SpellSelectedTextureUpdate(SpellSelectedEvent spellSelected)
    {
        firstCircle.UpdateSpellSelectedLine(spellSelected.spell.direction, spellSelected.spell.elementColor); 
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
            DealWithElement(Element.FIRE, FireControllerScript.instance.GetNumberPixelsInCircle(positionToCast, 25, reset));
        }
        if (SpellTypes.Contains(Element.NATURE))
        {
            DealWithElement(Element.NATURE, CrawlController.instance.GetNumberPixelsInCircle(positionToCast, 25, reset));
        }
        if(SpellTypes.Contains(Element.WATER))
        {
            DealWithElement(Element.WATER, WaterControllerScript.instance.GetNumberPixelsInCircle(positionToCast, 25, reset));
        }

        circlesToInstantiate.Sort();
        circlesToInstantiate.Reverse();

        foreach((Element, int) c in circlesToInstantiate)
        {
            if (castingElementDict.ContainsKey(Element.NONE))
            {
                CastingCircleScript castingCircleScript = castingElementDict[Element.NONE];
                castingElementDict.Remove(Element.NONE);
                castingCircleScript.amount = c.Item2;
                castingElementDict.Add(c.Item1, castingCircleScript);
            }
            else
            {
                castingElementDict.Add(c.Item1, instantiateCastingCircleScript(c.Item2, castingElementDict.Count));
            }

            


            foreach (Spell spell in spells)
            {
                if (spell.spellParams.element == c.Item1)
                {
                    makeSpellIcon(spell, 25f);
                }
            }

        }

        if (castingElementDict.Count == 0)
        {
            castingElementDict.Add(Element.NONE, instantiateCastingCircleScript(0, 0));
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

        Vector2 postion = new Vector2(positionToCast.x, positionToCast.y);

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

    void makeSpellIcon(Spell spell, float distance)
    {

        //TODO: i am assuming an icon has a radius of 4

        Vector3 position;
        //TODO: fix this, broke so i could work on separating casing circles
        // finalTexture.width/2;

        switch (spellIcons.Count) {

            case 0:
                position = new Vector3(positionToCast.x + (-1 * distance), positionToCast.y + distance);
                break;

            case 1:
                position = new Vector3(positionToCast.x + distance, positionToCast.y + distance);
                break;

            case 2:
                position = new Vector3(positionToCast.x + distance, positionToCast.y + (-1 * distance));
                break;

            case 3:
                position = new Vector3(positionToCast.x + (-1 * distance), positionToCast.y + (-1 * distance));
                break;

            default:
                position = new Vector3(positionToCast.x + (-1 * distance), positionToCast.y + distance);
                break;


        }

        GameObject obj = Instantiate(spellIconPrefab, position, Quaternion.identity);

        //GameObject obj = Instantiate(spellIconPrefab, position, Quaternion.identity);

        Spell_Icon_Script icon = obj.GetComponent<Spell_Icon_Script>();

        icon.direction = spellIcons.Count;

        icon.SetElementNumber(GetElementNumber(spell.spellParams.element));

        icon.Initialize(spell, getColor(spell.spellParams.element), character);

        spellIcons.Add(icon);



    }

    int GetElementNumber(Element element)
    {
        switch(element)
        {
            case Element.FIRE:
                return 0;

            case Element.NATURE:
                return 1;

            case Element.WATER:
                return 2;

            default:
                return 0;
        }
    }

    //void OnValidate()
    //{
    //    if (firstDeserialization)
    //    {
    //        // This is the first time the editor properties have been deserialized in the object.
    //        // We take the actual array size.

    //        numberSpells = spells.Length;
    //        firstDeserialization = false;
    //    }
    //    else
    //    {
    //        // Something have changed in the object's properties. Verify whether the array size
    //        // has changed. If it has been expanded, initialize the new elements.
    //        //
    //        // Without this, new elements would be initialized to zero / null (first new element)
    //        // or to the value of the last element.

    //        if (spells.Length != numberSpells)
    //        {
    //            if (spells.Length > numberSpells)
    //            {
    //                for (int i = numberSpells; i < spells.Length; i++)
    //                    spells[i] = new Spell();
    //            }

    //            numberSpells = spells.Length;
    //        }
    //    }
    //}

    public List<Vector2> getPixelList()
    {
        if(firstCircle != null)
        {
            return firstCircle.getPixelList();
        }

        return null;
    }

    #region callback functions

    void initializeCallbacks()
    {
        SpellSelectedEvent.RegisterListener(SpellSelectedTextureUpdate);
        CastingLocationChangedEvent.RegisterListener(CastingLocationChanged);
        StoppedCastingEvent.RegisterListener(CastingStopped);
    }

    

    void CastingLocationChanged(CastingLocationChangedEvent changedLocation)
    {
        StopCasting();
        positionToCast = changedLocation.go.transform.position;
        casting = true;


    }

    void CastingStopped(StoppedCastingEvent e)
    {
        StopCasting();
    }

    void StopCasting()
    {
        reset = true;
        //TODO: need to group these actions together
        CrawlController.instance.clearCasting();
        FireControllerScript.instance.clearCasting();

        foreach (CastingCircleScript element in castingElementDict.Values)
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
        unselected.Description = "Spell unselected callback firing";
        unselected.FireEvent();

        casting = false;
    }

    void UpdateCasting()
    {
        float elementDifference = 0;
        elementDifference = Time.deltaTime;

        GetSurroundingElementsPixels();

        updateElements();
    }

    #endregion

}
