using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MazeRun.UI {
    public class DelayedButton : Button {
        [SerializeField] AudioClip audioClip;
        [SerializeField] float volume = 1;
        [SerializeField] float delay = 0.2f;

        Coroutine _clickCoroutine = null;
        
        public override void OnPointerClick(PointerEventData eventData) {
            if (!IsActive() || !IsInteractable() || eventData?.button != PointerEventData.InputButton.Left)
                return;
            _clickCoroutine = StartCoroutine( clickRoutine() );
        }

        public override void OnSubmit(BaseEventData eventData) => OnPointerClick( eventData as PointerEventData );

        void press() {
            if (!IsActive() || !IsInteractable())
                return;
            onClick.Invoke();
        }
        
        IEnumerator clickRoutine() {
            SingletonAudioSource.SfxInstance.PlayOneShot( audioClip, volume );
            yield return new WaitForSecondsRealtime( delay );
            _clickCoroutine = null;
            press();
        }
    }
}