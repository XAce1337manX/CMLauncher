﻿using SimpleJSON;
using System;
using System.IO;

class Version : IVersion
{
    private readonly JSONObject versionInfo = new JSONObject();
    public int VersionNumber { get; private set; } = 0;
    public string VersionServer { get; private set; } = "";

    private readonly string VersionFilename;
    private static Version version = null;

    public static Version GetVersion()
    {
        if (version == null)
        {
            version = new Version();
        }
        return version;
    }

    private Version()
    {
        VersionFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cm-version");

        if (!File.Exists(VersionFilename)) return;

        var json = JSON.Parse(File.ReadAllText(VersionFilename));
        if (json is JSONObject j)
        {
            versionInfo = j;

            VersionNumber = versionInfo["version"]?.AsInt ?? 0;
            VersionServer = versionInfo["server"].Value;
        }
        else
        {
            var oldVersionInfo = File.ReadAllText(VersionFilename).Split('\n');
            VersionNumber = int.Parse(oldVersionInfo[0]);

            if (oldVersionInfo.Length > 1)
                VersionServer = oldVersionInfo[1];
        }
    }

    void IVersion.Update(int version, string server)
    {
        versionInfo["server"] = server;
        versionInfo["version"] = version;

        File.WriteAllText(VersionFilename, versionInfo.ToString());

        VersionNumber = version;
        VersionServer = server;
    }
}
