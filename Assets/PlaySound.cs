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
    private static extern void IOSPlaySound(float speed, String tag, int khandaCount, int misraCount);
    //[DllImport("__Internal")]
    //private static extern void IOSChangeSpeed(float speed);
    [DllImport("__Internal")]
    private static extern void IOSStopSound();
    [DllImport("__Internal")]
    private static extern bool IOSIsPlaying();
    [DllImport("__Internal")]
    private static extern float IOSGetSpeed();
    [DllImport("__Internal")]
    private static extern void IOSSetLoopCount();
    [DllImport("__Internal")]
    private static extern int IOSGetLoopCount();

#endif
    AudioSource audioSource;
    TMP_InputField inputField;
    float secperBeat;
    AndroidJavaObject jc;
    int counter;
    bool isMuted;
    const float lengthOfAudio = 0.260f;
    float beats;
    float currentTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //currentTime = Time.time * 1000;
        //float pastTime = animator.GetComponent<GetTime>().getTime();
        //Debug.Log(currentTime - pastTime);
        //animator.GetComponent<GetTime>().setTime(currentTime);
        isMuted = animator.GetComponent<SoundFuncs>().mute;
        inputField = animator.GetComponent<AdjustSpeed>().inputField;
        beats = float.Parse(inputField.text);
#if UNITY_ANDROID
            jc = animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
            bool isPlaying = jc.Call<bool>("isPlaying");
            float currBeats = jc.Call<float>("getBpm");
            beats *= stateInfo.speed;
            if (!isMuted && (!isPlaying || currBeats != beats || animator.GetBool("StartKhandaChapu") || animator.GetBool("StartMisra")) && !inputField.isFocused)
            {
                jc.Call("setBpm", beats);
                if (isPlaying && currBeats != beats)
                {
                    //Debug.Log("called stop");
                    jc.Call("stop");
                }
                if (animator.GetBool("StartKhandaChapu"))
                {
                    if (isPlaying){
                        Debug.Log("StoppedKhanda");
                        jc.Call("stop");
                    }
                    jc.Call("play", "KhandaChapu");
                }
                //else if (stateInfo.IsTag("MisraTag1"))
                //{
                //    jc.Call("play", "MisraTag1");
                //}
                else if (stateInfo.IsTag("MisraTag2"))
                {
                    jc.Call("play", "MisraTag2");
                }
                else
                {
                    jc.Call("play", "");
                }
            }
#endif

#if UNITY_IOS
            float currspeed = IOSGetSpeed();
            float speed = float.Parse(inputField.text);
            IOSSetLoopCount();
            int loopCount = IOSGetLoopCount();
            //if (stateInfo.IsTag("LastTapKhanda"))
            //{
            //    speed *= 2;
            //}
            speed *= stateInfo.speed;
            bool isPlaying = IOSIsPlaying();
            if (!isMuted && (!isPlaying || (speed != currspeed) || animator.GetBool("StartKhandaChapu") || animator.GetBool("StartMisra") || loopCount > 60))
            {
                //if (speed != currspeed && !stateInfo.IsTag("LastTapKhanda")) { IOSStopSound();}
                if (animator.GetBool("StartMisra"))
                {
                    IOSStopSound();
                    if (stateInfo.IsTag("MisraTag1")) { IOSPlaySound(speed, "MisraTag", 0, 1); }
                    else if (stateInfo.IsTag("MisraTag2")) { IOSPlaySound(speed, "MisraTag", 0, 2); }
                    else if(stateInfo.IsTag("MisraTag3") || stateInfo.IsTag("MisraTag4")) {IOSPlaySound(speed, "MisraTag", 0, 3); }
                    else { IOSPlaySound(speed, "MisraTag", 0, 0); }
                }
                else if(animator.GetBool("StartKhandaChapu"))
                {
                    IOSStopSound();
                    if (stateInfo.IsTag("KhandaTag2")) {IOSPlaySound(speed, "KhandaTag", 2,0); }
                    else if(stateInfo.IsTag("KhandaTag3")) { IOSPlaySound(speed, "KhandaTag", 3,0); }
                    else { IOSPlaySound(speed, "KhandaTag", 0, 0); }
                }
                else { IOSPlaySound(speed, "", 0, 0);}
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
                    jc.Call("stop");
                }
#endif

#if UNITY_IOS       
            if (IOSIsPlaying())
            {
                //Debug.Log("sound is still playing");
            }
            if(inputField.isFocused == true || !animator.GetComponent<AnimFuncs>().isStopButton)
            {
                //Debug.Log("stopping sound");
                IOSStopSound();
            }   
        #endif
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //currentTime = Time.time;
        //Debug.Log("Time to finish beat is " + (currentTime - startTime));
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
