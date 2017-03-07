using System.Linq;

/// <summary>
/// GDAL's Nuget packages are targeted at .NET >=4.0, but Unity can only go up to 3.5.
/// Naive Workaround: Install GDAL directly and use this wrapper classs to call
/// only the necessary utilities.
/// </summary>
public class HeightmapImporter
{
    public MaptileImportConfiguration maptileServerConfig; 

    private enum GdalCommand
    {
        Warp,
        Translate
    }

    private static readonly string[] GdalCommandNames = new string[]
    {
        "gdalwarp.exe",
        "gdal_translate.exe"
    };

    private static string GetGdalCommandName(GdalCommand gdalCommand)
    {
        return GdalCommandNames[(int)gdalCommand];
    }

    private string[] WorkingDirectoryFileExtensions;

    public HeightmapImporter(MaptileImportConfiguration config)
    {
        maptileServerConfig = config;
    }

    private int RunGdalProcess(GdalCommand gdalCommand, string[] args, MaptileWorkingDirectory inputDir, string inputFile, MaptileWorkingDirectory outputDir, string outputFile)
    {
        var gdalProcess = new System.Diagnostics.Process();
        var gdalStart = gdalProcess.StartInfo;

        gdalStart.Arguments = string.Format
        (
            "{0} {1} {2}",
            string.Join(" ", args),
            inputFile + maptileServerConfig.GetWorkingDirectoryFileExtension(inputDir),
            maptileServerConfig.GetFullFilePath(outputDir, outputFile)
        );

        gdalStart.FileName = maptileServerConfig.gdalDirectory + "/" +  GetGdalCommandName(gdalCommand);
        gdalStart.WorkingDirectory = maptileServerConfig.GetFullDirectoryPath(inputDir);
        
        gdalStart.CreateNoWindow = true;
        gdalStart.UseShellExecute = false;
        gdalStart.RedirectStandardOutput = true;
        gdalStart.RedirectStandardError = true;

        gdalProcess.Start();
        gdalProcess.BeginOutputReadLine();
        gdalProcess.BeginErrorReadLine();
        gdalProcess.WaitForExit();

        return gdalProcess.ExitCode;
    }

    private void MercatorToRaw(string quadKey)
    {
        var args = new string[]
        {
            "-ot", "UInt16",
            "-of", "ENVI",
            "-outsize", string.Format("{0} {0}", maptileServerConfig.outputTileSize + 1),
        };

        RunGdalProcess(GdalCommand.Translate, args, MaptileWorkingDirectory.Mercator, quadKey, MaptileWorkingDirectory.Heightmap, quadKey);
    }

    public void CreateHeightmap(string[] inputFiles, double minX, double minY, double maxX, double maxY, string quadKey)
    {
        var args = new string[]
        {
            "--config GDAL_CACHEMAX", maptileServerConfig.gdalCacheMax.ToString(),
            "-wm", maptileServerConfig.gdalCacheMax.ToString(),
            "-s_srs", "EPSG:4326",
            "-t_srs", "EPSG:3857",
            "-te_srs", "EPSG:4326",
            "-te", string.Format("{0} {1} {2} {3}", minX, minY, maxX, maxY),
        };

        var inputFileString = string.Join
        (
            maptileServerConfig.GetWorkingDirectoryFileExtension(MaptileWorkingDirectory.Srtm) + " ", 
            inputFiles.Where(s => !string.IsNullOrEmpty(s)).ToArray()
        );

        RunGdalProcess(GdalCommand.Warp, args, MaptileWorkingDirectory.Srtm, inputFileString, MaptileWorkingDirectory.Mercator, quadKey);
        MercatorToRaw(quadKey);
    }
}