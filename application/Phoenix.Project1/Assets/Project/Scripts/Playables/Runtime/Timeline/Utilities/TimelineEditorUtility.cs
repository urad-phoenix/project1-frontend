using UnityEngine;

namespace Phoenix.Playables.Editor.Utility
{    
    public class TimelineEditorUtility
    {
#if UNITY_EDITOR  
        public static AnimationClip GetAnimation(Animator animator, int stateLayer, string stateName)
        {
            if (animator == null)
                return null;
            
            UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            AnimatorOverrideController overrideController = null;
            if (ac == null)
            {
                overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                                
                ac = overrideController.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            }

            if (ac == null)
                return null;

            UnityEditor.Animations.AnimatorControllerLayer layer = null;
            
            layer = ac.layers[stateLayer];           

            if (layer == null)
                return null;

            UnityEditor.Animations.ChildAnimatorState[] states = layer.stateMachine.states;
            AnimationClip anim = null;
            if (states != null && states.Length > 0)
            {
                for (int i = 0; i < states.Length; ++i)
                {
                    if (states[i].state.name.Equals(stateName))
                    {
                        anim = states[i].state.motion as AnimationClip;
                        if (anim != null)
                        {
                            if (overrideController != null)
                            {
                                return overrideController[anim.name];
                            }

                            return anim;
                        }
                    }
                }
            }
            return null;
        }
        
        public static UnityEditor.Animations.ChildAnimatorState[] GetStates(Animator animator, int stateLayer)
        {
            if (animator == null)
                return null;
         
            UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            if (ac == null)
            {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if(overrideController != null)
                    ac = overrideController.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            }

            if (ac == null)
                return null;

            UnityEditor.Animations.AnimatorControllerLayer layer = null;
            
            layer = ac.layers[stateLayer];           

            if (layer == null)
                return null;

            return layer.stateMachine.states;
        }
#endif
    }
}

