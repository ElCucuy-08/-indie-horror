using UnityEngine;
using UnityEngine.UI;//Библиотека для работы с UI элементами
using UnityEngine.Rendering.PostProcessing;//Библиотека для работы с PostProcessing
public class Predmets : MonoBehaviour
{
    [SerializeField] Text textCountPaper;//Поле,которое хранит в себе текст с количеством листков на карта
    [SerializeField] Text textTakePaper;//Поле,которое хранит в себе текст,отвечающий за подбор записок
    [SerializeField] int countPaper;//Количество записок на карте

    [SerializeField] GameObject Enemy;//Добавляем для проверка,где находится наш слендер
    [SerializeField] PostProcessVolume effects;//Чтобы в последующем редактировать наши эффекты
    PostProcessVolume m_Volume;
    Grain m_Grain;

    private void Start()
    {
        countPaper = GameObject.FindGameObjectsWithTag("Predmet").Length;//Находим записки с помощью тэга
        //Настраиваем эффекты
        m_Grain = ScriptableObject.CreateInstance<Grain>();
        m_Grain.enabled.Override(true);
        m_Grain.intensity.Override(1f);
        m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_Grain);
    }

    private void Update()
    {
        //Определяем положение слендара относительно нас
        float DotResult = -1 * Vector3.Dot(transform.forward, Enemy.transform.forward);
        if (DotResult < 0.1f)
        {
            DotResult = 0.1f;
        }

        //Меняем значение силы эффектов
        m_Grain.intensity.value = DotResult;
        m_Grain.size.value = DotResult * 3;
        m_Grain.lumContrib.value = DotResult;

        UpdateText();
    }

    void UpdateText()
    {
        textCountPaper.text = "Predmets: " + countPaper;//выводим текстом,сколько записок осталось на карте
    }

    private void OnTriggerStay(Collider other)//Проверяем,что мы зашли внутрь триггера
    {
        if (other.CompareTag("Predmet"))//Проверяем,что это тэг Paper
        {
            textTakePaper.gameObject.SetActive(true);//Включаем текст для подбора записок
            if (Input.GetKeyDown(KeyCode.F))//Если нажата клавиша F
            {
                textTakePaper.gameObject.SetActive(false);//Выключаем текст для подбора записок
                Destroy(other.gameObject);//Внешне уничтожаем подобранную записку
                countPaper--;
            }
        }
    }
    private void OnTriggerExit(Collider other)//На тот случай,если просто вошли в зону триггера и вышли из него,но не подобрали записку
    {
        textTakePaper.gameObject.SetActive(false);//Выключаем текст для подбора записок
    }
}
