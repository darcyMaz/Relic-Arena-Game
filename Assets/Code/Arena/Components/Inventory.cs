using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour, IEnumerable<Relic>
{

    // A private list of Items.
    private List<Relic> _inventory = new List<Relic>();

    // An event that informs whoever is listening that the inventory has changed.
    public event Action<IEnumerator<Relic>> OnInventoryChange;

    // Implement the IEnumerable class with these two functions.
    public IEnumerator<Relic> GetEnumerator()
    {
        // Get the Enumeator from the inventory list.
        return _inventory.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    // Methods that can change the inventory.
    public void AddItem(Relic newItem)
    {
        _inventory.Add(newItem);
        // Invoke the event which informs others that the inventory has been changed.
        OnInventoryChange?.Invoke(GetEnumerator());
    }
    public void RemoveItem(Relic newItem)
    {
        _inventory.Remove(newItem);
        OnInventoryChange?.Invoke(GetEnumerator());
    }
    public void RemoveItemAt(int index)
    {
        _inventory.RemoveAt(index);
        OnInventoryChange?.Invoke(GetEnumerator());
    }

    /// <summary>
    /// Returns the count of the inventory.
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        return _inventory.Count;
    }

    /// <summary>
    /// Clears the inventory.
    /// </summary>
    public void Clear()
    {
        _inventory.Clear();
    }
}
