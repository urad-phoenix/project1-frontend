using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix.Project1.Client
{
    namespace UI
    {
        public class Console : MonoBehaviour, Regulus.Utility.Console.IInput, Regulus.Utility.Console.IViewer
        {
            readonly System.Collections.Generic.Queue<UnityEngine.UI.Text> _Items;
            public UnityEngine.GameObject ItemPrefab;
            public UnityEngine.Transform ItemParent;
            public UnityEngine.UI.Button Send;
            public UnityEngine.UI.InputField Input;
            public UnityEngine.UI.ScrollRect Rect;
            System.Action<string> _WriteLineHandle;
            System.Action<string> _WriteHandle;
            private Text _Current;
            readonly UniRx.CompositeDisposable _Disposables;
            public Console()
            {
                _Disposables = new CompositeDisposable();
                _Items = new System.Collections.Generic.Queue<UnityEngine.UI.Text>();
                _WriteLineHandle = _Empty;
                _WriteHandle = _Empty;
            }

            private void _Empty(string obj)
            {
            }

            void Start()
            {
                _NewItem();
                Send.OnClickAsObservable().Subscribe(_Send).AddTo(_Disposables);

                _WriteHandle = _Write;
                _WriteLineHandle = _WriteLine;
            }

            private void _Send(Unit obj)
            {

                if (string.IsNullOrWhiteSpace(Input.text))
                    return;
                _Current.text = Input.text;
                var commandArgs = Input.text.Split(' ');
                Input.text = "";
                _OutputEvent(commandArgs);
                _NewItem();
            }

            void OnDestroy()
            {
                _WriteLineHandle = _Empty;
                _WriteHandle = _Empty;
                _Disposables.Clear();
            }
            event Regulus.Utility.Console.OnOutput _OutputEvent;
            event Regulus.Utility.Console.OnOutput Regulus.Utility.Console.IInput.OutputEvent
            {
                add
                {
                    _OutputEvent += value;
                }

                remove
                {
                    _OutputEvent -= value;
                }
            }

            void Regulus.Utility.Console.IViewer.Write(string message)
            {
                _WriteHandle(message);
                //_Write(message);
            }

            private void _Write(string message)
            {
                if (string.IsNullOrWhiteSpace(message))
                    return;
                _Current.text += message;
            }

            void Regulus.Utility.Console.IViewer.WriteLine(string message)
            {
                _WriteLineHandle(message);
                //_WriteLine(message);

            }

            private void _WriteLine(string message)
            {
                if (string.IsNullOrWhiteSpace(message))
                    return;
                _Current.text += message;
                _NewItem();
            }

            private void _NewItem()
            {
                var item = GameObject.Instantiate(ItemPrefab, ItemParent);
                item.SetActive(true);
                var itemText = item.GetComponent<Text>();
                _Items.Enqueue(itemText);
                _Current = itemText;
                _Dequeue();
                Rect.verticalNormalizedPosition = 0;                
            }

            private void _Dequeue()
            {
                if(_Items.Count > 200)
                {
                    var item= _Items.Dequeue();
                    GameObject.Destroy(item.gameObject);
                }
            }
        }
    }
}

