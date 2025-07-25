using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosionController : MonoBehaviour
{
	[SerializeField] ParticleSystem _particleSystem;
	[SerializeField] AudioClip _explosionSound;

	public void Initialize(float explosionArea)
	{
		_particleSystem.startSpeed = explosionArea;
		_particleSystem.Play();
		GetComponent<AudioSource>().PlayOneShot(_explosionSound);
		StartCoroutine(DestroyByTime());
	}

	private IEnumerator DestroyByTime()
	{
		float lifetime = Mathf.Max(_particleSystem.main.duration, _explosionSound.length);
		yield return new WaitForSeconds(lifetime);
		Object.Destroy(base.gameObject);
	}
}
