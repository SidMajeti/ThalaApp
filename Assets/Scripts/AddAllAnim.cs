//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditor.Animations;
//using UnityEngine;
//using UnityEngine.UI;

//[ExecuteInEditMode]
//public class AddAllAnim : MonoBehaviour
//{
//    // Start is called before the first frame update
//    bool flag = true;
//    static AnimationClip[] jathis = new AnimationClip[9];
//    static AnimationClip[] drutham = new AnimationClip[2];
//    void Awake()
//    {

//    }
//    [MenuItem("MyMenu/Create Controller")]
//    static void addStates()
//    {
//        AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/AllThalas.controller");
//        var rootStateMachine = controller.layers[0].stateMachine;
//        var emptyState = rootStateMachine.AddState("Empty");
//        rootStateMachine.AddAnyStateTransition(emptyState);
//        jathis[0] = Resources.Load<AnimationClip>("FastPalmDown");
//        jathis[1] = Resources.Load<AnimationClip>("FastPinky");
//        jathis[2] = Resources.Load<AnimationClip>("FastRing");
//        jathis[3] = Resources.Load<AnimationClip>("FastMiddle");
//        jathis[4] = Resources.Load<AnimationClip>("AdjustedPointer");
//        jathis[5] = Resources.Load<AnimationClip>("ThumbDown");
//        jathis[6] = Resources.Load<AnimationClip>("FastPinky");
//        jathis[7] = Resources.Load<AnimationClip>("FastRing");
//        jathis[8] = Resources.Load<AnimationClip>("FastMiddle");

//        drutham[0] = Resources.Load<AnimationClip>("PalmDownWithFlip");
//        drutham[1] = Resources.Load<AnimationClip>("AdiBackTap");
//        //var stateA3 = controller.AddMotion(clip);

//        Dropdown thalas = Resources.Load<Dropdown>("Thalas");
//        Dropdown jathi = Resources.Load<Dropdown>("Jathi");

//        for (int i = 0; i < thalas.options.Count; i++)
//        {
//            for (int j = 0; j < jathi.options.Count; j++)
//            {
//                AnimatorState pastState = emptyState;
//                AnimatorState currentState = emptyState;
//                AnimatorState firstState = emptyState;
//                AnimatorStateTransition trans = null;
//                controller.AddParameter(thalas.options[i].text + jathi.options[j].text, AnimatorControllerParameterType.Bool);
//                if (i == 0)
//                {
//                    int counter = 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        if (k == 0)
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, true, ref firstState, counter);
//                        }
//                        else
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        }
//                        counter += 1;
//                    }

//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter +1);
//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter + 2);
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter + 3);

//                    trans = pastState.AddTransition(firstState);
//                    trans.exitTime = 0.8f;
//                    trans.hasFixedDuration = true;
//                    trans.duration = 0.2f;
//                    trans.hasExitTime = true;

//                }
//                else if (i == 1)
//                {
//                    int counter = 1;
//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, true, ref firstState, counter);
//                    counter += 1;
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    counter += 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        counter += 1;
//                    }

//                    trans = pastState.AddTransition(firstState);
//                    trans.exitTime = 0.8f;
//                    trans.hasFixedDuration = true;
//                    trans.duration = 0.2f;
//                    trans.hasExitTime = true;
//                }
//                else if (i == 2)
//                {
//                    int counter = 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        if (k == 0)
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, true, ref firstState, counter);
//                        }
//                        else
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        }
//                        counter += 1;
//                    }
//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    counter += 1;
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    counter += 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {

//                        addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        counter += 1;

//                    }
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        counter += 1;
//                    }
//                    trans = pastState.AddTransition(firstState);
//                    trans.exitTime = 0.8f;
//                    trans.hasFixedDuration = true;
//                    trans.duration = 0.2f;
//                    trans.hasExitTime = true;
//                }
//                else if (i == 3)
//                {
//                    int counter = 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        if (k == 0)
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, true, ref firstState, counter);
//                        }
//                        else
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        }
//                        counter += 1;
//                    }
//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    counter += 1;
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    counter += 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        counter += 1;
//                    }
//                    trans = pastState.AddTransition(firstState);
//                    trans.exitTime = 0.8f;
//                    trans.hasFixedDuration = true;
//                    trans.duration = 0.2f;
//                    trans.hasExitTime = true;
//                }
//                else if (i == 4)
//                {
//                    int counter = 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        if (k == 0)
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, true, ref firstState, counter);
//                        }
//                        else
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        }
//                        counter += 1;
//                    }
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        counter += 1;
//                    }
//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter + 1);
//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter + 2);
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter + 3);
//                    trans = pastState.AddTransition(firstState);
//                    trans.exitTime = 0.8f;
//                    trans.hasFixedDuration = true;
//                    trans.duration = 0.2f;
//                    trans.hasExitTime = true;
//                }
//                else if (i == 5)
//                {
//                    int counter = 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        if (k == 0)
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, true, ref firstState, counter);
//                        }
//                        else
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        }
//                        counter += 1;
//                    }
//                    trans = pastState.AddTransition(firstState);
//                    trans.exitTime = 0.8f;
//                    trans.hasFixedDuration = true;
//                    trans.duration = 0.2f;
//                    trans.hasExitTime = true;
//                }
//                else if (i == 6)
//                {
//                    int counter = 1;
//                    for (int k = 0; k < Int32.Parse(jathi.options[j].text); k++)
//                    {
//                        if (k == 0)
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, true, ref firstState, counter);
//                        }
//                        else
//                        {
//                            addAnim(jathis[k], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                        }
//                        counter += 1;
//                    }
//                    addAnim(jathis[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter);
//                    addAnim(drutham[0], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter + 1);
//                    addAnim(drutham[1], thalas, jathi, i, j, ref currentState, ref pastState, ref trans, ref controller, false, ref firstState, counter + 2);
//                    trans = pastState.AddTransition(firstState);
//                    trans.exitTime = 0.8f;
//                    trans.hasFixedDuration = true;
//                    trans.duration = 0.2f;
//                    trans.hasExitTime = true;
//                }
//            }
//        }

//    }

//    static void addAnim(AnimationClip clip, Dropdown thalas, Dropdown jathi, int thalaCount, int jathiCount, ref AnimatorState currentState, ref AnimatorState pastState, ref AnimatorStateTransition trans, ref AnimatorController controller, bool isFirst, ref AnimatorState firstState, int counter)
//    {
//        currentState = controller.AddMotion(clip);
//        currentState.AddStateMachineBehaviour<PlaySound>();
//        currentState.tag = counter.ToString();
//        trans = pastState.AddTransition(currentState);
//        if (isFirst)
//        {
//            firstState = currentState;
//            trans.exitTime = 0.75f;
//            trans.duration = 0.25f;
//            trans.AddCondition(AnimatorConditionMode.If, 0, thalas.options[thalaCount].text + jathi.options[jathiCount].text);
//        }
//        else
//        {
//            trans.exitTime = 0.8f;
//            trans.duration = 0.2f;
//        }
//        trans.hasFixedDuration = true;
//        trans.hasExitTime = true;
//        pastState = currentState;

//    }
//    void Update()
//    {
//    }
//}
