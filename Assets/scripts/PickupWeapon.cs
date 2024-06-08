using UnityEngine;
using UnityEngine.UI;

public class PickupWeapon : MonoBehaviour
{
    private bool canPickup = false;
    private GameObject player;
    private Text pickupText;

    void Start()
    {
        pickupText = GameObject.Find("PickupText").GetComponent<Text>();
        pickupText.text = ""; // Убедитесь, что текст пустой при старте
    }

    void Update()
    {
        if (canPickup)
        {
            pickupText.text = "[E] Take Weapon";

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Делаем оружие дочерним объектом руки игрока
                Transform handTransform = player.transform.Find("Hand");
                if (handTransform != null)
                {
                    transform.SetParent(handTransform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;

                    // Отключаем коллайдеры и физику для оружия
                    Collider weaponCollider = GetComponent<Collider>();
                    if (weaponCollider != null)
                    {
                        weaponCollider.enabled = false;
                    }

                    Rigidbody weaponRigidbody = GetComponent<Rigidbody>();
                    if (weaponRigidbody != null)
                    {
                        weaponRigidbody.isKinematic = true;
                    }

                    // Установить оружие в PlayerController
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.weaponTransform = this.transform;
                    }

                    // Скрываем текст после подбора
                    pickupText.text = "";
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickup = false;
            player = null;
            pickupText.text = ""; // Скрываем текст, когда игрок выходит из зоны
        }
    }
}
