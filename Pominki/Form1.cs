using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Pominki
{
    public partial class Form1 : Form
    {
        private int xMin = 0;//Минимальное значение зоны отрисовки по x
        private int xMax = 0;//Максимальное значение зоны отрисовки по x
        private int yMin = 0;//Минимальное значение зоны отрисовки по y
        private int yMax = 0;//Максимальное значение зоны отрисовки по y
        //То есть указывают в каких приделах отображать график в системах координат(Масштаб)

        private int dx=0, dy=0;//Шаг между двумя соседними значениями на осях x и y в области приложения(px)
        private int x0=0, y0=0;//Центры осей x и y (Координаты x и y будут указывать на середину окна)
        int rost = 0;//Переменная, запоминающая индекс массивов координат анимированного синего квадрата
        Bitmap bp1;//Битмап содержащий в себе систему координат с графиком и синим квадратом
        bool isOn = false;//Проверка на нажатие кнопки button1 ("Create")
        bool isVisible = true;//Проверка на видимость объектов интерфейса на экране(Нажатие кнопки button4 ("Hide")) 
        List<double> polarx = new List<double>();//Объявление листа координат графика по x в полярной системе координат
        List<double> polary = new List<double>();//Объявление листа координат графика по y в полярной системе координат
        List<double> x = new List<double>();//Объявление листа координат графика по x в декартовой системе координат
        List<double> y = new List<double>();//Объявление листа координат графика по y в декартовой системе координат
        private double a=0;//Параметрическая переменная, необходимая для построения графика
        Font f = new Font("Arial", 9);//Фонт для отрисовки некоторых элементов интерфейса
        
        //Переменные для масштабирования полярной системы координат
        double coinNum=0;//Количество колец, которые будут отрисованы на интерфейсе
        double coinDistance=0;//Диаметр кольца
        double coinStep=0;//Шаг между кольцами
        int multiDeg=0;//Множитель, влияющий на дальность отрисовки подписей градусных осей
        int multiСhart=0;//Множитель, влияющий на масштаб графика

        //private int xMin = -10;
        //private int xMax = 10;
        //private int yMin = -10;
        //private int yMax = 10;
        //private int dx = 0, dy = 0;
        //private int x0 = 0, y0 = 0;
        //int rost = 0;
        //Bitmap bp1;
        //bool isOn = false;
        //bool isVisible = true;
        //List<double> polarx = new List<double>();
        //List<double> polary = new List<double>();
        //List<double> x = new List<double>();
        //List<double> y = new List<double>();
        //private double a = 0;
        //Font f = new Font("Arial", 9);
        //double coinNum = 0;
        //double coinDistance = 0;
        //double coinStep = 0;
        //int multiDeg = 0;
        //int multiСhart = 0;

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //label3.Visible = false;
            coinNum = ((pictureBox1.Width / 2) + (pictureBox1.Height / 2)) / 1.5;//Дефолтное значение для coinNum
            coinDistance = ((pictureBox1.Width / 10) + (pictureBox1.Height / 10)) / 2;//Дефолтное значение для coinDistance
            coinStep = (pictureBox1.Width / 12 + pictureBox1.Height / 12) / 2;//Дефолтное значение для coinStep
            multiDeg = 10;//Дефолтное значение для multiDeg
            multiСhart = 48;//Дефолтное значение для multiChart
            //Бэкап на случай если что-то крашнется в полярной системе координат

            groupBox1.Visible = false;//Выключение видимости для groupBox1
            button2.Visible = false;//Выключение видимости для button2
            button3.Visible = false; button3.Enabled = false;//Выключение видимости и доступности для button3
            //Прятанье/выключение ненужных элементов пользовательского интерфейса при запуске приложения
            
            //button4.Location = new Point(Bottom-100, Right-100);
            comboBox1.Items.Add("Descartes");//Добавление в comboBox1 элемента Descartes
            comboBox1.Items.Add("Polar");//Добавление в comboBox1 элемента Polar
            comboBox1.SelectedIndex = 0;//Назначаем первый элемент в comboBox1 дефолтом
            textBox1.Text = "1";//Назначаем "1" дефолтом для параметрической переменной a
            pictureBox1.Image = SimpleDecart();//Назначаем декартовую систему координат дефолтом 
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;//Устанавливаем для comboBox1 стиль(Чтобы пользователь не мог вводить свои элементы в comboBox1)
            x.Clear();//Очищаем лист, содержащий координаты графика по x в декартовой системе координат
            y.Clear();//Очищаем лист, содержащий координаты графика по y в декартовой системе координат         
        }
        private void Scale()//Функция масштабирования
        {
            if (comboBox1.SelectedIndex == 0)//для DecartSys(Декартовой системы координат)
            {
                if (pictureBox1.Height + pictureBox1.Width <= 640)//Считаем периметр окна приложения и сравниваем с условием
                {
                    xMin = -2;//Установка минимального значения зоны отрисовки по x
                    xMax = 2;//Установка максимального значения зоны отрисовки по x
                    yMin = -2;//Установка минимального значения зоны отрисовки по y
                    yMax = 2;//Установка максимального значения зоны отрисовки по y
                    f = new Font("Arial", 13);//Установка фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 640 && pictureBox1.Height + pictureBox1.Width <= 960)//Считаем периметр окна приложения и сравниваем с условием
                {
                    xMin = -4;//Установка минимального значения зоны отрисовки по x
                    xMax = 4;//Установка максимального значения зоны отрисовки по x
                    yMin = -4;//Установка минимального значения зоны отрисовки по y
                    yMax = 4;//Установка максимального значения зоны отрисовки по y
                    f = new Font("Arial", 12);//Установка фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 960 && pictureBox1.Height + pictureBox1.Width <= 1440)//Считаем периметр окна приложения и сравниваем с условием
                {
                    xMin = -6;//Установка минимального значения зоны отрисовки по x
                    xMax = 6;//Установка максимального значения зоны отрисовки по x
                    yMin = -6;//Установка минимального значения зоны отрисовки по y
                    yMax = 6;//Установка максимального значения зоны отрисовки по y
                    f = new Font("Arial", 11);//Установка фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 1440 && pictureBox1.Height + pictureBox1.Width <= 2160)//Считаем периметр окна приложения и сравниваем с условием
                {
                    xMin = -8;//Установка минимального значения зоны отрисовки по x
                    xMax = 8;//Установка максимального значения зоны отрисовки по x
                    yMin = -8;//Установка минимального значения зоны отрисовки по y
                    yMax = 8;//Установка максимального значения зоны отрисовки по y
                    f = new Font("Arial", 10);//Установка фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 2160 && pictureBox1.Height + pictureBox1.Width <= 3240)//Считаем периметр окна приложения и сравниваем с условием
                {
                    xMin = -10;//Установка минимального значения зоны отрисовки по x
                    xMax = 10;//Установка максимального значения зоны отрисовки по x
                    yMin = -10;//Установка минимального значения зоны отрисовки по y
                    yMax = 10;//Установка максимального значения зоны отрисовки по y
                    f = new Font("Arial", 9);//Установка фонта
                    //was default settings                 
                }
            }
            if (comboBox1.SelectedIndex == 1)//для PolarSys(Полярной системы координат)
            {
                //coinNum = ((pictureBox1.Width / 2) + (pictureBox1.Height / 2)) / 1.3;
                //coinDistance = ((pictureBox1.Width / 10) + (pictureBox1.Height / 10)) / 2;
                //coinStep = (pictureBox1.Width / 12 + pictureBox1.Height / 12) / 2;
                //multiDeg = 10;
                //multiСhart = 48;
                //f = new Font("Arial", 9);
                ////default settings
                if (pictureBox1.Height + pictureBox1.Width <= 800)//Считаем периметр окна приложения и сравниваем с условием
                {
                    coinNum = ((pictureBox1.Width / 2) + (pictureBox1.Height / 2)) / 1;//Установка количества колец, которые будут отрисованы
                    coinDistance = ((pictureBox1.Width / 10) + (pictureBox1.Height / 10)) / 0.5;//Установка диаметра кольца
                    //coinStep = (pictureBox1.Width / 12 + pictureBox1.Height / 12) / 0.35;
                    coinStep = coinDistance;//Установка шага между кольцами
                    multiDeg = 5;//Установка множителя, влияющего на дальность отрисовки подписей градусных осей
                    multiСhart = 12;//Установка множителя, влияющего на масштаб графика
                    f = new Font("Arial", 10);//Установка нового фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 800 && pictureBox1.Height + pictureBox1.Width <= 1200)//Считаем периметр окна приложения и сравниваем с условием
                {
                    coinNum = ((pictureBox1.Width / 2) + (pictureBox1.Height / 2)) / 1.1;//Установка количества колец, которые будут отрисованы
                    coinDistance = ((pictureBox1.Width / 10) + (pictureBox1.Height / 10)) / 1;//Установка диаметра кольца
                    coinStep = coinDistance;//Установка шага между кольцами
                    multiDeg = 6;//Установка множителя, влияющего на дальность отрисовки подписей градусных осей
                    multiСhart = 24;//Установка множителя, влияющего на масштаб графика
                    f = new Font("Arial", 11);//Установка нового фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 1200 && pictureBox1.Height + pictureBox1.Width <= 1600)//Считаем периметр окна приложения и сравниваем с условием
                {
                    coinNum = ((pictureBox1.Width / 2) + (pictureBox1.Height / 2)) / 1.2;//Установка количества колец, которые будут отрисованы
                    coinDistance = ((pictureBox1.Width / 10) + (pictureBox1.Height / 10)) / 1.5;//Установка диаметра кольца
                    coinStep = coinDistance;//Установка шага между кольцами
                    multiDeg = 7;//Установка множителя, влияющего на дальность отрисовки подписей градусных осей
                    multiСhart = 36;//Установка множителя, влияющего на масштаб графика
                    f = new Font("Arial", 12);//Установка нового фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 1600 && pictureBox1.Height + pictureBox1.Width <= 2000)//Считаем периметр окна приложения и сравниваем с условием
                {
                    coinNum = ((pictureBox1.Width / 2) + (pictureBox1.Height / 2)) / 1.3;//Установка количества колец, которые будут отрисованы
                    coinDistance = ((pictureBox1.Width / 10) + (pictureBox1.Height / 10)) / 2;//Установка диаметра кольца
                    coinStep = coinDistance;//Установка шага между кольцами
                    multiDeg = 8;//Установка множителя, влияющего на дальность отрисовки подписей градусных осей
                    multiСhart = 48;//Установка множителя, влияющего на масштаб графика
                    f = new Font("Arial", 13);//Установка нового фонта
                }
                else if (pictureBox1.Height + pictureBox1.Width > 2000)//Считаем периметр окна приложения и сравниваем с условием
                {
                    coinNum = ((pictureBox1.Width / 2) + (pictureBox1.Height / 2)) / 1.4;//Установка количества колец, которые будут отрисованы
                    coinDistance = ((pictureBox1.Width / 10) + (pictureBox1.Height / 10)) / 2.5;//Установка диаметра кольца
                    coinStep = coinDistance;//Установка шага между кольцами
                    multiDeg = 9;//Установка множителя, влияющего на дальность отрисовки подписей градусных осей
                    multiСhart = 60;//Установка множителя, влияющего на масштаб графика
                    f = new Font("Arial", 13);//Установка нового фонта
                }              
            }
            //if (comboBox1.SelectedIndex == 0)
            //{
            //    if (pictureBox1.Height + pictureBox1.Width <= 640)
            //    {
            //        xMin = -10;
            //        xMax = 10;
            //        yMin = -10;
            //        yMax = 10;
            //        f = new Font("Arial", 9);
            //        //default settings
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 640 && pictureBox1.Height + pictureBox1.Width <= 960)
            //    {
            //        xMin = -8;
            //        xMax = 8;
            //        yMin = -8;
            //        yMax = 8;
            //        f = new Font("Arial", 10);
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 960 && pictureBox1.Height + pictureBox1.Width <= 1440)
            //    {
            //        xMin = -6;
            //        xMax = 6;
            //        yMin = -6;
            //        yMax = 6;
            //        f = new Font("Arial", 11);
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 1440 && pictureBox1.Height + pictureBox1.Width <= 2160)
            //    {
            //        xMin = -4;
            //        xMax = 4;
            //        yMin = -4;
            //        yMax = 4;
            //        f = new Font("Arial", 12);
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 2160 && pictureBox1.Height + pictureBox1.Width <= 3240)
            //    {
            //        xMin = -2;
            //        xMax = 2;
            //        yMin = -2;
            //        yMax = 2;
            //        f = new Font("Arial", 13);
            //    }
            //}
            //if (comboBox1.SelectedIndex == 1)
            //{
            //    if (pictureBox1.Height + pictureBox1.Width <= 640)
            //    {
            //        coinNum = 200;
            //        f = new Font("Arial", 9);
            //        //default settings
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 640 && pictureBox1.Height + pictureBox1.Width <= 960)
            //    {
            //        coinNum = 400;
            //        f = new Font("Arial", 10);
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 960 && pictureBox1.Height + pictureBox1.Width <= 1440)
            //    {
            //        coinNum = 600;
            //        f = new Font("Arial", 11);
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 1440 && pictureBox1.Height + pictureBox1.Width <= 2160)
            //    {
            //        coinNum = 800;
            //        f = new Font("Arial", 12);
            //    }
            //    else if (pictureBox1.Height + pictureBox1.Width > 2160 && pictureBox1.Height + pictureBox1.Width <= 3240)
            //    {
            //        coinNum = 1000;
            //        f = new Font("Arial", 13);
            //    }
            //}
        }

        private void button1_Click(object sender, EventArgs e)//Эвент нажатия кнопки button1
        {
            if (isOn == false)//Если кнопка нажата
            {
                try
                {
                    a = Convert.ToDouble(textBox1.Text);//Попытка конвертировать значение из textBox1.Text в параметрическую переменную a
                }
                catch (System.FormatException)//Системное исключение
                {
                    textBox1.BackColor = Color.Red;//Перекрашивание textBox1 в красный
                    MessageBox.Show("Проверьте правильность введенных данных в красном поле!\nВводить можно только числа с плавающей запятой!");//Ошибка!
                    textBox1.BackColor = Color.White;//Перекрашивание textBox1 в белый
                    return;//Выход из функции
                }
            }
            if (a > -0.01 && a < 0.01 && isOn == false)//Проверка на очень маленькое значение a(Несовершенство технологии построения графика :С)
            {
                textBox1.BackColor = Color.Yellow;//Перекрашивание textBox1 в желтый
                MessageBox.Show("Слишком маленькое число заданно в желтом поле, могут возникнуть проблемы с отображением!");//Предупреждение об исходном результате вычислений
                textBox1.BackColor = Color.White;//Перекрашивание textBox1 в белый
            }
            if (isOn == false)//Если кнопка нажата
            {
                groupBox1.Visible = true;//Включение видимости для groupBox1
                button2.Visible = true;//Включение видимости для button2
                button3.Visible = true;//Включение видимости для button3
            }
            else
            {
                groupBox1.Visible = false;//Выключение видимости для groupBox1
                button2.Visible = false;//Выключение видимости для button2
                button3.Visible = false;//Выключение видимости для button3
            }

            if (isOn == true && comboBox1.SelectedIndex == 0) { x.Clear(); y.Clear(); button1.Text = "Create"; textBox1.Enabled = true; button3.Enabled = false; pictureBox1.Image = SimpleDecart(); isOn = false; rost = 0; button2.Text = "Start"; return; }
            if (isOn == true && comboBox1.SelectedIndex == 1) { x.Clear(); y.Clear(); button1.Text = "Create"; textBox1.Enabled = true; button3.Enabled = false; pictureBox1.Image = SimplePolar(); isOn = false; rost = 0; button2.Text = "Start"; return; }
            if (isOn == false && comboBox1.SelectedIndex == 0) { x.Clear(); y.Clear(); button1.Text = "Destroy"; textBox1.Enabled = false; pictureBox1.Image = DecartSys(); isOn = true; return; }
            if (isOn == false && comboBox1.SelectedIndex == 1) { x.Clear(); y.Clear(); button1.Text = "Destroy"; textBox1.Enabled = false; pictureBox1.Image = PolarSys(); isOn = true; return; }
            //Условия, проверяющие какие действия совершил пользователь прежде чем нажать на кнопку, и ответные действия программы на его запросы и последующий выход из программы
        }

        private void Form1_Resize(object sender, EventArgs e)//Эвент изменения размеров формы
        {
            if (comboBox1.SelectedIndex == 0 && isOn==true)//Если пользователь выбрал "Декартовая система координат" и нажал на кнопку построить график
            {
                if (timer1.Enabled == false)//Если таймер выключен
                {
                    x.Clear();//Очищаем лист с координатами графика по x в декратовой системе координат
                    y.Clear();//Очищаем лист с координатами графика по y в декратовой системе координат
                    if (rost > 0)//Если синий квадрат в процессе анимации или на паузе
                    {
                        QuadDraw(rost);//Вычисляем координаты где находится синий квадрат, зная индекс массивов листов с его координатами
                        pictureBox1.Image = bp1;//Рисуем в pictureBox1 график с системой координат и синим квадратом
                        return;//Выход из функции
                    }
                    pictureBox1.Image = DecartSys();//Рисуем в pictureBox1 график с декартовой системой координат
                }
            }
            if (comboBox1.SelectedIndex == 1 && isOn == true)//Если пользователь выбрал "Полярная система координат" и нажал на кнопку построить график
            {
                if (timer1.Enabled == false)//Если таймер выключен
                {
                    x.Clear();//Очищаем лист с координатами графика по x в декратовой системе координат
                    y.Clear();//Очищаем лист с координатами графика по y в декратовой системе координат
                    if (rost > 0)//Если синий квадрат в процессе анимации или на паузе
                    {
                        QuadDraw(rost);//Вычисляем координаты где находится синий квадрат, зная индекс массивов листов с его координатами
                        pictureBox1.Image = bp1;//Рисуем в pictureBox1 график с системой координат и синим квадратом
                        return;//Выход из функции
                    }
                    pictureBox1.Image = PolarSys();//Рисуем в pictureBox1 график с полярной системой координат
                }
            }
            if (comboBox1.SelectedIndex == 0 && isOn == false)//Если пользователь выбрал "Декартовая система координат" и кнопка не нажата/отжата p.s. Отжатие - это когда на кнопку нажали дважды :)
            {
                x.Clear();//Очищаем лист с координатами графика по x в декратовой системе координат
                y.Clear();//Очищаем лист с координатами графика по y в декратовой системе координат
                pictureBox1.Image = SimpleDecart();//Рисуем в pictureBox1 декратовую систему координат без графика
            }
            if (comboBox1.SelectedIndex == 1 && isOn == false)//Если пользователь выбрал "Полярная система координат" и кнопка не нажата/отжата p.s. Отжатие - это когда на кнопку нажали дважды :)
            {
                if (timer1.Enabled == false)//Если таймер выключен
                {
                    x.Clear();//Очищаем лист с координатами графика по x в декратовой системе координат
                    y.Clear();//Очищаем лист с координатами графика по y в декратовой системе координат
                    if (rost > 0)//Если синий квадрат в процессе анимации или на паузе
                    {
                        QuadDraw(rost);//Вычисляем координаты где находится синий квадрат, зная индекс массивов листов с его координатами
                        pictureBox1.Image = bp1;//Рисуем в pictureBox1 график с системой координат и синим квадратом
                        return;//Выход из функции
                    }
                    pictureBox1.Image = SimplePolar();//Рисуем в pictureBox1 полярную систему координат без графика
                }
            }
        }

        private void QuadDraw(int count)//Функция для отрисовки синего квадрата в системах координат
        {
            bp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);//Считываем размеры для полотна из окна приложения
            Graphics graph = Graphics.FromImage(bp1);//Объявляем новый граф на полотне
            Pen pen = new Pen(Color.FromArgb(0, 0, 255, 0), 2);//Объявляем пен синего цвета

            if (comboBox1.SelectedIndex == 0)//Если пользователь выбрал "Декартовая система координат"
            {graph.DrawImage(DecartSys(), 0, 0, pictureBox1.Width, pictureBox1.Height); }//Рисуем декартовую систему координат на полотне
            if (comboBox1.SelectedIndex == 1)
            {graph.DrawImage(PolarSys(), 0, 0, pictureBox1.Width, pictureBox1.Height); }//Рисуем полярную систему координат на полотне

            Rectangle quad = new Rectangle((int)x[rost] - 5, (int)y[rost] - 5, 10, 10);//Объявляем квадрат с размерами 10 на 10(px)

            graph.FillRectangle(Brushes.Blue, quad);//Заполняем квадрат синим цветом
            graph.DrawRectangle(pen, quad);//Рисуем синий квадрат с синим конутром :D
            pictureBox1.Image = bp1;//Присваиваем полотно к pictureBox1       
        }
          
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)//Функция которая отвечает на действия пользователя с comboBox1
        {
            if (comboBox1.SelectedIndex == 0 && isOn==true) { x.Clear(); y.Clear(); Scale(); if (rost > 0) { QuadDraw(rost); pictureBox1.Image = bp1; return; } pictureBox1.Image = DecartSys(); return; };
            if (comboBox1.SelectedIndex == 0 && isOn == false) { x.Clear(); y.Clear(); Scale(); if (rost > 0) { QuadDraw(rost); pictureBox1.Image = bp1; return; } pictureBox1.Image = SimpleDecart(); return; };
            if (comboBox1.SelectedIndex == 1 && isOn ==true) { x.Clear(); y.Clear(); Scale(); if (rost > 0) { QuadDraw(rost);  pictureBox1.Image = bp1; return; } pictureBox1.Image = PolarSys(); return; };
            if (comboBox1.SelectedIndex == 1 && isOn == false) { x.Clear(); y.Clear(); Scale(); if (rost > 0) { QuadDraw(rost);  pictureBox1.Image = bp1; return; } pictureBox1.Image = SimplePolar(); return; };
        }

        private void timer1_Tick(object sender, EventArgs e)//Таймер
        {  
            if (isOn == false) { timer1.Stop(); pictureBox1.Image = SimpleDecart(); button2.Text = "Start"; return; }//Если кнопка "Построить график" не была нажата, тогда останавливаем таймер и рисуем декартовую систему координат без графика и выходим из функции
            QuadDraw(rost);//Рассчет координат синего квадрата по индексу
            rost++;//Увеличение индекса          
            if (rost > x.Count - 1)//Если количество координат синего квадрата больше количества координат графика
            {
                timer1.Stop();//Останавливаем таймер
                x.Clear();//Чистим лист с координатами графика в декартовой системе координат по x
                y.Clear();//Чистим лист с координатами графика в декартовой системе координат по y
                rost = 0;//Обнуляем индекс куба
                if (comboBox1.SelectedIndex == 0 && isOn == true)//Если пользователь выбрал "Декартовая система координат" и кнопка "Построить график" нажата
                {pictureBox1.Image = DecartSys();}//Строим график в декартовой системе координат
                if (comboBox1.SelectedIndex == 1)//Если пользователь выбрал "Полярная система координат" и кнопка "Построить график" нажата
                { pictureBox1.Image = PolarSys(); }//Строим график в полярной системе координат
                button2.Text = "Start";//Изменяем текс на кнопке button2
            }
            x.Clear();//Чистим лист с координатами графика в декартовой системе координат по x 
            y.Clear();//Чистим лист с координатами графика в декартовой системе координат по y          
        }

        Bitmap PolarSys()//Функция для постройки графика в полярной системе координат
        {
            Bitmap bp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//Считываем размеры для полотна из окна приложения
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 4);//Объявляем пен красного цвета, толщиной в 4 px
            //float heightr = pictureBox1.Height / 3;
            //float widthr = pictureBox1.Width / 3;
            Graphics graph = Graphics.FromImage(bp);//Объявляем новый граф на полотне
            int i = 1;//Индекс для листов
            graph.SmoothingMode = SmoothingMode.HighQuality;//Выставляем мод размытия для графа на повышенное качество
            Pen pen2 = new Pen(Color.FromArgb(255, 0, 0, 0), 3);//Объявляем пен красного цвета, толщиной в 3 px
            Pen pen3 = new Pen(Color.FromArgb(80, 23, 7, 240), 3);//Объявляем пен синеватого цвета, толщиной в 3 px
            Pen RedPen = new Pen(Color.FromArgb(70, 200, 0, 0), 3);//Объявляем пен красного полупрозрачного цвета, толщиной в 3 px
            dx = pictureBox1.Width / (xMax - xMin);//Высчитываем шаг между двумя соседними единицами измерения для оси x
            dy = pictureBox1.Height / (yMax - yMin);//Высчитываем шаг между двумя соседними единицами измерения для оси y
            x0 = -dx * xMin;//Высчитываем середину длины окна приложения
            y0 = dy * yMax;//Высчитываем середину ширины окна приложения
            x.Add(x0);//Чистим лист с координатами графика в декартовой системе координат по x 
            y.Add(y0);//Чистим лист с координатами графика в декартовой системе координат по y    
            // сетка вертикальная
            //for (int xe = xMin; xe <= xMax; xe++)
            //{
            //    graph.DrawLine(Pens.LightGray, x0 + xe * dx, 0, x0 + xe * dx, pictureBox1.Height);
            //}
            // сетка горизонтальная
            //for (int ye = yMin; ye <= yMax; ye++)
            //{
            //    graph.DrawLine(Pens.LightGray, 0, y0 - ye * dy, pictureBox1.Width, y0 - ye * dy);
            //} 
            //graph.DrawString("Y", f, Brushes.Black, x0 - 20, 0);
            int countkek = 0;//Переменная, которая используется для отрисовки подписей(Используется ее значение)
            //Font f = new Font("Arial", 9);
            for (int k = 0; k <= 11; k++)//Высчитывание координат, необходимых для построения линий под каждые 30 градусов
            {
                polarx.Add(x0 + (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Sin(Math.PI * k / 6)));
                polary.Add(y0 - (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Cos(Math.PI * k / 6)));
                polarx.Add(x0 - (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Sin(Math.PI * k / 6)));
                polary.Add(y0 + (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Cos(Math.PI * k / 6)));               
                graph.DrawLine(pen3, (int)polarx[0], (int)polary[0], (int)polarx[1], (int)polary[1]);//Рисуем линию
                //graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 - (int)polarx[1]/12, y0 - (int)polary[1]/12);
                //graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0-(int)polarx[0]/8, y0+(int)polary[0]/8);             
                //System.Console.WriteLine(Convert.ToString(polarx[0] + " bLYAT " + polary[0]));
                //System.Console.WriteLine(Convert.ToString(x0 + " CENTER " + y0));
                //Console.ReadKey();
                polarx.Clear();//Чистим лист с координатами графика в декартовой системе координат по x
                polary.Clear();//Чистим лист с координатами графика в декартовой системе координат по y
                //countkek+=30;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
            }
            //Подписываем оси
            countkek = 330;//Переменная, которая используется для отрисовки подписей(Используется ее значение)
            for (int k = 4; k <= 14; k++)//Высчитывание координат, необходимых для подписания линий под каждые 30 градусов
            {
                polarx.Add(x0 + (int)((pictureBox1.Width / multiDeg + pictureBox1.Height / multiDeg) * Math.Sin(Math.PI * k / 6)));
                polary.Add(y0 - (int)((pictureBox1.Width / multiDeg + pictureBox1.Height / multiDeg)  * Math.Cos(Math.PI * k / 6)));                              
                //graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 - (int)polarx[1]/12, y0 - (int)polary[1]/12);
                graph.DrawString(Convert.ToString(countkek)+ '°', f, Brushes.Black, (int)polarx[0], (int)polary[0]);//Подписывание градусов на линиях
                //System.Console.WriteLine(Convert.ToString(polarx[0] + " bLYAT " + polary[0]));
                //System.Console.WriteLine(Convert.ToString(x0 + " CENTER " + y0));
                //Console.ReadKey();
                polarx.Clear();//Чистим лист с координатами графика в декартовой системе координат по x
                polary.Clear();//Чистим лист с координатами графика в декартовой системе координат по y
                countkek-=30;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
            }          
            graph.DrawLine(pen2, 0, y0, pictureBox1.Width, y0);//Рисуем линию соответсвующую 0 градусов
            countkek = 1;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
            //float rofl = 0;//DEBUGsys
            Scale();//Подсчет необходимого масштаба в отношении окна приложения
            //CountCoin+= (pictureBox1.Width / 12 + pictureBox1.Height / 12)/2 //CountCoin <= ((pictureBox1.Width/2) + (pictureBox1.Height/2))/1.5
            for (float CountCoin= Convert.ToSingle(coinDistance); CountCoin <= coinNum; CountCoin+= Convert.ToSingle(coinStep))//Расчет и отрисовка колец
            {
                graph.DrawEllipse(RedPen, pictureBox1.Width / 2 - CountCoin/2, pictureBox1.Height / 2 - CountCoin/2, CountCoin, CountCoin);//Рисуем эллипс по заданным координатам
                graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 + CountCoin/2, pictureBox1.Height / 2 );//Подписываем каждое кольцо на оси, соответствующее 0 градусов
                countkek++;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
                //rofl = CountCoin;//debug here
            }
            countkek = 0;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
            //for (int xe = xMin; xe <= xMax; xe++)//положительные икс
            //{
            //    graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 + (x0 + xe * dx)*2, pictureBox1.Height / 2);
            //    countkek++;
            //}
            //graph.DrawEllipse(pen3, x0, y0, pictureBox1.Width , pictureBox1.Height );
            //30grad        
            //if (pictureBox1.Width == pictureBox1.Height) { polarx.Clear(); polary.Clear(); polarx.Add(pictureBox1.Width * 7/8); polary.Add(pictureBox1.Height * 3/4); System.Console.WriteLine(Convert.ToString(x0 + polarx[0]) + " " + Convert.ToString(y0 - polary[0])); }
            //graph.DrawLine(pen3, x0, y0, x0+(float)polarx[0], y0-(float)polary[0]);           
            //polarx.Clear(); polary.Clear();
            //60grad
            //if (polarx.Count <= 0) { polarx.Add(x0); polary.Add(y0); }
            //if (pictureBox1.Width == pictureBox1.Height) { polarx.Clear(); polary.Clear(); polarx.Add(pictureBox1.Width/4); polary.Add(pictureBox1.Height); System.Console.WriteLine(Convert.ToString(x0 + polarx[0]) + " " + Convert.ToString(y0 - polary[0])); }
            //graph.DrawLine(pen3, x0, y0, x0 + (float)polarx[0], y0 - (float)polary[0]);
            //polarx.Clear(); polary.Clear();

            for (double fi = 0; fi <= Math.PI; fi += 0.01)//Построение самой кривой "Декартов лист"
            {
                x.Add(x0 + Math.Round((((pictureBox1.Width / multiСhart) + (pictureBox1.Height / multiСhart)) * 3 * a * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));//Заполянем лист x
                y.Add(y0 - Math.Round((((pictureBox1.Width / multiСhart) + (pictureBox1.Height / multiСhart)) * 3 * a * Math.Tan(fi) * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));//Заполянем лист y
                //Pen pen = new Pen(Color.Black, 5);
                //graph.DrawEllipse(pen, (float)x.Last(), (float)y.Last(), 1, 1);
                graph.DrawLine(new Pen(Color.FromArgb(255, 0, 0, 0), 3), (float)x[i - 1], (float)y[i - 1], (float)x[i], (float)y[i]);//Соединям точки
                i++;//Увеличиваем индекс листа
            }
            return bp;//Возвращаем полотно
        }

        private void button3_Click(object sender, EventArgs e)//Эвент нажатия на кнопку button3
        {
            rost=0;//Обнуляем путь для куба
            button3.Enabled = false;//Делаем кнопку обнуления недоступной для пользователя
            button2.Text = "Start";//Изменяем названия кнопки
            timer1.Stop();//Останавливаем таймер
            if (comboBox1.SelectedIndex == 0 && isOn==true)//Если пользователь выбрал "Декартовая система координат" и кнопка "Построить график" была нажата
            { pictureBox1.Image = DecartSys(); }//Присваиваем полотно с графиком в декартовой системе координат к pictureBox1
            if (comboBox1.SelectedIndex == 1 && isOn==true)//Если пользователь выбрал "Полярная система координат" и кнопка "Построить график" была нажата
            { pictureBox1.Image = PolarSys(); }//Присваиваем полотно с графиком в полярной системе координат к pictureBox1
        }

        Bitmap DecartSys()//Рисование декартовой системы с Декартовым листом
        {           
                Bitmap bp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//Считываем размеры для полотна из окна приложения
                Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 4);//Объявляем пен красного цвета, толщиной в 4 px            

                int i = 1;//Индекс для листов            

                Graphics graph = Graphics.FromImage(bp);//Объявляем новый граф на полотне
                graph.SmoothingMode = SmoothingMode.HighQuality;//Выставляем мод размытия для графа на повышенное качество
                Pen pen2 = new Pen(Color.FromArgb(255, 0, 0, 0), 3);//Объявляем пен красного цвета, толщиной в 3 px               
                dx = pictureBox1.Width / (xMax - xMin);//Высчитываем шаг между двумя соседними единицами измерения для оси x
                dy = pictureBox1.Height / (yMax - yMin);//Высчитываем шаг между двумя соседними единицами измерения для оси y
                x0 = -dx * xMin;//Высчитываем середину длины окна приложения
                y0 = dy * yMax;//Высчитываем середину ширины окна приложения
                x.Add(x0);//Чистим лист с координатами графика в декартовой системе координат по x
                y.Add(y0);//Чистим лист с координатами графика в декартовой системе координат по y
                Scale();//масштабирование           
                int countkek = 0;//Переменная, которая используется для отрисовки подписей(Используется ее значение)               
                for (int xe = xMin; xe <= xMax; xe++)//сетка вертикальная
                {
                    graph.DrawLine(Pens.LightGray, x0 + xe * dx, 0, x0 + xe * dx, pictureBox1.Height);
                }               
                for (int ye = yMin; ye <= yMax; ye++)//сетка горизонтальная
                {
                    graph.DrawLine(Pens.LightGray, 0, y0 - ye * dy, pictureBox1.Width, y0 - ye * dy);
                }
                pen2.CustomEndCap = new AdjustableArrowCap(2.5f, 2.5f);//Ставим стиль на пен с концом прямой в виде стрелочки                              
                graph.DrawLine(pen2, 0, y0, pictureBox1.Width, y0);// ось абсцисс
                for (int xe = xMin; xe <= xMax; xe++)//подписываем положительные икс
                {
                    graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 + (x0 + xe * dx), pictureBox1.Height / 2+3);
                    countkek++;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
                }
                countkek = 0;//Обнуление переменной, которая используется для отрисовки подписей(Используется ее значение)
                for (int xe = xMin; xe <= xMax; xe++)//подписываем отрицательные икс
                {
                    graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 - (x0 + xe * dx), pictureBox1.Height / 2+3);
                    countkek--;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
                }
                countkek = 1;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
                for (int ye = yMin; ye <= yMax; ye++)//подписываем положительные игрек
                {
                    graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0+5, y0 - dy - (y0 + ye * dy));
                    countkek++;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
                }
                countkek = -1;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
                for (int ye = yMin; ye <= yMax; ye++)//подписываем отрицательные игрек
                {
                    graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0+5, y0 + dy + (y0 + ye * dy));
                    countkek--;//Изменение переменной, которая используется для отрисовки подписей(Используется ее значение)
                }

                //Подписываем ось ординат
                graph.DrawLine(pen2, x0, pictureBox1.Height, x0, 0);

                graph.DrawString("Y", f, Brushes.Black, x0 - 20, 0);
                graph.DrawString("X", f, Brushes.Black, pictureBox1.Width - 25, y0 + 10);

            if (a == 0)//Если a равно нулю, то нарисовать прямую с очень маленькой петлей[Придумать что-то другое!!!]
            {
                for (double fi = 0; fi <= Math.PI; fi += 0.01)
                {
                    x.Add(x0 + Math.Round((dx * 3 * 0.001 * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));//Заполнение листа
                    y.Add(y0 - Math.Round((dy * 3 * 0.001 * Math.Tan(fi) * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));//Заполнение листа
                    graph.DrawLine(new Pen(Color.FromArgb(255, 0, 0, 0), 3), (float)x[i - 1], (float)y[i - 1], (float)x[i], (float)y[i]);//Соединяем точки
                    i++;//Изменение индекса листа
                }
            }
            else//Иначе построить кривую в декартовой системе координат
            {
                for (double fi = 0; fi <= Math.PI; fi += 0.01)
                {
                    x.Add(x0 + Math.Round((dx * 3 * a * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));//Заполнение листа
                    y.Add(y0 - Math.Round((dy * 3 * a * Math.Tan(fi) * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));//Заполнение листа
                    graph.DrawLine(new Pen(Color.FromArgb(255, 0, 0, 0), 3), (float)x[i - 1], (float)y[i - 1], (float)x[i], (float)y[i]);//Соединяем точки
                    i++;//Изменение индекса листа
                }
            }
            return bp;//Вернуть полотно           
        }


        private void button2_Click(object sender, EventArgs e)//Эвент нажатия на кнопку button2
        {
            if (timer1.Enabled == false && isOn == true)//Если таймер выключен и кнопка "Построить график" нажата
            {
                button2.Text = "Stop";//Меняем текст на кнопке button2
                button3.Enabled = true;//Делаем кнопку button3 доступной
                y.Clear(); x.Clear();//Очищаем листы с координатами графика в декартовой системе координат
                timer1.Start();//Запускаем таймер
            }
            else if (timer1.Enabled == true && isOn == true && rost > 0)//Если таймер включен и кнопка "Построить график" нажата и синий квадрат запущен/на паузе
            {
                button2.Text = "Resume";//Меняем текст на кнопке button2
                y.Clear(); x.Clear();//Очищаем листы с координатами графика в декартовой системе координат
                timer1.Stop();//Останавливаем таймер
            }
            else//Иначе
            {
                button2.Text = "Start";//Меняем текст на кнопке button2               
                y.Clear(); x.Clear();//Очищаем листы с координатами графика в декартовой системе координат
                timer1.Stop();//Останавливаем таймер
            }           
        }

        private void Button4_Click(object sender, EventArgs e)//Эвент нажатия кнопки button4
        {
            if (isVisible == true)//Если кнопка button4 нажата
            {
                label2.Visible = false;
                textBox1.Visible = false;
                button1.Visible = false;
                comboBox1.Visible = false;
                //label3.Visible = false;
                groupBox1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                isVisible = false;
                //Выключаем видимость для некоторых элементов интерфейса
            }
            else if (isVisible == false && isOn == true)//Если кнопка не нажата/отжата и кнопка "Построить график" нажата
            {
                label2.Visible = true;
                textBox1.Visible = true;
                button1.Visible = true;
                comboBox1.Visible = true;
                //label3.Visible = true;
                groupBox1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                isVisible = true;
                //Включаем видимость для некоторых элементов интерфейса
            }
            else if (isVisible == false && isOn == false)//Если кнопка не нажата/отжата
            {
                label2.Visible = true;
                textBox1.Visible = true;
                button1.Visible = true;
                comboBox1.Visible = true;
                isVisible = true;
                //Включаем видимость для некоторых элементов интерфейса
            }
            if (isVisible) { button4.Text = "Hide"; }//Если кнопка не нажата то ставим ей название "Hide"
            else if (!isVisible) { button4.Text = "Return"; }//Если кнопка нажата то ставим ей название "Return"
        }

        Bitmap SimpleDecart()//Функция которая строит декартовую систему координат без графика
        {
            Bitmap bp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//Объявляем новое полотно по размерам окна

            Graphics graph = Graphics.FromImage(bp);//Объявляем новый граф на полотне
            graph.SmoothingMode = SmoothingMode.HighQuality;//Включаем мод для графа на высокое качество сглаживания
            Pen pen2 = new Pen(Color.FromArgb(255, 0, 0, 0), 3);//Объявляем новый пен черного цвета толщиной в 3 пикселя
            dx = pictureBox1.Width / (xMax - xMin);//Высчитываем шаг между двумя соседними единицами измерения для оси x
            dy = pictureBox1.Height / (yMax - yMin);//Высчитываем шаг между двумя соседними единицами измерения для оси y
            x0 = -dx * xMin;//Высчитываем середину длины экрана
            y0 = dy * yMax;//Высчитываем середины высоты экрана
            x.Add(x0);//Чистим лист с координатами графика в декартовой системе координат по x
            y.Add(y0);//Чистим лист с координатами графика в декартовой системе координат по y
            Scale();//Функция высчитывающая коэфициенты для масштабирования
            int countkek = 0;//Переменная, необходимая для подписей на интерфейсе

            // сетка вертикальная
            for (int xe = xMin; xe <= xMax; xe++)
            {
                graph.DrawLine(Pens.LightGray, x0 + xe * dx, 0, x0 + xe * dx, pictureBox1.Height);
            }
            // сетка горизонтальная
            for (int ye = yMin; ye <= yMax; ye++)
            {
                graph.DrawLine(Pens.LightGray, 0, y0 - ye * dy, pictureBox1.Width, y0 - ye * dy);
            }
            pen2.CustomEndCap = new AdjustableArrowCap(2.5f, 2.5f);//Пен с стрелочками в конце прямой
            
            graph.DrawLine(pen2, 0, y0, pictureBox1.Width, y0);// ось абсцисс
            for (int xe = xMin; xe <= xMax; xe++)//положительные икс
            {
                graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 + (x0 + xe * dx), pictureBox1.Height / 2+3);
                countkek++;
            }
            countkek = 0;
            for (int xe = xMin; xe <= xMax; xe++)//отрицательные икс
            {
                graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 - (x0 + xe * dx), pictureBox1.Height / 2+3);
                countkek--;
            }
            countkek = 1;
            for (int ye = yMin; ye <= yMax; ye++)//положительные игрек
            {
                graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0+5, y0 - dy - (y0 + ye * dy));
                countkek++;
            }
            countkek = -1;
            for (int ye = yMin; ye <= yMax; ye++)//отрицательные игрек
            {
                graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0+5, y0 + dy + (y0 + ye * dy));
                countkek--;
            }

            // ось ординат
            graph.DrawLine(pen2, x0, pictureBox1.Height, x0, 0);

            // подписи
            graph.DrawString("Y", f, Brushes.Black, x0 - 20, 0);
            graph.DrawString("X", f, Brushes.Black, pictureBox1.Width - 25, y0 + 10);
         
            return bp;//Возвращаем полотно
        }

        Bitmap SimplePolar()//Функция рисующая полярную систему координат без графика
        {
            Bitmap bp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//Объявляем новый граф на полотне
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 4);//Объявляем новый пен черного цвета толщиной в 3 пикселя
            Graphics graph = Graphics.FromImage(bp);//Объявляем новый граф на полотне
            graph.SmoothingMode = SmoothingMode.HighQuality;//Включаем мод для графа на высокое качество сглаживания
            Pen pen2 = new Pen(Color.FromArgb(255, 0, 0, 0), 3);//Объявляем новый пен черного цвета толщиной в 3 пикселя
            Pen pen3 = new Pen(Color.FromArgb(80, 23, 7, 240), 3);//Объявляем новый пен синеватого цвета толщиной в 3 пикселя
            Pen RedPen = new Pen(Color.FromArgb(70, 200, 0, 0), 3);//Объявляем новый пен красноватого цвета толщиной в 3 пикселя
            dx = pictureBox1.Width / (xMax - xMin);//Высчитываем шаг между двумя соседними единицами измерения для оси x
            dy = pictureBox1.Height / (yMax - yMin);//Высчитываем шаг между двумя соседними единицами измерения для оси y;
            x0 = -dx * xMin;//Высчитывание середины длины окна
            y0 = dy * yMax;//Высчитываем середину ширины окна
            x.Add(x0);//Чистим лист с координатами графика в декартовой системе координат по x
            y.Add(y0);//Чистим лист с координатами графика в декартовой системе координат по y           
            for (int k = 0; k <= 11; k++)//Постройка прямых под каждые 30 градусов
            {
                polarx.Add(x0 + (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Sin(Math.PI * k / 6)));
                polary.Add(y0 - (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Cos(Math.PI * k / 6)));
                polarx.Add(x0 - (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Sin(Math.PI * k / 6)));
                polary.Add(y0 + (int)((pictureBox1.Width / 2 + pictureBox1.Height / 2) * Math.Cos(Math.PI * k / 6)));
                graph.DrawLine(pen3, (int)polarx[0], (int)polary[0], (int)polarx[1], (int)polary[1]);
                polarx.Clear();
                polary.Clear();
            }
            int countkek = 330;//Перменная, необходимая для подписей
            for (int k = 4; k <= 14; k++)//Подписи для прямых каждые 30 градусов
            {
                polarx.Add(x0 + (int)((pictureBox1.Width / multiDeg + pictureBox1.Height / multiDeg) * Math.Sin(Math.PI * k / 6)));
                polary.Add(y0 - (int)((pictureBox1.Width / multiDeg + pictureBox1.Height / multiDeg) * Math.Cos(Math.PI * k / 6)));
                graph.DrawString(Convert.ToString(countkek) + '°', f, Brushes.Black, (int)polarx[0], (int)polary[0]);
                polarx.Clear();
                polary.Clear();
                countkek -= 30;
            }
            // ось абсцисс
            graph.DrawLine(pen2, 0, y0, pictureBox1.Width, y0);//Линяя посередине экрана
            countkek = 1;
            float rofl = 0;
            Scale();//Масштабирование
            for (float CountCoin= Convert.ToSingle(coinDistance); CountCoin <= coinNum; CountCoin+= Convert.ToSingle(coinStep))//Функция для рисования колец
            {
                graph.DrawEllipse(RedPen, pictureBox1.Width / 2 - CountCoin / 2, pictureBox1.Height / 2 - CountCoin / 2, CountCoin, CountCoin);
                graph.DrawString(Convert.ToString(countkek), f, Brushes.Black, x0 + CountCoin / 2, pictureBox1.Height / 2);
                countkek++;
                rofl = CountCoin;
            }
            countkek = 0;                  
            return bp;
        }

        //private void DecartCyrve()
        //{
        //    Bitmap bp1;
        //    int i = 1;
        //    //bp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        //    bp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);//second image
        //    Graphics graph = Graphics.FromImage(bp1);
        //    Bitmap bp2 = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb);//result image
        //    Graphics graph1 = Graphics.FromImage(bp2);//Создаем объект Graphics из результирующего изображения
        //    double h = 0.001;
        //    graph.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
        //    List<double> x = new List<double>();
        //    List<double> y = new List<double>();
        //    x.Add(x0);
        //    y.Add(y0);
        //    for (double fi = 0; fi <= 4; fi += h)
        //    {
        //        x.Add(x0 + Math.Round((3 * 120 * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));
        //        y.Add(y0 - Math.Round((3 * 120 * Math.Tan(fi) * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));
        //        Pen pen = new Pen(Color.Black, 5);
        //        //graph.DrawEllipse(pen, (float)x.Last(), (float)y.Last(), 1, 1);
        //        graph.DrawLine(Pens.Black, (float)x[i - 1], (float)y[i - 1], (float)x[i], (float)y[i]);
        //        i++;
        //    }
        //    graph.DrawImage(bp1, 0, 0, pictureBox1.Width, pictureBox1.Height);
        //    pictureBox1.Image = bp1;
        //}

        //private void DecartCyrveTest()
        //{
        //    Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 4);
        //    float heightr = pictureBox1.Height / 3;
        //    float widthr = pictureBox1.Width / 3;

        //    double h = 0.001;
        //    x.Add(x0);
        //    y.Add(y0);
        //    //points.Add(new PointF(pictureBox1.Width / 2 + (200f * -1), pictureBox1.Height / 2 - (200 * (float)Math.Sqrt(Math.Pow(-1, 2f) * (-1 + 1)))));
        //    int i = 1;
        //    cub = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        //    Graphics graph1 = Graphics.FromImage(cub);
        //    for (double fi = 0; fi <= 4; fi += h)
        //    {
        //        x.Add(x0 + Math.Round((3 * heightr * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));
        //        y.Add(y0 - Math.Round((3 * widthr * Math.Tan(fi) * Math.Tan(fi)) / (1 + Math.Pow(Math.Tan(fi), 3))));
        //        //Pen pen = new Pen(Color.Black, 5);
        //        //graph.DrawEllipse(pen, (float)x.Last(), (float)y.Last(), 1, 1);
        //        graph1.DrawLine(Pens.Black, (float)x[i - 1], (float)y[i - 1], (float)x[i], (float)y[i]);
        //        i++;
        //    }
        //    dec = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        //    Graphics graph = Graphics.FromImage(dec);
        //    // сетка вертикальная
        //    for (int xe = xMin; xe <= xMax; xe++)
        //    {
        //        graph.DrawLine(Pens.LightGray, x0 + xe * dx, 0, x0 + xe * dx, pictureBox1.Height);
        //    }
        //    // сетка горизонтальная
        //    for (int ye = yMin; ye <= yMax; ye++)
        //    {
        //        graph.DrawLine(Pens.LightGray, 0, y0 - ye * dy, pictureBox1.Width, y0 - ye * dy);
        //    }

        //    pen.CustomEndCap = new AdjustableArrowCap(2.5f, 2.5f);

        //    // ось абсцисс
        //    graph.DrawLine(pen, 0, y0, pictureBox1.Width, y0);
        //    // ось ординат
        //    graph.DrawLine(pen, x0, pictureBox1.Height, x0, 0);
        //    graph.DrawImage(cub, 0, 0, pictureBox1.Width, pictureBox1.Height);
        //    // подписи
        //    Font f = new Font("Arial", 12);
        //    graph.DrawString("Y", f, Brushes.Black, x0 + 10, 0);
        //    graph.DrawString("X", f, Brushes.Black, pictureBox1.Width - 25, y0 + 10);

        //    pictureBox1.Image = dec;
        //}
    }   
}  
