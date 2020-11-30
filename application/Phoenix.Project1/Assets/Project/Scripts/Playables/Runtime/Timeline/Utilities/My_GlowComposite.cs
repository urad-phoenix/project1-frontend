using UnityEngine;

namespace Phoenix.Playables.Utilities
{
	//[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class My_GlowComposite : MonoBehaviour
	{
		public Shader P_GlowComposite;
		[Range(0, 10)]
		public float Intensity = 0.88f;

		private Material _compositeMat;

		void OnEnable()
		{
			if(P_GlowComposite != null)
				_compositeMat = new Material(P_GlowComposite);
		}

		void OnRenderImage(RenderTexture src, RenderTexture dst)
		{

			_compositeMat.SetFloat("_Intensity", Intensity);
			Graphics.Blit(src, dst, _compositeMat, 0);

		}
	}
}