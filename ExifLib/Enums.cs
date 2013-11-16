using System;

namespace ExifLib
{
    public enum ExifOrientation
    {
        TopLeft = 1,
        BottomRight = 3,
        TopRight = 6,
        BottomLeft = 8,
        Undefined = 9
    }

    public enum ExifUnit
    {
        Undefined = 1,
        Inch = 2,
        Centimeter = 3
    }

    [Flags]
    public enum ExifFlash
    {
        No = 0,
        Fired = 1,
        StrobeReturnLightDetected = 6,
        On = 8,
        Off = 16,
        Auto = 24,
        FlashFunctionPresent = 32,
        RedEyeReduction = 64
    }

    public enum ExifGpsLatitudeRef
    {
        Unknown,
        North,
        South
    }

    public enum ExifGpsLongitudeRef
    {
        Unknown,
        East,
        West
    }

    public enum ExifTagFormat
    {
        BYTE = 1,
        STRING = 2,
        USHORT = 3,
        ULONG = 4,
        URATIONAL = 5,
        SBYTE = 6,
        UNDEFINED = 7,
        SSHORT = 8,
        SLONG = 9,
        SRATIONAL = 10,
        SINGLE = 11,
        DOUBLE = 12,
        NUM_FORMATS = 12
    }

    public enum ExifId
    {
        Unknown = -1,
        ImageWidth = 256,
        ImageHeight = 257,
        Description = 270,
        Make = 271,
        Model = 272,
        Orientation = 274,
        XResolution = 282,
        YResolution = 283,
        ResolutionUnit = 296,
        Software = 305,
        DateTime = 306,
        Artist = 315,
        ThumbnailOffset = 513,
        ThumbnailLength = 514,
        Copyright = 33432,
        ExposureTime = 33434,
        FNumber = 33437,
        FlashUsed = 37385,
        UserComment = 37510
    }

    public enum ExifIFD
    {
        Exif = 34665,
        Gps = 34853
    }

    public enum ExifGps
    {
        Version,
        LatitudeRef,
        Latitude,
        LongitudeRef,
        Longitude,
        AltitudeRef,
        Altitude,
        TimeStamp,
        Satellites,
        Status,
        MeasureMode,
        DOP,
        SpeedRef,
        Speed,
        TrackRef,
        Track,
        ImgDirectionRef,
        ImgDirection,
        MapDatum,
        DestLatitudeRef,
        DestLatitude,
        DestLongitudeRef,
        DestLongitude,
        DestBearingRef,
        DestBearing,
        DestDistanceRef,
        DestDistance,
        ProcessingMethod,
        AreaInformation,
        DateStamp,
        Differential
    }
}