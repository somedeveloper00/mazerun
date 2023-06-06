using System;
using MazeRun.Level;
using UnityEngine;

namespace MazeRun.UI {
    public abstract class UserDataViewElement : MonoBehaviour {
        public abstract void UpdateView(UserData.Data data);
    }
}