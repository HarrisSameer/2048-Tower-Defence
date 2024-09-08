using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Animations;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private int _winCondition = 2048;
    [SerializeField] private GameObject _winScreen, _loseScreen, _winScreenText;

    private List<Node> _nodes;
    private List<Block> _blocks;
    private GameState _state;
    private int _round;
    [SerializeField] private Sprite _mainTowerSprite;
    [SerializeField] private AnimatorController CentreTowerAnimation;

    //public NavMeshManager m_NavMeshManager;
    public Transform centreTowerTransform;

    private BlockType GetBlockTypeByValue(int value) => _types.FirstOrDefault(t => t.Value == value);

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else { 
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ChangeState(GameState.GenerateLevel);
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                _winScreen.SetActive(true);
                Invoke(nameof(DelayedWinScreenText), 1.5f);
                break;
            case GameState.Lose:
                _loseScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    void DelayedWinScreenText()
    {
        _winScreenText.SetActive(true);
    }

    void Update()
    {
        if (_state != GameState.WaitingInput) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Shift(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Shift(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Shift(Vector2.down);
    }

    void GenerateGrid()
    {
        _round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);

                // Check if this is the center-most node
                if (x == _width / 2 && y == _height / 2)
                {
                    node.IsStatic = true; // Set IsStatic to true for the center node
                    node.SetColor(Color.white); // Set the color to black for the static node
                    node.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = _mainTowerSprite;
                    //node.gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "MainTower";
                    node.gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 4;
                    node.gameObject.transform.GetChild(0).transform.position = new Vector3(node.gameObject.transform.GetChild(0).transform.position.x, node.gameObject.transform.GetChild(0).transform.position.y + 0.5f, node.gameObject.transform.GetChild(0).transform.position.z);
                    node.gameObject.GetComponentInChildren<SpriteRenderer>().gameObject.AddComponent<Animator>();
                    node.gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController= CentreTowerAnimation;
                    node.gameObject.tag = "CenterTower";
                    node.gameObject.AddComponent<BoxCollider2D>();
                    centreTowerTransform = node.gameObject.transform;
                }
                //else
                //{
                //    node.SetColor(Color.white); // Set the color to white or any other default color for other nodes
                //}

                _nodes.Add(node);
            }
        }

        var center = new Vector2((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f);

        var board = Instantiate(_boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);

        Camera.main.transform.position = new Vector3(center.x, center.y, -10);

        ChangeState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {
        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null && !n.IsStatic).OrderBy(b => UnityEngine.Random.value).ToList();

        foreach (var node in freeNodes.Take(amount))
        {
            SpawnBlock(node, UnityEngine.Random.value > 0.8f ? 4 : 2);
        }

        ChangeState(CheckEndGame());
    }

    void SpawnBlock(Node node, int value)
    {
        if (node == null) return;

        var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        block.SetBlock(node);
        _blocks.Add(block);

        //m_NavMeshManager.BakeNavMesh();
    }

    GameState CheckEndGame()
    {
        // Check for win condition: presence of a block with the win value
        if (_blocks.Any(b => b.Value == _winCondition && !b.Node.IsStatic))
        {
            return GameState.Win;
        }

        // Check for lose condition
        if (CheckLoseCondition())
        {
            return GameState.Lose;
        }

        return GameState.WaitingInput;
    }

    bool CheckLoseCondition()
    {
        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null && !n.IsStatic).ToList();

        // If there are free nodes, the game is not lost
        if (freeNodes.Count > 0)
        {
            return false;
        }

        // Check for possible merges
        foreach (var node in _nodes.Where(n => !n.IsStatic))
        {
            var pos = node.Pos;

            // Check adjacent nodes for possible merges
            foreach (var dir in new[] { Vector2.left, Vector2.right, Vector2.up, Vector2.down })
            {
                var adjacentNode = GetNodeAtPosition(pos + dir);
                if (adjacentNode != null && adjacentNode.OccupiedBlock != null &&
                    node.OccupiedBlock.CanMerge(adjacentNode.OccupiedBlock.Value))
                {
                    return false;
                }
            }
        }

        return true;
    }

    void Shift(Vector2 dir)
    {
        ChangeState(GameState.Moving);

        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        foreach (var block in orderedBlocks)
        {
            var next = block.Node;
            do
            {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + dir);
                if (possibleNode != null && !possibleNode.IsStatic)
                {
                    // We know a node is present
                    // If it's possible to merge, set merge
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Value))
                    {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                    }
                    // Otherwise, can we move to this spot?
                    else if (possibleNode.OccupiedBlock == null) next = possibleNode;

                    // None hit? End do while loop
                }
            } while (next != block.Node);
        }

        var sequence = DOTween.Sequence();

        foreach (var block in orderedBlocks)
        {
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime).SetEase(Ease.InQuad));
        }

        sequence.OnComplete(() => {
            var mergeBlocks = orderedBlocks.Where(b => b.MergingBlock != null).ToList();
            foreach (var block in mergeBlocks)
            {
                MergeBlocks(block.MergingBlock, block);
            }
            ChangeState(GameState.SpawningBlocks);
        });
    }

    void MergeBlocks(Block baseBlock, Block mergingBlock)
    {
        var newValue = baseBlock.Value * 2;

        SpawnBlock(baseBlock.Node, newValue);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block block)
    {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
        return _nodes.FirstOrDefault(n => n.Pos == pos);
    }
}

[Serializable]
public struct BlockType
{
    public int Value;
    public Color Color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}
