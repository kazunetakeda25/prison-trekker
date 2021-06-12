using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Visual.UI;

namespace Com.TheFallenGames.OSA.AdditionalComponents
{
	/// <summary>
	/// Very useful script when you want to attach arbitrary content anywhere in an OSA and have it scrollable as any other item.
	/// Needs to be attached to a child of OSA's Viewport
	/// </summary>
	public class OSAContentDecorator : MonoBehaviour
	{
		[SerializeField]
		InsetEdgeEnum _InsetEdge = InsetEdgeEnum.START;

		[SerializeField]
		[Tooltip("How far from the InsetEdge should this be positioned. Can be normalized ([0, 1]), if InsetIsNormalized=true, or raw. Default is 0")]
		float _Inset = 0f;

		[SerializeField]
		[Tooltip("If false, will interpret the Inset proeprty as the raw distance from InsetEdge, rather than the normalized inset relative to the content's full size")]
		bool _InsetIsNormalized = true;

		[SerializeField]
		bool _DisableWhenNotVisible = true;

		[SerializeField]
		[Tooltip("If false, won't be dragged together with the OSA's content when it's pulled when already at the scrolling limit")]
		bool _AffectedByElasticity = false;

		[SerializeField]
		[Tooltip(
			"Sets when the OSA's padding from InsetEdge will be controlled to be the same as this object's size.\n" +
			"Once at initialization, or adapting continuously, or none (i.e. you'll probably set OSA's padding manually, in case the decorator shouldn't overlap with items).\n" +
			"Only works if Inset is 0")]
		ControlOSAPaddingMode _ControlOSAPaddingAtInsetEdge = ControlOSAPaddingMode.ONCE_AT_INIT;

		[SerializeField]
		[Tooltip("If null, will use the first an implementation of IOSA found in parents")]
		RectTransform _OSATransform;

		RectTransform _ParentRT;
		RectTransform _RT;
		IOSA _OSA;
		bool _Initialized;
		double _MyLastKnownSize;


		/// <summary>
		/// Only to be called if OSA is initialized manually via <see cref="OSA{TParams, TItemViewsHolder}.Init"/>. Call it before that.
		/// With the default setup, where OSA initializes itself in its Start(), you don't need to call this, as it's called from this.Awake()
		/// </summary>
		public void Init()
		{
			_RT = transform as RectTransform;
			_ParentRT = _RT.parent as RectTransform;
			if (_OSA == null)
			{
				_OSA = _ParentRT.GetComponentInParent(typeof(IOSA)) as IOSA;
				if (_OSA == null)
					throw new OSAException("Component implementing IOSA not found in parents");
			}
			else
			{
				_OSA = _OSATransform.GetComponent(typeof(IOSA)) as IOSA;
				if (_OSA == null)
					throw new OSAException("Component implementing IOSA not found on the specified object '" + _OSATransform.name + "'");
			}

			if (_OSA.BaseParameters.Viewport != _ParentRT)
				throw new OSAException(typeof(OSAContentDecorator).Name + " can only work when attached to a direct child of OSA's Viewport.");

			if (_ControlOSAPaddingAtInsetEdge != ControlOSAPaddingMode.DONT_CONTROL)
			{
				if (_OSA.IsInitialized)
				{
					Debug.Log(
						"OSA's content padding can't be set after OSA was initialized. " +
						"You're most probably calling OSA.Init manually(), in which case make sure to also manually call Init() on this decorator, before OSA.Init()"
					);
				}
				else
				{
					if (_Inset != 0f)
					{
						Debug.Log(
							"OSA's content padding can't be controlled if Inset ("+ _Inset + ") is not 0 . "
						);
					}
					SetOSAPadding();
				}
			}

			_OSA.ScrollPositionChanged += OSAScrollPositionChanged;

			var aPos = _RT.localPosition;
			_RT.anchorMin = _RT.anchorMax = new Vector2(0f, 1f); // top-right
			_RT.localPosition = aPos;

			_Initialized = true;
		}


		void Awake()
		{
			if (!_Initialized)
				Init();

			gameObject.SetActive(false);
		}

		void Update()
		{
			if (_ControlOSAPaddingAtInsetEdge == ControlOSAPaddingMode.ADAPTIVE)
			{
				if (_OSA == null || !_OSA.IsInitialized) // make sure adapter wasn't disposed
					return;

				var rect = _RT.rect;
				var li = _OSA.GetLayoutInfoReadonly();
				double mySize = rect.size[li.hor0_vert1];

				if (_MyLastKnownSize != mySize)
				{
					SetPadding(mySize);
					_OSA.ScheduleForceRebuildLayout();
				}
			}	
		}

		void SetOSAPadding()
		{
			var rect = _RT.rect;
			// Commented: layout info may not be available
			//var li = _OSA.GetLayoutInfoReadonly();

			double mySize = rect.size[_OSA.IsHorizontal ? 0 : 1];
			SetPadding(mySize);
		}

		void SetPadding(double myNewSize)
		{
			var p = _OSA.BaseParameters;
			int mySizeRoundedByCeiling = (int)(myNewSize + .6f);
			if (_InsetEdge == InsetEdgeEnum.START)
			{
				if (p.IsHorizontal)
					p.ContentPadding.left = mySizeRoundedByCeiling;
				else
					p.ContentPadding.top = mySizeRoundedByCeiling;
			}
			else
			{
				if (p.IsHorizontal)
					p.ContentPadding.right = mySizeRoundedByCeiling;
				else
					p.ContentPadding.bottom = mySizeRoundedByCeiling;
			}
			_MyLastKnownSize = myNewSize;
		}

		void OSAScrollPositionChanged(double scrollPos)
		{
			// The terms 'before' and 'after' mean what they should, if _InsetEdge is START,
			// but their meaning is swapped when _InsetEdge is END.

			var li = _OSA.GetLayoutInfoReadonly();
			double osaInsetFromEdge;
			RectTransform.Edge edgeToInsetFrom;
			if (_InsetEdge == InsetEdgeEnum.START)
			{
				osaInsetFromEdge = _OSA.ContentVirtualInsetFromViewportStart;
				edgeToInsetFrom = li.startEdge;
			}
			else
			{
				osaInsetFromEdge = _OSA.ContentVirtualInsetFromViewportEnd;
				edgeToInsetFrom = li.endEdge;
			}

			double myExpectedInsetFromVirtualContent = _Inset;
			var rect = _RT.rect;
			double mySize = rect.size[li.hor0_vert1];
			double osaViewportSize = li.vpSize;
			if (_InsetIsNormalized)
			{
				myExpectedInsetFromVirtualContent *= _OSA.GetContentSize() - mySize;
			}

			double myExpectedInsetFromViewport = osaInsetFromEdge + myExpectedInsetFromVirtualContent;
			bool visible = true;
			if (myExpectedInsetFromViewport < 0d)
			{
				if (myExpectedInsetFromViewport <= -mySize) // completely 'before' the viewport
				{
					myExpectedInsetFromViewport = -mySize; // don't position it too far away
					visible = false;
				}
			}
			else
			{
				if (myExpectedInsetFromViewport >= osaViewportSize) // completely 'after' the viewport
				{
					myExpectedInsetFromViewport = osaViewportSize; // don't position it too far away
					visible = false;
				}
			}

			bool disable = false;
			if (!visible)
				disable = _DisableWhenNotVisible;

			if (gameObject.activeSelf == disable)
				gameObject.SetActive(!disable);

			if (disable)
				// No need to position it, since it's disabled now
				return;

			if (!_AffectedByElasticity)
			{
				if (myExpectedInsetFromViewport > 0d)
				{
					// Update: actually, it looks better to just keep it at the edge, no matter what
					// // only if the content is bigger than viewport, otherwise the decorator is forced to stay with the content
					//if (_OSA.GetContentSizeToViewportRatio() > 1d) 
					//{

					//}
					myExpectedInsetFromViewport = 0d;
				}
			}

			_RT.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(edgeToInsetFrom, (float)myExpectedInsetFromViewport, (float)mySize);
		}


		public enum InsetEdgeEnum
		{
			START,
			END
		}


		public enum ControlOSAPaddingMode
		{
			DONT_CONTROL,
			ONCE_AT_INIT,
			ADAPTIVE
		}
	}
}
