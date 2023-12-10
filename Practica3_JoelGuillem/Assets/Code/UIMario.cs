using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMario : MonoBehaviour
{
    [Header("Coins")]
    public Animation m_CoinsAnimation;
    public AnimationClip m_InCoinsAnimation;
    public AnimationClip m_OutCoinsAnimation;
    public Text m_CoinsText;
    public float m_ShowCoinsTime = 2f;
    bool m_CoinsAnimationStarting;


    //int m_CoinsDebug = 0;
    [Header("Life")]
    public Image m_LifeBarImage;
    public Animation m_LifeAnimation;
    public AnimationClip m_InLifeAnimation;
    public AnimationClip m_OutLifeAnimation;
    public float m_ShowLifeTime = 2f;
    bool m_LifeAnimationStarting;

    public Text m_LifesText;


    private void Start()
    {
        DependencyInjector.GetDependency<IScoreManager>().scoreChangedDelegate += UpdateScore;
    }
    private void OnDestroy()
    {
        DependencyInjector.GetDependency<IScoreManager>().scoreChangedDelegate -= UpdateScore;
    }

    void UpdateScore(IScoreManager scoreManager)
    {
        m_CoinsText.text = scoreManager.getPoints().ToString();
        StartCoroutine(ShowCoinsAnimation());
    }

    IEnumerator ShowCoinsAnimation()
    {
        if (!m_CoinsAnimationStarting)
        {
            m_CoinsAnimationStarting = true;
            m_CoinsAnimation.CrossFade(m_InCoinsAnimation.name);
            yield return new WaitForSeconds(m_ShowCoinsTime / 2);
            m_CoinsAnimationStarting = false;
            yield return new WaitForSeconds(m_ShowCoinsTime / 2);
            if (m_CoinsAnimationStarting == false)
                m_CoinsAnimation.CrossFade(m_OutCoinsAnimation.name);
        }
    }
    public void ShowLife(int Life)
    {
        float l_LifePct = Life / 8f;
        m_LifeBarImage.fillAmount = l_LifePct;
        StartCoroutine(ShowLifeAnimation());
    }

    IEnumerator ShowLifeAnimation()
    {
        if (!m_LifeAnimationStarting)
        {
            m_LifeAnimationStarting = true;
            m_LifeAnimation.CrossFade(m_InLifeAnimation.name);
            yield return new WaitForSeconds(m_ShowLifeTime / 2);
            m_LifeAnimationStarting = false;
            yield return new WaitForSeconds(m_ShowLifeTime / 2);
            if (m_LifeAnimationStarting == false)
                m_LifeAnimation.CrossFade(m_OutLifeAnimation.name);

        }

    }

    public void UpdateLifeText(int Life)
    {
        m_LifesText.text = Life.ToString();
    }
}
