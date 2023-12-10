using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject m_PickParticles;

    public void Pick()
    {
        GameObject l_Particles = Instantiate(m_PickParticles, transform.position, transform.rotation);
        l_Particles.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
        DependencyInjector.GetDependency<IScoreManager>().addPoints(1);
    }
}
