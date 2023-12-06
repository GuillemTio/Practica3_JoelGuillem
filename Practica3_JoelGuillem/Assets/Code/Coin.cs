using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public void Pick()
    {
        gameObject.SetActive(false);
        DependencyInjector.GetDependency<IScoreManager>().addPoints(1);
    }
}
