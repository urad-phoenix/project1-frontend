using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextJumpTests : MonoBehaviour
{
   public TextJumpComponent Source;
   
   public List<string> _Strings = new List<string>();

   public void SpawnAndJump()
   {
      var text = Instantiate(Source, transform);
      text.gameObject.SetActive(true);

      var obs = from jump in text.SetTextJumpAsObservable(_Strings[Random.Range(0, _Strings.Count)])
         select jump;

      var compeleteObs = text.RegisterCompleteCallback();            
         
      compeleteObs.Subscribe(_JumpFinished).AddTo(gameObject);

      obs.Subscribe(unit => _Compelete()).AddTo(gameObject);
   }

   private void _Compelete()
   {
      Debug.Log($"_Compelete");
   }

   private void _JumpFinished(GameObject go)
   {
      Destroy(go);
      Debug.Log($"JumpFinished {go.name}");
   }
}
