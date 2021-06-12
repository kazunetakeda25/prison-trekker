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
using HutongGames.PlayMaker;

namespace BeautifulTransitions.Scripts.DisplayItem.PlayMakerActions
{
    /// <summary>
    /// Sets the active state and trigger animation update. 
    /// </summary>
    [ActionCategory("Beautiful Transitions")]
    [Tooltip("Sets the active state for a Display Item (e.g. Button) with the appripriate animation controller added, triggering an animation.")]
    [Title("Set Active Animated")]
    public class BT_SetActiveAnimated : FsmStateAction
    {
        /// <summary>
        /// The target GameObject to act upon
        /// </summary>
        [Tooltip("The target GameObject to act upon.")]
        [RequiredField]
        public FsmOwnerDefault Target;
 
        /// <summary>
        /// The active status to set
        /// </summary>
        [Tooltip("The active status to set")]
        [RequiredField]
        public FsmBool ActiveStatus;

//#if UNITY_EDITOR
//        public override string AutoName()
//        {
//            return "My Custom Action Name";
//        }
//#endif

        public override void Reset()
        {
            base.Reset();
            Target = null;
            ActiveStatus = false;
        }

        public override void OnEnter()
        {
            PerformAction();
            Finish();
        }

        /// <summary>
        /// Perform the action
        /// </summary>
        /// <returns></returns>
        protected void PerformAction()
        {
            var gameObject = Fsm.GetOwnerDefaultTarget(Target);
            if (gameObject != null)
            {
                DisplayItemHelper.SetActiveAnimated(Fsm.Owner, gameObject, ActiveStatus.Value);
            }
        }
    }
}
#endif