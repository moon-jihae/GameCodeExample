using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMonsterColor : MonoBehaviour
{
    public MonsterState monsterState;
    public Material[] materials;

    void Start()
    {
        SkinnedMeshRenderer skinnedMesh = gameObject.GetComponent<SkinnedMeshRenderer>();
        
        switch (monsterState.m_elem)
        {
            case Element.FIRE:
                skinnedMesh.sharedMaterial = materials[0];
                break;
            case Element.WATER:
                skinnedMesh.sharedMaterial = materials[1];
                break;
            case Element.WOOD:
                skinnedMesh.sharedMaterial = materials[2];
                break;
            case Element.METAL:
                skinnedMesh.sharedMaterial = materials[3];
                break;
            case Element.EARTH:
                skinnedMesh.sharedMaterial = materials[4];
                break;
            default:
                break;
        }
    }
}
