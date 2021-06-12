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

using UnityEngine;
using UnityEditor;
using BeautifulTransitions.Scripts.Transitions.Editor.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.Components.Camera;

namespace BeautifulTransitions.Scripts.Transitions.Editor.Components.Camera
{
    [CustomEditor(typeof(WipeCamera))]
    public class WipeCameraEditor : TransitionBaseEditor
    {
        SerializedProperty _skipIdleRenderingProperty;
        SerializedProperty _inTextureProperty;
        SerializedProperty _inColorProperty;
        SerializedProperty _inMaskTextureProperty;
        SerializedProperty _inInvertMaskProperty;
        SerializedProperty _inSoftnessProperty;
        SerializedProperty _outTextureProperty;
        SerializedProperty _outColorProperty;
        SerializedProperty _outMaskTextureProperty;
        SerializedProperty _outInvertMaskProperty;
        SerializedProperty _outSoftnessProperty;

        protected override void OnEnable()
        {
            _skipIdleRenderingProperty = serializedObject.FindProperty("SkipIdleRendering");
            var inConfigProperty = serializedObject.FindProperty("InConfig");
            _inTextureProperty = inConfigProperty.FindPropertyRelative("Texture");
            _inColorProperty = inConfigProperty.FindPropertyRelative("Color");
            _inMaskTextureProperty = inConfigProperty.FindPropertyRelative("MaskTexture");
            _inInvertMaskProperty = inConfigProperty.FindPropertyRelative("InvertMask");
            _inSoftnessProperty = inConfigProperty.FindPropertyRelative("Softness");
            var outConfigProperty = serializedObject.FindProperty("OutConfig");
            _outTextureProperty = outConfigProperty.FindPropertyRelative("Texture");
            _outColorProperty = outConfigProperty.FindPropertyRelative("Color");
            _outMaskTextureProperty = outConfigProperty.FindPropertyRelative("MaskTexture");
            _outInvertMaskProperty = outConfigProperty.FindPropertyRelative("InvertMask");
            _outSoftnessProperty = outConfigProperty.FindPropertyRelative("Softness");
            base.OnEnable();
        }

        /// <summary>
        /// Override in subclasses to show common GUI elements.
        /// </summary>
        /// <returns></returns>
        protected override void ShowCommonGUI()
        {
            base.ShowCommonGUI();
            EditorGUILayout.PropertyField(_skipIdleRenderingProperty);
        }

        /// <summary>
        /// Override in subclasses to show Transition In GUI elements.
        /// </summary>
        /// <returns></returns>
        protected override void ShowTransitionInGUI() {
            GUILayout.Space(3f);
            EditorGUILayout.LabelField("Wipe Camera Specific", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_inTextureProperty);
            EditorGUILayout.PropertyField(_inColorProperty);
            EditorGUILayout.PropertyField(_inMaskTextureProperty);
            EditorGUILayout.PropertyField(_inInvertMaskProperty);
            EditorGUILayout.PropertyField(_inSoftnessProperty);
        }

        /// <summary>
        /// Override in subclasses to show Transition Out GUI elements.
        /// </summary>
        /// <returns></returns>
        protected override void ShowTransitionOutGUI() {
            GUILayout.Space(3f);
            EditorGUILayout.LabelField("Wipe Camera Specific", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_outTextureProperty);
            EditorGUILayout.PropertyField(_outColorProperty);
            EditorGUILayout.PropertyField(_outMaskTextureProperty);
            EditorGUILayout.PropertyField(_outInvertMaskProperty);
            EditorGUILayout.PropertyField(_outSoftnessProperty);
        }
    }
}
