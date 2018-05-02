using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public List<Ability> allAbilities;
    public List<Ability> activeAbilities;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //Place holder for active abilities
        for (int i = 0; i < allAbilities.Capacity; i++)
        {
            activeAbilities.Add(allAbilities[i]);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
