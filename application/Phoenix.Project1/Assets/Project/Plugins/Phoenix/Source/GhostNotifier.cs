using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Project1
{
    public class GhostNotifier<TSoul,TGhost> : INotifier<TGhost> where TSoul : TGhost 
    {
        readonly INotifier<TSoul> _Parent;

        readonly List<TGhost> _Ghosts;
        public GhostNotifier(INotifier<TSoul> parent)
        {
            _Ghosts = new List<TGhost>();
            _Parent = parent;            
            _UnsupplyEvent += _Empty;
            _SupplyEvent += _Empty;
            _Parent.Supply += _ToChildSupply;
            _Parent.Unsupply += _ToChildUnsupply;
        }

        private void _Empty(TGhost obj)
        {
            
        }

        private void _ToChildUnsupply(TSoul parent)
        {
            
            _Ghosts.Remove(parent);
            _UnsupplyEvent(parent);
        }

        private void _ToChildSupply(TSoul parent)
        {
            _Ghosts.Add(parent);
            _SupplyEvent(parent);
        }

        TGhost[] INotifier<TGhost>.Ghosts => _Parent.Ghosts.Cast<TGhost>().ToArray();

        event Action<TGhost> _SupplyEvent;
        event Action<TGhost> INotifier<TGhost>.Supply
        {
            add
            {
                _SupplyEvent += value;
                foreach (var item in _Ghosts)
                {
                    value(item);
                }
            }

            remove
            {
                _SupplyEvent -= value;
            }
        }

        event Action<TGhost> _UnsupplyEvent;
        event Action<TGhost> INotifier<TGhost>.Unsupply
        {
            add
            {
                _UnsupplyEvent += value;
            }

            remove
            {
                _UnsupplyEvent -= value;
            }
        }
    }
}
