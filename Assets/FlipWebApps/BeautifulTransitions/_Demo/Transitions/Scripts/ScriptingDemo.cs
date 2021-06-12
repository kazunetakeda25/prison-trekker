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

using UnityEngine;
using BeautifulTransitions.Scripts.Transitions;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using UnityEngine.UI;

namespace BeautifulTransitions._Demo.Transitions.Scripts
{
    /// <summary>
    /// Demonstration of using some of the scripting features in Beautiful Transitions
    /// </summary>
    public class ScriptingDemo : MonoBehaviour
    {

        // Step 1 - Basic transitions
        public GameObject TestGameObject;
        // Step 2 - Linked Transitions In One Call
        public GameObject TestGameObject2;
        // Step 3 - Transition With Callback
        public GameObject TestGameObject3;
        // Step 4 - Custom TransitionStep
        public Text Counter;
        // Step 5 - Scale and Fade
        public GameObject TestGameObject5;
        // Step 6 - Thats all folks
        public Text Description;

        void Start()
        {
            ShowTransitionedDescription("Basic linked transitions  with events");

            // Create a new transition to move the gameobject with a delay of 1 and duration of 3 and the specified actions
            var startPosition = new Vector3(10, 0, 0);
            var endPosition = Vector3.zero;
            var transition = new Move(TestGameObject, startPosition, endPosition, 1, 3,
                tweenType: TransitionHelper.TweenType.easeInOutBack, onStart: LogStart, onUpdate: LogUpdate, onComplete: LogComplete);

            // Add an additional complete action with custom data
            transition.AddOnCompleteAction(LogComplete2, "Complete Parameter");

            // chaing some additional transitions and on complete call next transition
            transition.ScaleToOriginal(Vector3.zero, 1, 1, runAtStart: true).
                RotateFromCurrent(new Vector3(360, 0, 0), 1, 3, coordinateMode: TransitionStep.CoordinateSpaceType.Local, runAtStart: true).
                RotateToOriginal(new Vector3(180, 0, 0), duration: 2).
                ScaleFromCurrent(Vector3.zero, delay: 1, duration: 2, runAtStart: true, onComplete: Step2LinkedTransitionsInOneCall);

            // start everything.
            transition.Start();
        }

        /// <summary>
        /// Several linked transitions in one call.
        /// </summary>
        void Step2LinkedTransitionsInOneCall(TransitionStep transitionStep)
        {
            ShowTransitionedDescription("Linked transitions in one call.");

            // transition the second item.
            var transition = new Scale(TestGameObject2, Vector3.zero, Vector3.one * 5, 0, 3, tweenType: TransitionHelper.TweenType.easeInOutBack).
                ScaleFromCurrent(new Vector3(7.5f, 2.5f, 1), 1, 2, tweenType: TransitionHelper.TweenType.easeInOutBack).
                ScaleFromCurrent(new Vector3(10, 10, 1), 1, 2, tweenType: TransitionHelper.TweenType.easeInOutBack).
                ScaleFromCurrent(Vector3.zero, 1, 2, tweenType: TransitionHelper.TweenType.easeInOutBack, onComplete: Step3TransitionWithCallback).GetChainRoot();
            transition.Start();
        }

        /// <summary>
        /// Trigger Component transition with callbacks.
        /// </summary>
        void Step3TransitionWithCallback(TransitionStep transitionStep)
        {
            ShowTransitionedDescription("Trigger a transition component with callback.");

            // transition the third item which is a component that has triggers to transition out again when the in
            // transition has completed and a transition out on complete trigger to run the next step.
            TransitionHelper.TransitionIn(TestGameObject3); //, CustomTransitionStep);

            // Rather than using a callback on the component we could also have passed an onComplete method 
            // to TransitionIn as shown below. That would however return after the transition in completed.
            // Here we want to run the components transition out first and then proceed so we set the oncomplete
            // callback in the components transition out configuration instead.
            //TransitionHelper.TransitionIn(TestGameObject3, CustomTransitionStep);
        }

        /// <summary>
        /// Demonstration of using TransitionStep in a generic fashion for your own 
        /// transitions with an update callback.
        /// </summary>
        public void Step4CustomTransitionStep()
        {
            ShowTransitionedDescription("Custom TransitionStep for your own transitions.");

            // scale and fade in text, run custom transition step and then scale back out..
            new Fade(Counter.gameObject, 0, 1, 0, 4).
                ScaleToOriginal(Vector3.zero, 0, 3, runAtStart: true).
                ChainCustomTransitionStep(0, 2, tweenType: TransitionHelper.TweenType.linear, onUpdate: CustomTransitionStepUpdateCallback).
                ScaleFromCurrent(Vector3.zero, 0.5f, 2, tweenType: TransitionHelper.TweenType.easeInOutBack, onComplete: Step5FadeAndScale).
                GetChainRoot().Start();
        }

        void CustomTransitionStepUpdateCallback(TransitionStep transitionStep)
        {
            Counter.text = transitionStep.Progress.ToString();
        }


        /// <summary>
        /// Step 5 - Demonstrates fading and scaling at the same time using runAtStart on the second transition.
        /// </summary>
        void Step5FadeAndScale(TransitionStep transitionStep)
        {
            ShowTransitionedDescription("Fade and Scale in one.");

            // fade the end text in while at the same time scaling.
            new Fade(TestGameObject5, 0, 1, 0, 4).
                ScaleToOriginal(Vector3.zero, 0, 3, runAtStart: true).GetChainRoot().Start();
        }

        #region Helper Methods

        void ShowTransitionedDescription(string text)
        {
            // fade the end text in while at the same time scaling.
            Description.text = text;
            var transition = new Fade(Description.gameObject, 0, 1);
            transition.FadeFromCurrent(0, delay: 3);
            transition.GetChainRoot().Start();
        }

        void LogStart(TransitionStep transitionStep)
        {
            Debug.Log("Start");
        }

        void LogUpdate(TransitionStep transitionStep)
        {
            Debug.Log("Update:" + transitionStep.Progress);
        }

        void LogComplete(TransitionStep transitionStep)
        {
            Debug.Log("Complete with user data:" + (transitionStep == null ? "<null>" : transitionStep.UserData.ToString()));
        }

        void LogComplete2(TransitionStep transitionStep)
        {
            Debug.Log("Complete (second callback)");
        }

        public void ShowRatePage()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/56442");
        }

        #endregion Helper Methods

    }
}