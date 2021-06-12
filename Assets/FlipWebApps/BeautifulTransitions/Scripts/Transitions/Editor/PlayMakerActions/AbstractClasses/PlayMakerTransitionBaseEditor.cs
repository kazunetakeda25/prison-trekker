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
using BeautifulTransitions.Scripts.Transitions.Editor.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.PlayMakerActions;
using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.Editor.PlayMakerActions.AbstractClasses
{
    public abstract class PlayMakerTransitionBaseEditor : CustomActionEditor
    {
        bool _showAdvancedSettings;
        bool _showEvents;

        /// <summary>
        /// Get a reference to properties
        /// </summary>
        public override void OnEnable()
        {
            //_durationProperty = serializedObject.FindProperty("Duration");
            //_timeUpdateMethodProperty = serializedObject.FindProperty("TimeUpdateMethod");
            //_transitionTypeProperty = serializedObject.FindProperty("TransitionType");
            //_animationCurveProperty = serializedObject.FindProperty("AnimationCurve");
            //_onTransitionStartProperty = serializedObject.FindProperty("OnTransitionStart");
            //_onTransitionUpdateProperty = serializedObject.FindProperty("OnTransitionUpdate");
            //_onTransitionCompleteProperty = serializedObject.FindProperty("OnTransitionComplete");
        }

        /// <summary>
        /// Draw the Editor GUI
        /// </summary>
        public override bool OnGUI()
        {
            var playMakerTransitionBase = target as PlayMakerTransitionBase;

            ShowHeaderGUI();

            //EditorHelper.DrawDefaultInspector(serializedObject, new List<string>() { "m_Script", "_animateChanges" });
            EditField("Delay");
            EditField("Duration");
            EditField("TransitionType");

            if ((TransitionHelper.TweenType)playMakerTransitionBase.TransitionType.Value == TransitionHelper.TweenType.none)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.HelpBox("This transition will be ignored!", MessageType.Info);
                EditorGUI.indentLevel -= 1;
            }

            else if ((TransitionHelper.TweenType)playMakerTransitionBase.TransitionType.Value == TransitionHelper.TweenType.AnimationCurve)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.HelpBox("Custom animation curve with absolute values.\nClick the curve below to edit.", MessageType.None);
                EditField("AnimationCurve");
                //EditorGUILayout.PropertyField(animationCurveProperty, GUIContent.none, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 4));
                EditorGUI.indentLevel -= 1;
            }

            else
            {
                EditorGUI.indentLevel += 1;

                var easingFunction = TransitionHelper.GetTweenFunction((TransitionHelper.TweenType)playMakerTransitionBase.TransitionType.Value);
                if (easingFunction == null)
                {
                    // should never happen, but worth checking
                    EditorGUILayout.HelpBox("Curve not found! Please report this error.", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("Fixed Transition Curve.", MessageType.None);

                    TransitionBaseEditor.DrawTweenPreview(easingFunction);
                    EditorGUI.indentLevel -= 1;
                }
            }

            GUILayout.Space(5);
            _showAdvancedSettings = EditorGUILayout.Foldout(_showAdvancedSettings, new GUIContent("Advanced"));
            if (_showAdvancedSettings)
            {
                EditorGUI.indentLevel += 1;
                EditField("TimeUpdateMethod");
                EditorGUI.indentLevel -= 1;
            }

            _showEvents = EditorGUILayout.Foldout(_showEvents, new GUIContent("Events"));
            if (_showEvents)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                EditorGUILayout.BeginVertical();
                // Below doesn't seem to work in PlayMaker Action Editor
                //EditField("OnTransitionStart");
                //EditField("OnTransitionUpdate");
                //EditField("OnTransitionComplete");
                EditField("TransitionStartEvent");
                EditField("TransitionCompleteEvent");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(3f);
            ShowFooterGUI();
            return GUI.changed;
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