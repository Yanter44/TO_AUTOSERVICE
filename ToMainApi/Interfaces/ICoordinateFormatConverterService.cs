using ToMainApi.Models.Coordinates;

namespace ToMainApi.Interfaces
{
    public interface ICoordinateFormatConverterService
    {
        DmsCoordinate ToLatitude(double latitude);
        DmsCoordinate ToLongitude(double longitude);

        double ToDecimalLatitude(DmsCoordinate dms);
        double ToDecimalLongitude(DmsCoordinate dms);
    }
}
