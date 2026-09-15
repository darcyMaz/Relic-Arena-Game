using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/**
 * TwoDTree was created by Adam Zeimann in Java and adapted into C# by Darcy Mazloum (without AI).
 * https://github.com/AdamZieman/2d-coordinate-bst
 */

public class TwoDTree
{
    TreeNode Root; // the root of the 2D tree

    /**
     * Default constructor: initializes an empty 2D Tree.
     */
    public TwoDTree()
    {
        Root = null;
    }

    /**
     * Parameterized constructor: initializes an empty 2D Tree.
     * @param Vector2s A list of Vector2s to be inserted into the 2D Tree.
     */
    public TwoDTree(List<Vector2> Vector2s)
    {
        // For each Vector2 in the list of Vector2s, the insert method is called to add it to the TwoDTree
        foreach (Vector2 Vector2 in Vector2s)
        {
            Insert(Vector2);
        }
    }

    /**
     * Inserts a Vector2 into the 2D Tree.
     * @param Vector2 The Vector2 to be inserted.
     */
    public void Insert(Vector2 Vector2)
    {
        // If the root node is null, create a new root node with the given Vector2
        if (Root == null)
        {
            Root = new TreeNode(Vector2, YComparator.GetInstance(), XComparator.GetInstance());
        }
        // Otherwise, call the insert method to insert of the root node
        else
        {
            Root.Insert(Vector2);
        }
    }

    /**
     * Searches for a Vector2 in the 2D Tree.
     * @param Vector2 The Vector2 to be searched for.
     * @return True if the Vector2 is found, false otherwise.
     */
    public bool Search(Vector2 Vector2)
    {
        // If the root node is null, return false
        if (Root == null)
        {
            //Debug.Log("Root node is null.");
            return false;
        }
        // Otherwise, call the search method of the root node
        else
        {
            //Debug.Log("Search method on " + Vector2);
            return Root.Search(Vector2);
        }
    }

    /**
     * Searches for all Vector2s within a given range.
     * @param Vector21 One corner of the range.
     * @param Vector22 The opposite corner of the range.
     * @return A list of all Vector2s within the given range.
     */
    public List<Vector2> SearchRange(Vector2 Vector21, Vector2 Vector22)
    {
        return Root.SearchRange(Vector21, Vector22);
    }

    class TreeNode
    {
        Vector2 Vector2; // the Vector2 stored in this object
        TreeNode leftSubtree; // the left subtree
        TreeNode rightSubtree; // the right subtree
        IComparer<Vector2> currentLevelComparator; // a comparator to make decisions on this level
        IComparer<Vector2> childLevelComparator; // a comparator to make decisions on the child level

        /**
         * Initializes a new TreeNode with the given Vector2 and comparators.
         * @param Vector2 The Vector2 to be stored in the node.
         * @param meComp The comparator used to make decisions on this level.
         * @param childComp The comparator used to make decisions on the child level.
         */ 
        public TreeNode(Vector2 Vector2, IComparer<Vector2> meComp, IComparer<Vector2> childComp)
        {
            this.Vector2 = Vector2;
            this.currentLevelComparator = meComp;
            this.childLevelComparator = childComp;
        }

        /**
         * Inserts a Vector2 into the subtree rooted at this node.
         * @param Vector2 The Vector2 to be inserted.
         */
        public void Insert(Vector2 Vector2)
        {
            /*
            If the Vector2 to insert is less than or equal to the current node's Vector2, insert the Vector2 in the
            left subtree
             */
            if (  (float) currentLevelComparator.Compare(this.Vector2, Vector2) / 1000  >= 0)
            {
                // If a left subtree exists, recursively insert into the left subtree
                if (leftSubtree != null)
                {
                    leftSubtree.Insert(Vector2);
                }
                // Otherwise, create a left subtree with the given Vector2
                else
                {
                    leftSubtree = new TreeNode(Vector2, childLevelComparator, currentLevelComparator);
                }
            }
            // Otherwise, insert the Vector2 in the right subtree
            else if (  (float) currentLevelComparator.Compare(this.Vector2, Vector2) / 1000 < 0)
            {
                // If a right subtree exists, recursively insert into the right subtree
                if (rightSubtree != null)
                {
                    rightSubtree.Insert(Vector2);
                }
                // Otherwise, create a new one with the given Vector2
                else
                {
                    rightSubtree = new TreeNode(Vector2, childLevelComparator, currentLevelComparator);
                }
            }
        }

        /**
         * Searches for a Vector2 in the K-D Tree.
         * @param Vector2 the Vector2 to search for
         * @return true if the Vector2 is found, false otherwise
         */
        public bool Search(Vector2 Vector2)
        {
            // Check if the current node is the target Vector2
            if ( (float) currentLevelComparator.Compare(this.Vector2, Vector2) / 1000 == 0 &&
                    (float) childLevelComparator.Compare(this.Vector2, Vector2) / 1000 == 0)
            {
                //Debug.Log("Current node is the target. " + this.Vector2);
                return true;
            }

            // Traverse the left subtree if the target Vector2 is to the left of the current node
            if ( (float) currentLevelComparator.Compare(this.Vector2, Vector2) / 1000 >= 0 &&
                    leftSubtree != null)
            {
                //Debug.Log("Traverse left subtree: " + this.Vector2);
                return leftSubtree.Search(Vector2);
            }
            // Traverse the right subtree if the target Vector2 is to the right of the current node
            else if ( (float) currentLevelComparator.Compare(this.Vector2, Vector2) / 1000 < 0 &&
                    rightSubtree != null)
            {
                //Debug.Log("Traverse right subtree: " + this.Vector2);
                return rightSubtree.Search(Vector2);
            }
            // Target Vector2 not found in the tree
            else
            {
                //Debug.Log("Dead end in search: " + this.Vector2);
                return false;
            }
        }

        /**
         * Searches for Vector2s within a given range in the K-D Tree.
         * @param Vector21 the first Vector2 defining the search range
         * @param Vector22 the second Vector2 defining the search range
         * @return an ArrayList of Vector2s within the search range
         */
        public List<Vector2> SearchRange(Vector2 Vector21, Vector2 Vector22)
        {
            // Find the lower left and upper right Vector2s of the search range
            Vector2 lowerLeft = new Vector2(Math.Min(Vector21.x, Vector22.x), Math.Min(Vector21.y, Vector22.y));
            Vector2 upperRight = new Vector2(Math.Max(Vector21.x, Vector22.x), Math.Max (Vector21.y, Vector22.y));

            // Create an ArrayList to store the Vector2s found within the search range
            List<Vector2> results = new List<Vector2>();

            // Search for all Vector2s within the search range and add them to the results ArrayList
            SearchRange(lowerLeft, upperRight, results);

            return results; // Return the results ArrayList
        }

        /**
         * Helper method for recursively searching for Vector2s within a given range in the K-D Tree.
         * @param lowerLeft the lower left Vector2 defining the search range
         * @param upperRight the upper right Vector2 defining the search range
         * @param results the ArrayList to add the Vector2s found within the search range
         * @return the ArrayList of Vector2s within the search range
         */
        private List<Vector2> SearchRange(Vector2 lowerLeft, Vector2 upperRight, List<Vector2> results)
        {
            // Check if the current Vector2 is within the specified range
            if (this.Vector2.x >= lowerLeft.x && this.Vector2.x <= upperRight.x &&
                    this.Vector2.y >= lowerLeft.y && this.Vector2.y <= upperRight.y)
            {
                results.Add(this.Vector2);
            }

            // Check if the left subtree could contain Vector2s within the range
            if ( (float) this.currentLevelComparator.Compare(Vector2, lowerLeft) / 1000 >= 0 && leftSubtree != null)
            {
                leftSubtree.SearchRange(lowerLeft, upperRight, results);
            }

            // Check if the right subtree could contain Vector2s within the range
            if ( (float) this.currentLevelComparator.Compare(Vector2, upperRight) / 1000 <= 0 && rightSubtree != null)
            {
                rightSubtree.SearchRange(lowerLeft, upperRight, results);
            }

            return results; // Return the results ArrayList
        }
    }
}

class XComparator : IComparer<Vector2>
{
    private static XComparator xComparator = new XComparator();

    public int Compare(Vector2 Vector21, Vector2 Vector22)
    {
        float distance = Vector21.x - Vector22.x;
        
        // Idea here is to return a value that can still recognize positions at decimal values without returning a float. 
        return (int) (distance*1000);
    }

    public static XComparator GetInstance()
    {
        return xComparator;
    }
}

class YComparator : IComparer<Vector2>
{
    private static YComparator yComparator = new YComparator();

    public int Compare(Vector2 Vector21, Vector2 Vector22)
    {
        float distance = Vector21.y - Vector22.y;

        // Idea here is to return a value that can still recognize positions at decimal values without returning a float. 
        return (int) (distance * 1000);
    }

    public static YComparator GetInstance()
    {
        return yComparator;
    }
}
