using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScoreText : MonoBehaviour
{
    [SerializeField] GameObject greedyAI;
    EpsilonGreedyAI aiScript;

    Text textComponent;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<Text>();
        aiScript = greedyAI.GetComponent<EpsilonGreedyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        textComponent.text = "Best Score: " + aiScript.GetBestScore();
    }
}
