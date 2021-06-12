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
    /// PlayMaker Action to fade the screen.
    /// </summary>
    [ActionCategory("Beautiful Transitions")]
    [HutongGames.PlayMaker.Tooltip("Fade the screen")]
    [Title("Fade Screen")]
    public class BT_FadeScreen : PlayMakerTransitionScreenBase
    {
        [HutongGames.PlayMaker.Tooltip("Optional overlay texture to use.")]
        [ObjectType(typeof(Texture2D))]
        public FsmObject Texture;

        [HutongGames.PlayMaker.Tooltip("Tint color.")]
        public FsmColor Color;

        #region PlayMaker Life Cycle

        public override void Reset()
        {
            base.Reset();
            Texture = null;
            Color = new FsmColor() { Value = UnityEngine.Color.black };
        }

        #endregion PlayMaker Life Cycle

        #region Create transitionStep

        /// <summary>
        /// Get an instance of the current transition item
        /// </summary>
        /// <returns></returns>
        public override TransitionStep CreateTransitionStep()
        {
            return new ScreenFade(Owner.gameObject);
        }


        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStep(TransitionStep transitionStep)
        {
            var transitionStepScreenFade = transitionStep as ScreenFade;
            if (transitionStepScreenFade != null)
            {
                transitionStepScreenFade.Color = Color.Value;
                transitionStepScreenFade.Texture = (Texture2D)Texture.Value;
                //transitionStepScreenFade.SkipOnCrossTransition = SkipOnCrossTransition.Value;
            }
            base.SetupTransitionStep(transitionStep);
        }
        #endregion Create transitionStep
    }
}
#endif