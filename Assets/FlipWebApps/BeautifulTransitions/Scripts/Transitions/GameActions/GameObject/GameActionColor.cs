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

#if GAME_FRAMEWORK
using BeautifulTransitions.Scripts.Transitions.GameActions.GameObject.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using GameFramework.Helper;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.GameActions.GameObject
{
    /// <summary>
    /// GameAction to transition the color of a canvas group, image or text.
    /// </summary>
    [System.Serializable]
    [ClassDetails("Color", "Beautiful Transitions/Transitions/Color", "Transition the color of a canvas group, image or text.")]
    public class GameActionColor : GameActionTransitionGameObjectBase
    {
        [Tooltip("Gradient to use for the transition in. Note the start / end color may be overridden depending on the mode.")]
        public Gradient Gradient;

        //Color _originalColor;

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <returns></returns>
        protected override void Initialise()
        {
            base.Initialise();
            //_originalColor = ((ColorTransition)CreateTransitionStep()).OriginalValue;
        }


        /// <summary>
        /// Get an instance of the current transition item
        /// </summary>
        /// <returns></returns>
        public override TransitionStep CreateTransitionStep()
        {
            return new ColorTransition(ResolveTarget());
        }


        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStep(TransitionStep transitionStep)
        {
            var transitionStepColor = transitionStep as ColorTransition;
            if (transitionStepColor != null)
            {
                transitionStepColor.Gradient = Gradient;
            }
            base.SetupTransitionStep(transitionStep);
        }
    }
}
#endif