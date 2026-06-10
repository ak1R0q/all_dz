using System;

namespace second_dz
{
    public class Shelf
    {
        private readonly string _name;
        private readonly string[] _slots;  

        public Shelf(string name, int slotCount)
        {
            _name = name;
            _slots = new string[slotCount];
        }

        public int SlotCount => _slots.Length;

        public bool IsSlotEmpty(int slotNumber)
        {
            ValidateSlotNumber(slotNumber);
            return _slots[slotNumber - 1] == null;
        }

        public string GetProduct(int slotNumber)
        {
            ValidateSlotNumber(slotNumber);
            return _slots[slotNumber - 1];
        }

        public void PutProduct(int slotNumber, string productName)
        {
            ValidateSlotNumber(slotNumber);
            if (!IsSlotEmpty(slotNumber))
                throw new InvalidOperationException($"Слот {slotNumber} уже занят товаром «{_slots[slotNumber - 1]}»");
            _slots[slotNumber - 1] = productName;
        }

        public string TakeProduct(int slotNumber)
        {
            ValidateSlotNumber(slotNumber);
            if (IsSlotEmpty(slotNumber))
                throw new InvalidOperationException($"Слот {slotNumber} пуст");
            string product = _slots[slotNumber - 1];
            _slots[slotNumber - 1] = null;
            return product;
        }

        public void Display()
        {
            Console.Write($"Полка {_name}: ");
            for (int i = 0; i < _slots.Length; i++)
            {
                string content = _slots[i] ?? "пусто";
                Console.Write($"[{i + 1}] {content} ");
            }
            Console.WriteLine();
        }

        private void ValidateSlotNumber(int slotNumber)
        {
            if (slotNumber < 1 || slotNumber > _slots.Length)
                throw new ArgumentOutOfRangeException(nameof(slotNumber), $"Слот должен быть от 1 до {_slots.Length}");
        }
    }
}