using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int Value;
    public Node Node;
    public Block MergingBlock;
    public bool Merging;
    public Vector2 Pos => transform.position;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;

    // Tower attributes
    private Tower _tower;
    private SpriteRenderer _weaponRenderer;
    private Vector3 _weaponPosition;
    private Animator _weaponAnimator;

    public Sprite TowerSprite { get; private set; }
    public float AttackRange { get; private set; }
    public float AttackSpeed { get; private set; }
    public GameObject BulletPrefab { get; private set; }
    public Sprite WeaponSprite { get; private set; }
    public Vector3 WeaponPosition { get; private set; }
    public RuntimeAnimatorController WeaponAnimatorController { get; private set; }

    public void Init(BlockType type)
    {
        Value = type.Value;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();

        // Initialize the weapon renderer and animator (assumes a child object named "Weapon")
        _weaponRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
        _weaponPosition = transform.GetChild(0).transform.GetChild(0).transform.position;
        _weaponAnimator = transform.GetChild(0).transform.GetChild(0).AddComponent<Animator>();

        // Initialize tower attributes based on the block value
        SetTowerAttributes(TowerManager.Instance.GetTowerData(Value));
    }

    public void SetBlock(Node node)
    {
        if (Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith)
    {
        // Set the block we are merging with
        MergingBlock = blockToMergeWith;

        // Set current node as unoccupied to allow blocks to use it
        Node.OccupiedBlock = null;

        // Set the base block as merging, so it does not get used twice.
        blockToMergeWith.Merging = true;

        // Update the block's value and attributes after merging
        Value *= 2; // Doubling the value as per 2048 mechanics
        SetTowerAttributes(TowerManager.Instance.GetTowerData(Value));
    }

    public bool CanMerge(int value) => value == Value && !Merging && MergingBlock == null;

    public void SetTowerAttributes(TowerData towerData)
    {
        if (towerData == null) return;

        TowerSprite = towerData.towerSprite;
        AttackRange = towerData.attackRange;
        AttackSpeed = towerData.attackSpeed;
        BulletPrefab = towerData.bulletPrefab;
        WeaponSprite = towerData.weaponSprite;
        WeaponPosition = towerData.weaponOffset;
        WeaponAnimatorController = towerData.weaponAnimController; // Get Animator Controller

        // Update the block's visual representation with the new tower sprite
        _renderer.sprite = TowerSprite;

        // Update the weapon's sprite and position
        if (_weaponRenderer != null)
        {
            _weaponRenderer.sprite = WeaponSprite;
            _weaponRenderer.transform.localPosition = WeaponPosition;
        }

        // Update the weapon's Animator Controller
        if (_weaponAnimator != null)
        {
            _weaponAnimator.runtimeAnimatorController = WeaponAnimatorController;
        }

        // Add or update the Tower component
        if (_tower == null)
        {
            _tower = gameObject.AddComponent<Tower>();
        }

        _tower.AttackRange = AttackRange;
        _tower.AttackSpeed = AttackSpeed;
        _tower.BulletPrefab = BulletPrefab;
    }
}
