using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class MenuManager : MonoBehaviourPunCallbacks
{
    public InputField create;
    public InputField join;

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(create.text, roomOptions);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(join.text);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main_Scene");
    }




}
