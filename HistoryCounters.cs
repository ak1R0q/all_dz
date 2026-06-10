namespace chetv_dz
{
    public class HistoryCounters
    {
        public int Attack { get; set; }
        public int Heal { get; set; }
        public int Event { get; set; }
        public int CastCancelled { get; set; }
        public int CastSuccess { get; set; }

        public override string ToString()
        {
            return $"ATTACK={Attack}, HEAL={Heal}, EVENT={Event}, CAST_CANCELLED={CastCancelled}, CAST_SUCCESS={CastSuccess}";
        }
    }
}