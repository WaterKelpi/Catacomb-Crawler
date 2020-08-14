using System.Collections;
using UnityEngine;
using System;

public class Heap<T> where T: IHeapItem<T> {
	T[] items;
	int curItemCount;

	public Heap (int maxHeapSize) {
		items = new T[maxHeapSize];
	}

	public void Add (T item) {
		item.HeapIndex = curItemCount;
		items [curItemCount] = item;
		SortUp (item);
		curItemCount++;
	}

	public T RemoveFirst () {
		T firstItem = items [0];
		curItemCount--;
		items [0] = items [curItemCount];
		items [0].HeapIndex = 0;
		SortDown (items [0]);
		return firstItem;
	}

	public void UpdateItem (T item) {
		SortUp (item);
	}

	public int Count {
		get {
			return curItemCount;
		}
	}



	public bool Contains (T item) {
		return Equals (items [item.HeapIndex], item);
	}

	void SortDown (T item) {
		while (true) {
			int childLIndex = item.HeapIndex * 2 + 1;
			int childRIndex = item.HeapIndex * 2 + 2;
			int swapIndex = 0;
			if (childLIndex < curItemCount) {
				swapIndex = childLIndex;
				if (childRIndex < curItemCount) {
					if (items [childLIndex].CompareTo (items [childRIndex]) < 0) {
						swapIndex = childRIndex;
					}
				}
					if (item.CompareTo (items [swapIndex]) < 0) {
						Swap (item, items [swapIndex]);
					} else
						return;
			} else
				return;
		}	
	}

	void SortUp (T item) {
		int parentIndex = (item.HeapIndex - 1) / 2;
		while (true) {
			T parentItem = items [parentIndex];
			if (item.CompareTo (parentItem) > 0) {
				Swap (item, parentItem);
			} else{
				break;}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	void Swap (T itemA, T itemB) {
		items [itemA.HeapIndex] = itemB;
		items [itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T> {
	int HeapIndex {
		get;
		set;
	}
}
