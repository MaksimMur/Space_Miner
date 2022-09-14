using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    //Применнеине Vector2. x хранит минимальное значение
    // а у максимально значение для метода Random.Range(),
    // который будет вызываться позже
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;//время до исчезновения
    public float fadeTime = 4f;//время растворения

    [Header("Set Dynamically")]
    public WeaponType type; //тип бонуса
    public GameObject cube; //Ссылка на вложенный куб
    public TextMesh letter;//Сссылка на TextMesh
    public Vector3 rotPerSecond;//Скорость вращения
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake() {
        //Получить ссылку на куб
        cube = transform.Find("Cube").gameObject;
        //Получть ссылки на TextMEsh и другие компоненты
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = GetComponent<Renderer>();

        //выбрать случайную скорость
        Vector3 vel = Random.onUnitSphere; //получить случайную скорость XYZ
        //Random.OnUnitSphere возвращает вектор, указывающий на случайную точку,
        //находящуюся на поверхности сферы с радиусом 1 м и с центром в начале координат
        vel.z = 0;
        vel.Normalize();//Нормалицазия устанваливает длину вектора, равную 1
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        //Установить угол поворота игрового объекта равным R[0,0,0]
        transform.rotation = Quaternion.identity;
        //Quaternion.identity идентично отсутствию поворота

        //Выбрать случайную скорость вращения для вложенного куба с
        // использованием rootMinMax.x u rootMinMax.y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        //Эффект растворения куба с течением времени
        //Со значением по умолчанию бонус существует 10 секнуд
        // а затем растворяется в течении 4 секунд
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        //в течении lifeTime секунд значение u будет <= 0. Затем оно станет
        //полложительным и через fadeTIme секнуд станет больше 1

        if (u > 1) {
            Destroy(gameObject);
            return;
        }

        //Использование u для определения альфа значения куба и буквы
        if (u > 0) {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color=c;

            //и букава тоже растворяется
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
        
        if (!bndCheck.isOnScreen) {
            Destroy(gameObject);
        }
    }
    public void SetType(WeaponType wt) {
        //Получить WeaponDefenition из Main
        WeaponDefenition def = Main.GetWeaponDefenition(wt);
        //Установить цвет дочернего куба
        cubeRend.material.color = def.color;
        letter.text = def.letter;//Установка отображаемой буквы
        type = wt;//установка типа
    }
    public void AbsorbedBy(GameObject target)
    {
        //Эта функция вызывается классом Hero, когда игрок подбирает боунс
        //Можно было бы реализовать эффект поглощения бонуса, умегьшая его
        // размеры в течении несколих кадров, но пока просто уничожение
        Destroy(this.gameObject);
    }
}
