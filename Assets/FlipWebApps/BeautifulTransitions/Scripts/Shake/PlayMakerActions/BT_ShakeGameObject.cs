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

namespace BeautifulTransitions.Scripts.Shake.PlayMakerActions
{
    /// <summary>
    /// Shake the specified target GameObject.
    /// </summary>
    [ActionCategory("Beautiful Transitions")]
    [Tooltip("Shake the specified GameObject")]
    [Title("Shake GameObject")]
    public class BT_ShakeGameObject : FsmStateAction
    {
        /// <summary>
        /// The target GameObject to act upon
        /// </summary>
        [Tooltip("The target GameObject to act upon.")]
        [RequiredField]
        public FsmOwnerDefault Target;

        /// <summary>
        /// How long to shake the GameObject for
        /// </summary>
        [Tooltip("How long to shake the GameObject for")]
        [RequiredField]
        public FsmFloat Duration;

        /// <summary>
        /// The shake movement range from the origin. Set any dimension to 0 to stop movement along that axis.
        /// </summary>
        [Tooltip("The shake movement range from the origin. Set any dimension to 0 to stop movement along that axis.")]
        [RequiredField]
        public FsmVector3 Range;

        /// <summary>
        /// The offset relative to duration after which to start decaying (slowing down) the movement in the range 0 to 1.
        /// </summary>
        [Tooltip("The offset relative to duration after which to start decaying (slowing down) the movement in the range 0 to 1.")]
        [HasFloatSlider(0, 1)]
        [RequiredField]
        public FsmFloat DecayStart;

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
            Duration = 1;
            Range = UnityEngine.Vector3.one;
            DecayStart = 0;
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
                ShakeHelper.Shake(Fsm.Owner, gameObject.transform, Duration.Value, Range.Value, DecayStart.Value);
            }
        }
    }
}
#endif