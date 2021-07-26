using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GunFire_Photon : MonoBehaviour, IPunObservable
{
    GameObject ship, missile, gunsGO;
    Vector3 direction;
    Rigidbody rigidBody;
    PhotonView photonView; //Я использую Photon Unity Network для реализации мультеплеера
    Joystick fireJoystick, moveJoystick;

    Gun gunComponent;

    public Transform firingSpot;

    [HideInInspector]
    public float fireRate, nextFire, rotateSpeed, prefireOnStraight, acceleration;
    [HideInInspector]
    public bool autoFire, firing, crossPlayer;
    string missileName;
    Team.teams team;
    Transform crossChecker;
    public enum firingTypes // Можно переключатсья между типами стрельбы, вперед или веером
    {
        type1, type2
    }
    public firingTypes fireType;

    ObjectPooler objectPooler;
    [SerializeField]
    List<GameObject> targetsList = new List<GameObject>();
    public Transform target;
    Rigidbody targetRigidBody;
    NavMeshAgent targetAgent;
    float autoFireRange, directionChange;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Hangar" || !GameManager.instance.gameOnline) 
        {
            Destroy(this); // В ангаре или однопользовательской игре этот скрипт удаляется
        }
    }

    private void Start()
    {
        firing = false;
        crossPlayer = false;
        photonView = GetComponent<PhotonView>();
        fireJoystick = GameManager.instance.fireJoystick;
        moveJoystick = GameManager.instance.moveJoystick;
        gunComponent = GetComponent<Gun>();
        missileName = gunComponent.missile.name;
        fireRate = gunComponent.fireRate;
        GameObject[] ships = GameObject.FindGameObjectsWithTag("Player"); // Поиск всех игроков на сцене.
        foreach (GameObject shipGO in ships)
        {
            if (shipGO.GetComponent<PhotonView>().Owner == photonView.Owner) // Выбор локального игрока
                ship = shipGO; 
        }
        rigidBody = ship.GetComponent<Rigidbody>();
        team = ship.GetComponent<Team>().team;
        acceleration = ship.GetComponent<ShipStats>().Acceleration;
        //mass = ship.GetComponent<ShipStats>().Mass;
        objectPooler = ObjectPooler.instance;
        rotateSpeed = gunComponent.rotationSpeed;
        prefireOnStraight = gunComponent.prefireOnStraight;
        gunsGO = gunsGO.gameObject;
        directionChange = gunsGO.GetComponent<GunSpot>().fireRotations[0];
        crossChecker = gunComponent.crossChecker.transform;
    }
    public void SetAutoFire() // Есть автострельба
    {
        autoFire = !autoFire;
        if (autoFire)
            StartCoroutine(GetTargets());
        else
            StopCoroutine(GetTargets());
    }
    [PunRPC]
    public void StartFiring() //Игрок переключается между стрельба On/off и это инфу получают все игроки, чтобы на их клиентах объект тоже стрелял
    {
        firing = true;
    }
    [PunRPC]
    public void StopFiring()
    {
        firing = false;
    }
    [PunRPC]
    public void SetFiringType1() //Аналогично переключается тип стрельбы
    {
        fireType = firingTypes.type1;
        directionChange = gunsGO.GetComponent<GunSpot>().fireRotations[0];
    }
    [PunRPC]
    public void SetFiringType2()
    {
        fireType = firingTypes.type2;
        directionChange = gunsGO.GetComponent<GunSpot>().fireRotations[1];
    }
    IEnumerator GetTargets() //Получение списка всех целей из объекта, содержащего списки всех активных объектов
    {
        for (; ; )
        {
            if (team == Team.teams.team1)
            {
                targetsList = UnitsStocker.singleton.team2All;
            }
            else if (team == Team.teams.team2)
            {
                targetsList = UnitsStocker.singleton.team1All;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    public void AutoFiring() // Автоогонь учитывает скорость движения игрока и цели, чтобы стрелять на упреждение
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (targetRigidBody == null) return;
        float charge = distance / (missile.GetComponent<Missile>().speed);
        if (target.CompareTag("Mob"))
        {
            var chaseType = target.GetComponent<AIChase>().chaseType;
            var chaseTarget = target.GetComponent<AIChase>().target;
            if (moveJoystick.Direction != Vector2.zero && !ship.GetComponent<PlayerMove_pvp>().speedMax)
            {
                var moveDirection = new Vector3(moveJoystick.Direction.x, 0f, moveJoystick.Direction.y);
                if (chaseTarget == ship && (chaseType == AIChase.chaseTypes.straight || chaseType == AIChase.chaseTypes.range || chaseType == AIChase.chaseTypes.stand))
                {
                    //Debug.Log("straight on move");
                    direction = target.position + (targetAgent.velocity.normalized * targetAgent.velocity.magnitude / prefireOnStraight - rigidBody.velocity.normalized * rigidBody.velocity.magnitude / prefireOnStraight - acceleration * moveDirection) * charge - transform.position;
                }
                else
                {
                    direction = target.position + (targetAgent.velocity.normalized * targetAgent.velocity.magnitude - rigidBody.velocity.normalized * rigidBody.velocity.magnitude - acceleration * moveDirection) * charge - transform.position;
                }
            }
            else
            {
                if (chaseTarget == ship && (chaseType == AIChase.chaseTypes.straight || chaseType == AIChase.chaseTypes.range || chaseType == AIChase.chaseTypes.stand))
                {
                    //Debug.Log("straight not move");
                    direction = (target.position + (targetAgent.velocity.normalized * targetAgent.velocity.magnitude - rigidBody.velocity.normalized * rigidBody.velocity.magnitude) * charge / prefireOnStraight) - transform.position;
                }
                else
                {
                    direction = (target.position + (targetAgent.velocity.normalized * targetAgent.velocity.magnitude - rigidBody.velocity.normalized * rigidBody.velocity.magnitude) * charge) - transform.position;
                }
            }
        }
        else if (target.CompareTag("Player"))
        {
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (moveJoystick.Direction != Vector2.zero && !ship.GetComponent<PlayerMove_pvp>().speedMax)
            {
                var moveDirection = new Vector3(moveJoystick.Direction.x, 0f, moveJoystick.Direction.y);
                direction = target.position + (rb.velocity.normalized * rb.velocity.magnitude - rigidBody.velocity.normalized * rigidBody.velocity.magnitude) * charge - transform.position;
            }
            else
            {
                direction = (target.position + (rb.velocity.normalized * rb.velocity.magnitude - rigidBody.velocity.normalized * rigidBody.velocity.magnitude) * charge) - transform.position;
            }
        }
        GameObject shot = ObjectPoolerPhoton.singleton.SpawnFromPoolMissile(missileName, firingSpot.transform.position, firingSpot.transform.rotation, rigidBody.velocity);
        shot.GetComponent<PhotonView>().RPC("SetTeam", RpcTarget.All, team);
        //shot.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.All, ship);

        shot.GetComponent<Missile_mover>().shipSpeed = rigidBody.velocity;
        shot.transform.gameObject.tag = "damageDealerPlayer";
        nextFire = Time.time + fireRate;
        return;
    }
    [PunRPC]
    public void Fire()
    {
        if (Time.time > nextFire)
        {
            GameObject shot = objectPooler.SpawnFromPoolMissile(missileName, firingSpot.transform.position, firingSpot.transform.rotation, rigidBody.velocity);
            //GameObject shot = Instantiate(missile, firingSpot.transform.position, firingSpot.transform.rotation) as GameObject;
            //shot.GetComponent<Missile_mover>().shipSpeed = ship.GetComponent<Rigidbody>().velocity;
            shot.GetComponent<Missile>().team = team;
            shot.GetComponent<Missile>().owner = ship;
            shot.transform.gameObject.tag = "damageDealerPlayer";
            nextFire = Time.time + fireRate;
            return;
        }
    }
    private void Update()
    {
        Ray gunRay = new Ray(crossChecker.position, transform.forward);
        Debug.DrawRay(crossChecker.position, transform.forward * 40f, Color.blue);

        RaycastHit hit;
        if (Physics.Raycast(gunRay, out hit))
        {
            if (hit.transform.gameObject == ship)
            {
                crossPlayer = true; // Орудия, которые попали бы в самого игрока (из-за угла поворота) не стреляют
            }
            else
            {
                crossPlayer = false;
            }
        }
        else
        {
            crossPlayer = false;
        }
        if (firing && !crossPlayer)
        {
            Fire();
            /*
            direction = new Vector3(fireJoystick.Direction.x, 0f, fireJoystick.Direction.y);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), rotateSpeed * Time.deltaTime);

            Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, rotateSpeed * Time.deltaTime, 0F);
            transform.rotation = Quaternion.LookRotation(newDir) * Quaternion.Euler(0, directionChange, 0); ;
            if (Time.time > nextFire)
            {
                GameObject shot = ObjectPoolerPhoton.singleton.SpawnFromPoolMissile(missileName, firingSpot.transform.position, firingSpot.transform.rotation, rigidBody.velocity);
                shot.GetComponent<PhotonView>().RPC("SetTeam", RpcTarget.All, team);
                //shot.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.All, ship);

                nextFire = Time.time + fireRate;
                return;
            }
            */
        }
        if (autoFire && fireJoystick.Direction == Vector2.zero)
        {
            for (var i = 0; i < targetsList.Count; i++)
            {
                if (targetsList[i] == null)
                {
                    targetsList.RemoveAt(i);
                    return;
                }
                if (target == null && Vector3.Distance(transform.position, targetsList[i].transform.position) < autoFireRange + 20f)
                {
                    target = targetsList[i].transform;
                    targetRigidBody = target.GetComponent<Rigidbody>();
                    targetAgent = target.GetComponent<NavMeshAgent>();
                }
                else if (target != null && Vector3.Distance(transform.position, targetsList[i].transform.position) < Vector3.Distance(transform.position, target.position))
                {
                    target = targetsList[i].transform;
                    targetRigidBody = target.GetComponent<Rigidbody>();
                    targetAgent = target.GetComponent<NavMeshAgent>();
                }
            }
            if (target != null && Time.time > nextFire && Vector3.Distance(transform.position, target.position) < autoFireRange)
            {
                AutoFiring();
            }
        }
    }

    private void FixedUpdate() //Вращение орудий в сторону стрельбы
    {
        if (fireJoystick.Direction != Vector2.zero && photonView.IsMine)
        {
            direction = new Vector3(fireJoystick.Direction.x, 0f, fireJoystick.Direction.y);
            Quaternion directQuat = new Quaternion();
            if (!(transform.localEulerAngles.y > 135f && transform.localEulerAngles.y < 225f))
            {
                directQuat = Quaternion.LookRotation(direction) * Quaternion.Euler(0, directionChange, 0);
            }
            else
            {
                directQuat = Quaternion.LookRotation(direction);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, directQuat, rotateSpeed * Time.deltaTime * 10f);
        }

        if (autoFire && fireJoystick.Direction == Vector2.zero && target != null)
        {
            if (Vector3.Distance(transform.position, target.position) < autoFireRange + 20f)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, rotateSpeed * Time.deltaTime, 0F);
                transform.rotation = Quaternion.LookRotation(newDir) * Quaternion.Euler(0, directionChange, 0);
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //Передача инфы о направлении орудия другим игрокам
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}