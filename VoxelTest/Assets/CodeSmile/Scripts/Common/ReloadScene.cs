using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
	[SerializeField] private KeyCode m_KeyCode;

	[SerializeField] private float m_ReloadAfterTime;

	private float m_ReloadTime;
	private bool m_TimedReloadEnabled;

	private void Start() => m_ReloadTime = Time.timeSinceLevelLoad + m_ReloadAfterTime;

	private void Update()
	{
		if (Input.GetKeyDown(m_KeyCode) || m_ReloadAfterTime > 0f && Time.timeSinceLevelLoad > m_ReloadTime)
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}