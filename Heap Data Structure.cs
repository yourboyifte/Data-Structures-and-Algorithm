﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public class Heap<K, D> where K : IComparable<K>
    {

        // This is a nested Node class whose purpose is to represent a node of a heap.
        private class Node : IHeapifyable<K, D>
        {
            // The Data field represents a payload.
            public D Data { get; set; }
            // The Key field is used to order elements with regard to the Binary Min (Max) Heap Policy, i.e. the key of the parent node is smaller (larger) than the key of its children.
            public K Key { get; set; }
            // The Position field reflects the location (index) of the node in the array-based internal data structure.
            public int Position { get; set; }

            public Node(K key, D value, int position)
            {
                Data = value;
                Key = key;
                Position = position;
            }

            // This is a ToString() method of the Node class.
            // It prints out a node as a tuple ('key value','payload','index')}.
            public override string ToString()
            {
                return "(" + Key.ToString() + "," + Data.ToString() + "," + Position + ")";
            }
        }

        // ---------------------------------------------------------------------------------
        // Here the description of the methods and attributes of the Heap<K, D> class starts

        public int Count { get; private set; }

        // The data nodes of the Heap<K, D> are stored internally in the List collection. 
        // Note that the element with index 0 is a dummy node.
        // The top-most element of the heap returned to the user via Min() is indexed as 1.
        private List<Node> data = new List<Node>();

        // We refer to a given comparer to order elements in the heap. 
        // Depending on the comparer, we may get either a binary Min-Heap or a binary  Max-Heap. 
        // In the former case, the comparer must order elements in the ascending order of the keys, and does this in the descending order in the latter case.
        private IComparer<K> comparer;

        // We expect the user to specify the comparer via the given argument.
        public Heap(IComparer<K> comparer)
        {
            this.comparer = comparer;

            // We use a default comparer when the user is unable to provide one. 
            // This implies the restriction on type K such as 'where K : IComparable<K>' in the class declaration.
            if (this.comparer == null) this.comparer = Comparer<K>.Default;

            // We simplify the implementation of the Heap<K, D> by creating a dummy node at position 0.
            // This allows to achieve the following property:
            // The children of a node with index i have indices 2*i and 2*i+1 (if they exist).
            data.Add(new Node(default(K), default(D), 0));
        }

        // This method returns the top-most (either a minimum or a maximum) of the heap.
        // It does not delete the element, just returns the node casted to the IHeapifyable<K, D> interface.
        public IHeapifyable<K, D> Min()
        {
            if (Count == 0) throw new InvalidOperationException("The heap is empty.");
            return data[1];
        }

        // Insertion to the Heap<K, D> is based on the private UpHeap() method
        public IHeapifyable<K, D> Insert(K key, D value)
        {
            Count++;
            Node node = new Node(key, value, Count);
            data.Add(node);
            UpHeap(Count);
            return node;
        }

        private void UpHeap(int start)
        {
            int position = start;
            while (position != 1)
            {
                if (comparer.Compare(data[position].Key, data[position / 2].Key) < 0) Swap(position, position / 2);
                position = position / 2;
            }
        }

        //not in the task sheet but required for delete method
        private void DownHeap(int start)
        {
            int tree_size = Count;
            int position = start;
            int right_child = position * 2 + 1;
            int left_child = position * 2;

            // first we compare right_child with left child to see if right child minimum. If right_child minimum then compare with parent.
            if (right_child <= tree_size && comparer.Compare(data[right_child].Key, data[left_child].Key) < 0 && comparer.Compare(data[right_child].Key, data[position].Key) < 0)
            {
                Swap(position, right_child);
                DownHeap(right_child);
            }
            //if first condition fails it means left child is minimum so compare left child with parent
            else if (left_child <= tree_size && comparer.Compare(data[left_child].Key, data[position].Key) < 0)
            {
                Swap(position, left_child);
                DownHeap(left_child);
            }

        }
        // This method swaps two elements in the list representing the heap. 
        // Use it when you need to swap nodes in your solution, e.g. in DownHeap() that you will need to develop.
        private void Swap(int from, int to)
        {
            Node temp = data[from];
            data[from] = data[to];
            data[to] = temp;
            data[to].Position = to;
            data[from].Position = from;
        }

        public void Clear()
        {
            for (int i = 0; i <= Count; i++) data[i].Position = -1;
            data.Clear();
            data.Add(new Node(default(K), default(D), 0));
            Count = 0;
        }

        public override string ToString()
        {
            if (Count == 0) return "[]";
            StringBuilder s = new StringBuilder();
            s.Append("[");
            for (int i = 0; i < Count; i++)
            {
                s.Append(data[i + 1]);
                if (i + 1 < Count) s.Append(",");
            }
            s.Append("]");
            return s.ToString();
        }

        // TODO: Your task is to implement all the remaining methods.
        // Read the instruction carefully, study the code examples from above as they should help you to write the rest of the code.
        public IHeapifyable<K, D> Delete()
        {
            if (Count == 0) throw new InvalidOperationException("The heap is empty.");
            Node deleted_node = data[1];
            data[1] = data[Count];
            data.RemoveAt(Count);
            Count--;
            DownHeap(1);

            return deleted_node;
        }

        // Builds a minimum binary heap using the specified data according to the bottom-up approach.
        public IHeapifyable<K, D>[] BuildHeap(K[] keys, D[] data)
        {
            if (Count != 0) throw new InvalidOperationException("The heap is not empty.");
            //int start = Min().Position;
            Node[] newHeap = new Node[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                Node node = new Node(keys[i], data[i], ++Count);//doing ++Count instead of Count++ because position needs to start from 1
                this.data.Add(node);
                newHeap[i] = node;
            }
            //now the order will not be correct so we need to do downheap to restore order
            //also we start from bottom and go up. We find our first non-leaf node, check its children, then iterate to a diff node, check its children and so on
            //first non-leaf node in this heap starts from (Count / 2)
            int firstNonleaf = Count / 2;
            for (int i = firstNonleaf; i > 0; i--)
            {
                DownHeap(i);
            }
            return newHeap;
        }

        public void DecreaseKey(IHeapifyable<K, D> element, K new_key)
        {

            if (element.Position != this.data.IndexOf(element as Node))
            {
                throw new InvalidOperationException("The element is inconsistent to the current state of the Heap.");
            }
            else
            {
                data[element.Position].Key = new_key;
                //now since we have decreased it to a lesser number key, we need to compare it to its parents and move it to the top using upheap
                UpHeap(element.Position);
            }


        }
        //DeleteElement will have complexity O(log n) because both DownHeap and Upheap has O(log n). Other operations are O(1)
        public IHeapifyable<K, D> DeleteElement(IHeapifyable<K, D> element)
        {
            Node deleted_node = data[element.Position];
            data[element.Position] = data[Count];
            data.RemoveAt(Count);
            Count--;
            int parent_node = element.Position / 2;
            //if element is at root node or if element at the current position is greater than its parent then push it down
            //else move it up
            if (element.Position == 1 || comparer.Compare(data[parent_node].Key, data[element.Position].Key) < 0)
            {
                DownHeap(element.Position);
            }
            else
            {
                UpHeap(element.Position);
            }

            return deleted_node;

        }
        //Finding KthMinElement will have complexity of O(K log n) where K is the number that we are trying to search for and log n is height.
        //We are considering height log n because delete() and insert() both have to heapify in worst case and hepifying is dependent on the height
        public IHeapifyable<K, D> KthMinElement(int k)
        {
            Heap<K, D> minList = new Heap<K, D>(Comparer<K>.Default);
            Node kthmin;
            for (int i = 1; i <= k; i++)
            {
                Node minimum = data[1];
                minList.Insert(minimum.Key, minimum.Data);
                Delete();
            }

            kthmin = minList.data[k];

            for (int i = 1; i <= minList.Count; i++)
            {
                Insert(minList.data[i].Key, minList.data[i].Data);
            }

            return kthmin;
        }


    }
}
