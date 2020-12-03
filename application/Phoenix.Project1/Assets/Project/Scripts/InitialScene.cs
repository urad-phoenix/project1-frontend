using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{   
   private void Awake()
   {
      UnityEngine.AddressableAssets.Addressables.InitializeAsync();
   }
   
   public void ToRootScene()
   {
      UnityEngine.AddressableAssets.Addressables.LoadSceneAsync("scene-root");
   }
}
