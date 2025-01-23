using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameObject playerScordPanelHolder;

    [Header("Option")]
    [SerializeField] private float refreshRate = 1f;

    [Header("UI")]
    [SerializeField] private GameObject[] slots;
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private TextMeshProUGUI[] KDTexts;

    private void Start()
    {
        InvokeRepeating(nameof(Refresh), 0f, refreshRate);
    }

    public void Refresh()
    {
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        var sortedPlayers = (from player in PhotonNetwork.PlayerList
                             orderby player.GetScore() descending
                             select player).ToList();

        int i = 0;
        foreach (var player in sortedPlayers)
        {
            slots[i].SetActive(true);

            if(player.NickName == "")
            {
                player.NickName = "Unnamed";
            }

           nameTexts[i].text = player.NickName;
           scoreTexts[i].text = player.GetScore().ToString();

            if (player.CustomProperties["Kills"] == null)
            {
                KDTexts[i].text = player.CustomProperties["kills"].ToString() + "/" + player.CustomProperties["deaths"].ToString();
            } 
            else
            {
                KDTexts[i].text = "0/0";
            }
            
            i++;
        }
    }

    private void Update()
    {
        playerScordPanelHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
