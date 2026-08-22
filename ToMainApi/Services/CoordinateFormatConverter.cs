using ToMainApi.Interfaces;
using ToMainApi.Models.Coordinates;

namespace ToMainApi.Services
{
    public class CoordinateFormatConverter : ICoordinateFormatConverterService
    {
        public DmsCoordinate ToLatitude(double latitude)
        {
            return ToDms(latitude, isLatitude: true);
        }

        public DmsCoordinate ToLongitude(double longitude)
        {
            return ToDms(longitude, isLatitude: false);
        }

        private DmsCoordinate ToDms(double value, bool isLatitude)
        {
            var abs = Math.Abs(value);

            int degrees = (int)Math.Floor(abs);
            double minutesFull = (abs - degrees) * 60;
            int minutes = (int)Math.Floor(minutesFull);
            double seconds = (minutesFull - minutes) * 60;

            char refChar;

            if (isLatitude)
                refChar = value >= 0 ? 'N' : 'S';
            else
                refChar = value >= 0 ? 'E' : 'W';

            return new DmsCoordinate
            {
                Degrees = degrees,
                Minutes = minutes,
                Seconds = Math.Round(seconds, 2),
                Reference = refChar
            };
        }

        public double ToDecimalLatitude(DmsCoordinate dms)
        {
            return ToDecimal(dms, isLatitude: true);
        }

        public double ToDecimalLongitude(DmsCoordinate dms)
        {
            return ToDecimal(dms, isLatitude: false);
        }

        private double ToDecimal(DmsCoordinate dms, bool isLatitude)
        {
            double value =
                dms.Degrees +
                dms.Minutes / 60.0 +
                dms.Seconds / 3600.0;

            bool isNegative =
                dms.Reference == 'S' || dms.Reference == 'W';

            return isNegative ? -value : value;
        }
    }
}
