using MazeRun.Level;
using TMPro;
using UnityEngine;

namespace MazeRun.UI {
    public class CurrentLevelText : UserDataViewElement {
        [SerializeField] TMP_Text text;
        [SerializeField] string format = "{0}";
        public override void UpdateView(UserData.Data data) => text.text = string.Format( format, data.level );
    }
}