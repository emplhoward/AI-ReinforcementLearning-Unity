using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int score; //Score when AI acts on the object.
    public int minScore; //Min of distribution.
    public int maxScore; //Max of distribution.

    public void PullLever(GameObject greedyAI) //AI will call this.
    {
        EpsilonGreedyAI epsilonGreedyScript = greedyAI.GetComponent<EpsilonGreedyAI>();

        GetComponent<PlayPlarticle>().PlayParticle(); //Show action.

        score = Random.RandomRange(minScore, maxScore); //Roll a score based on distribution.

        if (score > epsilonGreedyScript.GetBestScore()) //Check against the AI's memorized score.
        {
            epsilonGreedyScript.SetBestScore(score); //Set new best score for the AI if the score is high enough.
            epsilonGreedyScript.SetSlotInFocus(gameObject); //Set the choice as the best choice.
        }

        epsilonGreedyScript.SetLastScore(score); //For displaying purpose.
    }
}
