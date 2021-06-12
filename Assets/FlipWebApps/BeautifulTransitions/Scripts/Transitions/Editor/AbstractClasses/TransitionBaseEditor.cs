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

using BeautifulTransitions.Scripts.Transitions.Components;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.Editor.AbstractClasses
{
    public abstract class TransitionBaseEditor : UnityEditor.Editor
    {
        SerializedProperty _initForTransitionInProperty;
        SerializedProperty _autoRunProperty;
        SerializedProperty _repeatWhenEnabledProperty;
        SerializedProperty _transitionInConfigProperty;
        SerializedProperty _transitionOutConfigProperty;

#if GAME_FRAMEWORK
        List<GameFramework.Helper.ClassDetailsAttribute> _gameActionClassDetails;
        GameFramework.GameStructure.Game.Editor.GameActions.GameActionEditor[] inStartActionEditors;
        GameFramework.GameStructure.Game.Editor.GameActions.GameActionEditor[] inCompleteActionEditors;
        GameFramework.GameStructure.Game.Editor.GameActions.GameActionEditor[] outStartActionEditors;
        GameFramework.GameStructure.Game.Editor.GameActions.GameActionEditor[] outCompleteActionEditors;
#endif

        TransitionBase _transitionBase;

        protected virtual void OnEnable()
        {
            _transitionBase = (TransitionBase)target;

            _initForTransitionInProperty = serializedObject.FindProperty("InitForTransitionIn");
            _autoRunProperty = serializedObject.FindProperty("AutoRun");
            _repeatWhenEnabledProperty = serializedObject.FindProperty("RepeatWhenEnabled");
            _transitionInConfigProperty = serializedObject.FindProperty("TransitionInConfig");
            _transitionOutConfigProperty = serializedObject.FindProperty("TransitionOutConfig");

#if GAME_FRAMEWORK
            // setup actions types
            _gameActionClassDetails = GameFramework.GameStructure.Game.GameActionEditorHelper.FindTypesClassDetails();
#endif

        }


        protected void OnDisable()
        {
#if GAME_FRAMEWORK
            GameFramework.EditorExtras.Editor.EditorHelper.CleanupSubEditors(inStartActionEditors);
            GameFramework.EditorExtras.Editor.EditorHelper.CleanupSubEditors(inCompleteActionEditors);
            GameFramework.EditorExtras.Editor.EditorHelper.CleanupSubEditors(outStartActionEditors);
            GameFramework.EditorExtras.Editor.EditorHelper.CleanupSubEditors(outCompleteActionEditors);
            inStartActionEditors = null;
            inCompleteActionEditors = null;
            outStartActionEditors = null;
            outCompleteActionEditors = null;
#endif
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ShowHeaderGUI();

            EditorGUILayout.PropertyField(_initForTransitionInProperty);
            EditorGUILayout.PropertyField(_autoRunProperty);
            EditorGUILayout.PropertyField(_repeatWhenEnabledProperty);

            ShowCommonGUI();

            ShowTransitionConfiguration(_transitionInConfigProperty, "Transition In Settings", ShowTransitionInGUI, _transitionBase.TransitionInConfig, true);
            ShowTransitionConfiguration(_transitionOutConfigProperty, "Transition Out Settings", ShowTransitionOutGUI, _transitionBase.TransitionOutConfig, false);

            ShowFooterGUI();

            serializedObject.ApplyModifiedProperties();

        }

        private void ShowTransitionConfiguration(SerializedProperty property, string label, System.Action showCustomTransitionGUI, 
            TransitionBase.TransitionSettings transitionSettings, bool isInTransition)
        {
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, new GUIContent(label, property.tooltip));
            if (property.isExpanded)
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("Delay"));
                EditorGUILayout.PropertyField(property.FindPropertyRelative("Duration"));

                var transitionTypeProperty = property.FindPropertyRelative("TransitionType");
                var animationCurveProperty = property.FindPropertyRelative("AnimationCurve");
                ShowTransitionType(transitionTypeProperty, animationCurveProperty);

                EditorGUILayout.PropertyField(property.FindPropertyRelative("LoopMode"));

                EditorGUI.indentLevel++;

                GUILayout.Space(5);
                var transitionChildrenProperty = property.FindPropertyRelative("TransitionChildren");
                transitionChildrenProperty.isExpanded = EditorGUILayout.Foldout(transitionChildrenProperty.isExpanded, new GUIContent("Advanced"));
                if (transitionChildrenProperty.isExpanded)
                {
                    EditorGUILayout.PropertyField(transitionChildrenProperty);
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("MustTriggerDirect"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("TimeUpdateMethod"));
                }

                var onTransitionStartProperty = property.FindPropertyRelative("OnTransitionStart");
                onTransitionStartProperty.isExpanded = EditorGUILayout.Foldout(onTransitionStartProperty.isExpanded, new GUIContent("Events"));
                if (onTransitionStartProperty.isExpanded)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(onTransitionStartProperty);
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("OnTransitionUpdate"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("OnTransitionComplete"));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }

#if GAME_FRAMEWORK
                // Start Actions
                var onTransitionStartActionReferencesProperty = property.FindPropertyRelative("OnTransitionStartActionReferences");
                onTransitionStartActionReferencesProperty.isExpanded = EditorGUILayout.Foldout(onTransitionStartActionReferencesProperty.isExpanded, new GUIContent("Start Actions (" + transitionSettings.OnTransitionStartActionReferences.Length + ")"));
                if (onTransitionStartActionReferencesProperty.isExpanded)
                {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    EditorGUILayout.BeginVertical();
                    if (isInTransition)
                        GameFramework.GameStructure.Game.GameActionEditorHelper.DrawActions(serializedObject, onTransitionStartActionReferencesProperty, transitionSettings.OnTransitionStartActionReferences,
                            ref inStartActionEditors, _gameActionClassDetails, null, tooltip: onTransitionStartActionReferencesProperty.tooltip);
                    else
                        GameFramework.GameStructure.Game.GameActionEditorHelper.DrawActions(serializedObject, onTransitionStartActionReferencesProperty, transitionSettings.OnTransitionStartActionReferences,
                            ref outStartActionEditors, _gameActionClassDetails, null, tooltip: onTransitionStartActionReferencesProperty.tooltip);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                }

                // Complete Actions
                var onTransitionCompleteActionReferencesProperty = property.FindPropertyRelative("OnTransitionCompleteActionReferences");
                onTransitionCompleteActionReferencesProperty.isExpanded = EditorGUILayout.Foldout(onTransitionCompleteActionReferencesProperty.isExpanded, new GUIContent("Complete Actions (" + transitionSettings.OnTransitionCompleteActionReferences.Length + ")"));
                if (onTransitionCompleteActionReferencesProperty.isExpanded)
                {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(15f);
                    EditorGUILayout.BeginVertical();
                    if (isInTransition)
                        GameFramework.GameStructure.Game.GameActionEditorHelper.DrawActions(serializedObject, onTransitionCompleteActionReferencesProperty, transitionSettings.OnTransitionCompleteActionReferences,
                            ref inCompleteActionEditors, _gameActionClassDetails, null, tooltip: onTransitionCompleteActionReferencesProperty.tooltip);
                    else
                        GameFramework.GameStructure.Game.GameActionEditorHelper.DrawActions(serializedObject, onTransitionCompleteActionReferencesProperty, transitionSettings.OnTransitionCompleteActionReferences,
                            ref outCompleteActionEditors, _gameActionClassDetails, null, tooltip: onTransitionCompleteActionReferencesProperty.tooltip);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                }
#else
                EditorGUILayout.HelpBox("Add the FREE Game Framework asset from the asset store and start using Actions. With actions you can play audio, start other transitions and much more when this transition starts or completes.", MessageType.Info);
#endif
                EditorGUI.indentLevel--;
                showCustomTransitionGUI();
            }
        }

        internal static void ShowTransitionType(SerializedProperty transitionTypeProperty, SerializedProperty animationCurveProperty)
        {
            var transitionType = (TransitionHelper.TweenType)System.Enum.GetValues(typeof(TransitionHelper.TweenType)).GetValue(transitionTypeProperty.enumValueIndex);

            EditorGUILayout.PropertyField(transitionTypeProperty);

            if (transitionType == TransitionHelper.TweenType.none)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.HelpBox("This transition will be ignored!", MessageType.Info);
                EditorGUI.indentLevel -= 1;
            }

            else if (transitionType == TransitionHelper.TweenType.AnimationCurve)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.HelpBox("Custom animation curve with absolute values.\nClick the curve below to edit.", MessageType.None);
                EditorGUILayout.PropertyField(animationCurveProperty, GUIContent.none, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 4));
                EditorGUI.indentLevel -= 1;
            }

            else
            {
                EditorGUI.indentLevel += 1;

                var easingFunction = TransitionHelper.GetTweenFunction(transitionType);
                if (easingFunction == null)
                {
                    // should never happen, but worth checking
                    EditorGUILayout.HelpBox("Curve not found! Please report this error.", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("Fixed Transition Curve.", MessageType.None);

                    DrawTweenPreview(easingFunction);
                    EditorGUI.indentLevel -= 1;
                }
            }
        }

        public static void DrawTweenPreview(Helper.TweenMethods.TweenFunction easingFunction)
        {
            var texPreviewRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 5);
            texPreviewRect = EditorGUI.IndentedRect(texPreviewRect);
            if (texPreviewRect.width > 0 && texPreviewRect.height > 0)
            {
                // set default texture color
                var texture = new Texture2D((int)texPreviewRect.width, (int)texPreviewRect.height, TextureFormat.ARGB32,
                    false);
                var colors = new Color[texture.width * texture.height];
                for (var i = 0; i < colors.Length; i++)
                    colors[i] = Color.black;

                // First calculate min / max y as function might send values below 0 or above 1
                var normalisedWidth = 1f / texture.width;
                var minValue = float.MaxValue;
                var maxValue = float.MinValue;
                for (var i = 0; i < texture.width; i++)
                {
                    var normalisedX = normalisedWidth * i;
                    var y = easingFunction(0, 1, normalisedX);
                    if (y < minValue) minValue = y;
                    if (y > maxValue) maxValue = y;
                }

                // plot values for all columns. graph, 0 and 1
                var zeroRow = GetGraphRow(minValue, maxValue, 0, texture.height);
                var oneRow = GetGraphRow(minValue, maxValue, 1, texture.height);
                for (var i = 0; i < texture.width; i++)
                {
                    // lines at 0 and 1
                    PlotGraphPoint(i, zeroRow, texture.width, texture.height, colors, Color.gray);
                    PlotGraphPoint(i, oneRow, texture.width, texture.height, colors, Color.gray);

                    // graph value
                    var normalisedX = normalisedWidth * i;
                    var value = easingFunction(0, 1, normalisedX);
                    PlotGraphPosition(i, texture.width, minValue, maxValue, value, texture.height, colors, Color.green);
                }
                // Set and apply pixels
                texture.SetPixels(colors);
                texture.Apply();

                // workaround given DrawPreviewTexture doesn't seem to work properly (disappears after a short while)!
                GUIStyle style = new GUIStyle();
                style.normal.background = texture;
                EditorGUI.LabelField(texPreviewRect, GUIContent.none, style);

                EditorGUI.DrawPreviewTexture(texPreviewRect, texture);
            }
        }

        /// <summary>
        /// Override in subclasses to show GUI elements before content.
        /// </summary>
        /// <returns></returns>
        protected virtual void ShowHeaderGUI() { }

        /// <summary>
        /// Override in subclasses to show common GUI elements.
        /// </summary>
        /// <returns></returns>
        protected virtual void ShowCommonGUI() { }

        /// <summary>
        /// Override in subclasses to show Transition In GUI elements.
        /// </summary>
        /// <returns></returns>
        protected virtual void ShowTransitionInGUI() { }

        /// <summary>
        /// Override in subclasses to show Transition Out GUI elements.
        /// </summary>
        /// <returns></returns>
        protected virtual void ShowTransitionOutGUI() { }

        /// <summary>
        /// Override in subclasses to show GUI elements after content.
        /// </summary>
        /// <returns></returns>
        protected virtual void ShowFooterGUI() { }

        #region plotting helper functions
        static void PlotGraphPosition(int column, float graphWidth, float minValue, float maxValue, float value, float graphHeight, System.Collections.Generic.IList<Color> colors, Color color)
        {
            var row = GetGraphRow(minValue, maxValue, value, graphHeight);
            PlotGraphPoint(column, row, graphWidth, graphHeight, colors, color);
        }

        private static void PlotGraphPoint(int column, int row, float graphWidth, float graphHeight, System.Collections.Generic.IList<Color> colors, Color color)
        {
            if (row >= 0 && row < graphHeight)
                colors[column + row * (int)graphWidth] = color;
        }

        static int GetGraphRow(float minValue, float maxValue, float value, float graphHeight)
        {
            var graphYDistance = maxValue - minValue;
            var zeroOffsetRow = value - minValue;
            var row = (int)(graphHeight / graphYDistance * zeroOffsetRow);
            return row;
        }
        #endregion plotting helper functions
    }
}
