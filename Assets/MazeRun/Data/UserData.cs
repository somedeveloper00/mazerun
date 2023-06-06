using System;
using System.Collections;
using MazeRun.Core;
using MazeRun.Data;
using MazeRun.Main;
using TriInspector;
using UnityEngine;

namespace MazeRun.Level {
    public class UserData : MonoBehaviour {
        public Data data;
        public Action onDataUpdated = delegate {  };
        [SerializeField] GameSession gameSession;
        [SerializeField] GameManager gameManager;
        [SerializeField] DataIO dataio;

        long currentPoints = 0;
        
        void OnEnable() {
            gameSession.GameStartRoutines.Add( LoadDataRoutine() );

            gameManager.OnLose += () => {
                data.totalLose++;
                data.highScore = Math.Max( data.highScore, currentPoints );
                currentPoints = 0;
                StartCoroutine( Save() );
                onDataUpdated();
            };
            gameManager.OnWin += () => {
                data.totalWin++;
                data.level++;
                data.highScore = Math.Max( data.highScore, currentPoints );
                StartCoroutine( Save() );
                onDataUpdated();
            };
            gameManager.OnRevive += () => {
                data.totalRevive++;
                data.highScore = Math.Max( data.highScore, currentPoints );
                StartCoroutine( Save() );
                onDataUpdated();
            };
            gameManager.OnProgressPointsUpdated += () => {
                currentPoints++;
                onDataUpdated();
            };
        }

        IEnumerator LoadDataRoutine() {
            bool done = false;
            dataio.Load<Data>( data => {
                this.data = data;
                done = true;
            }, null );
            yield return new WaitUntil( () => done );
            gameManager.userData = data;
            Debug.Log( "data loaded" );
            onDataUpdated();
        }


        IEnumerator Save() {
            bool done = false;
            dataio.Save( data, () => {
                Debug.Log( $"Data saved" );
                done = true;
            }, null );
            yield return new WaitUntil( () => done );
        }

        [Button]
        void DeleteAll() {
            dataio.Save<Data>( null, null, null );
            data = new Data();
        }
        
        [Serializable] public class Data {
            public int totalWin;
            public int totalLose;
            public int totalRevive;
            public int level;
            public long highScore;
        }
    }
}