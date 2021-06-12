//----------------------------------------------
// Flip Web Apps: Game Framework
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
using BeautifulTransitions.Scripts.Transitions.Components;
using GameFramework.GameStructure.Game;
using GameFramework.GameStructure.Game.ObjectModel.Abstract;
using GameFramework.Helper;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeautifulTransitions.Scripts.Transitions.GameActions.ComponentInteraction
{
    /// <summary>
    ///GameAction to transition out the specified transition.
    /// </summary>
    [System.Serializable]
    [ClassDetails("Transition Out - Transition Component", "Beautiful Transitions/Component Interaction/Transition Out - Transition Component", "Transition out the specified transition.")]
    public class GameActionTransitionOut : GameAction
    {
        /// <summary>
        /// What Transition to target.
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
        [Tooltip("What Transition to target.")]
        [SerializeField]
        GameActionHelper.TargetType _targetType = GameActionHelper.TargetType.ThisGameObject;
        
        
        /// <summary>
        /// The target Transition if using a target type mode of Specified.
        /// </summary>
        public TransitionBase Target
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
        [Tooltip("The target Transition if using a target type mode of Specified.")]
        [SerializeField]
        TransitionBase _target;

        TransitionBase _cachedFinalTarget;


        /// <summary>
        /// Perform the action
        /// </summary>
        /// <returns></returns>
        protected override void Execute(bool isStart)
        {
            // use cached version unless target could be dynamic (TargetType.CollidingGameObject)
            var targetFinal = _cachedFinalTarget;
            if (targetFinal == null)
            {
                targetFinal = GameActionHelper.ResolveTarget<TransitionBase>(TargetType, this, Target);
                if (TargetType != GameActionHelper.TargetType.CollidingGameObject)
                    _cachedFinalTarget = targetFinal;
            }

            if (targetFinal == null) Debug.LogWarningFormat("No Target is specified for the action {0} on {1}", GetType().Name, Owner.gameObject.name);
            if (targetFinal != null)
            {
                targetFinal.TransitionOut();
            }

        }

#region IScriptableObjectContainerSyncReferences

        /// <summary>
        /// Workaround for ObjectReference issues with ScriptableObjects (See ScriptableObjectContainer for details)
        /// </summary>
        /// <param name="objectReferences"></param>
        public override void SetReferencesFromContainer(UnityEngine.Object[] objectReferences)
        {
            if (objectReferences != null && objectReferences.Length == 1)
                Target = objectReferences[0] as TransitionBase;
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