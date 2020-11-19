using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(SpellHandler))]
public class SpellHandlerEditor : Editor
{

    private SerializedObject TestSO;
    private SpellHandler myTestData;

    [SerializeField]
    private List<bool> showSpellData = new List<bool>();

    void OnEnable()
    {
        TestSO = new SerializedObject(target);
        myTestData = (SpellHandler)target;

        foreach (Spell spell in myTestData.spells)
        {
            showSpellData.Add(false);
        }
    }

    public override void OnInspectorGUI()
    {
        TestSO.Update();

        EditorGUILayout.BeginVertical();

        int i = 0;

        for (int index = 0; index < myTestData.spells.Count; index++)
        {
            Spell spell = myTestData.spells[i];

            showSpellData[i] = EditorGUILayout.Foldout(showSpellData[i], (spell.name != null && spell.name.Length > 0) ? spell.name : "Spell #" + i);
            if (showSpellData[i])
            {
                SpellParameters sp = spell.spellParams;

                spell.name = EditorGUILayout.TextField("Name:", spell.name);

                spell.spellType = (SpellTypes)EditorGUILayout.EnumPopup("Spell Type:", spell.spellType);

                sp.element = (Element)EditorGUILayout.EnumPopup("Element:", sp.element);

                sp.elementCost = EditorGUILayout.IntField("Element Cost:", sp.elementCost);

                sp.staminaCost = EditorGUILayout.IntField("Stamina Cost:", sp.staminaCost);

                sp.castTime = EditorGUILayout.FloatField("Cast Time:", sp.castTime);

                sp.damage = EditorGUILayout.IntField("Damage:", sp.damage);

               

                switch(sp.element)
                {
                    case Element.FIRE:
                        sp.fireStrength = EditorGUILayout.IntField("Fire Strength:", sp.fireStrength);
                        break;

                    case Element.EARTH:
                        sp.earthStrength = EditorGUILayout.IntField("Earth Strength:", sp.earthStrength);
                        break;

                    case Element.NATURE:
                        sp.natureStrength = EditorGUILayout.IntField("Nature Strength:", sp.natureStrength);
                        break;

                    case Element.WATER:
                        sp.waterStrength = EditorGUILayout.IntField("Water Strength:", sp.waterStrength);
                        break;
                }

                spell.castingBehaviour = (ICast) EditorGUILayout.ObjectField("Casting Behaviour", spell.castingBehaviour, typeof(ICast), true);

                if(spell.castingBehaviour != null)
                {
                    Debug.Log(spell.castingBehaviour.name);
                    switch(spell.castingBehaviour.name)
                    {
                        case "ICastProjectileObject":

                            EditorGUILayout.TextArea("Projectile Params:");

                            sp.projectileObject = (GameObject)EditorGUILayout.ObjectField("Projectile Object:", sp.projectileObject, typeof(GameObject), true);
                            sp.collisionBehaviour = (ICast)EditorGUILayout.ObjectField("Collision Behaviour:", sp.collisionBehaviour, typeof(ICast), true);
                            sp.maxRangeBehaviour = (ICast)EditorGUILayout.ObjectField("Max Range Behaviour:", sp.maxRangeBehaviour, typeof(ICast), true);
                            sp.projectileSpeed = EditorGUILayout.IntField("Projectile Speed:", sp.projectileSpeed);
                            sp.maxRange = EditorGUILayout.IntField("Max Range:", sp.maxRange);
                            sp.updateBehaviour = (ICast)EditorGUILayout.ObjectField("Update Behaviour:", sp.updateBehaviour, typeof(ICast), true);

                            sp.elementAmount = EditorGUILayout.IntField("Element amount:", sp.elementAmount);

                            switch(sp.element)
                            {
                                case Element.NATURE:
                                    sp.maxCrawl = EditorGUILayout.IntField("Max crawl:", sp.maxCrawl);
                                    sp.growthRate = EditorGUILayout.FloatField("Growth Rate:", sp.growthRate);
                                    break;
                            }

                            break;

                    }
                }

                if(GUILayout.Button("Reset Parameters", GUILayout.Width(150)))
                {
                    myTestData.spells[index] = new Spell();
                }

                if (GUILayout.Button("Remove Spell", GUILayout.Width(150)))
                {
                    myTestData.spells.RemoveAt(index);
                    showSpellData.RemoveAt(index);
                }

            }
            i++;
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Add Spell"))
        {
            myTestData.spells.Add(new Spell());
            showSpellData.Add(false);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(myTestData);
        }

        TestSO.ApplyModifiedProperties();
    }
}