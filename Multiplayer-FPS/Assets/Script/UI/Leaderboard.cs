using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private GameObject playerScorePanelHolder;

    [Header("Option")]
    [SerializeField] private float refreshRate = 1f;

    [Header("UI")]
    [SerializeField] private LeaderboardSlot[] slots;

    private void Start()
    {
        InvokeRepeating(nameof(Refresh), 0f, refreshRate);
    }

    public void Refresh()
    {
        // Désactiver tous les slots
        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(false);
        }

        // Trier les joueurs par score décroissant
        var sortedPlayers = PhotonNetwork.PlayerList
            .OrderByDescending(player => player.GetScore())
            .ToList();

        // Activer et mettre à jour les slots pour les joueurs triés
        for (int i = 0; i < sortedPlayers.Count && i < slots.Length; i++)
        {
            var player = sortedPlayers[i];
            var slot = slots[i];

            slot.gameObject.SetActive(true);
            slot.UpdateSlot(
                player.NickName == "" ? "Unnamed" : player.NickName,
                player.GetScore(),
                player.CustomProperties.ContainsKey("kills") && player.CustomProperties.ContainsKey("deaths")
                    ? $"{player.CustomProperties["kills"]}/{player.CustomProperties["deaths"]}"
                    : "0/0"
            );
        }
    }

    private void Update()
    {
        playerScorePanelHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}

[System.Serializable]
public class LeaderboardSlot
{
    public GameObject gameObject;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI kdText;

    public void UpdateSlot(string name, int score, string kd)
    {
        nameText.text = name;
        scoreText.text = score.ToString();
        kdText.text = kd;
    }
}
