using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstantiateShip : MonoBehaviour
{
    public void Instantiate()
    {
        List<string> slots = PlayerInfo.instance.heroSlots;
        List<string> items = PlayerInfo.instance.heroItems;
        if (SceneManager.GetActiveScene().name == "Hangar")
        {
            if (slots == null)
            {
                return;
            }
            if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.hero)
            {
                slots = PlayerInfoStatic.heroSlotsStatic;
                items = PlayerInfoStatic.heroItemsStatic;
                if (slots.Count > 0)
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i] == "")
                            continue;
                        //Debug.Log(slots[i] + " - " + i + " - " + items[i]);
                        //Debug.Log(transform.GetChild(0).transform.GetChild(1).name);//Find(slots[i]));
                        var children = this.GetComponentsInChildren<Transform>();
                        foreach (var child in children)
                        {
                            if (child.name == slots[i])
                            {
                                GameObject gun = Instantiate(Resources.Load("Guns/" + items[i]) as GameObject, child.transform.position, child.transform.parent.rotation, child);
                                gun.name = gun.name.Replace("(Clone)", "");
                            }
                        }
                    }
            }
            if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.station)
            {
                slots = PlayerInfoStatic.stationSlotsStatic;
                items = PlayerInfoStatic.stationItemsStatic;
                if (slots.Count > 0)
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i] == "")
                            continue;
                        //Debug.Log(slots[i] + " - " + i + " - " + items[i]);
                        //Debug.Log(transform.GetChild(0).transform.GetChild(1).name);//Find(slots[i]));
                        var children = this.GetComponentsInChildren<Transform>();
                        foreach (var child in children)
                        {
                            if (child.name == slots[i])
                            {
                                GameObject gun = Instantiate(Resources.Load("Guns/" + items[i]) as GameObject, child.transform.position, child.transform.parent.rotation, child);
                                gun.name = gun.name.Replace("(Clone)", "");
                            }
                        }
                    }
            }
        }
        else if (!GameManager.instance.gameOnline)
        {
            if (slots == null)
            {
                return;
            }
            if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.hero)
            {
                slots = PlayerInfoStatic.heroSlotsStatic;
                items = PlayerInfoStatic.heroItemsStatic;
                if (slots.Count > 0)
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i] == "")
                            continue;
                        //Debug.Log(slots[i] + " - " + i + " - " + items[i]);
                        //Debug.Log(transform.GetChild(0).transform.GetChild(1).name);//Find(slots[i]));
                        var children = this.GetComponentsInChildren<Transform>();
                        foreach (var child in children)
                        {
                            if (child.name == slots[i])
                            {
                                GameObject gun = Instantiate(Resources.Load("Guns/" + items[i]) as GameObject, child.transform.position, child.transform.parent.rotation, child);
                                gun.name = gun.name.Replace("(Clone)", "");
                            }
                        }
                    }
            }
            else if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.station)
            {
                slots = PlayerInfoStatic.stationSlotsStatic;
                items = PlayerInfoStatic.stationItemsStatic;
                if (slots.Count > 0)
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i] == "")
                            continue;
                        //Debug.Log(slots[i] + " - " + i + " - " + items[i]);
                        //Debug.Log(transform.GetChild(0).transform.GetChild(1).name);//Find(slots[i]));
                        var children = this.GetComponentsInChildren<Transform>();
                        foreach (var child in children)
                        {
                            if (child.name == slots[i])
                            {
                                GameObject gun = Instantiate(Resources.Load("Guns/" + items[i]) as GameObject, child.transform.position, child.transform.parent.rotation, child);
                                gun.name = gun.name.Replace("(Clone)", "");
                            }
                        }
                    }
            }
            else if (SceneManager.GetActiveScene().name != "Hangar")
            {
                if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.hero)
                {
                    slots = PlayerInfoStatic.heroSlotsStatic;
                    items = PlayerInfoStatic.heroItemsStatic;
                    for (int i = 0; i < this.transform.Find("Guns").transform.childCount; i++)
                    {
                        //Debug.Log(this.transform.Find("Guns").transform.GetChild(i));
                        GameObject gun = Instantiate(Resources.Load("Guns/ " + items[i]) as GameObject, this.transform.Find("Guns").transform.GetChild(i));//, this.transform.Find("Guns").transform.parent.rotation, this.transform.Find("Guns").transform);
                        gun.name = gun.name.Replace("(Clone)", "");
                    }
                }
                else if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.station)
                {
                    slots = PlayerInfoStatic.heroSlotsStatic;
                    items = PlayerInfoStatic.heroItemsStatic;
                    for (int i = 0; i < this.transform.Find("Guns").transform.childCount; i++)
                    {
                        //Debug.Log(this.transform.Find("Guns").transform.GetChild(i));
                        GameObject gun = Instantiate(Resources.Load("Guns/ " + items[i]) as GameObject, this.transform.Find("Guns").transform.GetChild(i));//, this.transform.Find("Guns").transform.parent.rotation, this.transform.Find("Guns").transform);
                        gun.name = gun.name.Replace("(Clone)", "");
                    }
                }
            }
        }
        else if (GameManager.instance.gameOnline && GetComponent<PhotonView>().IsMine)
        {
            if (slots == null)
            {
                return;
            }
            if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.hero)
            {
                slots = PlayerInfoStatic.heroSlotsStatic;
                items = PlayerInfoStatic.heroItemsStatic;
                if (slots.Count > 0)
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i] == "")
                            continue;
                        //Debug.Log(slots[i] + " - " + i + " - " + items[i]);
                        //Debug.Log(transform.GetChild(0).transform.GetChild(1).name);//Find(slots[i]));
                        var children = this.GetComponentsInChildren<Transform>();
                        foreach (var child in children)
                        {
                            if (child.name == slots[i])
                            {
                                GameObject gun = PhotonNetwork.Instantiate("Guns/" + items[i].Replace("(Clone)", ""), child.transform.position, child.transform.parent.rotation);
                                gun.transform.SetParent(child);
                                gun.transform.localScale = new Vector3(1, 1, 1);
                                GetComponent<PhotonView>().RPC("PlaceGuns", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer);
                            }
                        }
                    }
            }
            if (GetComponent<ShipStats>().shipType == ShipStats.shipTypes.station)
            {
                slots = PlayerInfoStatic.stationSlotsStatic;
                items = PlayerInfoStatic.stationItemsStatic;
                if (slots.Count > 0)
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i] == "")
                            continue;
                        //Debug.Log(slots[i] + " - " + i + " - " + items[i]);
                        //Debug.Log(transform.GetChild(0).transform.GetChild(1).name);//Find(slots[i]));
                        var children = this.GetComponentsInChildren<Transform>();
                        foreach (var child in children)
                        {
                            if (child.name == slots[i])
                            {
                                GameObject gun = PhotonNetwork.Instantiate("Guns/" + items[i].Replace("(Clone)", ""), child.transform.position, child.transform.parent.rotation);
                                gun.transform.SetParent(child);
                                gun.transform.localScale = new Vector3(1, 1, 1);
                                GetComponent<PhotonView>().RPC("PlaceGuns", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer);
                            }
                        }
                    }
            }
        }
    }
    [PunRPC] //online guns instantiate
    public void PlaceGuns(Player player)
    {
        List<GameObject> guns = new List<GameObject>();
        foreach (GameObject gun in GameObject.FindGameObjectsWithTag("Gun"))
        {
            if (gun.GetComponent<PhotonView>().Owner == player)
            {
                guns.Add(gun);
            }
        }
        for (int i = 0; i < guns.Count; i++)
        {
            foreach (var ship in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (ship.GetComponent<PhotonView>().Owner == player)
                {
                    guns[i].transform.SetParent(ship.transform.GetChild(0).transform.GetChild(i).transform);
                    guns[i].transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }
}
