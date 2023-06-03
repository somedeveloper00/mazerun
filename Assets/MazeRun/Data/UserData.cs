using System;
using System.Collections;
using System.Text;
using MazeRun.Core;
using MazeRun.Data;
using MazeRun.Main;
using UnityEngine;

namespace MazeRun.Level {
    public class UserData : MonoBehaviour {
        public Data data;
        [SerializeField] GameSession gameSession;
        [SerializeField] GameManager gameManager;
        [SerializeField] DataIO dataio;

        void OnEnable() {
            gameSession.GameStartRoutines.Add( LoadDataRoutine() );

            gameManager.OnLose += () => {
                data.totalLose++;
                StartCoroutine( Save() );
            };
            gameManager.OnWin += () => {
                data.totalWin++;
                StartCoroutine( Save() );
            };
            gameManager.OnRevive += () => {
                data.totalRevive++;
                StartCoroutine( Save() );
            };
        }

        IEnumerator LoadDataRoutine() {
            bool done = false;
            dataio.Load( bytes => {
                try {
                    // convert bytes to Data
                    var stringData = Encoding.UTF8.GetString( bytes );
                    data = JsonUtility.FromJson<Data>( stringData );
                }
                catch (Exception e) {
                    Debug.LogException( e );
                    data = new Data();
                }
                done = true;
            }, null );
            yield return new WaitUntil( () => done );
            Debug.Log( "Data loaded" );
        }


        public IEnumerator Save() {
            bool done = false;
            dataio.Save( System.Text.Encoding.UTF8.GetBytes( JsonUtility.ToJson( data ) ), () => done = true, null );
            yield return new WaitUntil( () => done );
            Debug.Log( "Data saved" );
        }
        
        [Serializable] public class Data {
            public int totalWin;
            public int totalLose;
            public int totalRevive;
            public long level;
        }
    }
}