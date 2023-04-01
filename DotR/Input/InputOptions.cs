using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{

    /// <summary>
    /// Настройки клавиш управления
    /// </summary>
    public class InputOptions
    {


        /// <summary>
        /// Клавиша контроллер вверх (ход) (трамвай)
        /// </summary>
        public int Tram_KeyPower;
        /// <summary>
        /// Клавиша контроллер вниз (тормоз) (трамвай)
        /// </summary>
        public int Tram_KeyBrake;

        /// <summary>
        /// Клавиша поднять/опустить токоприемник (трамвай)
        /// </summary>
        public int Tram_KeyPantograph;

        /// <summary>
        /// Клавиша педаль газа (троллейбус)
        /// </summary>
        public int Trolleybus_KeyGas;
        /// <summary>
        /// Клавиша педаль тормоза (троллейбус)
        /// </summary>
        public int Trolleybus_KeyBrake;
        /// <summary>
        /// Клавиша Поднять/опустить штанги (троллейбус)
        /// </summary>
        public int Trolleybus_KeyPoles;

        /// <summary>
        /// Клавиша Педаль газа (автобус)
        /// </summary>
        public int Bus_KeyGas;
        /// <summary>
        /// Клавиша Педаль тормоза (автобус)
        /// </summary>
        public int Bus_KeyBrake;
        /// <summary>
        /// Клавиша КПП вверх (автобус)
        /// </summary>
        public int Bus_ShiftUp;
        /// <summary>
        /// Клавиша КПП вниз (автобус)
        /// </summary>
        public int Bus_ShiftDown;







        //Загрузить настройки из файла
        public void Load(string filename)
        {

        }

        //Сохранить настройки в файл
        public void Save(string filename)
        {

        }

    }
}
