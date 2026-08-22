namespace ToMainApi.Models.Coordinates
{
    public class DmsCoordinate
    {
        public int Degrees { get; init; }

        public int Minutes { get; init; }

        public double Seconds { get; init; }

        public char Reference { get; init; }
    }
}
