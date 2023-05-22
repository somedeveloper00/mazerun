using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace MazeRun.Core {
    public class MapGenerator : MonoBehaviour {
        [SerializeField] MapTarget target;
        [SerializeField] Block[] spawnableBlockPrefabs;

        [ShowInInspector]
        readonly List<Block> _blocks = new();

        [Button]
        public void ResetMap() {
            _blocks.ForEach( b => safeDestroy( b != null ? b.gameObject : null ) );
            _blocks.Clear();
            _blocks.Add( Instantiate( spawnableBlockPrefabs[0], transform ) );
        }

        void Start() {
            _blocks.Clear();
            _blocks.AddRange( GetComponentsInChildren<Block>().ToList() );
            flushOpeningConnections();
            updateBlockNames();
        }

        [Button]
        void Update() {
            if (updateBlocks()) {
                flushOpeningConnections();
                updateBlockNames();
            }
        }

        bool updateBlocks() {
            bool changed = false;
            for (int i = 0; i < _blocks.Count; i++) {
                var block = _blocks[i];
                
                bool anyOpeningInside = false;
                
                // check for adding neighbors
                for (int j = 0; j < block.openings.Length; j++) {
                    var opening = block.openings[j];
                    
                    var inside = target.IsInsideTargetView( opening.transform.position );
                    if (!inside) continue;
                    anyOpeningInside = true;
                    
                    if (opening.connectedOpening) continue;
                    
                    // spawn new block here
                    var nblock = spawnBlock( opening.transform.position, opening );
                    if (nblock) {
                        _blocks.Add( nblock );
                        nblock.name = "block " + i + "-" + j;
                        changed = true;
                    }
                }
                
                // check for deletion
                if (!anyOpeningInside) {
                    safeDestroy( _blocks[i].gameObject );
                    _blocks.RemoveAt( i-- );
                    changed = true;
                }
            }

            return changed;
        }

        [Button]
        void flushOpeningConnections() {
            var openings = _blocks.SelectMany( b => b.openings ).ToList();
            for (var i = 0; i < openings.Count; i++) {
                bool connectionFound = false;
                for (var j = i + 1; j < openings.Count; j++) {
                    if (!openings[i].CanConnectTo(openings[j])) continue;
                    var dist = (openings[i].transform.position - openings[j].transform.position).sqrMagnitude;
                    if (dist > 0.1f) continue;
                    openings[i].connectedOpening = openings[j];
                    openings[j].connectedOpening = openings[i];
                    connectionFound = true;
                    break;
                }

                if (!connectionFound) {
                    openings[i].connectedOpening = null;
                }
            }
        }

        void updateBlockNames() {
            for (int i = 0; i < _blocks.Count; i++) {
                _blocks[i].name = $"block {i}";
            }
        }

        void safeDestroy(GameObject go) {
            if (!go) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate( go );
            else 
#endif
                Destroy( go );
        }

        Block spawnBlock(Vector3 position, Opening opening) {
            Block selectedBlock = null;
            Vector3 pos = Vector3.zero;
            
            foreach (var block in spawnableBlockPrefabs) {
                foreach (var newOpening in block.openings) {
                    if (!opening.CanConnectTo( newOpening )) continue;
                    pos = position - newOpening.GetOffsetPosition();
                    if (_blocks.Any(b => b.WillIntersectWithPrefabAtPos( block, pos ) ))
                        continue;
                    selectedBlock = block;
                    break;
                }

                if (selectedBlock) break;
            }

            if (!selectedBlock) return null;
            
            var newBlock = Instantiate( selectedBlock, pos, Quaternion.identity, transform );
            return newBlock;
        }
    }
}