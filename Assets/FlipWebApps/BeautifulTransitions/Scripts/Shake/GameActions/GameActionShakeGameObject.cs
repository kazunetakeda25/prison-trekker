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
using GameFramework.GameStructure.Game.ObjectModel.Abstract;
using GameFramework.Helper;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeautifulTransitions.Scripts.Shake.GameActions
{
    /// <summary>
    /// GameAction class that shakes the specified target GameObject.
    /// </summary>
    [System.Serializable]
    [ClassDetails("Shake GameObject", "Beautiful Transitions/Shake/Shake GameObject", "Shakes the specified GameObject.")]
    public class GameActionShakeGameObject : GameActionTarget
    {

        /// <summary>
        /// How long to shake the GameObject for
        /// </summary>
        public float Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }
        [Tooltip("How long to shake the GameObject for")]
        [SerializeField]
        float _duration;


        /// <summary>
        /// The shake movement range from the origin. Set any dimension to 0 to stop movement along that axis.
        /// </summary>
        public Vector3 Range
        {
            get
            {
                return _range;
            }
            set
            {
                _range = value;
            }
        }
        [Tooltip("The shake movement range from the origin. Set any dimension to 0 to stop movement along that axis.")]
        [SerializeField]
        Vector3 _range;


        /// <summary>
        /// The offset relative to duration after which to start decaying (slowing down) the movement in the range 0 to 1.
        /// </summary>
        public float DecayStart
        {
            get
            {
                return _decayStart;
            }
            set
            {
                _decayStart = value;
            }
        }
        [Tooltip("The offset relative to duration after which to start decaying (slowing down) the movement in the range 0 to 1.")]
        [SerializeField]
        [Range(0, 1)]
        float _decayStart;

        /// <summary>
        /// Perform the action
        /// </summary>
        /// <returns></returns>
        protected override void Execute(bool isStart)
        {
            if (Target != null)
            {
                ShakeHelper.Shake(Owner, Target.transform, Duration, Range, DecayStart);
            }
        }
    }
}
#endif