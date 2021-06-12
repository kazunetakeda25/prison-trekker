using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoUserFirebase
{
    public int index;

    public string userID;
    public string name;
    public int level;


    public InfoUserFirebase(int _index, string _userID, string _name, int _level) {
        index = _index;
        userID = _userID;
        name = _name;
        level = _level;
    }
}
