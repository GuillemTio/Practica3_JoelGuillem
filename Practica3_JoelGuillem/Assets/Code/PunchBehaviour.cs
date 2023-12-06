using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    public MarioController.TPunchHitColliderType m_PunchType;
    [Range(0f, 1f)]
    public float m_StartPunchPct;
    [Range(0f, 1f)]
    public float m_EndPunchPct;
    MarioController m_MarioController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioController = animator.GetComponent<MarioController>();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool l_Active = stateInfo.normalizedTime >= m_StartPunchPct && stateInfo.normalizedTime <= m_EndPunchPct;
        m_MarioController.SetPunchHitColliderActive(m_PunchType, l_Active);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioController.SetPunchHitColliderActive(m_PunchType, false);
        m_MarioController.SetIsOnPunch(false);
    }

}
