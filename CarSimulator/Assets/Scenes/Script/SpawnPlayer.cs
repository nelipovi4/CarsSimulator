using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    // Базовая точка спавна
    private Vector3 baseSpawnPosition = new Vector3(80f, 0.07f, 60f);
    private float offsetX = 10f;

    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (actorNumber > 4)
        {
            Debug.LogWarning("Максимум 4 игрока поддерживается этим скриптом.");
            return;
        }

        Vector3 spawnPosition = baseSpawnPosition + new Vector3((actorNumber - 1) * offsetX, 0f, 0f);

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }
}
