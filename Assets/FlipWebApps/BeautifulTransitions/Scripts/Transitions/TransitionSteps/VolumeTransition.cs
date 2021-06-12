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
using System.Linq;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using UnityEngine;
using UnityEngine.UI;

namespace BeautifulTransitions.Scripts.Transitions.TransitionSteps
{
    /// <summary>
    /// Transition step for changing the volume.
    /// </summary>
    public class VolumeTransition : TransitionStepFloat {

        AudioSource[] _audioSources = new AudioSource[0];
        bool _hasComponentReferences;

#region Constructors

        public VolumeTransition(UnityEngine.GameObject target,
            float startVolume = 0,
            float endVolume = 1,
            float delay = 0,
            float duration = 0.5f,
            TransitionModeType transitionMode = TransitionModeType.Specified,
            TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null) :
                base(target, startValue: startVolume, endValue: endVolume, 
                    delay: delay, duration: duration, transitionMode: transitionMode, timeUpdateMethod: timeUpdateMethod, tweenType: tweenType,
                    animationCurve: animationCurve, onStart: onStart,onUpdate: onUpdate, onComplete: onComplete)
        {
        }

#endregion Constructors

#region TransitionStepValue Overrides

        /// <summary>
        /// Get the current volume level.
        /// </summary>
        /// <returns></returns>
        public override float GetCurrent()
        {
            if (!_hasComponentReferences)
                SetupComponentReferences();

            if (_audioSources.Length > 0)
                return _audioSources[0].volume;
            return 1;
        }

        /// <summary>
        /// Set the current volume level
        /// </summary>
        /// <param name="transparency"></param>
        public override void SetCurrent(float volume)
        {
            if (!_hasComponentReferences)
                SetupComponentReferences();
            foreach (var audioSource in _audioSources)
                audioSource.volume = volume;
        }

        #endregion TransitionStepValue Overrides

        /// <summary>
        /// Get component references
        /// </summary>
        void SetupComponentReferences()
        {
            _audioSources = new AudioSource[0];
            var audioSource = Target.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                _audioSources = _audioSources.Concat(Enumerable.Repeat(audioSource, 1)).ToArray();
            }

            _hasComponentReferences = true;
        }
    }

#region TransitionStep extensions

    public static class VolumeTransitionExtensions
    {
        /// <summary>
        /// VolumeTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static VolumeTransition VolumeTransition(this TransitionStep transitionStep,
            float startVolume,
            float endVolume,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = new VolumeTransition(transitionStep.Target,
                startVolume,
                endVolume,
                delay,
                duration,
                transitionMode,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                onStart,
                onUpdate,
                onComplete);
            newTransitionStep.AddToChain(transitionStep, runAtStart);
            return newTransitionStep;
        }

        /// <summary>
        /// VolumeTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static VolumeTransition VolumeToOriginal(this TransitionStep transitionStep,
            float startVolume,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.VolumeTransition(startVolume,
                0,
                delay,
                duration,
                TransitionStep.TransitionModeType.ToOriginal,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }

        /// <summary>
        /// VolumeTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static VolumeTransition VolumeToCurrent(this TransitionStep transitionStep,
            float startVolume,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.VolumeTransition(startVolume,
                0,
                delay,
                duration,
                TransitionStep.TransitionModeType.ToCurrent,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }

        /// <summary>
        /// VolumeTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static VolumeTransition VolumeFromOriginal(this TransitionStep transitionStep,
            float endVolume,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.VolumeTransition(0,
                endVolume,
                delay,
                duration,
                TransitionStep.TransitionModeType.FromOriginal,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }

        /// <summary>
        /// VolumeTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static VolumeTransition VolumeFromCurrent(this TransitionStep transitionStep,
            float endVolume,
            float delay = 0,
            float duration = 0.5f,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = transitionStep.VolumeTransition(0,
                endVolume,
                delay,
                duration,
                TransitionStep.TransitionModeType.FromCurrent,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                runAtStart,
                onStart,
                onUpdate,
                onComplete);
            return newTransitionStep;
        }
    }
#endregion TransitionStep extensions
}
