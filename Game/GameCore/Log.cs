using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using System.Drawing.Printing;

namespace Practic2020.GameCore
{
    /// <summary>
    /// Класс организующий свяь между данными игры выводом и логированием.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Формат для вывода данных из модели
        /// </summary>
        /// <param name="author">автор сообщения</param>
        /// <param name="massage">текст сообщения</param>
        public delegate void ShowMassage(string author, string massage);
        /// <summary>
        /// событие для вывода информации
        /// </summary>
        public event ShowMassage OnSending;
        /// <summary>
        /// Отправка сообщения на вывод
        /// Вывод происходит подпиской на событие OnSending
        /// </summary>
        /// <param name="author">автор сообщения</param>
        /// <param name="massage">текст сообщения</param>
        public void MadeIvent(string author, string massage)
        {
            OnSending?.Invoke(author, massage);
        }
        /// <summary>
        /// переменная указывающая на то, нужно ли работать с файлом
        /// </summary>
        public bool WorckWithFIle { get; set; }
        /// <summary>
        /// переменная указывающая на то, нужно ли работать с консолью
        /// </summary>
        public bool WorckWithConsole { get; set; }
        /// <summary>
        /// досутуп к логом, ели логи хронатся в файле, то в в первую очередь идет обращение в файл.
        /// </summary>
        public string[] Strings
        {
            get
            {
                if (WorckWithFIle)
                {
                    StreamReader reader = new StreamReader("Log.txt");
                    string[] output = reader.ReadToEnd().Split('\n');
                    reader.Close();
                    return output;
                }
                else
                {
                    return Strings;
                }
            }
            set
            {

            }
        }
        /// <summary>
        /// Инициализация логов
        /// </summary>
        /// <param name="maxCount">максимальный размер</param>
        /// <param name="worckWithFIle">работать ли с файлом</param>
        /// <param name="worckWithConsole">работать ли с консолью</param>
        public Log(int maxCount = 1000, bool worckWithFIle = true, bool worckWithConsole = true)
        {
            MaxCount = maxCount;
            WorckWithFIle = worckWithFIle;
            WorckWithConsole = worckWithConsole;
            Strings = new string[maxCount];
            StreamWriter creator = new StreamWriter("Log.txt");
            creator.Close();
        }
        /// <summary>
        /// текущее количество логов
        /// </summary>
        private int CurrentCount {get; set;}
        /// <summary>
        /// максимальное количество логов
        /// </summary>
        private int MaxCount { get; set;}
        /// <summary>
        /// записать сроку в логи
        /// </summary>
        /// <param name="meage"></param>
        public void WriteStreang(string meage)
        {
            if (WorckWithFIle)
            {
                StreamWriter reader = new StreamWriter("Log.txt", append: true);
                reader.WriteLine(meage);
                reader.Close();
            }
            else
            {
                if(CurrentCount + 1 > MaxCount)
                {
                    throw new Exception("лог переполнен");
                }
                else
                {
                    Strings[CurrentCount] = meage;
                }
            }
        }
        public IEnumerator GetEnumerator()
        {
            return Strings.GetEnumerator();
        }

    }
}