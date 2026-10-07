using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour, IEnumerable<Relic>
{

    /// <summary>
    /// A private list of Items.
    /// </summary>
    private List<Relic> _inventory = new List<Relic>();

    /// <summary>
    /// An event that informs whoever is listening that the inventory has changed.
    /// </summary>
    public event Action<IEnumerator<Relic>> OnInventoryChange;

    /// <summary>
    /// Event invoked when the inventory is cleared.
    /// </summary>
    // public event Action OnInventoryCleared;

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
    /// Retrieve the Relic at the index.
    /// </summary>
    /// <param name="index"> Index of the Relic to retrieve. </param>
    /// <returns> The Relic to retrieve. </returns>
    public Relic GetRelicAt(int index)
    {
        // Debug.Log("Get Relic At: " + index);

        return _inventory[index];
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
        // OnInventoryCleared?.Invoke(); // Is this not necessary? If the inventory is cleared and then the notice that there is a change in inventory is sent (with an empty inventory) then shouldn't that auto cancel all effects eventually?
        OnInventoryChange?.Invoke(GetEnumerator());
    }
}
