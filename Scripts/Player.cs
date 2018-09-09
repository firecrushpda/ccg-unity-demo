using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{

    public PlayerStatus m_playerstatus;

    public GameObject m_instance;

    public PlayerForce m_playerforce;

    public GameObject m_sidesignal;

    public void selfloop()
    {
        m_playerstatus.dt += Time.fixedDeltaTime * m_playerstatus.speed;
        //Debug.Log(name + " " + dt);
    }
}

public enum PlayerForce
{
    player,
    enemy,
}
