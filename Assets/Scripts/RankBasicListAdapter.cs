/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "Assets/OSA/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;

// You should modify the namespace to your own or - if you're sure there won't ever be conflicts - remove it altogether
namespace moon
{
	// There are 2 important callbacks you need to implement, apart from Start(): CreateViewsHolder() and UpdateViewsHolder()
	// See explanations below
	public class RankBasicListAdapter : OSA<BaseParamsWithPrefab, MyListItemViewsHolder>
	{
		// Helper that stores data and notifies the adapter when items count changes
		// Can be iterated and can also have its elements accessed by the [] operator
		public SimpleDataHelper<MyListItemModel> Data { get; private set; }


		#region OSA implementation
		protected override void Start()
		{
			Data = new SimpleDataHelper<MyListItemModel>(this);

			// Calling this initializes internal data and prepares the adapter to handle item count changes
			base.Start();

            // Retrieve the models from your data source and set the items count
            /*
			RetrieveDataAndUpdate(500);
			*/

            //RetrieveDataAndUpdate(200);
        }

		// This is called initially, as many times as needed to fill the viewport, 
		// and anytime the viewport's size grows, thus allowing more items to be displayed
		// Here you create the "ViewsHolder" instance whose views will be re-used
		// *For the method's full description check the base implementation
		protected override MyListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyListItemViewsHolder();

			// Using this shortcut spares you from:
			// - instantiating the prefab yourself
			// - enabling the instance game object
			// - setting its index 
			// - calling its CollectViews()
			instance.Init(_Params.ItemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		// This is called anytime a previously invisible item become visible, or after it's created, 
		// or when anything that requires a refresh happens
		// Here you bind the data from the model to the item's views
		// *For the method's full description check the base implementation
		protected override void UpdateViewsHolder(MyListItemViewsHolder newOrRecycled)
		{
            // In this callback, "newOrRecycled.ItemIndex" is guaranteed to always reflect the
            // index of item that should be represented by this views holder. You'll use this index
            // to retrieve the model from your data set
            /*
			MyListItemModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.backgroundImage.color = model.color;
			newOrRecycled.titleText.text = model.title + " #" + newOrRecycled.ItemIndex;
			*/

            MyListItemModel model = Data[newOrRecycled.ItemIndex];

            newOrRecycled.IDText.text = ""+model.index;
            newOrRecycled.NameText.text = model.name;
            newOrRecycled.LevelText.text = ""+model.level;
            
            if (model.id.Equals(Config.userIdentify))
            {
                Debug.Log(model.id);
                newOrRecycled.NameText.color = new Color(0f, 1f, 238f / 255f, 1f);
            }
            else {
                newOrRecycled.NameText.color = new Color(1f, 213f/255f, 146f / 255f, 1f);
            }
        }

		// This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
		// especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
		// download request, if it's still in progress when the item goes out of the viewport.
		// <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
		// *For the method's full description check the base implementation
		/*
		protected override void OnBeforeRecycleOrDisableViewsHolder(MyListItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}
		*/

		// You only need to care about this if changing the item count by other means than ResetItems, 
		// case in which the existing items will not be re-created, but only their indices will change.
		// Even if you do this, you may still not need it if your item's views don't depend on the physical position 
		// in the content, but they depend exclusively to the data inside the model (this is the most common scenario).
		// In this particular case, we want the item's index to be displayed and also to not be stored inside the model,
		// so we update its title when its index changes. At this point, the Data list is already updated and 
		// shiftedViewsHolder.ItemIndex was correctly shifted so you can use it to retrieve the associated model
		// Also check the base implementation for complementary info
		/*
		protected override void OnItemIndexChangedDueInsertOrRemove(MyListItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.titleText.text = Data[shiftedViewsHolder.ItemIndex].title + " #" + shiftedViewsHolder.ItemIndex;
		}
		*/
		#endregion

		// These are common data manipulation methods
		// The list containing the models is managed by you. The adapter only manages the items' sizes and the count
		// The adapter needs to be notified of any change that occurs in the data list. Methods for each
		// case are provided: Refresh, ResetItems, InsertItems, RemoveItems
		#region data manipulation
		public void AddItemsAt(int index, IList<MyListItemModel> items)
		{
			// Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.InsertRange(index, items);
			//InsertItems(index, items.Length);

			Data.InsertItems(index, items);
		}


        public void AddItemsAt_ListUserRanks(int index, IList<InfoUserFirebase> listUserRanks)
        {
            // Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
            //YourList.InsertRange(index, items);
            //InsertItems(index, items.Length);
            List<MyListItemModel> myListItemModels = new List<MyListItemModel>();
            for (int i = 0; i < listUserRanks.Count; i++) {
                var model = new MyListItemModel()
                {
                    index = i+1,
                    id = listUserRanks[i].userID,
                    name = listUserRanks[i].name,
                    level = listUserRanks[i].level
                };
                myListItemModels.Add(model);
                Debug.Log("ID:"+ listUserRanks[i].userID);
            }
            Data.InsertItems(index, myListItemModels);
            Data.NotifyListChangedExternally();
        }

        public void RemoveItemsFrom(int index, int count)
		{
			// Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.RemoveRange(index, count);
			//RemoveItems(index, count);

			Data.RemoveItems(index, count);
		}

        public void RemoveAllItems()
        {
            //Commented: this only works with Lists. ATM, Remove for Grids only works by manually changing the list and calling NotifyListChangedExternally() after
            //Data.RemoveRange(index, count);
            Data.List.RemoveAll((item) => true);
            Data.NotifyListChangedExternally();
        }

        public void SetItems(IList<MyListItemModel> items)
		{
			// Commented: the below 3 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.Clear();
			//YourList.AddRange(items);
			//ResetItems(YourList.Count);

			Data.ResetItems(items);
		}
		#endregion


		// Here, we're requesting <count> items from the data source
		void RetrieveDataAndUpdate(int count)
		{
			StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
		}

		// Retrieving <count> models from the data source and calling OnDataRetrieved after.
		// In a real case scenario, you'd query your server, your database or whatever is your data source and call OnDataRetrieved after
		IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
		{
			// Simulating data retrieving delay
			yield return new WaitForSeconds(.5f);
			
			var newItems = new MyListItemModel[count];

			// Retrieve your data here
			/*
			for (int i = 0; i < count; ++i)
			{
				var model = new MyListItemModel()
				{
					title = "Random item ",
					color = new Color(
								UnityEngine.Random.Range(0f, 1f),
								UnityEngine.Random.Range(0f, 1f),
								UnityEngine.Random.Range(0f, 1f),
								UnityEngine.Random.Range(0f, 1f)
							)
				};
				newItems[i] = model;
			}
			*/

			OnDataRetrieved(newItems);
		}

		void OnDataRetrieved(MyListItemModel[] newItems)
		{
			Data.InsertItemsAtEnd(newItems);
		}
	}

	// Class containing the data associated with an item
	public class MyListItemModel
	{
        /*
		public string title;
		public Color color;
		*/

        public int index;
        public string id;
        public string name;
        public int level;
	}


	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	public class MyListItemViewsHolder : BaseItemViewsHolder
	{
		
		public Text IDText;
		public Text NameText;
		public Text LevelText;
		


		// Retrieving the views from the item's root GameObject
		public override void CollectViews()
		{
			base.CollectViews();

			// GetComponentAtPath is a handy extension method from frame8.Logic.Misc.Other.Extensions
			// which infers the variable's component from its type, so you won't need to specify it yourself
			
			root.GetComponentAtPath("ID", out IDText);
			root.GetComponentAtPath("Name", out NameText);
			root.GetComponentAtPath("Level", out LevelText);
			
		}

		// Override this if you have children layout groups or a ContentSizeFitter on root that you'll use. 
		// They need to be marked for rebuild when this callback is fired
		/*
		public override void MarkForRebuild()
		{
			base.MarkForRebuild();

			LayoutRebuilder.MarkLayoutForRebuild(yourChildLayout1);
			LayoutRebuilder.MarkLayoutForRebuild(yourChildLayout2);
			YourSizeFitterOnRoot.enabled = true;
		}
		*/

		// Override this if you've also overridden MarkForRebuild() and you have enabled size fitters there (like a ContentSizeFitter)
		/*
		public override void UnmarkForRebuild()
		{
			YourSizeFitterOnRoot.enabled = false;

			base.UnmarkForRebuild();
		}
		*/
	}
}
