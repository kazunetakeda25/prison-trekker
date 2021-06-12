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
    [HutongGames.PlayMaker.Tooltip("Rotate the specified target")]
    [Title("Rotate")]
    public class BT_Rotate : PlayMakerTransitionGameObjectBase
    {
        public enum RotationModeType
        {
            Global = TransitionStep.CoordinateSpaceType.Global,
            Local = TransitionStep.CoordinateSpaceType.Local
        };

        [HutongGames.PlayMaker.Tooltip("The rotation mode to use.")]
        [ObjectType(typeof(RotationModeType))]
        public FsmEnum RotationMode;

        [HutongGames.PlayMaker.Tooltip("Normalised Rotation at the start of the transition.")]
        public FsmVector3 StartRotation;
        
        [HutongGames.PlayMaker.Tooltip("Normalised Rotation at the end of the transition.")]
        public FsmVector3 EndRotation;

#region PlayMaker Life Cycle

        public override void Reset()
        {
            base.Reset();
            RotationMode = new FsmEnum(null, typeof(RotationModeType), (int)RotationModeType.Local);
            StartRotation = new FsmVector3();
            EndRotation = new FsmVector3();
        }

#endregion PlayMaker Life Cycle

#region Create transitionStep

        /// <summary>
        /// Get an instance of the current transition item
        /// </summary>
        /// <returns></returns>
        public override TransitionStep CreateTransitionStep()
        {
            return new Rotate(Fsm.GetOwnerDefaultTarget(Target), coordinateSpace: (TransitionStep.CoordinateSpaceType)RotationMode.Value);
        }


        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStep(TransitionStep transitionStep)
        {
            var transitionStepRotate = transitionStep as Rotate;
            if (transitionStepRotate != null)
            {
                transitionStepRotate.StartValue = StartRotation.Value;
                transitionStepRotate.EndValue = EndRotation.Value;
            }
            base.SetupTransitionStep(transitionStep);
        }

#endregion Create transitionStep
    }
}
#endif