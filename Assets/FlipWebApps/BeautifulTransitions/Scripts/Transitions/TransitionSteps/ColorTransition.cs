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
using System.Collections.Generic;

namespace BeautifulTransitions.Scripts.Transitions.TransitionSteps
{
    /// <summary>
    /// Transition step for transitinoing the color of a canvas group, image or text.
    /// </summary>
    public class ColorTransition : TransitionStep
    {
        public Color StartValue
        {
            get { return _startValue; }
            set
            {
                _startValue = value;
                if (Gradient == null)
                    Gradient = new UnityEngine.Gradient();
                // colorKeys & alphaKeys are sorted and always have >=2 elements
                var colorKeys = new List<GradientColorKey>(Gradient.colorKeys);
                if (Mathf.Approximately(Gradient.colorKeys[0].time, 0))
                    colorKeys[0] = new GradientColorKey(EndValue, 0);
                else
                    colorKeys.Insert(0, new GradientColorKey(StartValue, 0));
                Gradient.colorKeys = colorKeys.ToArray();

                var alphaKeys = new List<GradientAlphaKey>(Gradient.alphaKeys);
                if (Mathf.Approximately(Gradient.alphaKeys[0].time, 0))
                    alphaKeys[0] = new GradientAlphaKey(EndValue.a, 0);
                else
                    alphaKeys.Insert(0, new GradientAlphaKey(StartValue.a, 0));
                Gradient.alphaKeys = alphaKeys.ToArray();
            }
        }
        Color _startValue;

        public Color EndValue
        {
            get { return _endValue; }
            set
            {
                _endValue = value;
                if (Gradient == null)
                    Gradient = new UnityEngine.Gradient();
                // colorKeys & alphaKeys are sorted and always have >=2 elements
                var colorKeys = new List<GradientColorKey>(Gradient.colorKeys);
                if (Mathf.Approximately(Gradient.colorKeys[Gradient.colorKeys.Length - 1].time, 1))
                    colorKeys[colorKeys.Count - 1] = new GradientColorKey(EndValue, 1);
                else
                    colorKeys.Add(new GradientColorKey(EndValue, 1));
                Gradient.colorKeys = colorKeys.ToArray();

                var alphaKeys = new List<GradientAlphaKey>(Gradient.alphaKeys);
                if (Mathf.Approximately(Gradient.alphaKeys[Gradient.alphaKeys.Length - 1].time, 1))
                    alphaKeys[Gradient.alphaKeys.Length - 1] = new GradientAlphaKey(EndValue.a, 1);
                else
                    alphaKeys.Add(new GradientAlphaKey(EndValue.a, 1));
                Gradient.alphaKeys = alphaKeys.ToArray();
            }
        }
        Color _endValue;

        public Color Value { get; set; }

        public Color OriginalValue { get; set; }

        public UnityEngine.Gradient Gradient { get; set; }

        Image[] _images = new Image[0];
        RawImage[] _rawImages = new RawImage[0];
        Text[] _texts = new Text[0];
        SpriteRenderer[] _spriteRenderers = new SpriteRenderer[0];
        Material[] _materials = new Material[0];
#if TEXTMESH_PRO
        TMPro.TextMeshProUGUI[] _tmpuiTexts = new TMPro.TextMeshProUGUI[0];
#endif
        bool _hasComponentReferences;

        #region Constructors

        public ColorTransition(UnityEngine.GameObject target,
            Color startColor,
            Color endColor,
            float delay = 0,
            float duration = 0.5f,
            TransitionModeType transitionMode = TransitionModeType.Specified,
            TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null) :
                base(target,
                    delay: delay, duration: duration, transitionMode: transitionMode, timeUpdateMethod: timeUpdateMethod, tweenType: tweenType,
                    animationCurve: animationCurve, onStart: onStart, onUpdate: onUpdate, onComplete: onComplete)
        {
            Gradient = new UnityEngine.Gradient();
            StartValue = startColor;
            EndValue = endColor;
            OriginalValue = GetCurrent();
        }


        public ColorTransition(UnityEngine.GameObject target,
            UnityEngine.Gradient gradient = null,
            float delay = 0,
            float duration = 0.5f,
            TransitionModeType transitionMode = TransitionModeType.Specified,
            TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null) :
                base(target,
                    delay: delay, duration: duration, transitionMode: transitionMode, timeUpdateMethod: timeUpdateMethod, tweenType: tweenType,
                    animationCurve: animationCurve, onStart: onStart, onUpdate: onUpdate, onComplete: onComplete)
        {
            Gradient = gradient;
            OriginalValue = GetCurrent();
        }
        #endregion Constructors

        /// <summary>
        /// Set the start color for when progress is 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        TransitionStep SetStartValue(Color value)
        {
            StartValue = value;
            return this;
        }


        /// <summary>
        /// Set the end color for when progress is 1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        TransitionStep SetEndValue(Color value)
        {
            EndValue = value;
            return this;
        }


        /// <summary>
        /// Get the current transparency level.
        /// </summary>
        /// <returns></returns>
        public Color GetCurrent()
        {
            if (!_hasComponentReferences)
                SetupComponentReferences();

            if (_images.Length > 0)
                return _images[0].color;
            if (_rawImages.Length > 0)
                return _rawImages[0].color;
            if (_texts.Length > 0)
                return _texts[0].color;
            if (_spriteRenderers.Length > 0)
                return _spriteRenderers[0].color;
            if (_materials.Length > 0)
                return _materials[0].color;
#if TEXTMESH_PRO
            if (_tmpuiTexts.Length > 0)
                return _tmpuiTexts[0].color.a;
#endif
            return Color.black;
        }


        /// <summary>
        /// Set the current transparency level
        /// </summary>
        /// <param name="transparency"></param>
        public void SetCurrent(Color color)
        {
            if (!_hasComponentReferences)
                SetupComponentReferences();
            foreach (var image in _images)
                image.color = color;
            foreach (var rawImage in _rawImages)
                rawImage.color = color;
            foreach (var text in _texts)
                text.color = color;
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.color = color;
            foreach (var material in _materials)
                material.color = color;
#if TEXTMESH_PRO
            foreach (var text in _tmpuiTexts)
                text.color = color;
#endif
        }


        /// <summary>
        /// Get component references
        /// </summary>
        void SetupComponentReferences()
        {
            _images = new Image[0];
            _rawImages = new RawImage[0];
            _texts = new Text[0];
            _spriteRenderers = new SpriteRenderer[0];
            _materials = new Material[0];
#if TEXTMESH_PRO
            _tmpuiTexts = new TMPro.TextMeshProUGUI[0];
#endif
            // get the components to work on target
            var image = Target.GetComponent<Image>();
            if (image != null)
                _images = _images.Concat(Enumerable.Repeat(image, 1)).ToArray();

            var rawImage = Target.GetComponent<RawImage>();
            if (rawImage != null)
                _rawImages = _rawImages.Concat(Enumerable.Repeat(rawImage, 1)).ToArray();

            var text = Target.GetComponent<Text>();
            if (text != null)
                _texts = _texts.Concat(Enumerable.Repeat(text, 1)).ToArray();

            var spriteRenderer = Target.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                _spriteRenderers = _spriteRenderers.Concat(Enumerable.Repeat(spriteRenderer, 1)).ToArray();

            var meshRenderer = Target.GetComponent<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.material != null)
                _materials = _materials.Concat(Enumerable.Repeat(meshRenderer.material, 1)).ToArray();

#if TEXTMESH_PRO
            var tmpuiTexts = Target.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmpuiTexts != null)
            {
                _tmpuiTexts = _tmpuiTexts.Concat(Enumerable.Repeat(tmpuiTexts, 1)).ToArray();
            }
#endif

            _hasComponentReferences = true;
        }


        /// <summary>
        /// Override for start to set values based upon the mode.
        /// </summary>
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
            SetCurrent(Gradient.Evaluate(ProgressTweened));
        }
    }

    #region TransitionStep extensions

    public static class ColorTransitionExtensions
    {
        /// <summary>
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransition(this TransitionStep transitionStep,
            UnityEngine.Gradient gradient,
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
            var newTransitionStep = new ColorTransition(transitionStep.Target,
                gradient,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionToOriginal(this TransitionStep transitionStep,
            UnityEngine.Gradient gradient,
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
            var newTransitionStep = transitionStep.ColorTransition(gradient,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionToCurrent(this TransitionStep transitionStep,
            UnityEngine.Gradient gradient,
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
            var newTransitionStep = transitionStep.ColorTransition(gradient,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionFromOriginal(this TransitionStep transitionStep,
            UnityEngine.Gradient gradient,
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
            var newTransitionStep = transitionStep.ColorTransition(gradient,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionFromCurrent(this TransitionStep transitionStep,
            UnityEngine.Gradient gradient,
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
            var newTransitionStep = transitionStep.ColorTransition(gradient,
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

        /// <summary>
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransition(this TransitionStep transitionStep,
            Color startColor,
            Color endColor,
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
            var newTransitionStep = new ColorTransition(transitionStep.Target,
                startColor,
                endColor,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionToOriginal(this TransitionStep transitionStep,
            Color startColor,
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
            var newTransitionStep = transitionStep.ColorTransition(startColor,
                Color.black,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionToCurrent(this TransitionStep transitionStep,
            Color startColor,
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
            var newTransitionStep = transitionStep.ColorTransition(startColor,
                Color.black,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionFromOriginal(this TransitionStep transitionStep,
            Color endColor,
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
            var newTransitionStep = transitionStep.ColorTransition(Color.black,
                endColor,
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
        /// ColorTransition extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static ColorTransition ColorTransitionFromCurrent(this TransitionStep transitionStep,
            Color endColor,
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
            var newTransitionStep = transitionStep.ColorTransition(Color.black,
                endColor,
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

        #endregion TransitionStep extensions
    }
}
