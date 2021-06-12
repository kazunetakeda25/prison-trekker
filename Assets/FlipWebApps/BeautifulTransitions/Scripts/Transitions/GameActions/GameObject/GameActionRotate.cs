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
    /// GameAction to rotate a gameobject.
    /// </summary>
    [System.Serializable]
    [ClassDetails("Rotate", "Beautiful Transitions/Transitions/Rotate", "Rotate a gameobject.")]
    public class GameActionRotate : GameActionTransitionGameObjectBase
    {
        public enum RotationModeType
        {
            Global = TransitionStep.CoordinateSpaceType.Global,
            Local = TransitionStep.CoordinateSpaceType.Local
        };

        [Tooltip("The rotation mode to use.")]
        public RotationModeType RotationMode = RotationModeType.Local;

        [Tooltip("Start rotation.")]
        public Vector3 StartRotation = new Vector3(0, 0, 0);
        
        [Tooltip("End rotation.")]
        public Vector3 EndRotation = new Vector3(0, 0, 0);

        //Vector3 _originalRotation;

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <returns></returns>
        protected override void Initialise()
        {
            base.Initialise();
            //_originalRotation = ((Rotate)CreateTransitionStep()).OriginalValue;
        }


        /// <summary>
        /// Get an instance of the current transition item
        /// </summary>
        /// <returns></returns>
        public override TransitionStep CreateTransitionStep()
        {
            return new Rotate(ResolveTarget(), coordinateSpace: (TransitionStep.CoordinateSpaceType)RotationMode);
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
                transitionStepRotate.StartValue = StartRotation;
                transitionStepRotate.EndValue = EndRotation;
            }
            base.SetupTransitionStep(transitionStep);
        }
    }
}
#endif