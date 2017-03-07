using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Microsoft.MapPoint;

public enum MaptileGridSize
{
    OneByOne,
    ThreeByThree,
    FiveByFive
}

public class MaptileImporter
{
    public string imageTileUrlFormat = "http://ecn.t1.tiles.virtualearth.net/tiles/a{0}?g=5552";

    public MaptileImportConfiguration maptileServerConfig;
    private HeightmapImporter m_heightmapImporter;

    public MaptileImporter(MaptileImportConfiguration config)
    {
        maptileServerConfig = config;
        m_heightmapImporter = new HeightmapImporter(maptileServerConfig);
    }

    private static void LatLongToTileXY(double latitude, double longitude, int levelOfDetail, out int tileX, out int tileY)
    {
        int pixelX, pixelY;
        TileSystem.LatLongToPixelXY(latitude, longitude, levelOfDetail, out pixelX, out pixelY);
        TileSystem.PixelXYToTileXY(pixelX, pixelY, out tileX, out tileY);
    }

    private static void TileXYToLatLong(int tileX, int tileY, int levelOfDetail, out double latitude, out double longitude)
    {
        int pixelX, pixelY;
        TileSystem.TileXYToPixelXY(tileX, tileY, out pixelX, out pixelY);
        TileSystem.PixelXYToLatLong(pixelX, pixelY, levelOfDetail, out latitude, out longitude);
    }

    private static string LatlongToHeightmap(double latitude, double longitude)
    {
        var swLat = (int)Math.Floor(latitude);
        var swLong = (int)Math.Floor(longitude);

        var latString = swLat < 0 ? "S" + (swLat * -1).ToString("N0") : "N" + swLat.ToString("N0");
        var longString = swLong < 0 ? "W" + (swLong * -1).ToString("N0") : "S" + swLong.ToString("N0");

        return latString + longString;
    }

    public IEnumerable<string> GetMaptileQuadkeys(double latitude, double longitude, MaptileGridSize gridSize)
    {
        int tileX, tileY;
        LatLongToTileXY(latitude, longitude, maptileServerConfig.imageTileLevelOfDetail, out tileX, out tileY);

        var tileOffset = (int)gridSize;
        for (var dx = -1 * tileOffset; dx <= tileOffset; dx++)
        {
            for (var dy = -1 * tileOffset; dy <= tileOffset; dy++)
            {
                yield return GetMaptileQuadkey(tileX + dx, tileY + dy);
            }
        }
    }

    public void CreateImageTile(string quadKey)
    {
        var webClient = new WebClient();
        var outputFile = quadKey + maptileServerConfig.GetWorkingDirectoryFileExtension(MaptileWorkingDirectory.Image);
        var outputPath = maptileServerConfig.GetFullFilePath(MaptileWorkingDirectory.Image, quadKey);

        webClient.DownloadFile(string.Format(imageTileUrlFormat, outputFile), outputPath);
    }

    public string GetMaptileQuadkey(int tileX, int tileY)
    {
        double swLat, swLong, neLat, neLong;
        TileXYToLatLong(tileX, tileY + 1, maptileServerConfig.imageTileLevelOfDetail, out swLat, out swLong);
        TileXYToLatLong(tileX + 1, tileY, maptileServerConfig.imageTileLevelOfDetail, out neLat, out neLong);

        var quadKey = TileSystem.TileXYToQuadKey(tileX, tileY, maptileServerConfig.imageTileLevelOfDetail);

        if (!File.Exists(maptileServerConfig.GetFullFilePath(MaptileWorkingDirectory.Heightmap, quadKey)))
        {
            var swHeightmapLat = Math.Floor(swLat);
            var swHeightmapLong = Math.Floor(swLong);
            var neHeightmapLat = Math.Floor(neLat);
            var neHeightmapLong = Math.Floor(neLong);

            var inputHeightmaps = new string[4];
            inputHeightmaps[0] = LatlongToHeightmap(swHeightmapLat, swHeightmapLong);

            if (swHeightmapLat < neHeightmapLat)
            {
                inputHeightmaps[1] = LatlongToHeightmap(swHeightmapLat + 1, swHeightmapLong);
            }
            if (swHeightmapLong < neHeightmapLong)
            {
                inputHeightmaps[2] = LatlongToHeightmap(swHeightmapLat, swHeightmapLong + 1);
            }
            if (swHeightmapLat < neHeightmapLat && swHeightmapLong < neHeightmapLong)
            {
                inputHeightmaps[3] = LatlongToHeightmap(swHeightmapLat + 1, swHeightmapLong + 1);
            }

            m_heightmapImporter.CreateHeightmap(inputHeightmaps, swLong, swLat, neLong, neLat, quadKey);
            CreateImageTile(quadKey);
        }

        return quadKey;
    }
}