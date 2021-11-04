using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EpsilonGreedyAI : MonoBehaviour
{
    float epsilon = 1;  //Represent the factor that decides whether the AI will be exploring the choices, or exploiting the known best choice.
    float epsilonDecrease = 0.01f; //Represent the decrease in epsilon for each time step.
    [SerializeField] GameObject[] slotArray; //Holds the various bushes, or choices.
    GameObject slotInFocus; //The current choice in focus for a time step.
    int currentBestScore = 0; //The current best score that the AI remembers.
    int lastScore;  //The score from the last time step, used for display.
    float decisionTimer;    //The timer for deciding on exploration or exploitation.
    float timeToDecide = 6f; //The time taken to decide.
    [SerializeField] Text counterText;  //Test to display.
    [SerializeField] GameObject eagle;  //The AI's game object.
    Vector3 eagleOffset = new Vector3(0, .75f, 0);  //Used to move the eagle around.

    private void Start()
    {
        slotInFocus = slotArray[0]; //We start with the first choice.
    }

    private void Update()
    {
        if(decisionTimer >= 0) decisionTimer -= Time.deltaTime; //Count down the time for making a decision.

        if(decisionTimer < 0)
        {
            DecideExplore();
            decisionTimer = timeToDecide;
        }

        counterText.text = "Decision timer: " + decisionTimer.ToString("F2");
    }

    void DecideExplore()
    {
        float decideFactor = Random.Range(0f, 1f); //The AI will decide to explore various choices or exploit the known best choice.

        if(epsilon > 0) epsilon -= epsilonDecrease; //Epsilon will decrease overtime. This means the AI will be actively exploring at first, before eventually it settles on exploiting.

        if (decideFactor < epsilon) Explore(); //Condition for exploration or exploitation.
        else Exploit();
    }

    void Explore() //If it decides to explore, it will choose a random choice and act upon the choice (pulling a lever, catching a prey, etc, any actions).
    {
        int randomSlot = Random.RandomRange(0, slotArray.Length);
        slotArray[randomSlot].GetComponent<Slot>().PullLever(gameObject);
        eagle.transform.position = slotArray[randomSlot].transform.position + eagleOffset;
    }

    void Exploit() //If the AI decides to exploit, it will settle on the best known choice and act upon it (pull a lever in MAB term).
    {
        slotInFocus.GetComponent<Slot>().PullLever(gameObject);
        eagle.transform.position = slotInFocus.transform.position + eagleOffset;
    }

    public void SetSlotInFocus(GameObject newSlotInFocus) //Set the new choice as the best choice.
    {
        slotInFocus = newSlotInFocus;
    }

    public int GetBestScore() //Utility.
    {
        return currentBestScore;
    }

    public void SetBestScore(int newBestScore) //Utility.
    {
        currentBestScore = newBestScore;
    }

    public void SetLastScore(int newLastScore) //Utility.
    {
        lastScore = newLastScore;
    }

    public int GetLastScore() //Utility.
    {
        return lastScore;
    }
}
