using System;
using System.Collections.Generic;
using System.Linq;
using MazeRun.Level;
using TriInspector;
using UnityEngine;

namespace MazeRun.UI {
    public class UserDataViewElementProvider : MonoBehaviour {
        public UserData userData;
        [SerializeField] List<UserDataViewElement> elements = new();

        [Button("Fill")] void OnValidate() => elements = GetComponentsInChildren<UserDataViewElement>().ToList();

        void Start() {
            userData.onDataUpdated += onDataUpdated;
            onDataUpdated();
        }

        void onDataUpdated() => elements.ForEach( e => e.UpdateView( userData.data ) );
    }
}