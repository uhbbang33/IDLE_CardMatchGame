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
    public GameObject endPanel;
    
    public GameObject endTxt;
    public Text timeTxt;
    public Text addTxt;
    public Text maxScoreText;
    public Text thisScoreText;
    
    private RectTransform transaddtxt;
    private Button button;
    public int check;
    private int tempSpriteNum;
    private Sprite tempSprite;
    byte c;

    float time;
    public float maxTime;
    public float warningTime;
    bool isShuffle;

    public bool isRunning = true;
    int trialNum = 0;                          // 매칭 시도 횟수 n회의 자리 만들어줌
    public Text trialText;                 // 매칭 시도 횟수를 텍스트로 만들어주기 위한 자리를 만들어줌
    public int trialLeft;
    public Text trialLeftText;

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
    public GameObject pauseUI;
    //enum TeamName { 문원정, 조병웅, 김국민, 김종욱, 김희진, 박준형}

    Scene scene;

    private void Awake()
    {
        I = this;
        scene = SceneManager.GetActiveScene();
        isRunning = true;

    }

    void Start()
    {
        isShuffle = false;
        tempSprite = sprites[0];
        tempSpriteNum = 0;
        //PlayerPrefs.DeleteAll();

        transaddtxt = addTxt.GetComponent<RectTransform>();
        Time.timeScale = 1.0f;
        cardList = new List<GameObject>();
        //namelist = new List<GameObject>();

        // 12개의 카드 생성
        // 카드 sprite를 순차적으로 넣어줌
        if (scene.name == "MainScene1")
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

                float x = (i / 3) * 1.4f - 2.1f;
                float y = (i % 3) * 1.4f - 3.0f;
                newCard.transform.position = new Vector3(x, y, 1);
                points.Add(new Vector3(x, y, 1));

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
                newCard.transform.parent.localScale = new Vector3(0.8f, 0.8f, 1f);
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
            // swap
            Vector3 tempPosition = points[i];
            points[i] = points[randomNum];
            points[randomNum] = tempPosition;
        }

    }

    void Update()
    {
        if (isRunning)
        {
            float addy = transaddtxt.anchoredPosition.y;         // addtxt 위치
            addy += 0.5f;                                        // addtxt y값 상승
            transaddtxt.anchoredPosition = new Vector2(0, addy); // addtxt y값 상승

            Vector3 speed = Vector3.zero;
            if (scene.name!="MainScene1")
            {
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
            }

            if (time > 2)
            {
                isShuffle = true;
            }

            c -= 3;                                              
            addTxt.color = new Color32(255, 0, 0, c);            // 글자 색상 투명하게

            time += Time.deltaTime;
            if (time > maxTime)
                GameEnd();

            if (time > warningTime)
            {
                warningBackground.gameObject.SetActive(true);
                audioManager.GetComponent<AudioSource>().pitch = 1.5f;
            }
            timeTxt.text = time.ToString("N2");

            if (SceneManager.GetActiveScene().name == "MainScene3")
            {
                if (trialLeft == trialNum)
                {
                    GameEnd();
                }
            }
        }
    }

    public void IsMatched()
    {
        if (isRunning)
            trialNum++;                 // IsMatched가 실행될 때, trial Num에 1 추가
        if (SceneManager.GetActiveScene().name == "MainScene3")
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

            namelist[check].SetActive(false);
            check = int.Parse(info.Substring(info.Length - 1)) - 1;  // rtanx 의 x부분 자르기, int 로 변형
                                                                     // 배열은 0부터 시작하므로 -1
            
            namelist[check].SetActive(true);            // Active True
            StartCoroutine(nActiveFalse(check));

            Destroy(firstCard);
            Destroy(secondCard);
            //firstCard.GetComponent<Card>().DestrotyCard();
            //secondCard.GetComponent<Card>().DestrotyCard();

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
            Invoke("ActiveFalse", 0.3f);                            // 1초 후 ActiveFalse 실행
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

    /* 게임 종료 시 일어나는 함수 */
    void GameEnd()
    {
        string sceneName = scene.name;
        isRunning = false;
        trialText.text = trialNum.ToString() + "회";      // trialText를 업데이트
        warningBackground.gameObject.SetActive(false);
        Time.timeScale = 0f;

        endPanel.SetActive(true);

        thisScoreText.text = time.ToString("N2");

        if (time > maxTime || trialNum == trialLeft)         // 실패
        {
            thisScoreText.text = "Failed";
            time = maxTime;
        }
       
        /*하드 게임 플레이시 최고점수 및 현재점수 기록*/
        if (sceneName == "MainScene2")
        {
            GameHardScore();
        }
        /*헬 게임 플레이시 최고점수 및 현재점수 기록*/
        else if (sceneName == "MainScene3")
        {
            GameHellScore();
        }
        else
        {
            GameNormalScore();
        }
        //endTxt.SetActive(true);

    }

    /*노말 게임 최고점수 현재점수 기록*/
    void GameNormalScore()
    {
        string sceneName = scene.name;
        string bestscore = "normalscore";

        if (sceneName == "MainScene1")
        {
            //endTxt.SetActive(true);
            if (time >= maxTime)
            {
                audioSource.PlayOneShot(lowscoreSound);
            }
            else if (PlayerPrefs.HasKey(bestscore) == false)
            {
                // 게임종료시 베스트 스코어면 나오는 노래
                audioSource.PlayOneShot(bestscoreSound);
                PlayerPrefs.SetFloat(bestscore, time);
            }
            else if (time < PlayerPrefs.GetFloat(bestscore))
            {
                // 게임종료시 베스트 스코어보다 좋은 경우 나오는 노래
                audioSource.PlayOneShot(bestscoreSound);
                PlayerPrefs.SetFloat(bestscore, time);
            }
            else
            {
                // 게임종료시 베스트 스코어보다 좋지 않은 경우 나오는 노래
                audioSource.PlayOneShot(lowscoreSound);
            }

            float maxScore = PlayerPrefs.GetFloat(bestscore);
            maxScoreText.text = maxScore.ToString("N2");
            EndGameBgmStop();
        }
    }

    /*하드 게임 최고점수 현재점수 기록*/
    void GameHardScore()
    {
        string sceneName = scene.name;
        string bestscore = "hardscore";

        if (sceneName == "MainScene2")
        {
            //endTxt.SetActive(true);
            if (time >= maxTime)
            {
                audioSource.PlayOneShot(lowscoreSound);
            }
            else if (PlayerPrefs.HasKey(bestscore) == false)
            {
                // 게임종료시 베스트 스코어면 나오는 노래
                audioSource.PlayOneShot(bestscoreSound);
                PlayerPrefs.SetFloat(bestscore, time);
            }
            else if (time < PlayerPrefs.GetFloat(bestscore))
            {
                // 게임종료시 베스트 스코어보다 좋은 경우 나오는 노래
                audioSource.PlayOneShot(bestscoreSound);
                PlayerPrefs.SetFloat(bestscore, time);
            }
            else
            {
                // 게임종료시 베스트 스코어보다 좋지 않은 경우 나오는 노래
                audioSource.PlayOneShot(lowscoreSound);
            }

            float maxScore = PlayerPrefs.GetFloat(bestscore);
            maxScoreText.text = maxScore.ToString("N2");
            EndGameBgmStop();
        }
    }

    /*헬 게임 최고점수 현재점수 기록*/
    void GameHellScore()
    {
        string sceneName = scene.name;
        string bestscore = "hellscore";

        if (sceneName == "MainScene3")
        {
            //endTxt.SetActive(true);
            if (time >= maxTime)
            {
                audioSource.PlayOneShot(lowscoreSound);
            }
            else if (PlayerPrefs.HasKey(bestscore) == false)
            {
                // 게임종료시 베스트 스코어면 나오는 노래
                audioSource.PlayOneShot(bestscoreSound);
                PlayerPrefs.SetFloat(bestscore, time);
            }
            else if (time < PlayerPrefs.GetFloat(bestscore))
            {
                // 게임종료시 베스트 스코어보다 좋은 경우 나오는 노래
                audioSource.PlayOneShot(bestscoreSound);
                PlayerPrefs.SetFloat(bestscore, time);
            }
            else
            {
                // 게임종료시 베스트 스코어보다 좋지 않은 경우 나오는 노래
                audioSource.PlayOneShot(lowscoreSound);
            }

            float maxScore = PlayerPrefs.GetFloat(bestscore);
            maxScoreText.text = maxScore.ToString("N2");
            EndGameBgmStop();
        }
    }

    /* 게임 종료시 bgm 멈추는 함수*/
    void EndGameBgmStop()
    {
        if (audioManager != null && audioManager.audioSource != null)
        {
            audioManager.audioSource.Stop();
        }
    }

    /* 게임 다시시작 함수 */
    public void RetryGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(scene.name);
    }

    /* 홈으로 돌아가기 함수 */
    public void GoHomeBtn()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("StartScene");
    }

    public void ContinueGame()
    {
        Time.timeScale = 1.0f;
        pauseUI.SetActive(false);
        audioManager.audioSource.Play();
    }

    public void PauseBtn()
    {
        Time.timeScale = 0.0f;
        pauseUI.SetActive(true);
        audioManager.audioSource.Pause();
    }
}

