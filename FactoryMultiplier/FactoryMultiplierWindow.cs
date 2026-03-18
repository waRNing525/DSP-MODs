using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryMultiplier
{
    internal sealed class FactoryMultiplierWindow
    {
        private readonly FactoryMultiplier owner;
        private readonly Dictionary<FactoryMultiplier.MultiplierField, UIInputField> inputFields = new Dictionary<FactoryMultiplier.MultiplierField, UIInputField>();

        private GameObject rootObject;
        private UIInventoryWindow shellWindow;
        private GameObject closeButtonGroup;
        private RectTransform contentRoot;
        private Text bodyTextTemplate;
        private Text hotkeyText;
        private Text statusText;
        private UIToggle walkSpeedToggle;

        public FactoryMultiplierWindow(FactoryMultiplier owner)
        {
            this.owner = owner;
        }

        public bool IsVisible
        {
            get
            {
                return rootObject != null && rootObject.activeSelf;
            }
        }

        public void Tick(bool shouldShow)
        {
            if (!TryEnsureCreated())
            {
                return;
            }

            if (shouldShow)
            {
                if (!IsVisible)
                {
                    Show();
                }

                UICursor.forceShowCursor = true;
                RefreshStatus();
                RefreshWalkSpeedToggle();
            }
            else if (IsVisible)
            {
                Hide();
            }
        }

        public void Dispose()
        {
            if (rootObject != null)
            {
                Object.Destroy(rootObject);
                rootObject = null;
            }

            shellWindow = null;
            closeButtonGroup = null;
            contentRoot = null;
            bodyTextTemplate = null;
            hotkeyText = null;
            statusText = null;
            walkSpeedToggle = null;
            inputFields.Clear();
        }

        private bool TryEnsureCreated()
        {
            if (rootObject != null)
            {
                return true;
            }

            UIRoot uiRoot = UIRoot.instance;
            if (uiRoot == null || uiRoot.uiGame == null || !uiRoot.uiGame.inited)
            {
                return false;
            }

            UIGame uiGame = uiRoot.uiGame;
            if (uiGame.windowGroup == null || uiGame.inventoryWindow == null || uiGame.minerWindow == null || uiGame.blueprintCopyInspector == null)
            {
                return false;
            }

            if (uiRoot.optionWindow == null || uiRoot.optionWindow.applyButton == null || uiRoot.optionWindow.backgroundPause == null)
            {
                return false;
            }

            BuildWindow(uiGame, uiRoot);
            return rootObject != null;
        }

        private void BuildWindow(UIGame uiGame, UIRoot uiRoot)
        {
            rootObject = Object.Instantiate(uiGame.inventoryWindow.gameObject, uiGame.windowGroup, false);
            rootObject.name = "FactoryMultiplierWindow";
            rootObject.SetActive(false);

            shellWindow = rootObject.GetComponent<UIInventoryWindow>();
            if (shellWindow == null || shellWindow.windowTrans == null || shellWindow.titleText == null)
            {
                Dispose();
                return;
            }

            RectTransform rootRect = rootObject.transform as RectTransform;
            if (rootRect != null)
            {
                rootRect.anchorMin = new Vector2(0.5f, 0.5f);
                rootRect.anchorMax = new Vector2(0.5f, 0.5f);
                rootRect.pivot = new Vector2(0.5f, 0.5f);
                rootRect.anchoredPosition = Vector2.zero;
                rootRect.localScale = Vector3.one;
            }

            shellWindow.windowTrans.sizeDelta = new Vector2(900f, 640f);
            bodyTextTemplate = uiRoot.optionWindow.copyMsg;
            if (bodyTextTemplate == null)
            {
                bodyTextTemplate = uiRoot.optionWindow.applyButton.GetComponentInChildren<Text>(true);
            }

            HideTemplateContent();
            HideTemplateTextAndButtons();
            CreateCloseButtonGroup(uiGame);
            RewireCloseButton();
            CreateContent(uiRoot);
        }

        private void HideTemplateContent()
        {
            if (shellWindow.inventory != null)
            {
                shellWindow.inventory.gameObject.SetActive(false);
            }

            if (shellWindow.deliveryPanel != null)
            {
                shellWindow.deliveryPanel.gameObject.SetActive(false);
            }
        }

        private void HideTemplateTextAndButtons()
        {
            HashSet<GameObject> hiddenObjects = new HashSet<GameObject>();

            Text[] texts = shellWindow.windowTrans.GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                if (text != null && hiddenObjects.Add(text.gameObject))
                {
                    text.gameObject.SetActive(false);
                }
            }

            UIButton[] uiButtons = shellWindow.windowTrans.GetComponentsInChildren<UIButton>(true);
            foreach (UIButton uiButton in uiButtons)
            {
                if (uiButton != null && hiddenObjects.Add(uiButton.gameObject))
                {
                    uiButton.gameObject.SetActive(false);
                }
            }

            Button[] buttons = shellWindow.windowTrans.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button != null && hiddenObjects.Add(button.gameObject))
                {
                    button.gameObject.SetActive(false);
                }
            }
        }

        private void CreateCloseButtonGroup(UIGame uiGame)
        {
            if (uiGame.minerWindow == null || uiGame.minerWindow.closeButtonGroupGo == null)
            {
                return;
            }

            closeButtonGroup = Object.Instantiate(uiGame.minerWindow.closeButtonGroupGo, shellWindow.windowTrans, false);
            closeButtonGroup.name = "FactoryMultiplierCloseButton";
            closeButtonGroup.SetActive(true);
            closeButtonGroup.transform.SetAsLastSibling();
        }

        private void RewireCloseButton()
        {
            if (closeButtonGroup == null)
            {
                return;
            }

            Button[] closeButtons = closeButtonGroup.GetComponentsInChildren<Button>(true);
            foreach (Button closeButton in closeButtons)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            UIButton[] uiButtons = closeButtonGroup.GetComponentsInChildren<UIButton>(true);
            foreach (UIButton uiButton in uiButtons)
            {
                uiButton.button?.onClick.RemoveAllListeners();
                ClearTips(uiButton);
                uiButton.BindOnClickSafe(_ => OnCloseButtonClick());
            }
        }

        private void CreateContent(UIRoot uiRoot)
        {
            contentRoot = CreateRect("FactoryMultiplierContent", shellWindow.windowTrans, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f));
            contentRoot.offsetMin = new Vector2(28f, 18f);
            contentRoot.offsetMax = new Vector2(-28f, -54f);

            CreateText("WindowTitle", contentRoot, shellWindow.titleText, 20, 18f, 4f, 300f, 26f, TextAnchor.MiddleLeft, shellWindow.titleText.color).text = owner.GetWindowTitle();
            hotkeyText = CreateText("HotkeyText", contentRoot, bodyTextTemplate, 20, 0f, 0f, 828f, 24f, TextAnchor.MiddleRight, uiRoot.optionWindow.applyButton.GetComponentInChildren<Text>(true)?.color ?? Color.white);
            hotkeyText.text = owner.GetHotkeyHint();

            RectTransform leftPanel = CreateSectionPanel("LeftPanel", new Vector2(0f, -40f), new Vector2(404f, 430f), owner.GetSectionTitle(isLeftSection: true));
            RectTransform rightPanel = CreateSectionPanel("RightPanel", new Vector2(424f, -40f), new Vector2(404f, 430f), owner.GetSectionTitle(isLeftSection: false));

            CreateFieldRow(leftPanel, FactoryMultiplier.MultiplierField.Smelt, 60f);
            CreateFieldRow(leftPanel, FactoryMultiplier.MultiplierField.Chemical, 118f);
            CreateFieldRow(leftPanel, FactoryMultiplier.MultiplierField.Refine, 176f);
            CreateFieldRow(leftPanel, FactoryMultiplier.MultiplierField.Assemble, 234f);
            CreateFieldRow(leftPanel, FactoryMultiplier.MultiplierField.Particle, 292f);
            CreateFieldRow(leftPanel, FactoryMultiplier.MultiplierField.Lab, 350f);

            CreateFieldRow(rightPanel, FactoryMultiplier.MultiplierField.Fractionator, 60f);
            CreateFieldRow(rightPanel, FactoryMultiplier.MultiplierField.Ejector, 118f);
            CreateFieldRow(rightPanel, FactoryMultiplier.MultiplierField.Silo, 176f);
            CreateFieldRow(rightPanel, FactoryMultiplier.MultiplierField.Gamma, 234f);
            CreateFieldRow(rightPanel, FactoryMultiplier.MultiplierField.Mining, 292f);
            CreateFieldRow(rightPanel, FactoryMultiplier.MultiplierField.WalkSpeed, 350f);
            CreateWalkSpeedToggle(rightPanel, 394f, uiRoot);

            RectTransform statusPanel = CreateRect("StatusPanel", contentRoot, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            statusPanel.anchoredPosition = new Vector2(0f, -486f);
            statusPanel.sizeDelta = new Vector2(828f, 74f);
            Image statusBg = statusPanel.gameObject.AddComponent<Image>();
            statusBg.color = new Color(0.12f, 0.16f, 0.22f, 0.68f);

            statusText = CreateText("StatusText", statusPanel, bodyTextTemplate, 16, 18, 16, 792, 42, TextAnchor.UpperLeft, bodyTextTemplate.color);
            statusText.horizontalOverflow = HorizontalWrapMode.Wrap;
            statusText.verticalOverflow = VerticalWrapMode.Truncate;
            statusText.text = owner.GetStatusText();
        }

        private RectTransform CreateSectionPanel(string name, Vector2 anchoredPosition, Vector2 size, string title)
        {
            RectTransform panel = CreateRect(name, contentRoot, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            panel.anchoredPosition = anchoredPosition;
            panel.sizeDelta = size;

            Image panelBg = panel.gameObject.AddComponent<Image>();
            panelBg.color = new Color(0.08f, 0.12f, 0.17f, 0.72f);

            Text titleText = CreateText($"{name}Title", panel, shellWindow.titleText, 18, 18, 16, size.x - 36f, 24f, TextAnchor.MiddleLeft, shellWindow.titleText.color);
            titleText.text = title;

            RectTransform divider = CreateRect($"{name}Divider", panel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            divider.anchoredPosition = new Vector2(18f, -44f);
            divider.sizeDelta = new Vector2(size.x - 36f, 1f);
            Image dividerImage = divider.gameObject.AddComponent<Image>();
            dividerImage.color = new Color(1f, 1f, 1f, 0.18f);

            return panel;
        }

        private void CreateFieldRow(RectTransform parent, FactoryMultiplier.MultiplierField field, float topOffset)
        {
            CreateText($"{field}Label", parent, shellWindow.titleText, 15, 18, topOffset, 146f, 28f, TextAnchor.MiddleLeft, shellWindow.titleText.color).text = owner.GetFieldLabel(field);

            UIInputField inputField = CloneInputField(parent, field);
            RectTransform inputRect = inputField.transform as RectTransform;
            inputRect.anchorMin = new Vector2(0f, 1f);
            inputRect.anchorMax = new Vector2(0f, 1f);
            inputRect.pivot = new Vector2(0f, 1f);
            inputRect.anchoredPosition = new Vector2(162f, 0f - topOffset);
            inputRect.sizeDelta = new Vector2(96f, 28f);
            inputFields[field] = inputField;

            UIButton applyButton = CloneButton(parent, owner.GetApplyButtonText());
            RectTransform buttonRect = applyButton.transform as RectTransform;
            buttonRect.anchorMin = new Vector2(0f, 1f);
            buttonRect.anchorMax = new Vector2(0f, 1f);
            buttonRect.pivot = new Vector2(0f, 1f);
            buttonRect.anchoredPosition = new Vector2(274f, 0f - topOffset);
            buttonRect.sizeDelta = new Vector2(110f, 28f);
            applyButton.BindOnClickSafe(_ =>
            {
                owner.SetFieldValue(field, inputField.inputField.text);
                owner.ApplyField(field);
                RefreshField(field);
                RefreshStatus();
                RefreshWalkSpeedToggle();
            });
        }

        private void CreateWalkSpeedToggle(RectTransform parent, float topOffset, UIRoot uiRoot)
        {
            walkSpeedToggle = CloneToggle(parent, uiRoot.optionWindow.backgroundPause);
            RectTransform toggleRect = walkSpeedToggle.rectTransform ?? (walkSpeedToggle.transform as RectTransform);
            toggleRect.anchorMin = new Vector2(0f, 1f);
            toggleRect.anchorMax = new Vector2(0f, 1f);
            toggleRect.pivot = new Vector2(0f, 1f);
            toggleRect.anchoredPosition = new Vector2(162f, 0f - topOffset);
            toggleRect.sizeDelta = new Vector2(22f, 22f);
            walkSpeedToggle.toggle.onValueChanged.RemoveAllListeners();
            walkSpeedToggle.toggle.onValueChanged.AddListener(OnWalkSpeedToggleChanged);

            CreateText("WalkSpeedToggleLabel", parent, shellWindow.titleText, 14, 192f, topOffset - 4f, 190f, 28f, TextAnchor.MiddleLeft, shellWindow.titleText.color).text = owner.GetWalkSpeedModeText();
        }

        private UIInputField CloneInputField(RectTransform parent, FactoryMultiplier.MultiplierField field)
        {
            UIInputField template = UIRoot.instance.uiGame.blueprintCopyInspector.shortUIInput;
            UIInputField clone = Object.Instantiate(template, parent, false);
            clone.name = $"{field}InputField";
            clone.gameObject.SetActive(true);
            ClearTooltips(clone.gameObject);

            if (clone.iconSelectBtn != null)
            {
                Object.Destroy(clone.iconSelectBtn.gameObject);
                clone.iconSelectBtn = null;
            }

            clone.inputField.onEndEdit.RemoveAllListeners();
            clone.inputField.onValueChanged.RemoveAllListeners();
            clone.inputField.lineType = InputField.LineType.SingleLine;
            clone.inputField.contentType = InputField.ContentType.IntegerNumber;
            clone.inputField.characterLimit = owner.GetCharacterLimit(field);
            clone.inputField.text = owner.GetFieldValue(field);
            clone.inputField.textComponent.alignment = TextAnchor.MiddleCenter;
            clone.overText.alignment = TextAnchor.MiddleCenter;
            clone.inputField.ForceLabelUpdate();

            return clone;
        }

        private UIButton CloneButton(RectTransform parent, string text)
        {
            UIButton template = UIRoot.instance.optionWindow.applyButton;
            UIButton clone = Object.Instantiate(template, parent, false);
            clone.name = "ApplyButton";
            clone.gameObject.SetActive(true);
            clone.button?.onClick.RemoveAllListeners();
            ClearTips(clone);

            Text[] buttonTexts = clone.GetComponentsInChildren<Text>(true);
            foreach (Text buttonText in buttonTexts)
            {
                buttonText.text = text;
                buttonText.alignment = TextAnchor.MiddleCenter;
            }

            return clone;
        }

        private UIToggle CloneToggle(RectTransform parent, UIToggle template)
        {
            UIToggle clone = Object.Instantiate(template, parent, false);
            clone.name = "WalkSpeedToggle";
            clone.gameObject.SetActive(true);
            clone.toggle.onValueChanged.RemoveAllListeners();
            return clone;
        }

        private void Show()
        {
            RefreshAllFields();
            RefreshWalkSpeedToggle();
            RefreshStatus();
            if (hotkeyText != null)
            {
                hotkeyText.text = owner.GetHotkeyHint();
            }

            rootObject.SetActive(true);
            rootObject.transform.SetAsLastSibling();
        }

        private void Hide()
        {
            if (rootObject != null)
            {
                rootObject.SetActive(false);
            }
        }

        private void OnCloseButtonClick()
        {
            owner.SetWindowVisible(false);
            Hide();
        }

        private void OnWalkSpeedToggleChanged(bool isOn)
        {
            owner.DirectWalkSpeedMode = isOn;
            RefreshStatus();
        }

        private void RefreshAllFields()
        {
            foreach (KeyValuePair<FactoryMultiplier.MultiplierField, UIInputField> entry in inputFields)
            {
                RefreshField(entry.Key);
            }
        }

        private void RefreshField(FactoryMultiplier.MultiplierField field)
        {
            if (!inputFields.TryGetValue(field, out UIInputField inputField) || inputField == null)
            {
                return;
            }

            string value = owner.GetFieldValue(field);
            if (inputField.inputField.text != value)
            {
                inputField.inputField.text = value;
                inputField.inputField.ForceLabelUpdate();
            }
        }

        private void RefreshStatus()
        {
            if (statusText != null)
            {
                statusText.text = owner.GetStatusText();
            }
        }

        private void RefreshWalkSpeedToggle()
        {
            if (walkSpeedToggle != null && walkSpeedToggle.toggle.isOn != owner.DirectWalkSpeedMode)
            {
                walkSpeedToggle.toggle.isOn = owner.DirectWalkSpeedMode;
            }
        }

        private static RectTransform CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.localScale = Vector3.one;
            return rect;
        }

        private static Text CreateText(string name, Transform parent, Text template, int fontSize, float x, float y, float width, float height, TextAnchor alignment, Color color)
        {
            RectTransform rect = CreateRect(name, parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            rect.anchoredPosition = new Vector2(x, 0f - y);
            rect.sizeDelta = new Vector2(width, height);

            Text text = rect.gameObject.AddComponent<Text>();
            text.font = template.font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.supportRichText = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static void SetInactive(Component component)
        {
            if (component != null)
            {
                component.gameObject.SetActive(false);
            }
        }

        private static void ClearTooltips(GameObject root)
        {
            if (root == null)
            {
                return;
            }

            UIButton[] buttons = root.GetComponentsInChildren<UIButton>(true);
            foreach (UIButton button in buttons)
            {
                ClearTips(button);
            }
        }

        private static void ClearTips(UIButton button)
        {
            if (button == null)
            {
                return;
            }

            button.tips.tipTitle = string.Empty;
            button.tips.tipText = string.Empty;
            button.tips.corner = 0;
            button.tipTitleFormatString = string.Empty;
            button.tipTextFormatString = string.Empty;
            button.CloseTip();
        }
    }
}
