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
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using GameFramework.GameStructure.Game.ObjectModel.Abstract;
using UnityEngine;
using UnityEngine.Events;

namespace BeautifulTransitions.Scripts.Transitions.GameActions
{
    /// <summary>
    /// Abstract base class for all transition components.
    /// </summary>
    public abstract class GameActionTransitionBase : GameAction {
        /// <summary>
        /// The transition mode
        /// </summary>
        public enum TransitionModeType
        {
            Specified = TransitionStep.TransitionModeType.Specified,
            FromCurrent = TransitionStep.TransitionModeType.FromCurrent,
            ToCurrent = TransitionStep.TransitionModeType.ToCurrent
        };

        [Tooltip("How long this transition will / should run for.")]
        public float Duration = 0.3f;
        [Tooltip("What time source is used to update transitions")]
        public TransitionStep.TimeUpdateMethodType TimeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime;
        [Tooltip("How the transition should be run.")]
        public TransitionHelper.TweenType TransitionType = TransitionHelper.TweenType.linear;
        [Tooltip("A custom curve to show how the transition should be run.")]
        public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // COMMENTED OUT DUE TO SERIALISATION ISSUES (CAN'T SET IN EDITOR)!!
        //[Tooltip("Methods that should be called when the transition is started.")]
        //public TransitionStepEvent OnTransitionStart;
        //[Tooltip("Methods that should be called when the transition progress is updated.")]
        //public TransitionStepEvent OnTransitionUpdate;
        //[Tooltip("Methods that should be called when the transition has completed.")]
        //public TransitionStepEvent OnTransitionComplete;

        //#if GAME_FRAMEWORK
        //            [Tooltip("A list of actions that should be run when the transition is started.")]
        //            public GameFramework.GameStructure.Game.ObjectModel.GameActionReference[] OnTransitionStartActionReferences = new GameFramework.GameStructure.Game.ObjectModel.GameActionReference[0];
        //            [Tooltip("A list of actions that should be run when the transition completes.")]
        //            public GameFramework.GameStructure.Game.ObjectModel.GameActionReference[] OnTransitionCompleteActionReferences = new GameFramework.GameStructure.Game.ObjectModel.GameActionReference[0];
        //#endif


#region Callbacks

        /// <summary>
        /// Called when an out transition is started
        /// </summary>
        protected virtual void TransitionStart(TransitionStep transitionStep)
        {
            //if (OnTransitionStart != null)
            //    OnTransitionStart.Invoke(transitionStep);
//#if GAME_FRAMEWORK
//            GameFramework.GameStructure.Game.GameActionHelper.PerformActions(OnTransitionStartActionReferences, this, false);
//#endif
        }


        /// <summary>
        /// Override this if you need to do any specific action when the value is updated
        /// </summary>
        /// <param name="amount"></param>
        protected virtual void TransitionUpdate(TransitionStep transitionStep)
        {
            //if (OnTransitionUpdate != null)
            //    OnTransitionUpdate.Invoke(transitionStep);
        }


        /// <summary>
        /// Called when an in transition has been completed (or interupted)
        /// </summary>
        protected virtual void TransitionComplete(TransitionStep transitionStep)
        {
            //if (OnTransitionComplete != null)
            //    OnTransitionComplete.Invoke(transitionStep);
//#if GAME_FRAMEWORK
//            GameFramework.GameStructure.Game.GameActionHelper.PerformActions(TransitionInConfig.OnTransitionCompleteActionReferences, this, false);
//#endif
        }

#endregion Callbacks

#region Create transitionStep

        /// <summary>
        /// Create a transitionStep. Implement this to create the correct subclass of transitionStep
        /// </summary>
        /// <returns></returns>
        public abstract TransitionStep CreateTransitionStep();


        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public virtual void SetupTransitionStep(TransitionStep transitionStep)
        {
            transitionStep.Delay = Delay;   // TODO should be 0? as delay, - comes from base action?
            transitionStep.Duration = Duration;
            transitionStep.TimeUpdateMethod = TimeUpdateMethod;
            transitionStep.TweenType = TransitionType;
            transitionStep.AnimationCurve = AnimationCurve;
            transitionStep.OnStart = TransitionStart;
            transitionStep.OnComplete = TransitionComplete;
            transitionStep.OnUpdate = TransitionUpdate;
        }


#endregion Create transitionStep


        /// <summary>
        /// Perform the action
        /// </summary>
        /// <returns></returns>
        protected override void Execute(bool isStart)
        {
            var transitionStep = CreateTransitionStep();
            SetupTransitionStep(transitionStep);
            transitionStep.Start();
        }


        /// <summary>
        /// Internal class to allow for serialisation of events taking a TransitionStep parameter
        /// </summary>
        [System.Serializable]
        public class TransitionStepEvent : UnityEvent<TransitionStep>
        {
        }
    }
}
#endif