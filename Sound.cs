using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Data;

public class Sound : MonoBehaviour
{
    public static float volume; 

    public Slider Soundslider;    // �����̴�
    public AudioSource AudioSource;  // ����� �ҽ�
    // Start is called before the first frame update
    void Start()
    {

        // �ʱ� �����̴� ���� ����� �ҽ� ������ ����ȭ
        if (Soundslider != null && AudioSource != null)
        {
            Soundslider.value = volume;
            AudioSource.volume = volume;
        }

        // �����̴� ���� ����� ������ ������ ����
        //Soundslider.onValueChanged.AddListener(delegate { SetVolume(); });
    }

    // Update is called once per frame
    void Update()
    {
        // �����̴��� ���� �ǽð����� AudioSource�� ������ �ݿ���
        Soundcheck();
    }

    // �����̴� ���� ���� ����� ������ �����ϴ� �Լ�
    public void SetVolume()
    {
        if (AudioSource != null)
        {
            volume = Soundslider.value;  // �����̴� ���� ���� ���� ����
            AudioSource.volume = volume;
        }
    }

    // ������ Ȯ���ϴ� �Լ� (����� ���� �ʿ� �� Ȱ��)
    public void Soundcheck()
    {
        if (AudioSource != null)
        {
            volume = AudioSource.volume;
            AudioSource.volume = volume;
        }
    }
    
    
}

