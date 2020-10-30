namespace TP.Scene.Locators
{
	using UnityEngine;

	public class GroupLocator : MonoBehaviour
	{
		public int Index;

		#if UNITY_EDITOR
		
		[Header("Scene Editor")]
		[SerializeField]
		public Color Color = Color.blue;

		[SerializeField]
		public float ShownPos = 1.8f;
        
		[SerializeField]
		public float LabelShownPos = 2.3f;

		[SerializeField] 
		public int FontMaxSize = 10;
        
		[SerializeField] 
		public int FontMinSize = 3;

		[SerializeField] 
		public float ShowSize = 0.3f;		
		#endif
	}
}  