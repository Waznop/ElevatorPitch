using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonFactory : MonoBehaviour
{

    public Sprite[] Hairs;
    public Sprite[] Accessories;
    public Sprite[] Outers;
    public Sprite[] Tops;
    public Sprite[] Bottoms;

    public Sprite Eyes;
    public Sprite Shoes;
    public Sprite Skin;

    public GameObject PersonPrefab;

    const float hairProb = 0.95f;
    const float outerProb = 0.5f;
    const float accProb = 0.5f;

    public GameObject CreatePerson()
    {
        GameObject person = Instantiate(PersonPrefab);
        PersonScript ps = person.GetComponent<PersonScript>();

        ps.Eyes = Eyes;
        ps.Shoes = Shoes;
        ps.Skin = Skin;

        if (Random.value < hairProb) ps.Hair = Hairs[Random.Range(0, Hairs.Length)];
        if (Random.value < accProb) ps.Accessory = Accessories[Random.Range(0, Accessories.Length)];
        if (Random.value < outerProb) ps.Outer = Outers[Random.Range(0, Outers.Length)];
        ps.Top = Tops[Random.Range(0, Tops.Length)];
        ps.Bottom = Bottoms[Random.Range(0, Bottoms.Length)];

        return person;
    }

}
