using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DirectoryManager
{
    private static DirectoryInfo m_dataDirectory = null;
    public static DirectoryInfo DataDirectory
    {
        get
        {
            if(m_dataDirectory == null)
            {
                m_dataDirectory = Directory.CreateDirectory("Data");
            }
            return m_dataDirectory;
        }
    }

    private static Dictionary<string, DirectoryInfo> m_subdirectories = new Dictionary<string, DirectoryInfo>();
    public static DirectoryInfo GetDirectory(string _directoryName)
    {
        if(!m_subdirectories.ContainsKey(_directoryName))
        {
            m_subdirectories[_directoryName] = DataDirectory.CreateSubdirectory(_directoryName);
        }
        return m_subdirectories[_directoryName];
    }
}
