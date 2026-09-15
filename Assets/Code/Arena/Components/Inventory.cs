 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour, IEnumerable<Item>
{

    // A private list of Items.
    private List<Item> _inventory = new List<Item>();

    // An event that informs whoever is listening that the inventory has changed.
    public event Action<IEnumerator<Item>> OnInventoryChange;

    // Implement the IEnumerable class with these two functions.
    public IEnumerator<Item> GetEnumerator()
    {
        // Get the Enumeator from the inventory list.
        return _inventory.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Methods that can change the inventory.
    public void AddItem(Item newItem)
    {
        _inventory.Add(newItem);
        // Invoke the event which informs others that the inventory has been changed.
        OnInventoryChange?.Invoke(GetEnumerator());
    }
    public void RemoveItem(Item newItem)
    {
        _inventory.Remove(newItem);
        OnInventoryChange?.Invoke(GetEnumerator());
    }
    public void RemoveItemAt(int index)
    {
        _inventory.RemoveAt(index);
        OnInventoryChange?.Invoke(GetEnumerator());
    } 
}
