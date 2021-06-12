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

#if PLAYMAKER
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using HutongGames.PlayMaker;

namespace BeautifulTransitions.Scripts.Transitions.PlayMakerActions
{
    /// <summary>
    /// Abstract base class for all Playmaker transition components.
    /// </summary>
    /// 
    /// - UnityEvents don't seem to show in the PlayMaker Action Editor so aren't added
    /// - Game Framework Actions aren't added for transitio lifecycle events to keep things simple.
    /// 
    public abstract class PlayMakerTransitionBase : FsmStateAction
    {
        /// <summary>
        /// The transition mode
        /// </summary>
        public enum TransitionModeType
        {
            Specified = TransitionStep.TransitionModeType.Specified,
            FromCurrent = TransitionStep.TransitionModeType.FromCurrent,
            ToCurrent = TransitionStep.TransitionModeType.ToCurrent
        };

        [HutongGames.PlayMaker.Tooltip("Time in seconds before this transition should be started.")]
        public FsmFloat Delay = 0.0f;

        [HutongGames.PlayMaker.Tooltip("How long this transition will / should run for.")]
        public FsmFloat Duration = 0.3f;

        [HutongGames.PlayMaker.Tooltip("What time source is used to update transitions")]
        [ObjectType(typeof(TransitionStep.TimeUpdateMethodType))]
        public FsmEnum TimeUpdateMethod;

        [HutongGames.PlayMaker.Tooltip("How the transition should be run.")]
        [ObjectType(typeof(TransitionHelper.TweenType))]
        public FsmEnum TransitionType;

        [HutongGames.PlayMaker.Tooltip("A custom curve to show how the transition should be run.")]
        public FsmAnimationCurve AnimationCurve;

        [HutongGames.PlayMaker.Tooltip("Optional event to send when the transition starts.")]
        public FsmEvent TransitionStartEvent;

        [HutongGames.PlayMaker.Tooltip("Optinoal eto send when the transition has completed.")]
        public FsmEvent TransitionCompleteEvent;

        #region PlayMaker Life Cycle

        public override void Reset()
        {
            base.Reset();
            Delay = 0.0f;
            Duration = 0.3f;
            TimeUpdateMethod = new FsmEnum(null, typeof(TransitionStep.TimeUpdateMethodType), (int)TransitionStep.TimeUpdateMethodType.GameTime);
            TransitionType = new FsmEnum(null, typeof(TransitionHelper.TweenType), (int)TransitionHelper.TweenType.linear);
            AnimationCurve = new FsmAnimationCurve() { curve = UnityEngine.AnimationCurve.EaseInOut(0, 0, 1, 1) };
            TransitionStartEvent = null;
            TransitionCompleteEvent = null;
        }


        public override void OnEnter()
        {
            PerformAction();
            Finish();
        }

        #endregion PlayMaker Life Cycle

        #region Callbacks

        /// <summary>
        /// Called when an out transition is started
        /// </summary>
        protected virtual void TransitionStart(TransitionStep transitionStep)
        {
            if (TransitionStartEvent != null)
            {
                Fsm.Event(TransitionStartEvent);
            }
        }


        /// <summary>
        /// Called when an in transition has been completed (or interupted)
        /// </summary>
        protected virtual void TransitionComplete(TransitionStep transitionStep)
        {
            if (TransitionCompleteEvent != null)
            {
                Fsm.Event(TransitionCompleteEvent);
            }
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
            transitionStep.Delay = Delay.Value;
            transitionStep.Duration = Duration.Value;
            transitionStep.TimeUpdateMethod = (TransitionStep.TimeUpdateMethodType)TimeUpdateMethod.Value;
            transitionStep.TweenType = (TransitionHelper.TweenType)TransitionType.Value;
            transitionStep.AnimationCurve = AnimationCurve.curve;
            transitionStep.OnStart = TransitionStart;
            transitionStep.OnComplete = TransitionComplete;
        }

        #endregion Create transitionStep

        /// <summary>
        /// Perform the action
        /// </summary>
        /// <returns></returns>
        protected void PerformAction()
        {
            var transitionStep = CreateTransitionStep();
            SetupTransitionStep(transitionStep);
            transitionStep.Start();
        }

    }
}
#endif