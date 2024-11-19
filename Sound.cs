using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Data;

public class Sound : MonoBehaviour
{
    public static float volume; 

    public Slider Soundslider;    // 슬라이더
    public AudioSource AudioSource;  // 오디오 소스
    // Start is called before the first frame update
    void Start()
    {

        // 초기 슬라이더 값을 오디오 소스 볼륨과 동기화
        if (Soundslider != null && AudioSource != null)
        {
            Soundslider.value = volume;
            AudioSource.volume = volume;
        }

        // 슬라이더 값이 변경될 때마다 볼륨을 조정
        //Soundslider.onValueChanged.AddListener(delegate { SetVolume(); });
    }

    // Update is called once per frame
    void Update()
    {
        // 슬라이더의 값이 실시간으로 AudioSource의 볼륨에 반영됨
        Soundcheck();
    }

    // 슬라이더 값에 따라 오디오 볼륨을 조정하는 함수
    public void SetVolume()
    {
        if (AudioSource != null)
        {
            volume = Soundslider.value;  // 슬라이더 값에 맞춰 볼륨 조정
            AudioSource.volume = volume;
        }
    }

    // 볼륨을 확인하는 함수 (사용자 정의 필요 시 활용)
    public void Soundcheck()
    {
        if (AudioSource != null)
        {
            volume = AudioSource.volume;
            AudioSource.volume = volume;
        }
    }
    
    
}

