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

#if PLAYMAKER
using BeautifulTransitions.Scripts.Transitions.PlayMakerActions.Screen.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using HutongGames.PlayMaker;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.PlayMakerActions.Screen
{
    /// <summary>
    /// PlayMaker Action to wipe the screen.
    /// </summary>
    [ActionCategory("Beautiful Transitions")]
    [HutongGames.PlayMaker.Tooltip("Wipe the screen")]
    [Title("Wipe Screen")]
    public class BT_WipeScreen : PlayMakerTransitionScreenBase
    {
        [HutongGames.PlayMaker.Tooltip("Optional overlay texture to use.")]
        [ObjectType(typeof(Texture2D))]
        public FsmObject Texture;

        [HutongGames.PlayMaker.Tooltip("Tint color.")]
        public FsmColor Color;

        [HutongGames.PlayMaker.Tooltip("Gray scale wipe mask.")]
        public Texture2D MaskTexture;

        [HutongGames.PlayMaker.Tooltip("Whether to invert the wipe mask.")]
        public FsmBool InvertMask;

        [HutongGames.PlayMaker.Tooltip("The amount of softness to apply to the wipe")]
        [HasFloatSlider(0, 1)]
        public FsmFloat Softness;


        #region PlayMaker Life Cycle

        public override void Reset()
        {
            base.Reset();
            Texture = null;
            Color = new FsmColor() { Value = UnityEngine.Color.white };
            MaskTexture = null;
            InvertMask = null;
            Softness = new FsmFloat();
        }

        #endregion PlayMaker Life Cycle

        #region Create transitionStep

        /// <summary>
        /// Get an instance of the current transition item
        /// </summary>
        /// <returns></returns>
        public override TransitionStep CreateTransitionStep()
        {
            return new ScreenWipe(Owner.gameObject, null);
        }


        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStep(TransitionStep transitionStep)
        {
            var transitionStepScreenWipe = transitionStep as ScreenWipe;
            if (transitionStepScreenWipe != null)
            {
                transitionStepScreenWipe.MaskTexture = MaskTexture;
                transitionStepScreenWipe.InvertMask = InvertMask.Value;
                transitionStepScreenWipe.Color = Color.Value;
                transitionStepScreenWipe.Texture = (Texture2D)Texture.Value;
                transitionStepScreenWipe.Softness = Softness.Value;
            }
            base.SetupTransitionStep(transitionStep);
        }
        #endregion Create transitionStep
    }
}
#endif