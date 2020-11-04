using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Project1.Game
{
    public class CircularQueue<T>
    {
        
        class Node
        {
            public T Value;
            public Node Next;
        }
        Node _Current;

        public CircularQueue(IEnumerable<T> values)
        {
            var prev = new Node();
            _Current = prev;
            prev.Value = values.First();
            
            foreach (var item in values.Skip(1))
            {
                var node = new Node();
                node.Value = item;
                node.Next = null;

                prev.Next = node;
                prev = node;
            }

            prev.Next = _Current;

        }


        public T GetCurrentAndNext()
        {

            var val = _Current.Value;
            _Current = _Current.Next;
            return val;

        }
    }
}