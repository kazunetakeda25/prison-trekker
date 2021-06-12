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

using UnityEditor;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.Editor.GameActions.AbstractClasses
{
    public abstract class GameActionTransitionScreenEditor : GameActionTransitionBaseEditor
    {
        SerializedProperty _transitionDirectionProperty;
        SerializedProperty _sceneChangeModeProperty;
        SerializedProperty _sceneToLoadProperty;

        /// <summary>
        /// Get a reference to properties
        /// </summary>
        protected override void Initialise()
        {
            base.Initialise();
            _transitionDirectionProperty = serializedObject.FindProperty("TransitionDirection");
            _sceneChangeModeProperty = serializedObject.FindProperty("SceneChangeMode");
            _sceneToLoadProperty = serializedObject.FindProperty("SceneToLoad");
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
            EditorGUILayout.PropertyField(_transitionDirectionProperty, new GUIContent("Direction", _transitionDirectionProperty.tooltip));
            EditorGUILayout.PropertyField(_sceneChangeModeProperty);
            if (_sceneChangeModeProperty.enumValueIndex != 0)
                EditorGUILayout.PropertyField(_sceneToLoadProperty);
        }

    }
}

#endif