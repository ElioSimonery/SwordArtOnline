using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeechController : MonoBehaviour {
	// 싱글톤 객체로 핸들. 
	// 기능 : 이름라벨 getset, 내용 getset, 보이게 true false, 타이핑 효과 true false, 타이핑 즉시 보이기
	// 큐에 데이터 저장하여 다중 효과까지 핸들링하자.
	// 리팩토링 시 엑셀로 데이터 관리하기.
	public GameObject NameLabelObj;
	public GameObject ContentLabelObj;
	public GameObject PortraitObj;
	public GameObject skipObj;

	public bool noSpeech = true;
	public bool useEffect;
	public float typeEffectInterval;
	private List<Node> papers;
	private int cursor;

	private string nameLabel;
	private string contentLabel;
	private float effectTimer;

	private string dynamicContent;
	private int dynamicContentSize;
	
	private static SpeechController instanceObj;

	public struct Node
	{
		public enum n_type{ NORMAL, NARRATION, MESSAGE }
		public n_type mType;
		public string mName;
		public string mContent;
		public UIAtlas mAtlas;
		public string mSpriteName;

		public UIAtlas mBackgroundAtlas;
		public string mBackgroundSpriteName;

		public Node(string Name, string Content)
		{
			mType = n_type.NORMAL;
			mName = Name;
			mContent = Content;
			mAtlas = null;
			mSpriteName = null;
			mBackgroundAtlas = null;
			mBackgroundSpriteName = null;
		}

		public Node(string Name, string Content, UIAtlas atlas, string spriteName)
		{
			mType = n_type.NORMAL;
			mName = Name;
			mContent = Content;
			mAtlas = atlas;
			mSpriteName = spriteName;
			mBackgroundAtlas = null;
			mBackgroundSpriteName = null;
		}

		public Node(string Name, string Content, n_type t)
		{
			mType = t;
			mName = Name;
			mContent = Content;
			mAtlas = null;
			mSpriteName = null;
			mBackgroundAtlas = null;
			mBackgroundSpriteName = null;
		}

		public Node(n_type t, string Name, string Content, UIAtlas port_atlas, string port_spriteName, UIAtlas back_atlas, string back_spriteName)
		{
			mType = t;
			mName = Name;
			mContent = Content;
			mAtlas = port_atlas;
			mSpriteName = port_spriteName;

			mBackgroundAtlas = back_atlas;
			mBackgroundSpriteName = back_spriteName;
		}
	}

	protected string Name 
	{
		get {return nameLabel;}
		set
		{ 
			nameLabel = value;
			NameLabelObj.GetComponent<UILabel>().text = nameLabel;
		}
	}

	protected string Content
	{ 
		get{return contentLabel;}
		set
		{
			contentLabel = value;
			ContentLabelObj.GetComponent<UILabel>().text = contentLabel;
		}
	} 

	// Use this for initialization
	void Start () {
		if(instanceObj != null)
		{
			Debug.LogWarning("Speech GameObject has already created. check this GameObject.");
		}
		papers = new List<Node> ();
		gameObject.SetActive (false);
		dynamicContent = "";
	
		instanceObj = gameObject.GetComponent<SpeechController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (papers.Count == 0)
			return;

		if(useEffect)
		{
			if(effectTimer >= typeEffectInterval)
			{
				// typing sound 
				effectTimer = 0;

				if(dynamicContent.Length >= papers[cursor].mContent.Length)
				{
					int last = dynamicContent.LastIndexOf("_");
					if(last >= 0)
						dynamicContent = dynamicContent.Remove(last);
					else
						dynamicContent += "_";
				}
				else
				{
					if(papers[cursor].mType == Node.n_type.MESSAGE)
						GameUIPanelManager.GetInstance().animActivation(false);

					if(papers[cursor].mType == Node.n_type.NARRATION)
					{
						PortraitObj.SetActive(false);
						NameLabelObj.SetActive(false);
					}
					else
					{
						PortraitObj.SetActive(true);
						NameLabelObj.SetActive(true);
						if(papers[cursor].mAtlas != null)
							PortraitObj.GetComponent<UISprite>().atlas = papers[cursor].mAtlas;
						if(papers[cursor].mSpriteName != null)
							PortraitObj.GetComponent<UISprite>().spriteName = papers[cursor].mSpriteName;

						if(papers[cursor].mBackgroundAtlas != null)
						{
							GameUIPanelManager.GetInstance().setUIAnimBackground(
								papers[cursor].mBackgroundAtlas,
								papers[cursor].mBackgroundSpriteName,
								true);
							GameUIPanelManager.GetInstance().animActivation(true);
						}
					}
					dynamicContent += papers[cursor].mContent[dynamicContentSize];
					dynamicContentSize++;

					GetComponent<AudioSource> ().Play ();

					if(!string.IsNullOrEmpty(papers[cursor].mName))
						Name = papers[cursor].mName;
				}
				Content = dynamicContent;
			}
			effectTimer += Time.deltaTime;
		}
	}

	public static SpeechController GetInstance()
	{
		if (instanceObj)
		{
			return instanceObj;
		}
		else
		{
			Debug.LogError("Speech GameObject is null.");
			return null;
		}
	}

	private void shrinkWidthToFit()
	{
		ContentLabelObj.GetComponent<UILabel> ().lineWidth = Screen.width;
	}

	public void skipAvailable(bool isAvail)
	{
		if(isAvail)
			skipObj.SetActive(true);
		else
			skipObj.SetActive(false);
	}

	public void pushScript(List<Node> script)
	{
		gameObject.SetActive (true);
		foreach(Node n in script)
			papers.Add(n);
		noSpeech = false;
	}

	public void pushPrint(string name, string content)
	{
		gameObject.SetActive (true);

		Node n = new Node (name, content);

		papers.Add (n);
		noSpeech = false;
	}

	public void pushPrint(string name, string content, UIAtlas atlas, string spriteName)
	{
		gameObject.SetActive (true);
		Node n = new Node (name, content, atlas, spriteName);
		papers.Add (n);
		noSpeech = false;
	}
	
	public void pushPrint(string name, string[] content, UIAtlas atlas, string spriteName)
	{
		gameObject.SetActive (true);
		foreach (string cont in content) 
		{
			Node n = new Node (name, cont, atlas, spriteName);
			papers.Add (n);
		}
		noSpeech = false;
	}

	public void pushPrint(string name, string content, 
	                      UIAtlas port_atlas, string port_sprName, 
	                      UIAtlas back_atlas, string back_sprName)
	{
		gameObject.SetActive (true);
		Node n = new Node (Node.n_type.NORMAL, name, content, port_atlas, port_sprName, back_atlas, back_sprName);
		papers.Add (n);
		noSpeech = false;
	}

	public void pushPrints(int size, string[] name, string[] content, UIAtlas[] atlas, string[] spriteName)
	{
		gameObject.SetActive (true);
		for (int i = 0; i < size; i++) 
		{
			UIAtlas atl = (atlas == null) ? null : atlas[i];
			string spr_name = (spriteName == null) ? null : spriteName[i];
			Node n = new Node(name[i], content[i], atl, spr_name);
			papers.Add(n);
		}
		noSpeech = false;
	}

	public void pushPrints(int size, string[,] scripts, UIAtlas[] atlas, string[] spriteName)
	{
		gameObject.SetActive (true);
		for (int i = 0; i < size; i++) 
		{
			UIAtlas atl = (atlas == null) ? null : atlas[i];
			string spr_name = (spriteName == null) ? null : spriteName[i];
			Node n = new Node(scripts[i,0], scripts[i,1], atl, spr_name);
			papers.Add(n);
		}
		noSpeech = false;
	}


	public void pushNarration(string content)
	{
		gameObject.SetActive (true);
		Node n = new Node ("", content, Node.n_type.NARRATION);
		papers.Add (n);
		noSpeech = false;
	}

	public void pushNarration(string[] contents)
	{
		gameObject.SetActive (true);
		foreach (string cont in contents) 
		{
			Node n = new Node ("", cont, Node.n_type.NARRATION);
			papers.Add (n);
		}
		noSpeech = false;
	}

	public void clear(bool initialize)
	{
		if(initialize)
		{
			papers.Clear ();
			cursor = 0;
			effectTimer = 0;
			Name = "";
		}
		dynamicContent = "";
		Content = "";
		dynamicContentSize = 0;

	}

	public void skip()
	{
		clear (true);
		noSpeech = true;
		GameUIPanelManager.GetInstance().animActivation(false);
		gameObject.SetActive (false);
	}

	public void OnTouchedNext()
	{
		if (FileScriptReader.loading) 
			return;

		shrinkWidthToFit ();
		if(papers[cursor].mContent.Length <= dynamicContent.Length) //next
		{
			cursor++;
			clear (false);
			if(papers.Count == cursor)
			{
				// finish.
				noSpeech = true;
				if(GameUIPanelManager.GetInstance().isAnimActivated())
					GameUIPanelManager.GetInstance().animActivation(false);
				gameObject.SetActive(false);
			}
		}
		else
		{
			// force to flush to dynamicContent
			dynamicContent = papers[cursor].mContent;
		}
	}
}
