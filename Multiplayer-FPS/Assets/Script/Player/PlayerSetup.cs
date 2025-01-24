using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviour
{
    private string nickname;

    [SerializeField] private Movement movement;
    [SerializeField] private Sliding sliding;
    [SerializeField] private GameObject camera;
    [SerializeField] private TextMeshPro nicknameText;
    public void IsLocalPlayer()
    {
        movement.enabled = true;
        sliding.enabled = true;
        camera.SetActive(true);
    }

    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;

        nicknameText.text = nickname;
    }
}
