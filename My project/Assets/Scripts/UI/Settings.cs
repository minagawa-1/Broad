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
        //�f�[�^�����Z�b�g
        SaveSystem.Reset();
        //���Z�b�g�����f�[�^��ۑ�����
        SaveSystem.Save();
    }
}
