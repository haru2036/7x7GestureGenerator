using System;
using System.Linq;
using animations = UnityEditor.Animations;
namespace AnimationControllerEditor
{
    class ControllerUtils {
        public static animations.AnimatorControllerLayer GetLayerByName(animations.AnimatorControllerLayer[] layers, string desiredName)
        {
            return Array.Find<animations.AnimatorControllerLayer>(layers, layer => layer.name == desiredName);
        }

        public static animations.AnimatorControllerLayer[] ReplaceLayerByName(animations.AnimatorControllerLayer[] layers, animations.AnimatorControllerLayer newLayer)
        {
            return layers.Select(layer =>
            {
                if (layer.name == newLayer.name)
                {
                    return newLayer;
                }
                else
                {
                    return layer;
                }
            }).ToArray();
        }

        public static animations.AnimatorState GetStateByName(animations.AnimatorStateMachine stateMachine, string desiredName)
        {
               var state = Array.Find(stateMachine.states, childState => childState.state.name.Equals(desiredName)).state;
               if(state != null){
                   return state;
               }
               else
               {
                   throw new Exception("startState or endState not found");
               }
        }
    }
}