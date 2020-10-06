
using System.Linq;
using UniRx;
using UnityEngine;
namespace Phoenix.Project1.Client.UI
{
    public class MessageBoxProvider : MonoBehaviour
    {
        public Transform Root;
        public GameObject Prefab;
        public static MessageBoxProvider Instance => UnityEngine.GameObject.FindObjectOfType<Phoenix.Project1.Client.UI.MessageBoxProvider>();

        public System.IObservable<MessageBox> OpenObservable(string title, string message, params string[] buttons)
        {
            return UniRx.Observable.Return(Open(message, title, buttons));
        }
        public MessageBox Open(string message,string title , params string[] buttons)
        {
            var obj =  GameObject.Instantiate(Prefab , Root);
            obj.SetActive(true);
            var msg = obj.GetComponent<MessageBox>();
            msg.Title.text = title;
            msg.Message.text = message;

            var pairs = msg.ButtonTexts.Zip(buttons, (f, s) => new { Button = f, Text = s });

            foreach (var pair in pairs)
            {
                pair.Button.text = pair.Text;
            }
            return msg;
        }

        public System.IObservable<Unit>  Close(MessageBox msg_box)
        {
            GameObject.Destroy(msg_box.gameObject);
            return UniRx.Observable.Return(new Unit());
        }
    }

}

