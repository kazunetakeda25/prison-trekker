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

using BeautifulTransitions.Scripts.Transitions.Editor.AbstractClasses;
using GameFramework.GameStructure.Game.Editor.GameActions;
using UnityEditor;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.Editor.GameActions.AbstractClasses
{
    public abstract class GameActionTransitionBaseEditor : GameActionEditor
    {
        SerializedProperty _durationProperty;
        SerializedProperty _timeUpdateMethodProperty;
        SerializedProperty _transitionTypeProperty;
        SerializedProperty _animationCurveProperty;
        SerializedProperty _onTransitionStartProperty;
        SerializedProperty _onTransitionUpdateProperty;
        SerializedProperty _onTransitionCompleteProperty;
        // COMMENTED OUT DUE TO SERIALISATION ISSUES (CAN'T SET IN EDITOR)!!
        //SerializedProperty _onTransitionStartTransitionStepParameterProperty;
        //SerializedProperty _onTransitionUpdateTransitionStepParameterProperty;
        //SerializedProperty _onTransitionCompleteTransitionStepParameterProperty;

        /// <summary>
        /// Get a reference to properties
        /// </summary>
        protected override void Initialise()
        {
            _durationProperty = serializedObject.FindProperty("Duration");
            _timeUpdateMethodProperty = serializedObject.FindProperty("TimeUpdateMethod");
            _transitionTypeProperty = serializedObject.FindProperty("TransitionType");
            _animationCurveProperty = serializedObject.FindProperty("AnimationCurve");
            //_onTransitionStartProperty = serializedObject.FindProperty("OnTransitionStart");
            //_onTransitionUpdateProperty = serializedObject.FindProperty("OnTransitionUpdate");
            //_onTransitionCompleteProperty = serializedObject.FindProperty("OnTransitionComplete");
        }

        /// <summary>
        /// Draw the Editor GUI
        /// </summary>
        protected override void DrawGUI()
        {
            ShowHeaderGUI();

            //EditorHelper.DrawDefaultInspector(serializedObject, new List<string>() { "m_Script", "_animateChanges" });
            EditorGUILayout.PropertyField(DelayProperty);
            EditorGUILayout.PropertyField(_durationProperty);

            TransitionBaseEditor.ShowTransitionType(_transitionTypeProperty, _animationCurveProperty);

            EditorGUI.indentLevel++;

            GUILayout.Space(5);
            _timeUpdateMethodProperty.isExpanded = EditorGUILayout.Foldout(_timeUpdateMethodProperty.isExpanded, new GUIContent("Advanced"));
            if (_timeUpdateMethodProperty.isExpanded)
            {
                EditorGUILayout.PropertyField(_timeUpdateMethodProperty);
            }

            //_onTransitionStartProperty.isExpanded = EditorGUILayout.Foldout(_onTransitionStartProperty.isExpanded, new GUIContent("Events"));
            //if (_onTransitionStartProperty.isExpanded)
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    GUILayout.Space(15f);
            //    EditorGUILayout.BeginVertical();
            //    EditorGUILayout.PropertyField(_onTransitionStartProperty);
            //    EditorGUILayout.PropertyField(_onTransitionUpdateProperty);
            //    EditorGUILayout.PropertyField(_onTransitionCompleteProperty);
            //    EditorGUILayout.EndVertical();
            //    EditorGUILayout.EndHorizontal();
            //}

            EditorGUI.indentLevel--;

            GUILayout.Space(3f);
            ShowFooterGUI();
        }


        /// <summary>
        /// Override in subclasses to show GUI elements before content.
        /// </summary>
        /// <returns></returns>
        protected virtual void ShowHeaderGUI() { }


        /// <summary>
        /// Override in subclasses to show GUI elements after content.
        /// </summary>
        /// <returns></returns>
        protected virtual void ShowFooterGUI() { }

    }
}

#endif