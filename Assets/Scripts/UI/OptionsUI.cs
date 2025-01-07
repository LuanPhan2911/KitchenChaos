using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private Button closeButton;

    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button escapeButton;

    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDowntext;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI escapeText;

    [SerializeField] private Transform rebindKeyTransform;





    public static OptionsUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.UpdateSoundEffectVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.UpdateSoundEffectVolume();
            UpdateVisual();

        });
        closeButton.onClick.AddListener(() =>
        {
            GamePauseUI.Instance.Show();
            Hide();
        });
        GameManager.Instance.OnGamePaused += (sender, args) => Hide();

        UpdateVisual();
        Hide();
        HideRebindKey();

        moveUpButton.onClick.AddListener(() => RebindBiding(GameInput.Binding.MoveUp));
        moveDownButton.onClick.AddListener(() => RebindBiding(GameInput.Binding.MoveDown));
        moveLeftButton.onClick.AddListener(() => RebindBiding(GameInput.Binding.MoveLeft));
        moveRightButton.onClick.AddListener(() => RebindBiding(GameInput.Binding.MoveRight));
        interactButton.onClick.AddListener(() => RebindBiding(GameInput.Binding.Interact));
        interactAlternateButton.onClick.AddListener(() => RebindBiding(GameInput.Binding.InteractAlternate));
        escapeButton.onClick.AddListener(() => RebindBiding(GameInput.Binding.Escape));

    }

    private void UpdateVisual()
    {
        float soundEffectVolume = Mathf.Round(SoundManager.Instance.GetSoundEffectVolume() * 10);
        float musicVolume = Mathf.Round(MusicManager.Instance.GetSoundEffectVolume() * 10);
        soundEffectText.text = $"Sound Effect: {soundEffectVolume}";
        musicText.text = $"Music: {musicVolume}";

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        moveDowntext.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        escapeText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Escape);


    }

    public void Show()
    {
        gameObject.SetActive(true);
        soundEffectButton.Select();
    }
    public void Hide()
    {

        gameObject.SetActive(false);
    }

    private void ShowRebindKey()
    {
        rebindKeyTransform.gameObject.SetActive(true);
    }
    private void HideRebindKey()
    {
        rebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBiding(GameInput.Binding binding)
    {
        ShowRebindKey();
        GameInput.Instance.BindingKey(binding, () =>
        {
            UpdateVisual();
            HideRebindKey();
        });
    }
}
