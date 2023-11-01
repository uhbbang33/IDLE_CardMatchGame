using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Profiling;
using UnityEngine.Events;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public GameObject card;
    public GameObject firstCard;
    public GameObject secondCard;

    public Text timeTxt;
    public GameObject endTxt;
    public Text addTxt;
    public Text maxScoreText;
    public Text thisScoreText;

    public GameObject endPanel;

    private RectTransform transaddtxt;
    private Button button;
    byte c;
    public int check;

    float time;
    public float maxTime;
    public float warningTime;
    bool isRunning = true;
    int trialNum = 0;                          // 매칭 시도 횟수 n회의 자리 만들어줌
    public Text trialText;                 // 매칭 시도 횟수를 텍스트로 만들어주기 위한 자리를 만들어줌
    public int trialLeft;
    public Text trialLeftText;


    public AudioManager audioManager;

    public AudioClip match;
    public AudioClip failed;
    public AudioClip bestscore;
    public AudioClip lowscore;
    public AudioSource audioSource;

    public Sprite[] sprites;    // sprite를 Inspector창에서 받기 위한 선언
    List<GameObject> cardList;  // card들을 담을 cardList, 현재는 card를 섞는데 사용
    public List<GameObject> namelist;

    public GameObject warningBackground;
    //enum TeamName { 문원정, 조병웅, 김국민, 김종욱, 김희진, 박준형}

    private void Awake()
    {
        I = this;
    }

    void Start()
    {

        transaddtxt = addTxt.GetComponent<RectTransform>();
        Time.timeScale = 1.0f;

        cardList = new List<GameObject>();
        //namelist = new List<GameObject>();
        Sprite tempSprite = sprites[0];
        int tempSpriteNum = 0;

        // 12개의 카드 생성
        // 카드 sprite를 순차적으로 넣어줌
        for (int i = 0; i < 12; ++i)
        {
            // 카드는 12개 생성되어야 하는데 sprite는 6개
            // 2개의 카드는 같은 카드여야 하므로
            if (i % 2 == 0)     // 0, 2, 4, 6, 8, 10 일때만 sprite가 바뀜
            {
                tempSprite = sprites[i / 2];
                tempSpriteNum = i / 2;
            }

            GameObject newCard = Instantiate(card);
            newCard.transform.parent = GameObject.Find("Cards").transform;

            float x = (i / 3) * 1.4f - 2.1f;
            float y = (i % 3) * 1.4f - 3.0f;
            newCard.transform.position = new Vector3(x, y, 0);


            newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = tempSprite;
            newCard.GetComponent<Card>().spriteNum = tempSpriteNum; // card에 spriteNum 넣어주기
            cardList.Add(newCard);  // List에 생성한 카드 넣어주기
        }

        // 카드 섞기
        for (int i = cardList.Count - 1; i > 0; --i)
        {
            int randomNum = Random.Range(0, i);

            // swap
            Vector3 tempPosition = cardList[i].transform.position;
            cardList[i].transform.position = cardList[randomNum].transform.position;
            cardList[randomNum].transform.position = tempPosition;
        }
    }

    void Update()
    {
        float addy = transaddtxt.anchoredPosition.y;         // addtxt 위치
        addy += 0.5f;                                        // addtxt y값 상승
        transaddtxt.anchoredPosition = new Vector2(0, addy); // addtxt y값 상승

        c -= 1;                                              // 글자 색상 투명하게
        addTxt.color = new Color32(255, 0, 0, c);            // 글자 색상 투명하게

        time += Time.deltaTime;

        if (time > warningTime)
        {
            warningBackground.gameObject.SetActive(true);
            audioManager.GetComponent<AudioSource>().pitch = 1.5f;
        }

        if (time > maxTime)
            GameEnd();

        timeTxt.text = time.ToString("N2");

        if (SceneManager.GetActiveScene().name == "MainScene3_Woong")
        {
            if (trialLeft == trialNum)
            {
                GameEnd();
                time = maxTime;
            }
        }
    }

    public void IsMatched()
    {
        trialNum++;                 // IsMatched가 실행될 때, trial Num에 1 추가
        if (SceneManager.GetActiveScene().name == "MainScene3_Woong")
        {
            trialLeftText.text = (trialLeft - trialNum).ToString() + "회";  // 변화 있을 때마다 업데이트
            if (trialLeft - trialNum < 10)
            {
                trialLeftText.color = new Color32(255, 0, 0, 255);          // 남은 기회가 10회 미만일 때 빨간색으로 바뀜
            }
        }
        int firstCardSpriteNum = firstCard.GetComponent<Card>().spriteNum;
        int secondCardSpriteNum = secondCard.GetComponent<Card>().spriteNum;

        if (firstCardSpriteNum == secondCardSpriteNum)
        {
            audioSource.PlayOneShot(match);

            string info = firstCard.GetComponentInChildren<SpriteRenderer>().sprite.name;   // sprite의 이름 rtanx info에 저장
            check = int.Parse(info.Substring(info.Length - 1)) - 1;  // rtanx 의 x부분 자르기, int 로 변형
            // 배열은 0부터 시작하므로 -1

            //check = firstCard.GetComponent<Card>().spriteNum;

            namelist[check].SetActive(true);            // Active True
            StartCoroutine(nActiveFalse(check));


            firstCard.GetComponent<Card>().DestrotyCard();
            secondCard.GetComponent<Card>().DestrotyCard();

            int cardsLeft = GameObject.Find("Cards").transform.childCount;
            if (cardsLeft == 2)
                Invoke("GameEnd", 0.5f);
        }
        else
        {
            audioSource.PlayOneShot(failed);

            firstCard.GetComponent<Card>().CloseCard();
            secondCard.GetComponent<Card>().CloseCard();

            // 시간 추가 기능
            time += 5;
            addTxt.color = new Color32(255, 0, 0, 255);             // 글자색 RED
            c = 0;                                                  // 투명도 초기화
            transaddtxt.anchoredPosition = new Vector2(0, 450);     // 글자 위치 초기화 (진행시간 위)
            addTxt.gameObject.SetActive(true);                      // addTXT 활성화
            Invoke("ActiveFalse", 1.0f);                            // 1초 후 ActiveFalse 실행
        }

        firstCard = null;
        secondCard = null;
    }

    void ActiveFalse()
    {
        addTxt.gameObject.SetActive(false);                          // addtxt 비활성화 하기
    }

    IEnumerator nActiveFalse(int check)
    {
        yield return new WaitForSeconds(1.0f);
        namelist[check].SetActive(false);
    }

    void GameEnd()
    {
        trialText.text = trialNum.ToString() + "회";      // trialText를 업데이트
        warningBackground.gameObject.SetActive(false);
        Time.timeScale = 0f;
        isRunning = false;
        endPanel.SetActive(true);

        if (time > maxTime)         // 남은 횟수가 0이면 클리어를 못한 것이므로!
            time = maxTime;

        thisScoreText.text = time.ToString("N2");

        //endTxt.SetActive(true);
        if (time >= maxTime)
        {
            audioSource.PlayOneShot(lowscore);
        }
        else if (PlayerPrefs.HasKey("bestscore") == false)
        {
            // 게임종료시 베스트 스코어면 나오는 노래
            audioSource.PlayOneShot(bestscore);
            PlayerPrefs.SetFloat("bestscore", time);
        }
        else if (time < PlayerPrefs.GetFloat("bestscore"))
        {
            // 게임종료시 베스트 스코어보다 낮으면 나오는 노래
            audioSource.PlayOneShot(bestscore);
            PlayerPrefs.SetFloat("bestscore", time);
        }
        else
        {
            // 게임종료시 베스트 스코어보다 낮으면 나오는 노래
            audioSource.PlayOneShot(lowscore);
        }

        float maxScore = PlayerPrefs.GetFloat("bestscore");
        maxScoreText.text = maxScore.ToString("N2");
        EndGameBgmStop();
    }

    void EndGameBgmStop()
    {
        if (audioManager != null && audioManager.audioSource != null)
        {
            audioManager.audioSource.Stop();
        }
    }

    public void RetryGame()
    {
        // SceneManager.LoadScene("GameScene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void GoHomeBtn()
    {
        SceneManager.LoadScene("StartScene");
    }
}
