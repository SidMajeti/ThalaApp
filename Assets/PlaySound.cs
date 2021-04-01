using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//plays sound with respect to speed
public class PlaySound : StateMachineBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void IOSPlaySound(float speed, String tag, int khandaCount, int misraCount, int sankeernaCount, int[] thalas, int sizeOfArr, int laghuCount, int otherCount);
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
    InputField inputField;
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
        //bool isSubscribed = animator.GetComponent<AnimFuncs>().canvas.GetComponent<Purchaser>().isSubscribed;

        //temp for when we don't have a subs button(ie no subscription)
        bool isSubscribed = true;
        if (isSubscribed)
        {
            inputField = animator.GetComponent<SetUpScene>().inputField2;
        }
        else
        {
            inputField = animator.GetComponent<SetUpScene>().inputField;
        }
        beats = float.Parse(inputField.text);
#if UNITY_ANDROID
            jc = animator.GetComponent<InstatiateGlobalVars>().GetPluginJavaClass();
            bool isPlaying = jc.Call<bool>("isPlaying");
            float currBeats = jc.Call<float>("getBpm");
            if (!isMuted && (!isPlaying || currBeats != beats) && !inputField.isFocused)
            {
                jc.Call("setBpm", beats);
                if (isPlaying && currBeats != beats)
                {
                    jc.Call("stop");
                }
                if (animator.GetBool("StartKhandaChapu"))
                {
                    int[] a = { };
                    if (stateInfo.IsTag("KhandaTag1")) { jc.Call("play", "KhandaChapu1", 0, 0,a ); }
                    else if (stateInfo.IsTag("KhandaTag3")) { jc.Call("play", "KhandaChapu3", 0, 0, a); }
                    else { jc.Call("play", "KhandaChapu2", 0, 0, a); }
                }
                else if (animator.GetBool("StartMisra"))
                {
                    int[] a = { };
                    if (stateInfo.IsTag("MisraTag1")) {
                        //Debug.Log("Misra1 gets called");
                        jc.Call("play", "MisraTag1", 0, 0, a);
                    }
                    else if (stateInfo.IsTag("MisraTag2")) { jc.Call("play", "MisraTag2", 0, 0, a); }
                    else if (stateInfo.IsTag("MisraTag3")) { jc.Call("play", "MisraTag3", 0,0 , a); }
                    else { jc.Call("play", "MisraTag4", 0, 0 , a);}
                }
                else if (animator.GetBool("StartRupakam"))
                {
                    int[] a = { };
                    for (int i = 1; i <= 3; i++)
                    {
                        string s = i.ToString();
                        if (stateInfo.IsTag(s)) { jc.Call("play", "Rupakam", i, 0, a); }
                    }
                }
                else if (animator.GetBool("StartSankeerna"))
                {
                    int[] a = { };
                    if (stateInfo.IsTag("Sankeerna1")) { jc.Call("play", "Sankeerna", 1, 0, a); }
                    else if (stateInfo.IsTag("Sankeerna2")) { jc.Call("play", "Sankeerna", 2, 0, a); }
                    else if (stateInfo.IsTag("Sankeerna3")) {jc.Call("play", "Sankeerna", 3, 0, a); }
                    else if (stateInfo.IsTag("Sankeerna4")) {jc.Call("play", "Sankeerna", 4, 0, a); }
                    else
                    {
                        jc.Call("play", "Sankeerna", 5, 0, a);
                    }
                }
                else if (animator.GetBool("StartAdi"))
                {
                    //if (animator.GetComponent<AnimFuncs>().canvas.GetComponent<Purchaser>().isSubscribed)
                    if(true)
                    {
                        if (animator.GetComponent<AnimFuncs>().kalaiDropdown2.value == 0)
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 8; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { jc.Call("play", "", i, 4, a); break; }
                            }
                        }
                        else
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 16; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { jc.Call("play", "DoubleKalai", i, 4, a); break; }
                            }
                        }
                    }
                    else
                    {
                        if (animator.GetComponent<AnimFuncs>().kalaiDropdown.value == 0)
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 8; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { jc.Call("play", "", i, 4, a); break; }
                            }
                        }
                        else
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 16; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { jc.Call("play", "DoubleKalai", i, 4, a); break; }
                            }
                        }
                    }
                }
                else
                {
                    AnimatorControllerParameter[] param = animator.parameters;
                    String thala = "";
                    int jathi = 0;
                    foreach (AnimatorControllerParameter p in param)
                    {
                        if (animator.GetBool(p.name))
                        {
                            thala = p.name.Substring(0, p.name.Length - 1);
                            jathi = Int32.Parse(p.name.Substring(p.name.Length - 1, 1));
                            //Debug.Log("thala: " + thala);
                            //Debug.Log("jathi num: " + jathi);
                        }
                    }
                    if (thala.Equals("Triputa"))
                    {
                        int[] a = { 1, 2, 2 };

                        for (int i = 1; i <= 13; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { jc.Call("play", "", i, jathi, a); break; }
                        }
                    }
                    else if (thala.Equals("Rupaka"))
                    {
                        int[] a = { 2, 1 };

                        for (int i = 1; i <= 11; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { jc.Call("play", "", i, jathi, a); break; }
                        }
                    }
                    else if (thala.Equals("Dhruva"))
                    {
                        int[] a = { 1, 2, 1, 1 };

                        for (int i = 1; i <= 29; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { jc.Call("play", "", i, jathi, a); break; }
                        }
                    }
                    else if (thala.Equals("Matya"))
                    {
                        int[] a = { 1, 2, 1 };

                        for (int i = 1; i <= 20; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { jc.Call("play", "", i, jathi, a); break; }
                        }
                    }
                    else if (thala.Equals("Ata"))
                    {
                        int[] a = { 1, 1, 2, 2 };

                        for (int i = 1; i <= 22; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { jc.Call("play", "", i, jathi, a); break; }
                        }
                    }
                    else if (thala.Equals("Eka"))
                    {
                        int[] a = { 1 };

                        for (int i = 1; i <= 9; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { jc.Call("play", "", i, jathi, a); break; }
                        }
                    }
                    else if (thala.Equals("Jhampa"))
                    {
                        int[] a = { 1, 0, 2 };

                        for (int i = 1; i <= 12; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { jc.Call("play", "", i, jathi, a); break; }
                        }
                    }
                }
            }
#endif

#if UNITY_IOS
            float currspeed = IOSGetSpeed();
            //float speed = float.Parse(inputField.text);
            bool isPlaying = IOSIsPlaying();
            if (!isMuted && (!isPlaying || (beats != currspeed)) && !inputField.isFocused)
            {
                if(isPlaying && currspeed != beats) {
                    //Debug.Log("Sound is stopped");
                    IOSStopSound();
                }
                if (animator.GetBool("StartMisra"))
                {
                    int[] a = { };
                    if (stateInfo.IsTag("MisraTag1")) { IOSPlaySound(beats, "MisraTag", 0, 1,0,a, 0, 0, 0); }
                    else if (stateInfo.IsTag("MisraTag2")) { IOSPlaySound(beats, "MisraTag", 0, 2,0, a, 0, 0, 0); }
                    else if (stateInfo.IsTag("MisraTag3")) { IOSPlaySound(beats, "MisraTag", 0, 3,0, a, 0, 0, 0); }
                    else { IOSPlaySound(beats, "MisraTag", 0, 4,0, a, 0, 0, 0); }
                }
                else if (animator.GetBool("StartKhandaChapu"))
                {
                    int[] a = { };
                    if (stateInfo.IsTag("KhandaTag1")) { IOSPlaySound(beats, "KhandaTag", 1, 0, 0, a, 0, 0, 0); }
                    else if (stateInfo.IsTag("KhandaTag2")) { IOSPlaySound(beats, "KhandaTag", 2, 0, 0, a, 0, 0, 0); }
                    else { IOSPlaySound(beats, "KhandaTag", 3, 0,0, a, 0, 0, 0); }
                }
                else if (animator.GetBool("StartSankeerna"))
                {
                    int[] a = { };
                    if (stateInfo.IsTag("Sankeerna1")) { IOSPlaySound(beats, "SankeernaTag", 0, 0, 1, a, 0, 0, 0); }
                    else if (stateInfo.IsTag("Sankeerna2")) { IOSPlaySound(beats, "SankeernaTag", 0,0, 2, a, 0, 0, 0); }
                    else if (stateInfo.IsTag("Sankeerna3")) { IOSPlaySound(beats, "SankeernaTag", 0,0, 3, a, 0, 0, 0); }
                    else if (stateInfo.IsTag("Sankeerna4")) { IOSPlaySound(beats, "SankeernaTag", 0,0, 4, a, 0, 0, 0); }
                    else
                    {
                        IOSPlaySound(beats, "SankeernaTag", 0,0, 5, a, 0, 0, 0);
                    }
                }
                else if (animator.GetBool("StartRupakam"))
                {
                    int[] a = { };
                    for (int i = 1; i <= 3; i++)
                    {
                        string s = i.ToString();
                        if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "Rupakam", 0, 0, 0, a , 0, 0, i); break; }
                    }
                }
                else if (animator.GetBool("StartAdi"))
                {
                    //if (animator.GetComponent<AnimFuncs>().canvas.GetComponent<Purchaser>().isSubscribed)

                    //temp for when no subscription
                    if(true)
                    {
                        if (animator.GetComponent<AnimFuncs>().kalaiDropdown2.value == 0)
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 8; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 3, 4, i); break; }
                            }
                        }
                        else
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 16; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "DoubleKalai", 0, 0, 0, a, 3, 4, i); break; }
                            }
                        }
                    }   
                    else
                    {
                        if (animator.GetComponent<AnimFuncs>().kalaiDropdown.value == 0)
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 8; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 3, 4, i); break; }
                            }
                        }
                        else
                        {
                            int[] a = { 1, 2, 2 };
                            for (int i = 1; i <= 16; i++)
                            {
                                string s = i.ToString();
                                if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "DoubleKalai", 0, 0, 0, a, 3, 4, i); break; }
                            }
                        }
                    }
                }
                //else if (animator.GetBool("StartAta"))
                //{
                //    int[] a = { 1, 1, 2, 2 };

                //    for (int i = 1; i <= 14; i++)
                //    {
                //        string s = i.ToString();
                //        if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 4, 5, i); break; }
                //    }
                //}
                else {
                    AnimatorControllerParameter[] param = animator.parameters;
                    String thala = "";
                    int jathi = 0;
                    foreach (AnimatorControllerParameter p in param)
                    {
                        if (animator.GetBool(p.name))
                        {
                            thala = p.name.Substring(0, p.name.Length - 1);
                            jathi = Int32.Parse(p.name.Substring(p.name.Length - 1, 1));
                            //Debug.Log("thala: " +thala);
                            //Debug.Log("jathi num: " + jathi);
                        }
                    }
                    if (thala.Equals("Triputa"))
                    {
                        int[] a = { 1, 2, 2 };

                        for (int i = 1; i <= 13; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 3, jathi, i); break; }
                        }
                    }
                    else if (thala.Equals("Rupaka"))
                    {
                        int[] a = { 2, 1 };

                        for (int i = 1; i <= 11; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 2, jathi, i); break;}
                        }
                    }
                    else if (thala.Equals("Dhruva"))
                    {
                        int[] a = { 1,2, 1,1 };

                        for (int i = 1; i <= 29; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 4, jathi, i); break; }
                        }
                    }
                    else if (thala.Equals("Matya"))
                    {
                        int[] a = { 1, 2, 1};

                        for (int i = 1; i <= 20; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 3, jathi, i);break; }
                        }
                    }
                    else if (thala.Equals("Ata"))
                    {
                        int[] a = { 1, 1, 2, 2 };

                        for (int i = 1; i <= 22; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 4, jathi, i); break; }
                        }
                    }
                    else if (thala.Equals("Eka"))
                    {
                        int[] a = { 1};

                        for (int i = 1; i <= 9; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 1, jathi, i); break; }
                        }
                    }
                    else if (thala.Equals("Jhampa"))
                    {
                        int[] a = { 1, 0, 2 };

                        for (int i = 1; i <= 12; i++)
                        {
                            string s = i.ToString();
                            if (stateInfo.IsTag(s)) { IOSPlaySound(beats, "", 0, 0, 0, a, 3, jathi, i); break; }
                        }
                    }
                }
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
            if(inputField.isFocused == true)
            {
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
