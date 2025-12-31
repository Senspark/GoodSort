using System.Collections.Generic;
using Dialog.Controller;
using manager.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class ProfileDialog : Dialog<ProfileDialog>
    {
        [SerializeField] private Image avatarImage;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private Button btnSave;
        [SerializeField] private GameObject avatarPrefab;
        [SerializeField] private Transform avatarContainer;
        [SerializeField] private Sprite[] avatarSprites;

        private IProfileDialogController _controller;
        private int _observerId = -1;
        private string _tempAvatarId;
        private readonly List<Transform> _avatarButtons = new();

        protected override void Awake()
        {
            base.Awake();
            btnSave?.onClick.AddListener(() =>
            {
                // set focus into input field
                nameInputField.Select();
                nameInputField.ActivateInputField();
            });
        }

        public override void Show(Canvas canvas)
        {
            OnWillShow(() =>
            {
                RegisterObserver();
                UpdateUI();
            });
            OnWillHide(() =>
            {
                _controller.SaveProfile(nameInputField.text.Trim(), _tempAvatarId);
                UnregisterObserver();
            });
            base.Show(canvas);
        }

        private void RegisterObserver()
        {
            _observerId = _controller.RegisterObserver(new ProfileManagerObserver
            {
                OnAvatarIdChanged = UpdateAvatarDisplay
            });
        }

        private void UnregisterObserver()
        {
            _controller.UnregisterObserver(_observerId);
        }

        private void UpdateUI()
        {
            nameInputField.text = _controller.GetName();
            _tempAvatarId = _controller.GetAvatarId();
            UpdateAvatarDisplay(_tempAvatarId);
            SetupAvatarSelection();
        }

        private void UpdateAvatarDisplay(string avatarId)
        {
            if (int.TryParse(avatarId, out var index) && index >= 0 && index < avatarSprites.Length)
                avatarImage.sprite = avatarSprites[index];
        }

        private void SetupAvatarSelection()
        {
            foreach (Transform child in avatarContainer)
                Destroy(child.gameObject);

            _avatarButtons.Clear();

            for (int i = 0; i < avatarSprites.Length; i++)
            {
                var avatarObj = Instantiate(avatarPrefab, avatarContainer);
                var avatarImg = avatarObj.transform.Find("Mask/AvatarImage").GetComponent<Image>();
                var avatarBtn = avatarObj.GetComponent<Button>();

                _avatarButtons.Add(avatarObj.transform);

                if (avatarImg != null)
                    avatarImg.sprite = avatarSprites[i];

                if (avatarBtn != null)
                {
                    var index = i;
                    avatarBtn.onClick.AddListener(() => OnAvatarSelected(index));
                }
            }

            UpdateAvatarSelection(_tempAvatarId);
        }

        private void OnAvatarSelected(int avatarIndex)
        {
            _tempAvatarId = avatarIndex.ToString();
            UpdateAvatarDisplay(_tempAvatarId);
            UpdateAvatarSelection(_tempAvatarId);
        }

        private void UpdateAvatarSelection(string selectedAvatarId)
        {
            if (!int.TryParse(selectedAvatarId, out var selectedIndex)) return;

            for (int i = 0; i < _avatarButtons.Count; i++)
            {
                var selectedNode = _avatarButtons[i].GetChild(2); // Or Find("Selected")
                selectedNode.gameObject.SetActive(i == selectedIndex);
            }
        }
    }
}