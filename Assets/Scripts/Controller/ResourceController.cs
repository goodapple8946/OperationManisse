using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    // Block连接时的粒子预设
    public GameObject particleLinkPrefab;
    
    // Block连接时的音效
    public AudioClip[] audiosLink;

    // 删除时的音效
    public AudioClip[] audiosDelete;
}
