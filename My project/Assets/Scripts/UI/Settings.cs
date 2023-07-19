using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void DataReset()
    {
        //データをリセット
        SaveSystem.Reset();
        //リセットしたデータを保存する
        SaveSystem.Save();
    }
}
