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
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using HutongGames.PlayMaker;

namespace BeautifulTransitions.Scripts.Transitions.PlayMakerActions.Screen.AbstractClasses
{
    public abstract class PlayMakerTransitionScreenBase : PlayMakerTransitionBase
    {
        public enum TransitionDirectionType
        {
            In,
            Out
            // , Custom - add in and use StartLevel / EndLevel below.
        };

        //[Range(0, 1)]
        //[Tooltip("Start progress. To transition out set to 0, to transition in set to 1.")]
        //public float StartLevel = 0;
        //[Range(0, 1)]
        //[Tooltip("End progress. To transition out set to 1, to transition in set to 0.")]
        //public float EndLevel = 1;
        [HutongGames.PlayMaker.Tooltip("Whether to transition in or out.")]
        [ObjectType(typeof(TransitionDirectionType))]
        public FsmEnum TransitionDirection;

        //[Tooltip("Skip running this if there is already a cross transition in progress. Useful for e.g. your entry scene where on first run you enter directly (running this transition), but might later cross transition to from another scene and so not want this transition to run.")]
        //public bool SkipOnCrossTransition = true;
        [HutongGames.PlayMaker.Tooltip("Whether and how to transition to a new scene.")]
        [ObjectType(typeof(TransitionStepScreen.SceneChangeModeType))]
        public FsmEnum SceneChangeMode;

        [HutongGames.PlayMaker.Tooltip("If transitioning to a new scene then the name of the scene to transition to.")]
        public FsmString SceneToLoad;

        #region PlayMaker Life Cycle

        public override void Reset()
        {
            base.Reset();
            TransitionDirection = new FsmEnum(null, typeof(TransitionDirectionType), (int)TransitionDirectionType.Out);
            SceneChangeMode = new FsmEnum(null, typeof(TransitionStepScreen.SceneChangeModeType), (int)TransitionStepScreen.SceneChangeModeType.None);
            TransitionDirection = TransitionDirectionType.Out;
            SceneChangeMode = TransitionStepScreen.SceneChangeModeType.None;
            SceneToLoad = new FsmString();
        }

        #endregion PlayMaker Life Cycle

        #region Create transitionStep
        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStep(TransitionStep transitionStep)
        {
            var transitionStepScreen = transitionStep as TransitionStepScreen;
            if (transitionStepScreen != null)
            {
                transitionStepScreen.StartValue = (TransitionDirectionType)TransitionDirection.Value == TransitionDirectionType.In ? 1 : 0;
                transitionStepScreen.EndValue = (TransitionDirectionType)TransitionDirection.Value == TransitionDirectionType.In ? 0 : 1;
                //transitionStepScreenWipe.SkipOnCrossTransition = SkipOnCrossTransition;
                transitionStepScreen.SceneChangeMode = (TransitionStepScreen.SceneChangeModeType)SceneChangeMode.Value;
                transitionStepScreen.SceneToLoad = SceneToLoad.Value;
            }
            base.SetupTransitionStep(transitionStep);
        }

        #endregion Create transitionStep
    }
}
#endif