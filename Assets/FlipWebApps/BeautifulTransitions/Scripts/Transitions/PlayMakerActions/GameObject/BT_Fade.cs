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
using BeautifulTransitions.Scripts.Transitions.PlayMakerActions.GameObject.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using HutongGames.PlayMaker;

namespace BeautifulTransitions.Scripts.Transitions.PlayMakerActions.GameObject
{
    /// <summary>
    /// GameAction to transition in the specified transition.
    /// </summary>
    [ActionCategory("Beautiful Transitions")]
    [HutongGames.PlayMaker.Tooltip("Fade the specified target")]
    [Title("Fade")]
    public class BT_Fade : PlayMakerTransitionGameObjectBase
    {
        [HutongGames.PlayMaker.Tooltip("Normalised transparency at the start of the transition.")]
        [HasFloatSlider(0, 1)]
        public FsmFloat StartTransparency = 0;
        
        [HutongGames.PlayMaker.Tooltip("Normalised transparency at the end of the transition.")]
        [HasFloatSlider(0, 1)]
        public FsmFloat EndTransparency = 0;

#region PlayMaker Life Cycle

        public override void Reset()
        {
            base.Reset();
            StartTransparency = new FsmFloat();
            EndTransparency = new FsmFloat();
        }

#endregion PlayMaker Life Cycle

#region Create transitionStep

        /// <summary>
        /// Get an instance of the current transition item
        /// </summary>
        /// <returns></returns>
        public override TransitionStep CreateTransitionStep()
        {
            return new Fade(Fsm.GetOwnerDefaultTarget(Target));
        }


        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStep(TransitionStep transitionStep)
        {
            var transitionStepFade = transitionStep as Fade;
            if (transitionStepFade != null)
            {
                transitionStepFade.StartValue = StartTransparency.Value;
                transitionStepFade.EndValue = EndTransparency.Value;
            }
            base.SetupTransitionStep(transitionStep);
        }

#endregion Create transitionStep
    }
}
#endif