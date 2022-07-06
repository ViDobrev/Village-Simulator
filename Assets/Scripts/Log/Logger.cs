using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    #region Data
    private string pathToLogFile;
    #endregion Data


    #region Methods
    public Logger(string pathToLogFile)
    {
        this.pathToLogFile = pathToLogFile;
    }

    public bool Log(string message)
    {
        return true;
    }
    #endregion Methods
}