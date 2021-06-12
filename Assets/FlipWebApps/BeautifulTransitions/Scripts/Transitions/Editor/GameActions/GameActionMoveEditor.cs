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

using BeautifulTransitions.Scripts.Transitions.GameActions.GameObject;
using UnityEditor;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.Editor.GameActions
{
    [CustomEditor(typeof(GameActionMove))]
    public class GameActionMoveEditor : GameActionTransitionGameObjectBaseEditor
    {
        SerializedProperty _transitionModeProperty;
        SerializedProperty _coordinateModeProperty;
        SerializedProperty _startPositionTypeProperty;
        SerializedProperty _startPositionProperty;
        SerializedProperty _endPositionTypeProperty;
        SerializedProperty _endPositionProperty;

        /// <summary>
        /// Get a reference to properties
        /// </summary>
        protected override void Initialise()
        {
            base.Initialise();
            _transitionModeProperty = serializedObject.FindProperty("TransitionMode");
            _coordinateModeProperty = serializedObject.FindProperty("CoordinateMode");
            _startPositionTypeProperty = serializedObject.FindProperty("StartPositionType");
            _startPositionProperty = serializedObject.FindProperty("StartPosition");
            _endPositionTypeProperty = serializedObject.FindProperty("EndPositionType");
            _endPositionProperty = serializedObject.FindProperty("EndPosition");
        }

        /// <summary>
        /// Override in subclasses to show GUI elements before content.
        /// </summary>
        /// <returns></returns>
        //protected override void ShowHeaderGUI() { }


        /// <summary>
        /// Override in subclasses to show GUI elements after content.
        /// </summary>
        /// <returns></returns>
        protected override void ShowFooterGUI() {
            EditorGUILayout.LabelField("Move Specific", EditorStyles.boldLabel);
            base.ShowFooterGUI();
            EditorGUILayout.PropertyField(_transitionModeProperty, new GUIContent("Mode", _transitionModeProperty.tooltip)); //TODO move to header?
            EditorGUILayout.PropertyField(_coordinateModeProperty);
            if (_transitionModeProperty.enumValueIndex != 1)
            {
                EditorGUILayout.PropertyField(_startPositionTypeProperty);
                EditorGUILayout.PropertyField(_startPositionProperty);
            }
            if (_transitionModeProperty.enumValueIndex != 2)
            {
                EditorGUILayout.PropertyField(_endPositionTypeProperty);
                EditorGUILayout.PropertyField(_endPositionProperty);
            }
        }

    }
}

#endif