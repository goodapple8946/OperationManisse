using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Controller;

public class Ambience: MonoBehaviour
{
    public Color bgColor;
    public AudioClip audioClip;
	public GameObject effectPrefab;

	// 实例化粒子效果
	private GameObject effect;

	public void Apply()
	{
		// 设置颜色
		Camera.main.backgroundColor = bgColor;
		// 设置音效
		Camera.main.GetComponent<AudioSource>().clip = audioClip;
		Camera.main.GetComponent<AudioSource>().Play();
		// 实例化粒子效果
		if(effectPrefab != null)
		{
			effect = GameObject.Instantiate(effectPrefab);
			effect.name = effectPrefab.name;

			// 对每种粒子特殊讨论
			if (effect.gameObject.name.Equals("RainEffect")) // 下雨
			{
				// 只用改变宽度
				float width = cameraController.GetRightTopMost().x - cameraController.GetLeftBottomMost().x;
				effect.transform.localScale = new Vector2(width * 3, 1);
				// 从视角顶部中央创造粒子
				Vector2 originPos = new Vector2
				(
					(cameraController.GetLeftBottomMost().x  + cameraController.GetRightTopMost().x) / 2,
					cameraController.GetRightTopMost().y
				);
				effect.transform.position = originPos;
				// 改变发射粒子的数量
				ParticleSystem particle = effect.transform.
					Find("Rain").GetComponent<ParticleSystem>();
				var emission = particle.emission;
				float maxWidth = 100 * Grid.GRID_SIZE;
				emission.rateOverTime = Mathf.Lerp(100, 500, width/maxWidth);
			}
			
		}
	}

	public void Clear()
	{
		// 默认颜色
		Camera.main.backgroundColor = Color.blue;

		// 关闭音效
		Camera.main.GetComponent<AudioSource>().clip = null;

		// 清除创建的effect
		if(effect != null)
		{
			GameObject.Destroy(effect);
		}
	}
}