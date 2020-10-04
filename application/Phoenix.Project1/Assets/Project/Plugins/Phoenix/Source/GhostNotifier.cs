using Regulus.Remote;
using System;
using System.Linq;

namespace Phoenix.Project1
{
    public class GhostNotifier<TSoul,TGhost> : INotifier<TGhost> where TSoul : TGhost 
    {
        INotifier<TSoul> _Parent;
        public GhostNotifier(INotifier<TSoul> parent)
        {
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
            TGhost child = parent;
            _UnsupplyEvent(child);
        }

        private void _ToChildSupply(TSoul parent)
        {
            TGhost child = parent;
            _SupplyEvent(child);
        }

        TGhost[] INotifier<TGhost>.Ghosts => _Parent.Ghosts.Cast<TGhost>().ToArray();

        event Action<TGhost> _SupplyEvent;
        event Action<TGhost> INotifier<TGhost>.Supply
        {
            add
            {
                _SupplyEvent += value;
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
