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

using System;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.TransitionSteps
{
    /// <summary>
    /// Transition step for moving a gameobject.
    /// </summary>
    public class Move : TransitionStepVector3 {

        #region Constructors

        public Move(UnityEngine.GameObject target,
            Vector3? startPosition = null,
            Vector3? endPosition = null,
            float delay = 0,
            float duration = 0.5f,
            TransitionModeType transitionMode = TransitionModeType.Specified,
            TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null) :
                base(target, startPosition, endPosition, delay: delay, duration: duration, transitionMode: transitionMode, timeUpdateMethod:timeUpdateMethod, tweenType: tweenType,
                animationCurve: animationCurve, coordinateSpace: coordinateSpace, onStart: onStart, onUpdate: onUpdate, onComplete: onComplete)
        {
            //TODO: Validation . where to place!
            //if (MoveMode == MoveModeType.AnchoredPosition)
            //    Assert.IsNotNull(Target.transform as RectTransform, "The target of TransitionMove must contain a RectTransform component (not just a standard Transform component) when using MoveMode of type AnchoredPosition");

        }

        #endregion Constructors

        #region TransitionStepValue Overrides

        /// <summary>
        /// Get the current position based upon the current CoordinateMode
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetCurrent()
        {
            if (CoordinateSpace == CoordinateSpaceType.Global)
                return Target.transform.position;
            else if (CoordinateSpace == CoordinateSpaceType.Local)
                return Target.transform.localPosition;
            else //CoordinateSpaceType.AnchoredPosition
                return ((RectTransform)Target.transform).anchoredPosition;
        }

        /// <summary>
        /// Set the current position based upon the current CoordinateMode
        /// </summary>
        /// <param name="position"></param>
        public override void SetCurrent(Vector3 position)
        {
            if (CoordinateSpace == CoordinateSpaceType.Global)
                Target.transform.position = position;
            else if (CoordinateSpace == CoordinateSpaceType.Local)
                Target.transform.localPosition = position;
            else //CoordinateSpaceType.AnchoredPosition
                ((RectTransform)Target.transform).anchoredPosition = position;
        }

        #endregion TransitionStepValue Overrides
    }

    #region TransitionStep extensions

    public static class MoveExtensions
    {
        /// <summary>
        /// Move extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Move Move(this TransitionStep transitionStep,
            Vector3 startPosition,
            Vector3 endPosition,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = new Move(transitionStep.Target,
                startPosition,
                endPosition,
                delay,
                duration,
                transitionMode,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                coordinateMode,
                onStart,
                onUpdate,
                onComplete);
            newTransitionStep.AddToChain(transitionStep, runAtStart);
            return newTransitionStep;
        }

        /// <summary>
        /// Move extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Move MoveToOriginal(this TransitionStep transitionStep,
            Vector3 startPosition,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.Move(startPosition,
                Vector3.zero,
                delay,
                duration,
                TransitionStep.TransitionModeType.ToOriginal,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                coordinateMode,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }

        /// <summary>
        /// Move extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Move MoveToCurrent(this TransitionStep transitionStep,
            Vector3 startPosition,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.Move(startPosition,
                Vector3.zero,
                delay,
                duration,
                TransitionStep.TransitionModeType.ToCurrent,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                coordinateMode,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }

        /// <summary>
        /// Move extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Move MoveFromOriginal(this TransitionStep transitionStep,
            Vector3 endPosition,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.Move(Vector3.zero,
                endPosition,
                delay,
                duration,
                TransitionStep.TransitionModeType.FromOriginal,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                coordinateMode,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }

        /// <summary>
        /// Move extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Move MoveFromCurrent(this TransitionStep transitionStep,
            Vector3 endPosition,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            TransitionStep.CoordinateSpaceType coordinateMode = TransitionStep.CoordinateSpaceType.Global,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.Move(Vector3.zero,
                endPosition,
                delay,
                duration,
                TransitionStep.TransitionModeType.FromCurrent,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                coordinateMode,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }
    }

    #endregion TransitionStep extensions

}
