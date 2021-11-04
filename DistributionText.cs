using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistributionText : MonoBehaviour
{
    Text textComponent;
    Slot slot;
    // Start is called before the first frame update
    void Awake()
    {
        textComponent = gameObject.GetComponent<Text>();

        slot = transform.root.gameObject.GetComponent<Slot>();
    }

    private void Start()
    {
        textComponent.fontSize = 12;
    }

    // Update is called once per frame
    void Update()
    {
        textComponent.text = "Hidden Distribution:" + "\n" + "Min: " + slot.minScore + "\n" + "Max: " + slot.maxScore;
    }
}
