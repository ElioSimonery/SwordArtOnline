using UnityEngine;
using System.Collections;

// <<View Controller>>
public class MiniHud : MonoBehaviour {
	/* Unity Interface */
	public bool hasPointLabel = false;
	public bool isDynamicView = true;
	public GameObject hudObject = null;
	public UILabel hpLabel = null;
	public UILabel mpLabel = null;
	public GameObject hpGauge;
	public GameObject mpGauge;
	public float appearanceTime = 3.0f;
	protected bool isReveal;

	private Vector3 maxGauge;
	private Vector3 gaugeRate;
	private SpriteRenderer[] sprites;
	private IEnumerator hpEffect;
	private IEnumerator mpEffect;
	private Vector3 prev_label_scale;
	protected float appear_timer = 0.0f;

	// Use this for initialization
	void Start () {
		isReveal = true;
		gaugeRate = hpGauge.transform.localScale;
		maxGauge = hpGauge.transform.localScale;
		if(hasPointLabel)
		{
			prev_label_scale = hpLabel.gameObject.transform.localScale;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isDynamicView)
						return;
		if (isReveal) 
		{
			appear_timer += Time.deltaTime;
			setAlpha(1.0f-appear_timer/appearanceTime);

			if(appear_timer >= appearanceTime)
			{
				setAlpha(0.0f);
				gameObject.SetActive(false);
				isReveal = false;
				appear_timer = 0;	
			}
		}
	}

	public void setPoint(bool isHP, int point)
	{
		if (!hasPointLabel)
		{
			Debug.LogError("[MiniHud] hasPointLabel is false. check your MiniHud.");
			return;
		}
		if(isHP)
			hpLabel.text = point.ToString();
		else
			mpLabel.text = point.ToString();
	}
	
	// [0 ~ 1.0f] of rate of the gauge length.
	public void setGauge(bool isHP, float rate)
	{
		reveal ();
		if(rate < 0) rate = 0;
		if(rate > 1) rate = 1;
		gaugeRate.x = maxGauge.x*rate;
		if (isHP)
			hpGauge.transform.localScale = gaugeRate;
		else
			mpGauge.transform.localScale = gaugeRate;
	}

	public void setData(bool isHP, int point, float rate)
	{
		setGauge (isHP, rate);
		if (!hasPointLabel) return;

		setPoint (isHP, point);
		if(isHP)
		{
			if (hpEffect != null)
				StopCoroutine (hpEffect);
			StartCoroutine (hpEffect = ScaleEffect (hpLabel, 2f - rate));
		}
		else
		{
			if (mpEffect != null)
				StopCoroutine (mpEffect);
			StartCoroutine (mpEffect = ScaleEffect (mpLabel));
		}
	}

	protected void setAlpha(float rate)
	{
		sprites = hudObject.GetComponentsInChildren<SpriteRenderer> ();
		foreach(SpriteRenderer s in sprites)
			s.color = new Color(s.color.r, s.color.g, s.color.b, rate);
	}

	protected void reveal()
	{
		if (!isDynamicView)
						return;

		hudObject.SetActive(true);
		sprites = hudObject.GetComponentsInChildren<SpriteRenderer> ();
		foreach(SpriteRenderer s in sprites)
			s.color = new Color(s.color.r, s.color.g, s.color.b, 1.0f);

		appear_timer = 0.0f;
		isReveal = true;
	}

	protected IEnumerator ScaleEffect(UILabel label, float rate = 1.5f)
	{		
		// Shrink 2% per 0.02 sec
		for(float r = rate; r > 1f; r -= 0.05f)
		{
			label.transform.localScale = prev_label_scale * r;
			yield return new WaitForSeconds(0.02f);
		}
		
		// Recovery
		label.transform.localScale = prev_label_scale;
		yield return new WaitForSeconds (0.02f);
	}
}
