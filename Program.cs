using second_dz;
using System;
using System.Collections.Generic;

namespace second_dz
{
    class Program
    {
        private const int SLOT_COUNT = 5;

        private static Shelf shelfA = new Shelf("A", SLOT_COUNT);
        private static Shelf shelfB = new Shelf("B", SLOT_COUNT);

        private static Journal<PlacedEvent> placedJournal = new Journal<PlacedEvent>();
        private static Journal<TakenEvent> takenJournal = new Journal<TakenEvent>();
        private static Journal<MovedEvent> movedJournal = new Journal<MovedEvent>();
        private static Journal<FailedAttemptEvent> failedJournal = new Journal<FailedAttemptEvent>();

        static void Main(string[] args)
        {
            LoadJournals();

            bool exit = false;

            while (!exit)
            {
                DisplayShelves();
                ShowMenu();

                int choice = ReadIntFromUser(1, 5, "Ваш выбор: ");

                switch (choice)
                {
                    case 1:
                        PutProduct();
                        break;
                    case 2:
                        TakeProduct();
                        break;
                    case 3:
                        MoveProduct();
                        break;
                    case 4:
                        ShowJournals();
                        break;
                    case 5:
                        SaveJournals();
                        Console.WriteLine("Сохранение журналов выполнено. пока");
                        exit = true;
                        break;
                }
            }
        }

        private static void LoadJournals()
        {
            placedJournal.LoadFromFile("placed.log", PlacedEvent.FromLogLine);
            takenJournal.LoadFromFile("taken.log", TakenEvent.FromLogLine);
            movedJournal.LoadFromFile("moved.log", MovedEvent.FromLogLine);
            failedJournal.LoadFromFile("failed.log", FailedAttemptEvent.FromLogLine);

            RestoreShelvesFromJournals();

            Console.WriteLine("Журналы загружены из файлов (или пусто, если первый запуск).");
        }

        private static void RestoreShelvesFromJournals()
        {
            for (int i = 1; i <= SLOT_COUNT; i++)
            {
                try { shelfA.TakeProduct(i); } catch { }
                try { shelfB.TakeProduct(i); } catch { }
            }

            var allPlaceEvents = new List<(DateTime time, string shelf, int slot, string product)>();
            foreach (var e in placedJournal.GetAll())
            {
                allPlaceEvents.Add((e.Time, e.Shelf, e.Slot, e.ProductName));
            }

            allPlaceEvents.Sort((a, b) => a.time.CompareTo(b.time));

            foreach (var ev in allPlaceEvents)
            {
                Shelf targetShelf = ev.shelf == "A" ? shelfA : shelfB;
                try
                {
                    targetShelf.PutProduct(ev.slot, ev.product);
                }
                catch { }
            }

            foreach (var e in takenJournal.GetAll())
            {
                Shelf targetShelf = e.Shelf == "A" ? shelfA : shelfB;
                try
                {
                    targetShelf.TakeProduct(e.Slot);
                }
                catch { }
            }

            foreach (var e in movedJournal.GetAll())
            {
                Shelf fromShelf = e.FromShelf == "A" ? shelfA : shelfB;
                Shelf toShelf = e.ToShelf == "A" ? shelfA : shelfB;
                try
                {
                    string product = fromShelf.TakeProduct(e.FromSlot);
                    toShelf.PutProduct(e.ToSlot, product);
                }
                catch { }
            }
        }

        private static void SaveJournals()
        {
            placedJournal.SaveToFile("placed.log");
            takenJournal.SaveToFile("taken.log");
            movedJournal.SaveToFile("moved.log");
            failedJournal.SaveToFile("failed.log");
        }

        private static void DisplayShelves()
        {
            Console.WriteLine("\n=== Склад ===");
            shelfA.Display();
            shelfB.Display();
            Console.WriteLine();
        }

        private static void ShowMenu()
        {
            Console.WriteLine("1 - Положить товар");
            Console.WriteLine("2 - Забрать товар");
            Console.WriteLine("3 - Перенести товар");
            Console.WriteLine("4 - Показать журналы");
            Console.WriteLine("5 - Выход");
        }

        private static int ReadIntFromUser(int min, int max, string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out int result) && result >= min && result <= max)
                    return result;
                Console.WriteLine($"Введите число от {min} до {max}.");
            }
        }

        private static string ReadShelfLetter(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.ToUpper();
                if (input == "A" || input == "B")
                    return input;
                Console.WriteLine("Введите A или B.");
            }
        }

        private static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(input))
                    return input;
                Console.WriteLine("Название товара не может быть пустым.");
            }
        }

        private static void PutProduct()
        {
            string shelfLetter = ReadShelfLetter("Полка (A или B): ");
            int slot = ReadIntFromUser(1, SLOT_COUNT, "Номер слота (1-5): ");
            string productName = ReadNonEmptyString("Название товара: ");

            Shelf shelf = shelfLetter == "A" ? shelfA : shelfB;

            if (!shelf.IsSlotEmpty(slot))
            {
                string product = shelf.GetProduct(slot);
                string errorMsg = product == null ? "слот занят" : $"слот уже занят товаром «{product}»";
                Console.WriteLine($"Нельзя положить: {errorMsg}");
                failedJournal.Add(new FailedAttemptEvent("Положить", shelfLetter, slot, errorMsg));
                return;
            }

            shelf.PutProduct(slot, productName);
            placedJournal.Add(new PlacedEvent(shelfLetter, slot, productName));
            Console.WriteLine("Операция выполнена.");
        }

        private static void TakeProduct()
        {
            string shelfLetter = ReadShelfLetter("Полка (A или B): ");
            int slot = ReadIntFromUser(1, SLOT_COUNT, "Номер слота (1-5): ");

            Shelf shelf = shelfLetter == "A" ? shelfA : shelfB;

            if (shelf.IsSlotEmpty(slot))
            {
                string errorMsg = "слот пуст";
                Console.WriteLine($"Нельзя забрать: {errorMsg}");
                failedJournal.Add(new FailedAttemptEvent("Забрать", shelfLetter, slot, errorMsg));
                return;
            }

            string product = shelf.TakeProduct(slot);
            takenJournal.Add(new TakenEvent(shelfLetter, slot, product));
            Console.WriteLine($"Забран товар: {product}");
        }

        private static void MoveProduct()
        {
            string fromShelfLetter = ReadShelfLetter("Полка-источник (A или B): ");
            int fromSlot = ReadIntFromUser(1, SLOT_COUNT, "Слот-источник (1-5): ");
            string toShelfLetter = ReadShelfLetter("Полка-назначение (A или B): ");
            int toSlot = ReadIntFromUser(1, SLOT_COUNT, "Слот-назначение (1-5): ");

            Shelf fromShelf = fromShelfLetter == "A" ? shelfA : shelfB;
            Shelf toShelf = toShelfLetter == "A" ? shelfA : shelfB;

            if (fromShelf.IsSlotEmpty(fromSlot))
            {
                string errorMsg = "слот-источник пуст";
                Console.WriteLine($"Нельзя перенести: {errorMsg}");
                failedJournal.Add(new FailedAttemptEvent("Перенести", fromShelfLetter, fromSlot, errorMsg));
                return;
            }

            if (!toShelf.IsSlotEmpty(toSlot))
            {
                string product = toShelf.GetProduct(toSlot);
                string errorMsg = product == null ? "слот-назначение занят" : $"слот-назначение уже занят товаром «{product}»";
                Console.WriteLine($"Нельзя перенести: {errorMsg}");
                failedJournal.Add(new FailedAttemptEvent("Перенести", toShelfLetter, toSlot, errorMsg));
                return;
            }

            string movedProduct = fromShelf.TakeProduct(fromSlot);
            toShelf.PutProduct(toSlot, movedProduct);
            movedJournal.Add(new MovedEvent(fromShelfLetter, fromSlot, toShelfLetter, toSlot, movedProduct));
            Console.WriteLine($"Операция выполнена. Перенесён товар: {movedProduct}");
        }

        private static void ShowJournals()
        {
            Console.WriteLine("\n--- Размещения ---");
            foreach (var e in placedJournal.GetAll())
                Console.WriteLine(e.ToScreenLine());

            Console.WriteLine("\n--- Изъятия ---");
            foreach (var e in takenJournal.GetAll())
                Console.WriteLine(e.ToScreenLine());

            Console.WriteLine("\n--- Переносы ---");
            foreach (var e in movedJournal.GetAll())
                Console.WriteLine(e.ToScreenLine());

            Console.WriteLine("\n--- Неуспешные попытки ---");
            foreach (var e in failedJournal.GetAll())
                Console.WriteLine(e.ToScreenLine());

            Console.WriteLine();
        }
    }
}