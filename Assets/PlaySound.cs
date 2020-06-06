using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

//plays sound with respect to speed
public class PlaySound : StateMachineBehaviour
{
    AudioSource audioSource;
    TMP_InputField inputField;
    float secperBeat;
    AndroidJavaClass jc;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.GetComponent<AudioSource>();
        inputField = animator.GetComponent<AdjustSpeed>().inputField;
        secperBeat = 60/(float.Parse(inputField.text));
        float lengthOfAudio = audioSource.clip.length;
        jc = animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
        jc.CallStatic("audioPlayer", lengthOfAudio / secperBeat);
    }


    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (inputField.isFocused == true)
        {
            jc.CallStatic("stopMp");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
