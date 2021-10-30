using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using animations = UnityEditor.Animations;
using AnimationControllerEditor;

namespace AnimationControllerEditor
{
    using GestureCombination = ValueTuple<GestureIndex, GestureIndex>;
    public class CreateHandGestureStateMachine : EditorWindow
    {
        private GestureCombination[] handCombinations = Enum.GetValues(typeof(GestureIndex)).Cast<GestureIndex>().Skip(1).Select(rightIndex => Enum.GetValues(typeof(GestureIndex)).Cast<GestureIndex>().Skip(1).Select(leftIndex => (leftIndex, rightIndex))).SelectMany(item => item).ToArray();
        private string targetLayerName = "";
        private string startStateName = "";
        private string endStateName = "";
        private animations.AnimatorController controller;

        private animations.AnimatorState startState;
        private animations.AnimatorState endState;

        [MenuItem("Tools/haru2036/CreateHandGestureStateMachine")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CreateHandGestureStateMachine));
        }

        void OnGUI()
        {
            GUILayout.Label("Target Controller", EditorStyles.boldLabel);
            controller = (animations.AnimatorController)EditorGUILayout.ObjectField(controller, typeof(animations.AnimatorController));
            targetLayerName = EditorGUILayout.TextField("Target layer name", targetLayerName);
            startStateName = EditorGUILayout.TextField("Start state", startStateName);
            endStateName = EditorGUILayout.TextField("End state", endStateName);

            if (GUILayout.Button("Create states 7*7"))
            {
                try
                {
                    animations.AnimatorControllerLayer[] layers = controller.layers;
                    var foundLayer = ControllerUtils.GetLayerByName(layers, targetLayerName);
                    if (foundLayer != null)
                    {
                        var stateMachine = foundLayer.stateMachine;
                        startState = ControllerUtils.GetStateByName(stateMachine, startStateName);
                        endState = ControllerUtils.GetStateByName(stateMachine, endStateName);
                        CreateStatesWithTransitions(stateMachine);
                        controller.layers = ControllerUtils.ReplaceLayerByName(layers, foundLayer);
                    }
                    else
                    {
                        throw new Exception("No layers of that name found.");
                    }

                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Error", e.Message, "ok");
                }
            }

            if(GUILayout.Button("Dump status")){
                try
                {
                    animations.AnimatorControllerLayer[] layers = controller.layers;
                    var foundLayer = ControllerUtils.GetLayerByName(layers, targetLayerName);
                    if (foundLayer != null)
                    {
                        foreach(animations.ChildAnimatorState state in foundLayer.stateMachine.states){
                            if(state.state.motion != null){
                                Debug.Log(state.state.name + ":" + state.state.motion.name);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("No layers of that name found.");
                    }
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Error", e.Message, "ok");
                }

            }
        }

        private void CreateStatesWithTransitions(animations.AnimatorStateMachine stateMachine){
            foreach ((GestureIndex left, GestureIndex right) in handCombinations)
            {
                Debug.Log($"Left: {(int)left}, {left.ToString()},Right: {(int)right}, {right.ToString()}");
                var state = stateMachine.AddState($"Left{left.ToString()},Right{right.ToString()}");
                CreateAndTransition(stateMachine, animations.AnimatorConditionMode.Equals, (left, right), startState, state);
                CreateOrTransition(stateMachine, animations.AnimatorConditionMode.NotEqual, (left, right), state, endState);
            }
        }

        private void CreateOrTransition(animations.AnimatorStateMachine stateMachine, animations.AnimatorConditionMode conditionMode, GestureCombination gestureCombination, animations.AnimatorState source, animations.AnimatorState dest)
        {
            animations.ChildAnimatorState[] states = stateMachine.states;

            if (source != null && dest != null)
            {
                var newTransition1 = source.AddTransition(dest);
                newTransition1.destinationState = dest;
                newTransition1.AddCondition(conditionMode, (int)gestureCombination.Item1, "GestureLeft");
                
                var newTransition2 = source.AddTransition(dest);
                newTransition2.destinationState = dest;
                newTransition2.AddCondition(conditionMode, (int)gestureCombination.Item2, "GestureRight");
            }
            else
            {
                throw new Exception("sourceState or destState is null");
            }

        }

        private void CreateAndTransition(animations.AnimatorStateMachine stateMachine, animations.AnimatorConditionMode conditionMode, GestureCombination gestureCombination, animations.AnimatorState source, animations.AnimatorState dest)
        {
            animations.ChildAnimatorState[] states = stateMachine.states;

            if (source != null && dest != null)
            {
                var newTransition = source.AddTransition(dest);
                newTransition.destinationState = dest;
                newTransition.AddCondition(conditionMode, (int)gestureCombination.Item1, "GestureLeft");
                newTransition.AddCondition(conditionMode, (int)gestureCombination.Item2, "GestureRight");
            }
            else
            {
                throw new Exception("sourceState or destState is null");
            }

        }

    }
}