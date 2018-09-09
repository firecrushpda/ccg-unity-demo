using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainLoop : MonoBehaviour {
    // Use this for initialization
    bool m_aswitch;
    Player currentplayer;
    GameObject signal;
    bool[] b_signal = new bool[4];
    List<Player> players = new List<Player>();
    List<Player> enemies = new List<Player>();

    public Text showtag;
    public GameObject canvas;
    public float maxtime = 5;
    public GameObject btn;
    public GameObject scrollbar;


    void Start () {
        m_aswitch = true;
        currentplayer = new Player();
        InstantiatePlayer();
        //StartCoroutine("mainloop");
    }

    void InstantiatePlayer()
    {
        for (int i = 0; i < 4; i++)
        {
            Player player = new Player();
            player.m_playerforce = PlayerForce.player;
            var instance = Resources.Load<GameObject>("Prefabs/instance");
            var signalins = Resources.Load<GameObject>("Prefabs/SideSignalInstance");
            player.m_instance = Instantiate(instance,canvas.transform.Find("Players/" + i));
            player.m_sidesignal = Instantiate(signalins, scrollbar.transform);
            player.m_sidesignal.GetComponentInChildren<Text>().text = (i + 1).ToString();
            player.m_instance.transform.localPosition = Vector3.zero;
            player.m_playerstatus = new PlayerStatus();
            player.m_playerstatus.dt = 0;
            player.m_playerstatus.speed = (float)0.1 * (i + 1);
            player.m_sidesignal.name = "playerbar_" + (i + 1);
            player.m_playerstatus.name = "player" + (i + 1);
            player.m_instance.transform.Find("name").GetComponent<Text>().text = player.m_playerstatus.name;
            players.Add(player);
        }
        for (int i = 0; i < 4; i++)
        {
            Player enemy = new Player();
            enemy.m_playerforce = PlayerForce.enemy;
            var instance = Resources.Load<GameObject>("Prefabs/instance");
            enemy.m_instance = Instantiate(instance, canvas.transform.Find("Enemies/" + i));
            enemy.m_instance.transform.localPosition = Vector3.zero;
            enemy.m_instance.GetComponentInChildren<Button>().interactable = false;
            //start courtine
            enemy.m_instance.GetComponentInChildren<Button>().onClick.AddListener(
                ()=>
                {
                    print("into sequence");
                    Sequence se = DOTween.Sequence();
                    TweenCallback tcb = () =>
                    {
                        System.Random rd = new System.Random();
                        int rdnum = rd.Next(100) % 2;
                        print("rdnum is " + rdnum);
                        if (rdnum == 0)
                        {
                            //se.Kill();
                            extrigger();
                        }
                        else
                        {
                            normalAttak();
                        }
                    };
                    var pos = enemy.m_instance.transform.position;
                    Vector3 newpos = new Vector3(pos.x - 250,pos.y,pos.z);
                    se.Append(currentplayer.m_instance.transform.DOMove(newpos, 1)).OnComplete(tcb);
                });
            enemy.m_playerstatus = new PlayerStatus();
            enemy.m_playerstatus.dt = 0;
            enemy.m_playerstatus.speed = (float)0.1 * (i + 1);
            enemy.m_playerstatus.name = "enemy" + (i + 1);
            enemy.m_instance.transform.Find("name").GetComponent<Text>().text = enemy.m_playerstatus.name;
            enemies.Add(enemy);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void LateUpdate()
    {
        if (m_aswitch)
        {
            AddDeltaTime();
            CheckTimer();
            UpdateAllSideSignal();
        }
    }

    void UpdateSideSignal(Player player)
    {
        if (player.m_playerstatus.dt > maxtime)
        {
            player.m_playerstatus.dt = maxtime;
        }
        var posx = player.m_sidesignal.transform.localPosition.x;
        var posy = (player.m_playerstatus.dt / maxtime) * scrollbar.GetComponent<RectTransform>().rect.height - scrollbar.GetComponent<RectTransform>().rect.height/2;
        player.m_sidesignal.transform.localPosition = new Vector3(posx, posy, 0);
    }

    void UpdateAllSideSignal()
    {
        for (int i = 0; i < players.Count; i++)
        {
            UpdateSideSignal(players[i]);
        }
    }

    public IEnumerator mainloop()
    {
        while (true)
        {
            
        }
    }

    private void AddDeltaTime()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].selfloop();
        }
        //for (int i = 0; i < enemies.Count; i++)
        //{
        //    enemies[i].selfloop();
        //}
    }
    void CheckTimer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].m_playerstatus.dt >= maxtime)
            {
                StartCoroutine(Tirrger(players[i]));
            }
            //if (enemies[i].m_playerstatus.dt >= maxtime)
            //{
            //    StartCoroutine(Tirrger(enemies[i]));
            //}
        }
        
    } 

    IEnumerator Tirrger(Player ps)
    {
        print("trigger");
        m_aswitch = false;
        currentplayer = ps;
        ps.m_playerstatus.dt = 0;

        if (ps.m_playerforce == PlayerForce.player)
        {
            ShowPannel(true);
        }
        else if (ps.m_playerforce == PlayerForce.enemy)
        {
            m_aswitch = true;
            showtag.text = "敌方跳过";
        }
        
        yield return new WaitUntil(() => m_aswitch == true);
        //ShowPannel(false);
        print("out select mode");
    }

    void ShowPannel(bool b_show)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].m_instance.GetComponentInChildren<Button>().interactable = b_show;
        }
    }

    void extrigger()
    {
        RefreshSignal();
        StartCoroutine(qte_rountine());
    }

    void normalAttak()
    {
        //play animation
        showtag.text = "普攻";
        print("普攻");

        ShowPannel(false);
        Sequence se = DOTween.Sequence();
        var cparent = currentplayer.m_instance.transform.parent;
        se.Append(currentplayer.m_instance.transform.DOMove(cparent.position, 1)).OnComplete(
        () =>
        {
            m_aswitch = true;
        });
    }

    void RefreshSignal()
    {
        for (int i = 0; i < b_signal.Length; i++)
        {
            b_signal[i] = false;
        }
        if (signal == null)
        {
            var res = Resources.Load<GameObject>("Prefabs/Signal");
            signal = Instantiate(res,canvas.transform);
        }
        signal.SetActive(true);
        var images = signal.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = Color.white;
        }
    }

    IEnumerator qte_rountine()
    {
        while (true)
        {
            if (Input.GetKeyDown("a"))
            {
                b_signal[1] = true;
                signal.GetComponentsInChildren<Image>()[0].color = Color.red;
            }
            if (Input.GetKeyDown("s") && b_signal[1] == true)
            {
                b_signal[2] = true;
                signal.GetComponentsInChildren<Image>()[1].color = Color.red;
            }
            if (Input.GetKeyDown("d") && b_signal[1] == true && b_signal[2] == true)
            {
                b_signal[3] = true;
                signal.GetComponentsInChildren<Image>()[2].color = Color.red;
            }
            if (b_signal[1] == true && b_signal[2] == true && b_signal[3] == true)
            {
                b_signal[0] = true;
                //play animation
                signal.gameObject.SetActive(false);
                showtag.text = "必杀";
                print("必杀！！！");
                ShowPannel(false);
                Sequence se = DOTween.Sequence();
                var cparent = currentplayer.m_instance.transform.parent;
                se.Append(currentplayer.m_instance.transform.DOMove(cparent.position, 1)).OnComplete(
                    ()=>
                    {
                        m_aswitch = true;
                    });
                yield break;
            }
            yield return null;
        }
    }
}
