using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "I_", menuName = "Interactable/Item", order = 1)]
public class SO_Item : SO_Interactable
{
    [Header("Items specific")]
    public string itemName;
    public int weaponNumber;

}
