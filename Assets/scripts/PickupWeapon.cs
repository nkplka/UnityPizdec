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
                Transform weaponHolder = player.transform.Find("Main Camera/WeaponHolder");
                if (weaponHolder != null)
                {
                    transform.SetParent(weaponHolder);
                    transform.localPosition = new Vector3(0f, -0.5f, 2); // Позиция относительно держателя
                    transform.localRotation = Quaternion.Euler(0, 0, 0); // Поворот относительно держателя
                    transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Установка масштаба оружия

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
            pickupText.text = "";
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
