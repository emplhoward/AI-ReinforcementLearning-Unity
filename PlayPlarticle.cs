using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPlarticle : MonoBehaviour
{
    [SerializeField] ParticleSystem scoringParticle;

    public void PlayParticle()
    {
        scoringParticle.Play();
    }
}
