using System.IO;

public enum MaptileWorkingDirectory
{
    Srtm,
    Mercator,
    Heightmap,
    Image
}

public class MaptileImportConfiguration
{
    public int outputTileSize = 256;
    public int imageTileLevelOfDetail = 11;
    public string workingPathRoot = "C:/Users/aqiu/unity/HoloImpact-Shared/TileServer";

    private string[] WorkingDirectoryNames;
    private string[] WorkingDirectoryFileExtensions;

    public MaptileImportConfiguration()
    {
        WorkingDirectoryNames = new string[]
        {
            "EPSG3857",
            "EPSG4326",
            "RAW",
            "IMAGES"
        };

        WorkingDirectoryFileExtensions = new string[]
        {
            ".tif",
            ".tif",
            ".raw",
            ".jpg",
        };
    }

    public string GetWorkingDirectoryName(MaptileWorkingDirectory directoryType)
    {
        return WorkingDirectoryNames[(int)directoryType];
    }

    public void SetWorkingDirectoryName(MaptileWorkingDirectory directoryType, string directoryName)
    {
        WorkingDirectoryNames[(int)directoryType] = directoryName;
    }

    public string GetWorkingDirectoryFileExtension(MaptileWorkingDirectory directoryType)
    {
        return WorkingDirectoryFileExtensions[(int)directoryType];
    }

    public void SetWorkingDirectoryFileExtension(MaptileWorkingDirectory directoryType, string fileExtension)
    {
        WorkingDirectoryFileExtensions[(int)directoryType] = fileExtension;
    }

    public string GetFullDirectoryPath(MaptileWorkingDirectory directoryType)
    {
        return Path.Combine(workingPathRoot, GetWorkingDirectoryName(directoryType));
    }

    public string GetFullFilePath(MaptileWorkingDirectory directoryType, string filename)
    {
        return Path.Combine(GetFullDirectoryPath(directoryType), filename + GetWorkingDirectoryFileExtension(directoryType));
    }
}