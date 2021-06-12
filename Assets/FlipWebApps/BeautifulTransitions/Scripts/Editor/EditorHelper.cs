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

using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Editor
{
    /// <summary>
    /// Helper functions for dealing with editor windows, inspectors etc...
    /// </summary>
    public class EditorHelper
    {
        #region Hideable Help Box
        /// <summary>
        /// Shows an editor help box that is hideable through a small button to the top right. 
        /// </summary>
        /// Pass back in teh returned rect to subsequent calls for the same helpbox
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <param name="lastRect"></param>
        /// <returns></returns>
        public static Rect ShowHideableHelpBox(string key, string text, Rect lastRect)
        {
            var hidden = EditorPrefs.GetBool(key, false);
            if (!hidden)
            {
                EditorGUILayout.HelpBox(text,
                    MessageType.Info);
                if (Event.current.type == EventType.Repaint)
                {
                    lastRect = GUILayoutUtility.GetLastRect();
                }
                var newRect = new Rect(lastRect.xMax - 15, lastRect.yMin, 15, 15);
                if (GUI.Button(newRect, "x")) EditorPrefs.SetBool(key, true);
            }
            return lastRect;
        }
        #endregion Hideable Help Box

        #region GUIStyle

        /// <summary>
        /// Get a bold style for a label
        /// </summary>
        /// <returns></returns>
        public static GUIStyle BoldLabelStyle
        {
            get
            {
                if (_boldLabelStyle != null) return _boldLabelStyle;

                _boldLabelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft
                };
                return _boldLabelStyle;
            }
        }
        static GUIStyle _boldLabelStyle;

        #endregion GUIStyle

    }
}