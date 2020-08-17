using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

//plays sound with respect to speed
public class PlaySound : StateMachineBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void IOSPlaySound(float speed, String tag);
    //[DllImport("__Internal")]
    //private static extern void IOSChangeSpeed(float speed);
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
    [DllImport("__Internal")]
    private static extern bool IOSIsPlaying();
    [DllImport("__Internal")]
    private static extern float IOSGetSpeed();

#endif
    AudioSource audioSource;
    TMP_InputField inputField;
    float secperBeat;
    AndroidJavaObject jc;
    int counter;
    bool isMuted;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isMuted = animator.GetComponent<SoundFuncs>().mute;
        audioSource = animator.GetComponent<AudioSource>();
        inputField = animator.GetComponent<AdjustSpeed>().inputField;
        secperBeat = 60 / (float.Parse(inputField.text));
        float lengthOfAudio = audioSource.clip.length;
#if UNITY_ANDROID
            jc = animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
            jc.Call("metroSetBpm", int.Parse(inputField.text));
            bool isPlaying = jc.Get<bool>("isplaying");
            if (!isPlaying)
            {
                jc.Call("playSound");
            }
#endif

#if UNITY_IOS
            float currspeed = IOSGetSpeed();
            float speed = float.Parse(inputField.text);
            if (stateInfo.IsTag("LastTapKhanda"))
            {
                speed *= 2;
            }
            speed *= stateInfo.speed;
            bool isPlaying = IOSIsPlaying();
            if (!isMuted && (!isPlaying || (speed != currspeed)))
            {
                if (speed != currspeed && !stateInfo.IsTag("LastTapKhanda")) { IOSStopSound();}
                if (stateInfo.IsTag("MisraTag")) { IOSPlaySound(speed, "MisraTag");}
                else { IOSPlaySound(speed, "");}
                
            }
#endif

    }


    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isMuted = animator.GetComponent<SoundFuncs>().mute;
        //if (isMuted == true)
        //    IOSStopSound();
        #if UNITY_ANDROID
                if (inputField.isFocused == true)
                {
                    jc.CallStatic("metroStopBpm");
                }
        #endif

        #if UNITY_IOS
            if(inputField.isFocused == true || !animator.GetComponent<AnimFuncs>().isStopButton)
            {
                IOSStopSound();
            }   
        #endif
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

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
