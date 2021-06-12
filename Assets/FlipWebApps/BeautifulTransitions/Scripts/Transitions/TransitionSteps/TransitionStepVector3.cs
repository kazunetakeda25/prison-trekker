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
    /// A transition step that is based upon a Vector3
    /// </summary>
    public class TransitionStepVector3 : TransitionStepValue<Vector3>
    {

        #region Constructors

        public TransitionStepVector3(UnityEngine.GameObject target = null,
            Vector3? startValue = null,
            Vector3? endValue = null,
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
                base(target, delay, duration, transitionMode, timeUpdateMethod, tweenType,
                animationCurve, coordinateSpace, onStart, onUpdate, onComplete)
        {
            StartValue = startValue.GetValueOrDefault();
            EndValue = endValue.GetValueOrDefault();
            OriginalValue = GetCurrent();
        }

        #endregion Constructors

        #region TransitionStep Overrides

        /// <summary>
        /// Set the start value for when progress is 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        TransitionStep SetStartValue(Vector3 value)
        {
            StartValue = value;
            return this;
        }


        /// <summary>
        /// Set the end value for when progress is 1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        TransitionStep SetEndValue(Vector3 value)
        {
            EndValue = value;
            return this;
        }

        /// <summary>
        /// Start the transition. 
        /// </summary>
        /// This override set values based upon the TransitionMode.
        public override void Start()
        {
            if (TransitionMode == TransitionModeType.ToOriginal)
                EndValue = OriginalValue;
            else if (TransitionMode == TransitionModeType.ToCurrent)
                EndValue = GetCurrent();
            else if (TransitionMode == TransitionModeType.FromCurrent)
                StartValue = GetCurrent();
            else if (TransitionMode == TransitionModeType.FromOriginal)
                StartValue = OriginalValue;

            base.Start();
        }


        /// <summary>
        /// Override this if you need to update based upon the progress (0..1)
        /// </summary>
        protected override void ProgressUpdated()
        {
            Value = new Vector3(
                ValueFromProgressTweened(StartValue.x, EndValue.x),
                ValueFromProgressTweened(StartValue.y, EndValue.y),
                ValueFromProgressTweened(StartValue.z, EndValue.z));
            SetCurrent(Value);
        }

        #endregion TransitionStep Overrides
    }
}
