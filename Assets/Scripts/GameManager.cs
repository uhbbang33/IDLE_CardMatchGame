using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Profiling;
using UnityEngine.Events;
using System.Threading;
using TMPro;
using System.Drawing;

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
    static public int level;
    public GameObject endPanel;

    private RectTransform transaddtxt;
    private Button button;
    byte c;
    public int check;
    private int tempSpriteNum;
    private Sprite tempSprite;

    float time;
    public float maxTime;
    public float warningTime;
    public bool isRunning = false;
    bool isShuffle;
    public AudioManager audioManager;

    public AudioClip match;
    public AudioClip failed;
    public AudioClip bestscoreSound;
    public AudioClip lowscoreSound;  
    public AudioSource audioSource;

    public Sprite[] sprites;    // sprite를 Inspector창에서 받기 위한 선언
    private List<GameObject> cardList;  // card들을 담을 cardList, 현재는 card를 섞는데 사용
    public List<GameObject> namelist;
    public List<Vector3> points;

    public GameObject warningBackground;
    //enum TeamName { 문원정, 조병웅, 김국민, 김종욱, 김희진, 박준형}

    private void Awake()
    {
        I = this;
        
    }

    void Start()
    {
        isShuffle = false;
        tempSprite = sprites[0];
        tempSpriteNum = 0;

        transaddtxt = addTxt.GetComponent<RectTransform>();
        Time.timeScale = 1.0f;
        cardList = new List<GameObject>();
        //namelist = new List<GameObject>();

        // 12개의 카드 생성
        // 카드 sprite를 순차적으로 넣어줌
        if (level == 0)
        {
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
                newCard.transform.parent.localScale = new Vector3(0.7f, 0.7f, 1f);

                float x = (i / 3) * 1.4f - 2.1f;
                float y = (i % 3) * 1.4f - 3.0f;
                newCard.transform.position = new Vector3(x, y, 0);

                newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = tempSprite;
                newCard.GetComponent<Card>().spriteNum = tempSpriteNum; // card에 spriteNum 넣어주기
                cardList.Add(newCard);  // List에 생성한 카드 넣어주기
            }
        }
        else
        {
            for (int i = 0; i < 24; ++i)
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
                newCard.transform.parent.localScale = new Vector3(0.7f, 0.7f, 1f);
                newCard.transform.position = new Vector3(0, -1.5f, 1);

                float x = (i / 6) * 1.4f - 2.1f;
                float y = (i % 6) * 1.1f - 4.2f;
                points.Add(new Vector3(x, y, 1));
                

                newCard.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = tempSprite;
                newCard.GetComponent<Card>().spriteNum = tempSpriteNum; // card에 spriteNum 넣어주기
                cardList.Add(newCard);  // List에 생성한 카드 넣어주기
            }
        }

        // 카드 섞기
        for (int i = 0; i < cardList.Count; i++)
        {
            int randomNum = Random.Range(0, cardList.Count);
            Debug.Log(points);
            // swap
/*            Vector3 tempPosition = points[i];
            points[i] = points[randomNum];
            points[randomNum] = tempPosition;*/
        }
    }

    void Update()
    {
        float addy = transaddtxt.anchoredPosition.y;         // addtxt 위치
        addy += 0.5f;                                        // addtxt y값 상승
        transaddtxt.anchoredPosition = new Vector2(0, addy); // addtxt y값 상승
        Vector3 speed = Vector3.zero;
        
        if (!isShuffle)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (cardList[i])
                {
                    cardList[i].transform.position =
                    //Vector3.MoveTowards(cardList[i].transform.position, points[i], 0.01f);
                    //Vector3.SmoothDamp(cardList[i].transform.position, points[i], ref speed, 0.05f);
                    Vector3.Lerp(cardList[i].transform.position, points[i], 0.05f);
                    //Vector3.Slerp(cardList[i].transform.position, points[i], 0.01f);
                }
            }
        }
        if (time > 2)
        {
            isShuffle = true;
        }

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
    }

    public void IsMatched()
    {
        int firstCardSpriteNum = firstCard.GetComponent<Card>().spriteNum;
        int secondCardSpriteNum = secondCard.GetComponent<Card>().spriteNum;

        if(firstCardSpriteNum == secondCardSpriteNum)
        {
            audioSource.PlayOneShot(match);

            string info = firstCard.GetComponentInChildren<SpriteRenderer>().sprite.name;   // sprite의 이름 rtanx info에 저장
            check = int.Parse(info.Substring(info.Length - 1)) -1;  // rtanx 의 x부분 자르기, int 로 변형
            // 배열은 0부터 시작하므로 -1
            // check = firstCard.GetComponent<Card>().spriteNum;
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
        warningBackground.gameObject.SetActive(false);
        Time.timeScale = 0f;
        isRunning = false;
        endPanel.SetActive(true);

        if (time > maxTime)
            time = maxTime;

        thisScoreText.text = time.ToString("N2");

        //endTxt.SetActive(true);
        if (time >= maxTime)
        {
            audioSource.PlayOneShot(lowscoreSound);
        }
        else if (PlayerPrefs.HasKey("bestscore") == false)
        {
            // 게임종료시 베스트 스코어면 나오는 노래
            audioSource.PlayOneShot(bestscoreSound);
            PlayerPrefs.SetFloat("bestscore", time);
        }
        else if (time < PlayerPrefs.GetFloat("bestscore"))
        {
            // 게임종료시 베스트 스코어보다 낮으면 나오는 노래
            audioSource.PlayOneShot(bestscoreSound);
            PlayerPrefs.SetFloat("bestscore", time);
        }
        else
        {
            // 게임종료시 베스트 스코어보다 낮으면 나오는 노래
            audioSource.PlayOneShot(lowscoreSound);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoHomeBtn()
    {
        SceneManager.LoadScene("StartScene");
    }
}
