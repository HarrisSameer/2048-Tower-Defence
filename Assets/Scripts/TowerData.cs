using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public class TowerData
{
    public Sprite towerSprite;
    public float attackRange;
    public float attackSpeed;
    public GameObject bulletPrefab;

    public Sprite weaponSprite;
    public Vector3 weaponOffset;

    public RuntimeAnimatorController weaponAnimController;
}
