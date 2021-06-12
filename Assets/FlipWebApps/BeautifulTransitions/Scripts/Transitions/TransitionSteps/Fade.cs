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
    /// Transition step for fading a canvas group, image or text.
    /// </summary>
    public class Fade : TransitionStepFloat {

        CanvasGroup[] _canvasGroups = new CanvasGroup[0];
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

        public Fade(UnityEngine.GameObject target,
            float startTransparency = 0,
            float endTransparency = 1,
            float delay = 0,
            float duration = 0.5f,
            TransitionModeType transitionMode = TransitionModeType.Specified,
            TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null) :
                base(target, startValue: startTransparency, endValue: endTransparency, 
                    delay: delay, duration: duration, transitionMode: transitionMode, timeUpdateMethod: timeUpdateMethod, tweenType: tweenType,
                    animationCurve: animationCurve, onStart: onStart,onUpdate: onUpdate, onComplete: onComplete)
        {
        }

#endregion Constructors

#region TransitionStepValue Overrides

        /// <summary>
        /// Get the current transparency level.
        /// </summary>
        /// <returns></returns>
        public override float GetCurrent()
        {
            if (!_hasComponentReferences)
                SetupComponentReferences();

            if (_canvasGroups.Length > 0)
                return _canvasGroups[0].alpha;
            if (_images.Length > 0)
                return _images[0].color.a;
            if (_rawImages.Length > 0)
                return _rawImages[0].color.a;
            if (_texts.Length > 0)
                return _texts[0].color.a;
            if (_spriteRenderers.Length > 0)
                return _spriteRenderers[0].color.a;
            if (_materials.Length > 0)
                return _materials[0].color.a;
#if TEXTMESH_PRO
            if (_tmpuiTexts.Length > 0)
                return _tmpuiTexts[0].color.a;
#endif
            return 1;
        }

        /// <summary>
        /// Set the current transparency level
        /// </summary>
        /// <param name="transparency"></param>
        public override void SetCurrent(float transparency)
        {
            if (!_hasComponentReferences)
                SetupComponentReferences();
            foreach (var canvas in _canvasGroups)
                canvas.alpha = transparency;
            foreach (var image in _images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, transparency);
            foreach (var rawImage in _rawImages)
                rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, transparency);
            foreach (var text in _texts)
                text.color = new Color(text.color.r, text.color.g, text.color.b, transparency);
            foreach (var spriteRenderer in _spriteRenderers)
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, transparency);
            foreach (var material in _materials)
                material.color = new Color(material.color.r, material.color.g, material.color.b, transparency);
#if TEXTMESH_PRO
            foreach (var text in _tmpuiTexts)
                text.color = new Color(text.color.r, text.color.g, text.color.b, transparency);
#endif
        }

        #endregion TransitionStepValue Overrides

        /// <summary>
        /// Get component references
        /// </summary>
        void SetupComponentReferences()
        {
            _canvasGroups = new CanvasGroup[0];
            _images = new Image[0];
            _rawImages = new RawImage[0];
            _texts = new Text[0];
            _spriteRenderers = new SpriteRenderer[0];
            _materials = new Material[0];
#if TEXTMESH_PRO
            _tmpuiTexts = new TMPro.TextMeshProUGUI[0];
#endif
            // get the components to work on target
            var canvasGroup = Target.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                _canvasGroups = _canvasGroups.Concat(Enumerable.Repeat(canvasGroup, 1)).ToArray();
            }
            else
            {
                var image = Target.GetComponent<Image>();
                if (image != null)
                    _images = _images.Concat(Enumerable.Repeat(image, 1)).ToArray();

                var rawImage = Target.GetComponent<RawImage>();
                if (rawImage != null)
                    _rawImages = _rawImages.Concat(Enumerable.Repeat(rawImage, 1)).ToArray();

                var text = Target.GetComponent<Text>();
                if (text != null)
                    _texts = _texts.Concat(Enumerable.Repeat(text, 1)).ToArray();
            }

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
    }

#region TransitionStep extensions

    public static class FadeExtensions
    {
        /// <summary>
        /// Fade extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Fade Fade(this TransitionStep transitionStep,
            float startTransparency,
            float endTransparency,
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
            var newTransitionStep = new Fade(transitionStep.Target,
                startTransparency,
                endTransparency,
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
        /// Fade extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Fade FadeToOriginal(this TransitionStep transitionStep,
            float startTransparency,
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
            var newTransitionStep = transitionStep.Fade(startTransparency,
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
        /// Fade extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Fade FadeToCurrent(this TransitionStep transitionStep,
            float startTransparency,
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
            var newTransitionStep = transitionStep.Fade(startTransparency,
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
        /// Fade extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Fade FadeFromOriginal(this TransitionStep transitionStep,
            float endTransparency,
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
            var newTransitionStep = transitionStep.Fade(0,
                endTransparency,
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
        /// Fade extension method for TransitionStep
        /// </summary>
        /// <returns></returns>
        public static Fade FadeFromCurrent(this TransitionStep transitionStep,
            float endTransparency,
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
            var newTransitionStep = transitionStep.Fade(0,
                endTransparency,
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
