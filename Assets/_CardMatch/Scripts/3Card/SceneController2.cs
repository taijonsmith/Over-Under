using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class SceneController2 : MonoBehaviour {
	public const int gridRows = 3;
	public const int gridCols = 4;
	public const float offsetX = 2f;
	public const float offsetY = 2f;

	[SerializeField] private MemoryCard2 originalCard;
	[SerializeField] private Sprite[] images;
	[SerializeField] private TextMesh scoreLabel;
    private int _score = 0;
    private Stopwatch timer = new Stopwatch();
    private bool start_timer = true;
	private MemoryCard2 _firstRevealed;
	private MemoryCard2 _secondRevealed;
    private MemoryCard2 _thirdRevealed;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.RESTART, Restart);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.RESTART, Restart);
    }
    
    public bool canReveal {
		get {return (_secondRevealed == null || _thirdRevealed == null); }
    }

	// Use this for initialization
	void Start() {
		Vector3 startPos = originalCard.transform.position;        
        // create shuffled list of cards
        int[] numbers = {0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3};
		numbers = ShuffleArray(numbers);

		// place cards in a grid
		for (int i = 0; i < gridCols; i++) {
			for (int j = 0; j < gridRows; j++) {
				MemoryCard2 card;

				// use the original for the first grid space
				if (i == 0 && j == 0) {
					card = originalCard;
				} else {
					card = Instantiate(originalCard) as MemoryCard2;
				}

				// next card in the list for each grid space
				int index = j * gridCols + i;
				int id = numbers[index];
				card.SetCard(id, images[id]);

				float posX = (offsetX * i) + startPos.x;
				float posY = -(offsetY * j) + startPos.y;
				card.transform.position = new Vector3(posX, posY, startPos.z);
			}
		}
	}

	// Knuth shuffle algorithm
	private int[] ShuffleArray(int[] numbers) {
		int[] newArray = numbers.Clone() as int[];
		for (int i = 0; i < newArray.Length; i++ ) {
			int tmp = newArray[i];
			int r = Random.Range(i, newArray.Length);
			newArray[i] = newArray[r];
			newArray[r] = tmp;
		}
		return newArray;
	}

	public void CardRevealed(MemoryCard2 card) {
		if (_firstRevealed == null) {
            if (start_timer)
            {
                timer.Start();
                start_timer = false;
            }
			_firstRevealed = card;
		} else if (_secondRevealed == null){
            _secondRevealed = card;
			//StartCoroutine(CheckMatch());
		} else {
            _thirdRevealed = card;
            StartCoroutine(CheckMatch());
        }
	}
	
	private IEnumerator CheckMatch() {

		// increment score if the cards match
		if (_firstRevealed.id == _secondRevealed.id && _secondRevealed.id == _thirdRevealed.id)
        {
            _score++;
            if (_score == 4) { timer.Stop(); }          
            yield return new WaitForSeconds(.001f);
        } else {// otherwise turn them back over after .2s pause
            yield return new WaitForSeconds(.2f);
            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
            _thirdRevealed.Unreveal();
        }
        _firstRevealed = null;
        _secondRevealed = null;
        _thirdRevealed = null;
    }

    void Update()
    {
        if (_score != 4) { scoreLabel.text = "Time: " + timer.Elapsed.Seconds + "." + Mathf.Floor(timer.Elapsed.Milliseconds/10); }
    }

    public void Restart() {
		Application.LoadLevel("3CardMatch");
        _score = 0;
    }
}
