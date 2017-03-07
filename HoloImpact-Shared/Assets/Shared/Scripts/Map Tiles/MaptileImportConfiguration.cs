using System;

public enum MaptileWorkingDirectory
{
    Srtm,
    Mercator,
    Heightmap,
    Image
}

/// <summary>
/// Holds configuration for tile server directory setup 
/// and URIs for all necessary data sources.
/// </summary>
[Serializable]
public struct MaptileImportConfiguration
{
    [Serializable]
    public struct MaptileDirectoryEntry
    {
        public string directoryName;
        public string datafileExtension;

        public MaptileDirectoryEntry(string directoryName, string datafileExtension)
        {
            this.directoryName = directoryName;
            this.datafileExtension = datafileExtension;
        }
    }

    public string maptileDirectory;
    public string gdalDirectory;
    public string maptileServerAddress;
    public string imageryUrlFormat;

    public int outputTileSize;
    public int imageTileLevelOfDetail;
    public int gdalCacheMax;

    public MaptileDirectoryEntry srtmConfig, mercatorConfig, heightmapConfig, imageryConfig;

    public MaptileImportConfiguration(string maptileDirectory, string gdalDirectory)
    {
        this.maptileDirectory = maptileDirectory;
        this.gdalDirectory = gdalDirectory;

        maptileServerAddress = "http://172.16.80.1:7775";
        imageryUrlFormat = "http://ecn.t1.tiles.virtualearth.net/tiles/a{0}?g=-1";

        outputTileSize = 256;
        imageTileLevelOfDetail = 11;
        gdalCacheMax = 3000;

        srtmConfig = new MaptileDirectoryEntry("EPSG3857", ".tif");
        mercatorConfig = new MaptileDirectoryEntry("EPSG4326", ".tif");
        heightmapConfig = new MaptileDirectoryEntry("RAW", ".raw");
        imageryConfig = new MaptileDirectoryEntry("IMAGES", ".jpg");
    }

    private MaptileDirectoryEntry GetMaptileDirectory(MaptileWorkingDirectory workingDirectory)
    {
        switch (workingDirectory)
        {
            case MaptileWorkingDirectory.Srtm: return srtmConfig;
            case MaptileWorkingDirectory.Mercator: return mercatorConfig;
            case MaptileWorkingDirectory.Heightmap: return heightmapConfig;
            default: return imageryConfig;
        }
    }

    public string GetWorkingDirectoryName(MaptileWorkingDirectory directoryType)
    {
        return GetMaptileDirectory(directoryType).directoryName;
    }

    public string GetWorkingDirectoryFileExtension(MaptileWorkingDirectory directoryType)
    {
        return GetMaptileDirectory(directoryType).datafileExtension;
    }

    public string GetFullDirectoryPath(MaptileWorkingDirectory directoryType)
    {
        return maptileDirectory + "/" + GetWorkingDirectoryName(directoryType);
    }

    public string GetFullFilePath(MaptileWorkingDirectory directoryType, string filename)
    {
        return GetFullDirectoryPath(directoryType) + "/" + filename + GetWorkingDirectoryFileExtension(directoryType);
    }

    public string GetWebFilePath(MaptileWorkingDirectory directoryType, string filename)
    {
        return GetWorkingDirectoryName(directoryType) + "/" + filename + GetWorkingDirectoryFileExtension(directoryType);
    }
}