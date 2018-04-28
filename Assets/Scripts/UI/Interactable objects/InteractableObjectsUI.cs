using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class InteractableObjectsUI : MonoBehaviour
{

    SO_Item item;
    TextMeshProUGUI text;

    private void Start()
    {
        item = GetComponentInParent<InteractableObject>()._Item;
        text = GetComponentInChildren<TextMeshProUGUI>();

        IntializeText();
    }

    private void Update()
    {
        Vector3 v = Camera.main.transform.position - transform.position;

        v.x = v.z = 0.0f;

        transform.LookAt(Camera.main.transform.position - v);

        transform.rotation = (Camera.main.transform.rotation); // Take care about camera rotation
    }

    void IntializeText()
    {
        text.text = item.itemName + "\n" + "Value: " + item.value + "$";
    }

}
