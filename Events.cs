using second_dz;
using System;

namespace second_dz
{
    public class PlacedEvent : IJournalEntry
    {
        public string Shelf { get; }
        public int Slot { get; }
        public string ProductName { get; }
        public DateTime Time { get; }

        public PlacedEvent(string shelf, int slot, string productName)
        {
            Shelf = shelf;
            Slot = slot;
            ProductName = productName;
            Time = DateTime.Now;
        }

        private PlacedEvent(string shelf, int slot, string productName, DateTime time)
        {
            Shelf = shelf;
            Slot = slot;
            ProductName = productName;
            Time = time;
        }

        public string ToLogLine()
        {
            return $"{Time:yyyy-MM-dd HH:mm:ss}|{Shelf}|{Slot}|{ProductName}";
        }

        public string ToScreenLine()
        {
            return $"Размещение | полка {Shelf} | слот {Slot} | товар «{ProductName}»";
        }

        public static PlacedEvent FromLogLine(string line)
        {
            string[] parts = line.Split('|');
            DateTime time = DateTime.Parse(parts[0]);
            string shelf = parts[1];
            int slot = int.Parse(parts[2]);
            string productName = parts[3];
            return new PlacedEvent(shelf, slot, productName, time);
        }
    }

    public class TakenEvent : IJournalEntry
    {
        public string Shelf { get; }
        public int Slot { get; }
        public string ProductName { get; }
        public DateTime Time { get; }

        public TakenEvent(string shelf, int slot, string productName)
        {
            Shelf = shelf;
            Slot = slot;
            ProductName = productName;
            Time = DateTime.Now;
        }

        private TakenEvent(string shelf, int slot, string productName, DateTime time)
        {
            Shelf = shelf;
            Slot = slot;
            ProductName = productName;
            Time = time;
        }

        public string ToLogLine()
        {
            return $"{Time:yyyy-MM-dd HH:mm:ss}|{Shelf}|{Slot}|{ProductName}";
        }

        public string ToScreenLine()
        {
            return $"Изъятие | полка {Shelf} | слот {Slot} | товар «{ProductName}»";
        }

        public static TakenEvent FromLogLine(string line)
        {
            string[] parts = line.Split('|');
            DateTime time = DateTime.Parse(parts[0]);
            string shelf = parts[1];
            int slot = int.Parse(parts[2]);
            string productName = parts[3];
            return new TakenEvent(shelf, slot, productName, time);
        }
    }

    public class MovedEvent : IJournalEntry
    {
        public string FromShelf { get; }
        public int FromSlot { get; }
        public string ToShelf { get; }
        public int ToSlot { get; }
        public string ProductName { get; }
        public DateTime Time { get; }

        public MovedEvent(string fromShelf, int fromSlot, string toShelf, int toSlot, string productName)
        {
            FromShelf = fromShelf;
            FromSlot = fromSlot;
            ToShelf = toShelf;
            ToSlot = toSlot;
            ProductName = productName;
            Time = DateTime.Now;
        }

        private MovedEvent(string fromShelf, int fromSlot, string toShelf, int toSlot, string productName, DateTime time)
        {
            FromShelf = fromShelf;
            FromSlot = fromSlot;
            ToShelf = toShelf;
            ToSlot = toSlot;
            ProductName = productName;
            Time = time;
        }

        public string ToLogLine()
        {
            return $"{Time:yyyy-MM-dd HH:mm:ss}|{FromShelf}|{FromSlot}|{ToShelf}|{ToSlot}|{ProductName}";
        }

        public string ToScreenLine()
        {
            return $"Перенос | с {FromShelf}:{FromSlot} на {ToShelf}:{ToSlot} | товар «{ProductName}»";
        }

        public static MovedEvent FromLogLine(string line)
        {
            string[] parts = line.Split('|');
            DateTime time = DateTime.Parse(parts[0]);
            string fromShelf = parts[1];
            int fromSlot = int.Parse(parts[2]);
            string toShelf = parts[3];
            int toSlot = int.Parse(parts[4]);
            string productName = parts[5];
            return new MovedEvent(fromShelf, fromSlot, toShelf, toSlot, productName, time);
        }
    }

    public class FailedAttemptEvent : IJournalEntry
    {
        public string OperationType { get; }
        public string Shelf { get; }
        public int Slot { get; }  
        public bool HasSlot { get; }  
        public string Reason { get; }
        public DateTime Time { get; }

        public FailedAttemptEvent(string operationType, string shelf, int slot, string reason, bool hasSlot = true)
        {
            OperationType = operationType;
            Shelf = shelf;
            Slot = slot;
            HasSlot = hasSlot;
            Reason = reason;
            Time = DateTime.Now;
        }

        private FailedAttemptEvent(string operationType, string shelf, int slot, string reason, bool hasSlot, DateTime time)
        {
            OperationType = operationType;
            Shelf = shelf;
            Slot = slot;
            HasSlot = hasSlot;
            Reason = reason;
            Time = time;
        }

        public string ToLogLine()
        {
            string slotStr = HasSlot ? Slot.ToString() : "-";
            return $"{Time:yyyy-MM-dd HH:mm:ss}|{OperationType}|{Shelf}|{slotStr}|{Reason}";
        }

        public string ToScreenLine()
        {
            string slotInfo = HasSlot ? $" полка {Shelf} слот {Slot}" : $" полка {Shelf}";
            return $"Неудача | {OperationType} |{slotInfo} | причина: {Reason}";
        }

        public static FailedAttemptEvent FromLogLine(string line)
        {
            string[] parts = line.Split('|');
            DateTime time = DateTime.Parse(parts[0]);
            string operationType = parts[1];
            string shelf = parts[2];
            string slotStr = parts[3];
            bool hasSlot = slotStr != "-";
            int slot = hasSlot ? int.Parse(slotStr) : 0;
            string reason = parts[4];
            return new FailedAttemptEvent(operationType, shelf, slot, reason, hasSlot, time);
        }
    }
}