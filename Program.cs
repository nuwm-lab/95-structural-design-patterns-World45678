using System;
using System.Collections.Generic;

namespace AdapterLab
{
    // -----------------------------------------------------------
    // 1. TARGET (Цільовий інтерфейс)
    // Це інтерфейс, який очікує наша нова 3D-система.
    // -----------------------------------------------------------
    public interface IPoint3D
    {
        double X { get; }
        double Y { get; }
        double Z { get; }
        void DisplayCoordinates();
    }

    // -----------------------------------------------------------
    // 2. ADAPTEE (Адаптовуваний клас)
    // Це старий клас або формат даних, який ми маємо (2D).
    // Він несумісний з IPoint3D, бо не має координати Z.
    // -----------------------------------------------------------
    public class Point2D
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Show2D()
        {
            Console.WriteLine($"2D Point: ({X}, {Y})");
        }
    }

    // -----------------------------------------------------------
    // 3. ADAPTER (Адаптер)
    // Цей клас робить 2D точку придатною для використання в 3D системі.
    // -----------------------------------------------------------
    public class Adapter2DTo3D : IPoint3D
    {
        private readonly Point2D _point2D;
        private readonly double _fixedZ;

        // Ми приймаємо 2D точку і опціонально фіксовану висоту Z (за замовчуванням 0)
        public Adapter2DTo3D(Point2D point2D, double fixedZ = 0.0)
        {
            _point2D = point2D;
            _fixedZ = fixedZ;
        }

        // Реалізація інтерфейсу IPoint3D
        public double X => _point2D.X; // Проксіюємо виклик до реального X
        public double Y => _point2D.Y; // Проксіюємо виклик до реального Y
        
        // Адаптація: вигадуємо Z, якого не існує в оригіналі
        public double Z => _fixedZ;    

        public void DisplayCoordinates()
        {
            // Виводимо як 3D, хоча дані взяті з 2D
            Console.WriteLine($"[3D Adapter] Point mapped to: ({X}, {Y}, {Z})");
        }
    }

    // -----------------------------------------------------------
    // 4. CLIENT (3D Система)
    // Цей клас вміє малювати тільки 3D точки. Він нічого не знає про Point2D.
    // -----------------------------------------------------------
    public class System3DPlotter
    {
        public void DrawGraph(List<IPoint3D> points)
        {
            Console.WriteLine("\n--- 3D System: Rendering Points ---");
            foreach (var point in points)
            {
                Console.Write("Rendering... ");
                point.DisplayCoordinates();
                
                // Тут могла б бути складна логіка рендерингу з використанням Z
                // if (point.Z > 0) { ... }
            }
            Console.WriteLine("-----------------------------------\n");
        }
    }

    // -----------------------------------------------------------
    // ЗАПУСК
    // -----------------------------------------------------------
    class Program
    {
        static void Main(string[] args)
        {
            // 1. У нас є дані у старому форматі (наприклад, зчитані з CSV файлу)
            var raw2DData = new List<Point2D>
            {
                new Point2D(1.5, 2.0),
                new Point2D(3.0, 4.5),
                new Point2D(5.1, 1.1)
            };

            // 2. У нас є 3D система
            var plotter = new System3DPlotter();

            // 3. Ми хочемо передати 2D дані в 3D систему.
            // plotter.DrawGraph(raw2DData); // Помилка компіляції! Типи несумісні.

            // 4. Використовуємо АДАПТЕР
            var adaptedPoints = new List<IPoint3D>();

            Console.WriteLine("Адаптація даних...");
            foreach (var p2d in raw2DData)
            {
                // Обгортаємо кожну 2D точку в адаптер
                // Z залишиться 0.0, тобто графік ляже на "підлогу" 3D сцени
                adaptedPoints.Add(new Adapter2DTo3D(p2d)); 
            }

            // Додамо одну точку на іншій висоті для прикладу (Z = 5.0)
            var floatingPoint = new Point2D(10, 10);
            adaptedPoints.Add(new Adapter2DTo3D(floatingPoint, 5.0));

            // 5. Тепер 3D система приймає наші дані
            plotter.DrawGraph(adaptedPoints);

            Console.ReadLine();
        }
    }
}
