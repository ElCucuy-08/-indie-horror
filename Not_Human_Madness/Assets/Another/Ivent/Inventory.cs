using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с Image

public class Inventory : MonoBehaviour
{
    [Header("Настройки управления")]
    public KeyCode PickUp = KeyCode.F;
    public KeyCode inventKey = KeyCode.Q;
    public float PickUpRange = 2f;
    public GameObject invent;
    public Transform holdPosition;

    [Header("UI Настройки")]
    // Массив из 9 картинок на Canvas
    public Image[] slotImages = new Image[9];

    [Header("Вращение предмета")]
    public float rotationSpeed = 100f;

    private List<GameObject> inventoryItems = new List<GameObject>();
    private GameObject currentHeldItem;

    // --- Скрытые переменные для создания снимка предмета ---
    private Camera renderCamera;
    private RenderTexture renderTexture;

    private void Start()
    {
        if (invent != null) invent.SetActive(false);

        // Создаем камеру, которая будет делать фото предметов
        SetupSnapshotCamera();
    }

    private void Update()
    {
        // Переключение инвентаря
        if (Input.GetKeyDown(inventKey) && invent != null)
        {
            invent.SetActive(!invent.activeSelf);
        }

        // Подбор предмета
        if (Input.GetKeyDown(PickUp))
        {
            TryPickUp();
        }

        // Спавн предмета по клавишам 1-9
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < inventoryItems.Count)
            {
                SpawnItem(i);
            }
        }

        // Вращение предмета в руке (добавлена проверка на null, чтобы не было ошибок)
        if (currentHeldItem != null)
        {
            if (Input.GetKey(KeyCode.P))
            {
                currentHeldItem.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
            }

            if (Input.GetKey(KeyCode.O))
            {
                currentHeldItem.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }

    void TryPickUp()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, PickUpRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("PickUp"))
            {
                GameObject item = collider.gameObject;

                // Проверяем, не подобрали ли мы уже этот предмет (чтобы не дублировать)
                if (!inventoryItems.Contains(item))
                {
                    inventoryItems.Add(item);

                    // 1. Делаем снимок предмета и ставим в UI
                    int slotIndex = inventoryItems.Count - 1;
                    if (slotIndex < slotImages.Length)
                    {
                        Sprite itemSprite = TakeSnapshot(item);
                        slotImages[slotIndex].sprite = itemSprite;
                        slotImages[slotIndex].enabled = true; // Включаем картинку
                    }

                    // 2. Скрываем предмет в мире
                    item.SetActive(false);
                }
            }
        }
    }

    void SpawnItem(int index)
    {
        // Удаляем текущий удерживаемый предмет, если он есть
        if (currentHeldItem != null)
        {
            Destroy(currentHeldItem);
        }

        // Спавним новый предмет из инвентаря
        if (index < inventoryItems.Count)
        {
            GameObject itemPrefab = inventoryItems[index];
            currentHeldItem = Instantiate(itemPrefab, holdPosition.position, holdPosition.rotation, holdPosition);
            currentHeldItem.SetActive(true);
            Debug.Log("Появился предмет из слота " + (index + 1) + ": " + itemPrefab.name);
        }
    }

    // ==========================================
    // МЕТОДЫ ДЛЯ СОЗДАНИЯ ФОТО ПРЕДМЕТА (СНИМОК)
    // ==========================================

    private void SetupSnapshotCamera()
    {
        // Создаем камеру для снимков
        GameObject camObj = new GameObject("SnapshotCamera");
        renderCamera = camObj.AddComponent<Camera>();
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = new Color(0, 0, 0, 0); // Прозрачный фон
        renderCamera.cullingMask = ~0; // Отрисовывать всё
        renderCamera.enabled = false; // Камера не должна рендерить сцену постоянно

        // Создаем текстуру, в которую будем сохранять снимок (128x128 пикселей)
        renderTexture = new RenderTexture(128, 128, 24);
        renderCamera.targetTexture = renderTexture;
    }

    private Sprite TakeSnapshot(GameObject item)
    {
        // 1. Временно активируем предмет, чтобы камера его увидела
        bool wasActive = item.activeSelf;
        item.SetActive(true);

        // Сохраняем изначальную позицию и поворот
        Vector3 originalPos = item.transform.position;
        Quaternion originalRot = item.transform.rotation;

        // 2. Перемещаем предмет перед камерой (в точку 0,0,-5)
        item.transform.position = new Vector3(0, 0, -5);
        item.transform.rotation = Quaternion.identity;

        // Если у предмета есть Rigidbody, отключаем его на время снимка
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // 3. Делаем снимок
        renderCamera.Render();

        // 4. Конвертируем RenderTexture в Sprite
        Texture2D tex = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, 128, 128), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        // 5. Возвращаем предмет обратно (на его место и в скрытое состояние)
        item.transform.position = originalPos;
        item.transform.rotation = originalRot;
        if (rb != null) rb.isKinematic = false;

        if (!wasActive) item.SetActive(false);

        return newSprite;
    }

    private void OnDestroy()
    {
        // Очищаем память при удалении объекта
        if (renderTexture != null) renderTexture.Release();
    }
}