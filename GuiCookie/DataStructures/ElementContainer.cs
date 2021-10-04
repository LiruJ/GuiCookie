using GuiCookie.Elements;
using LiruGameHelper.Signals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GuiCookie.DataStructures
{
    public struct ElementEnumerator : IEnumerator<Element>, IEnumerator<ElementContainer>
    {
        #region Fields
        private readonly ICollection<ElementContainer> mainCollection;

        private readonly IEnumerator<ElementContainer> mainEnumerator;

        private readonly ICollection<ElementContainer> additionCollection;

        private readonly IEnumerator<ElementContainer> additionEnumerator;

        private readonly ICollection<ElementContainer> removalCollection;

        private bool mainDone;
        #endregion

        public Element Current => !mainDone ? mainEnumerator.Current?.Element : additionEnumerator.Current?.Element;

        object IEnumerator.Current => !mainDone ? mainEnumerator.Current : additionEnumerator.Current;

        ElementContainer IEnumerator<ElementContainer>.Current => !mainDone ? mainEnumerator.Current : additionEnumerator.Current;

        public bool MoveNext()
        {
            // If the main collection should still be enumerated through, do so.
            if (!mainDone)
            {
                // Keep moving the main enumerator forward until either the end of the collection has been reached or an element that is not in the removal queue is reached.
                do
                {
                    // Move the main enumerator forward, save the inverted result.
                    mainDone = !mainEnumerator.MoveNext();
                } while (!mainDone && (removalCollection.Contains(Current.ElementContainer) || additionCollection.Contains(Current.ElementContainer)));

                // If the main enumerator is not done, return true. Otherwise; fall through to the addition enumerator.
                if (!mainDone) return true;
            }

            // Check the addition queue until an item that does not exist in the removal queue and main collection is found.
            bool additionDone;
            do
            {
                additionDone = !additionEnumerator.MoveNext();
            } while (!additionDone && (removalCollection.Contains(Current.ElementContainer) && !mainCollection.Contains(Current.ElementContainer)));

            return !additionDone;
        }

        public void Reset()
        {
            mainEnumerator.Reset();
            additionEnumerator.Reset();
            mainDone = false;
        }

        #region Constructors
        public ElementEnumerator(ICollection<ElementContainer> children, ICollection<ElementContainer> addition, ICollection<ElementContainer> removal)
        {
            mainDone = false;

            mainCollection = children ?? throw new ArgumentNullException(nameof(children));
            mainEnumerator = mainCollection.GetEnumerator();
            additionCollection = addition ?? throw new ArgumentNullException(nameof(addition));
            additionEnumerator = additionCollection.GetEnumerator();
            removalCollection = removal ?? throw new ArgumentNullException(nameof(removal));
        }
        #endregion

        #region IDisposable Support
        public void Dispose()
        {
            mainEnumerator.Dispose();
            additionEnumerator.Dispose();
        }
        #endregion
    }

    public class ElementContainer : IEnumerable<Element>
    {
        #region Fields
        private readonly List<ElementContainer> children = new List<ElementContainer>();

        private readonly Dictionary<string, ElementContainer> childrenByName = new Dictionary<string, ElementContainer>();

        /// <summary> 
        /// The collection of elements to be removed from the <see cref="children"/> and/or <see cref="additionQueue"/> collection nex time <see cref="flushQueues"/> is called. 
        /// Each item within this collection exists in the children and/or addition collection and there are no duplicates.
        /// </summary>
        private readonly HashSet<ElementContainer> removalQueue = new HashSet<ElementContainer>();

        /// <summary>
        /// If an item in this collection also exists within the main children collection, then it also exists within the removal queue. This basically means that the next flush will remove and re-add the item.
        /// </summary>
        private readonly List<ElementContainer> additionQueue = new List<ElementContainer>();
        #endregion

        #region Backing Fields
        private readonly Root root;

        private ElementContainer parent;
        #endregion

        #region Properties
        /// <summary> The Element that this container represents. </summary>
        public Element Element { get; private set; }

        public Root Root => root ?? Element?.Root;

        public ElementContainer Parent
        {
            get => parent;
            set
            {
                // If the given value is the same as the current value, do nothing.
                if (value == parent) return;

                // If the given value is null, then just remove this container from its parent.
                if (value == null)
                    { if (!parent.RemoveChild(this)) throw new Exception("Child could not be removed from its parent."); }
                // Otherwise; if the given value is another container, add this child to it. This will handle setting the parent and handling the switch.
                else if (!value.AddChild(this)) throw new Exception("Child could not be added to new parent.");
            }
        }

        /// <summary> The number of active children within this container. </summary>
        public int Count { get; private set; } = 0;//=> children.Count((e) => !additionQueue.Contains(e)) + additionQueue.Count - removalQueue.Count;

        public int NamedCount => childrenByName.Count;
        #endregion

        #region Signals
        public IConnectableSignal<Element> OnChildAdded => onChildAdded;
        private readonly Signal<Element> onChildAdded = new Signal<Element>();

        public IConnectableSignal<Element> OnChildRemoved => onChildRemoved;
        private readonly Signal<Element> onChildRemoved = new Signal<Element>();
        #endregion

        #region Constructors
        /// <summary> Creates an element container for the root. </summary>
        /// <param name="root"> The layout's root. </param>
        internal ElementContainer(Root root)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            Element = null;
        }

        internal ElementContainer(Element element)
        {
            Element = element ?? throw new ArgumentNullException(nameof(element));
            root = null;
        }
        #endregion

        #region Element Functions
        internal void onElementDestroyed()
        {
            // Disconnect all events.
            onChildAdded.DisconnectAll();
            onChildRemoved.DisconnectAll();
        }

        private int calculateCount()
        {
            // Start with the elements within the main collection.
            int count = children.Count;

            // Add to the count the amount of elements that are to be added to the main collection and do not already exist in there.
            // If an item exists in the addition queue and also in the main collection, it does not add to the count as the main collection already counted it.
            // If an item exists in the addition queue and also in the removal queue, but not the main collection, it does not add to the count as it will be removed next flush.
            count += additionQueue.Count((e) => !children.Contains(e) && !removalQueue.Contains(e));

            // Subtract from the count the amount of elements that are to be removed from the main collection which will not be added again.
            count -= removalQueue.Count((e) => !additionQueue.Contains(e));

            // Return the total count.
            return count;
        }

        public T GetChild<T>() where T : Element
        {
            // Go over each child, if one is assignable from the given type, return it.
            foreach (Element child in this)
                if (child != null && typeof(T).IsAssignableFrom(child.GetType())) return (T)child;

            // If no child was found with the type, return null.
            return null;
        }

        public Element GetChildByName(string name, bool recursive = false)
        {
            // Ensure the given name is correct.
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Cannot find a child by name with the given name as it is null or empty.", nameof(name));

            // Try to get the element with the name, if it is found then return it.
            if (childrenByName.TryGetValue(name, out ElementContainer child)) return child.Element;
            // Otherwise; if the element could not be found, handle it.
            else
            {
                // Search the addition queue.
                foreach (ElementContainer additionChild in additionQueue)
                    if (additionChild.Element != null && additionChild.Element.Name == name) return additionChild.Element;

                // If the search is recursive, try to find the element in the children.
                if (recursive)
                    // If the element was found, return it.
                    foreach (Element searchChild in this)
                    {
                        Element foundChild = searchChild.GetChildByName(name, true);
                        if (foundChild != null) return foundChild;
                    }
                // Regardless of if the search is recursive or not, if the code reaches here, nothing could be found, so return null.
                return null;
            }
        }

        public T GetChildByName<T>(string name, bool recursive = false) where T : Element
            => GetChildByName(name, recursive) as T;

        public Element GetChildByIndex(int index)
        {
            // Check for range.
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));

            // If the addition and removal queue are both empty, use the quick indexing method.
            if (additionQueue.Count == 0 && removalQueue.Count == 0)
                return children[index].Element;

            // This is an annoying way of doing this, but essentially there's no way to know which indices are valid without just going through the entire valid collection.
            int i = 0;
            foreach (Element child in this)
            {
                if (index == i) return child;
                i++;
            }

            // The code should never reach here, but if it does, it's an out of range exception.
            throw new IndexOutOfRangeException(nameof(index));
        }

        public T GetChildByIndex<T>(int index) where T : Element => GetChildByIndex(index) as T;

        public bool RemoveChild(ElementContainer child)
        {
            // Ensure the child is not null.
            if (child == null) throw new ArgumentNullException(nameof(child));

            // If the element does not exist in this container, return false.
            if (!Contains(child)) return false;

            // Add the child to the queue.
            removalQueue.Add(child);

            // Unset the parent of the child immediately.
            child.parent = null;

            // Recalculate the count.
            Count = calculateCount();

            // Call the signal.
            if (child.Element != null) onChildRemoved.Invoke(child.Element);

            // Return true as the removal was successful.
            return true;
        }

        /// <summary>  </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool AddChild(ElementContainer child)
        {
            // Ensure the child is not null.
            if (child == null) throw new ArgumentNullException(nameof(child));

            // If the element has already been added to this container, return false.
            if (Contains(child)) return false;

            // Add the child to the queue.
            additionQueue.Add(child);

            // If the child has a parent, remove the child from it. Ensure it succeeded as there is no valid state in which this can fail.
            if (child.parent != null && !child.parent.RemoveChild(child)) throw new Exception("Failed to remove child from old parent.");

            // Ensure the parent of the child is null, there is no valid state in which the parent cannot be null.
            if (child.parent != null) throw new Exception("Failed to remove old parent from child.");

            // Set the parent of the child to this container.
            child.parent = this;

            // Recalculate the count.
            Count = calculateCount();

            // Call the signal.
            if (child.Element != null) onChildAdded.Invoke(child.Element);

            // Return true as the addition was successful.
            return true;
        }

        public bool Contains(ElementContainer child)
        {
            // Ensure the child is not null.
            if (child == null) throw new ArgumentNullException(nameof(child));

            // If the removal queue contains the child, check to see if the addition queue does too.
            if (removalQueue.Contains(child))
            {
                // If both contain the child, then it means that the child is going to be removed and readded, but it still technically counts as contained.
                if (additionQueue.Contains(child)) return true;
                // Otherwise; it means that the child is going to be removed, so does not count as contained.
                else
                {
#if DEBUG
                    // If the removal queue contains the element but the addition queue does not, then it should exist in the main collection.
                    if (!children.Contains(child)) throw new Exception("Removal queue contains element and addition queue does not, yet it did not exist in main collection.");
#endif
                    return false;
                }
            }
            // Otherwise; since the element does not exist in the removal queue, the only other places it could exist is in the main collection or addition queue.
            else
            {
                // If the child is in the main collection, return true.
                if (children.Contains(child))
                {
#if DEBUG
                    // If the main collection contains the child and the removal queue does not, then there's no way that the addition queue should contain the child.
                    if (additionQueue.Contains(child)) throw new Exception("Removal queue did not contain element, yet both main collection and addition queue did.");
#endif
                    return true;
                }
                // Otherwise; if the child is in the addition queue, return true.
                else if (additionQueue.Contains(child))
                {
#if DEBUG
                    // If the addition queue contains the child and the removal queue does not, then there's no way that the main collection should contain the child.
                    if (children.Contains(child)) throw new Exception("Removal queue did not contain element, yet both main collection and addition queue did.");
#endif
                    return true;
                }
                // Finally; if the child was not found in any of the collections, then there's absolutely no way it's contained, so return false.
                else return false;
            }
        }

        internal void renameChild(ElementContainer child, string oldName)
        {
            // Ensure the child is not null.
            if (child == null) throw new ArgumentNullException(nameof(child));

            // Ensure the child is actually a child.
            if (!Contains(child)) throw new Exception("Given container is not a child of this container.");

            // If the old name was empty, it does not need to be removed.
            if (!string.IsNullOrWhiteSpace(oldName) && !childrenByName.Remove(oldName)) throw new Exception("Failed to remove child from named collection despite child being named.");

            // If the element now has a name, add it.
            if (child.Element.HasName) childrenByName.Add(child.Element.Name, child);
        }

        internal void flushQueues()
        {
            // If no changes need to be made, do nothing.
            if (removalQueue.Count == 0 && additionQueue.Count == 0) return;

#if DEBUG
            // The enumerator should only go over elements that are contained, and should only do each one once. The enumerator should also respect indices.
            int c = 0;
            foreach (Element child in this)
            {
                if (!Contains(child.ElementContainer)) throw new Exception("Container does not contain child.");
                if (GetChildByIndex(c) != child) throw new Exception($"Child {GetChildByIndex(c)} at index {c} is not {child}.");
                c++;
            }
            if (c != Count) throw new Exception($"Iterated {c} times when there's only {Count} children.");
#endif

            // Remove any elements in the queue.
            foreach (ElementContainer child in removalQueue)
            {
                // If the child was removed from the main collection successfully, don't bother checking for it in the addition queue. If the child was not found in the main collection, remove it from the addition queue.
                // This basically means that an element can be removed and added to the collection in the same frame and it will be re-added if it was already a child, or will not be added at all if it was not.
                if (children.Remove(child))
                {
                    if (child.Element.HasName && !childrenByName.Remove(child.Element.Name)) throw new Exception("Failed to remove child from named collection despite child being named.");
                }
                else if (!additionQueue.Remove(child)) throw new Exception("Failed to remove child from collection.");
            }
            removalQueue.Clear();

            // Add any elements in the queue.
            foreach (ElementContainer child in additionQueue)
            {
                if (child.Element.HasName)
                {
#if DEBUG
                    if (childrenByName.ContainsKey(child.Element.Name)) throw new Exception("Failed to add named child to named collection as a child with this name already exists.");
#endif
                    childrenByName.Add(child.Element.Name, child);
                }
#if DEBUG
                if (children.Contains(child)) throw new Exception("Failed to add child as child already exists in collection.");
#endif
                children.Add(child);
            }
            additionQueue.Clear();

#if DEBUG
            // The count should not change due to this function.
            int newCount = calculateCount();
            if (Count != newCount) throw new Exception($"The element count was somehow changed during the queue flushing operation. Was {Count}, now {newCount}.");

            AssertValidity();
#endif
        }

        public void AssertValidity()
        {
            // All children should belong to this element.
            foreach (ElementContainer child in children)
                if (!Contains(child)) throw new Exception("Container does not contain child.");

            // Ensure that every element within the named children dictionary also exist within the main list.
            foreach (ElementContainer child in childrenByName.Values)
                if (!children.Contains(child)) throw new Exception("Child exists in named collection but not in main collection.");

            foreach (ElementContainer child in children)
            {
                // Ensure that every element with a name within the list also exist within the dictionary.
                if (child.Element.HasName && !childrenByName.ContainsKey(child.Element.Name)) throw new Exception("Main child collection contains named child who is not in named collection.");

                // Ensure that the parent of every child is this.
                if (child.Parent != this) throw new Exception("Child's parent is not the containing element.");
            }
        }
        #endregion

        #region Enumerator Functions
        public IEnumerator<Element> GetEnumerator() => new ElementEnumerator(children, additionQueue, removalQueue);

        IEnumerator IEnumerable.GetEnumerator() => new ElementEnumerator(children, additionQueue, removalQueue);
        #endregion

        #region String Functions
        public override string ToString() => Element == null ? "Root container" : Element.ToString();
        #endregion
    }
}