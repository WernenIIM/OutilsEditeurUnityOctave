using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro.EditorUtilities;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

public class Rendu : EditorWindow
{
    
    private Animator[] _allAnimator;
    private string[] _animatorList = null;
    private string _animatorListIntermediaire;
    private string _Name;
    private int _IndexAnimator;
    private int _currentIndex = 0;

    private AnimationClip[] _animationClips = null;
    private string[] _animationName = null;
    private int _animationCurrentindex = 0;

    private bool _isPlaying = false;
    private float _editorLastTime = 0f;
    




    [MenuItem ("Tools/Rendu Tool")]
    public static void  ShowWindow ()
    {
        Rendu window = EditorWindow.GetWindow<Rendu> ();
        GUIContent titleContent = new GUIContent ("Animator Tools");
        window.titleContent = titleContent;
    }

    private void OnGUI()
    {
        if (_allAnimator == null)
        {
            _animatorList = getAllAnimator();

        }

        _currentIndex = EditorGUILayout.Popup(_currentIndex, _animatorList);
        
            _animationClips = GetAnimClip(_allAnimator[_currentIndex]);
        

        
        
            _animationName = getAnimationName(_animationClips);
        
        

        _animationCurrentindex = EditorGUILayout.Popup(_animationCurrentindex, _animationName);
        if (!Application.isPlaying)
        {
            if (_isPlaying)
            {
                if (GUILayout.Button("Stop"))
                {
                    StopAnim();
                }
            }
            else if (GUILayout.Button("Play"))
            {
                PlayAnim();
            }
        }

        else
        {
            StopAnim();
        }
        
        
        if (GUILayout.Button("Animator search"))
        {
            Selection.activeObject = _allAnimator[_currentIndex].gameObject;
            SceneView.lastActiveSceneView.FrameSelected();


        }
        
        
    }

    private AnimationClip[] GetAnimClip(Animator animator)
    {
        List<AnimationClip> result = new List<AnimationClip>();
        AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
        AnimatorControllerLayer animatorControllerLayer = animatorController.layers[0];

        foreach (ChildAnimatorState child in animatorControllerLayer.stateMachine.states)
        {
            AnimationClip animClip = child.state.motion as AnimationClip;

            if (animClip != null)
            {
                result.Add(animClip);
            }
        }

        return result.ToArray();
    }

    private string[] getAnimationName(AnimationClip[] animationClips)
    {
        List<string> result = new List<string>();
        foreach (AnimationClip animationClip in animationClips)
        {
            result.Add(animationClip.name);
        }

        return result.ToArray();
    }

    private string[] getAllAnimator()
    {
        _allAnimator = UnityEngine.Animator.FindObjectsOfType<Animator>();
        List<string> ListAnimator = new List<string>();
        for (int i = 0; i < _allAnimator.Length - 1; i++)
        {
            Debug.Log(_allAnimator.GetValue(i));

            ListAnimator.Add(_allAnimator[i].gameObject.name);
                
        }

        return ListAnimator.ToArray();
    }
    
    private void PlayAnim()
    {
        if (_isPlaying) return;
        _editorLastTime = Time.realtimeSinceStartup;
        EditorApplication.update += _OnEditorUpdate;
        AnimationMode.StartAnimationMode();
        _isPlaying = true;
    }

    private void StopAnim()
    {
        if (!_isPlaying) return;
        EditorApplication.update -= _OnEditorUpdate;
        AnimationMode.StopAnimationMode();
        _isPlaying = false;
    }

    private void _OnEditorUpdate()
    {
        float animTime = Time.realtimeSinceStartup - _editorLastTime;
        AnimationClip animClip = _animationClips[_animationCurrentindex];
        animTime %= animClip.length;
        AnimationMode.SampleAnimationClip(_allAnimator[_currentIndex].gameObject, animClip, animTime);
    }


}
