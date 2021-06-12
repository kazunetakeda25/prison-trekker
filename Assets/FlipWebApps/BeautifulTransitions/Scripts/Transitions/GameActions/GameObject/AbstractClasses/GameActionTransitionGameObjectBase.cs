//----------------------------------------------
// Flip Web Apps: Beautiful Transitions
// Copyright © 2016 Flip Web Apps / Mark Hewitt
//
// Please direct any bugs/comments/suggestions to http://www.flipwebapps.com
// 
// The copyright owner grants to the end user a non-exclusive, worldwide, and perpetual license to this Asset
// to integrate only as incorporated and embedded components of electronic games and interactive media and 
// distribute such electronic game and interactive media. End user may modify Assets. End user may otherwise 
// not reproduce, distribute, sublicense, rent, lease or lend the Assets. It is emphasized that the end 
// user shall not be entitled to distribute or transfer in any way (including, without, limitation by way of 
// sublicense) the Assets in any other way than as integrated components of electronic games and interactive media. 

// The above copyright notice and this permission notice must not be removed from any files.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//----------------------------------------------

#if GAME_FRAMEWORK
using UnityEngine;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using GameFramework.GameStructure.Game;

namespace BeautifulTransitions.Scripts.Transitions.GameActions.GameObject.AbstractClasses
{
    public abstract class GameActionTransitionGameObjectBase : GameActionTransitionBase
    {
        /// <summary>
        /// What GameObject this transition is targeting.
        /// </summary>
        public GameActionHelper.TargetType TargetType
        {
            get
            {
                return _targetType;
            }
            set
            {
                _targetType = value;
            }
        }
        [Tooltip("What GameObject this transition is targeting.")]
        [SerializeField]
        GameActionHelper.TargetType _targetType = GameActionHelper.TargetType.ThisGameObject;

        /// <summary>
        /// The target gameobject upon which to perform the transition if using a target type mode of Secified.
        /// </summary>
        public UnityEngine.GameObject Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }
        [Tooltip("The target gameobject upon which to perform the transition if using a target type mode of Secified.")]
        [SerializeField]
        UnityEngine.GameObject _target;

        [Tooltip("The transition mode relative to specified and current values.")]
        public TransitionModeType TransitionMode = TransitionModeType.FromCurrent;


        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStep(TransitionStep transitionStep)
        {
            transitionStep.TransitionMode = (TransitionStep.TransitionModeType)TransitionMode;
            base.SetupTransitionStep(transitionStep);
        }


        /// <summary>
        /// Determine teh target that should be used based upon the chosen TargetType
        /// </summary>
        /// <returns></returns>
        public UnityEngine.GameObject ResolveTarget()
        {
            var targetFinal = GameActionHelper.ResolveTarget(TargetType, this, Target);
            if (targetFinal == null) Debug.LogWarningFormat("No Target is specified for the action {0} on {1}", GetType().Name, Owner.gameObject.name);
            return targetFinal;
        }


#region IScriptableObjectContainerSyncReferences

        /// <summary>
        /// Workaround for ObjectReference issues with ScriptableObjects (See ScriptableObjectContainer for details)
        /// </summary>
        /// <param name="objectReferences"></param>
        public override void SetReferencesFromContainer(UnityEngine.Object[] objectReferences)
        {
            if (objectReferences != null && objectReferences.Length == 1)
                Target = objectReferences[0] as UnityEngine.GameObject;
        }

        /// <summary>
        /// Workaround for ObjectReference issues with ScriptableObjects (See ScriptableObjectContainer for details)
        /// </summary>
        public override UnityEngine.Object[] GetReferencesForContainer()
        {
            var objectReferences = new Object[1];
            objectReferences[0] = Target;
            return objectReferences;
        }

#endregion IScriptableObjectContainerSyncReferences
    }
}
#endif