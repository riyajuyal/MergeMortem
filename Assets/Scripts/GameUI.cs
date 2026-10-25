using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MergeMortem
{
    public class GameUI : MonoBehaviour
    {
        const string PP_CH1 = "mm_ch1_complete";
        const string PP_TS = "mm_ch1_timestamp";
        const string PP_COINS = "mm_coins";
        const string PP_XP = "mm_xp";

        GameState state;
        Font uiFont;

        // Top bar
        Text energyText;
        Text coinsText;
        Text levelText;
        Image xpBarFill;

        // Hint bar
        Text hintText;

        // Board
        RectTransform boardRoot;
        Image[] cellBg;
        GameObject[] cellTileGO;
        Image[] cellTileImg;
        Text[] cellTileLabel;
        Text[] cellTileDots;
        Image[] cellTileOutline;

        // Generators & file button
        Button fabricBtn, printsBtn, phoneBtn, fileBtn;
        Text fabricBtnLabel, printsBtnLabel, phoneBtnLabel;

        // Panels
        Text objFabricText, objPrintsText, objPhoneText;
        Text suspectKaranChip, suspectDevChip, suspectMeenaChip;
        Image suspectKaranBg, suspectDevBg, suspectMeenaBg;
        Text hqSlot1Text, hqSlot2Text, hqSlot3Text;

        // Modal
        GameObject modalRoot;
        Image modalPanelBg;
        Text modalTitleText;
        Text modalBodyText;
        Transform modalButtonRow;

        static readonly Color SlateBackground = new Color(0.29f, 0.33f, 0.41f, 1f);
        static readonly Color PanelWhite = Color.white;
        static readonly Color EvidenceYellow = Data.HexToColor("#f2c230");
        static readonly Color TextDark = new Color(0.12f, 0.14f, 0.18f, 1f);
        static readonly Color DisabledGrey = new Color(0.75f, 0.76f, 0.78f, 1f);
        static readonly Color CellEmptyColor = new Color(0.85f, 0.87f, 0.90f, 1f);

        public void Init(GameState gameState)
        {
            state = gameState;
            uiFont = LoadFont();

            BuildBackground();
            BuildTopBar();
            BuildHintBar();
            BuildBoard();
            BuildGeneratorsAndFileButton();
            BuildPanels();
            BuildModalLayer();

            RefreshAll();

            if (PlayerPrefs.GetInt(PP_CH1, 0) == 1)
            {
                state.coins = PlayerPrefs.GetInt(PP_COINS, 0);
                state.xp = PlayerPrefs.GetInt(PP_XP, 0);
                for (int i = 0; i < 3; i++) state.chainFiled[i] = true;
                RefreshAll();
                ShowSeeYouTomorrowModal();
            }
            else
            {
                ShowIntroModal();
            }
        }

        static Font LoadFont()
        {
            Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return f;
        }

        // ---------------------------------------------------------------
        // Background
        // ---------------------------------------------------------------

        void BuildBackground()
        {
            GameObject bg = new GameObject("Background", typeof(Image));
            bg.transform.SetParent(transform, false);
            RectTransform rt = bg.GetComponent<RectTransform>();
            StretchFull(rt);
            bg.GetComponent<Image>().color = SlateBackground;
        }

        // ---------------------------------------------------------------
        // Top bar
        // ---------------------------------------------------------------

        void BuildTopBar()
        {
            GameObject panel = CreatePanel("TopBar", transform, new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(10, -96), new Vector2(-10, -10));

            HorizontalLayoutGroup hlg = panel.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(16, 16, 10, 10);
            hlg.spacing = 16;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;

            energyText = CreateLabel("Energy", panel.transform, 22, TextDark, FontStyle.Bold, TextAnchor.MiddleCenter);
            coinsText = CreateLabel("Coins", panel.transform, 22, TextDark, FontStyle.Bold, TextAnchor.MiddleCenter);
            levelText = CreateLabel("Level", panel.transform, 22, TextDark, FontStyle.Bold, TextAnchor.MiddleCenter);

            GameObject xpBarBack = new GameObject("XpBarBack", typeof(Image));
            xpBarBack.transform.SetParent(transform, false);
            RectTransform xpRt = xpBarBack.GetComponent<RectTransform>();
            xpRt.anchorMin = new Vector2(0, 1);
            xpRt.anchorMax = new Vector2(1, 1);
            xpRt.pivot = new Vector2(0.5f, 1f);
            xpRt.anchoredPosition = new Vector2(0, -98);
            xpRt.sizeDelta = new Vector2(-20, 8);
            xpBarBack.GetComponent<Image>().color = new Color(1, 1, 1, 0.35f);

            GameObject xpFillGO = new GameObject("XpBarFill", typeof(Image));
            xpFillGO.transform.SetParent(xpBarBack.transform, false);
            RectTransform fillRt = xpFillGO.GetComponent<RectTransform>();
            fillRt.anchorMin = new Vector2(0, 0);
            fillRt.anchorMax = new Vector2(0, 1);
            fillRt.pivot = new Vector2(0, 0.5f);
            fillRt.offsetMin = Vector2.zero;
            fillRt.offsetMax = Vector2.zero;
            fillRt.sizeDelta = new Vector2(0, 0);
            xpBarFill = xpFillGO.GetComponent<Image>();
            xpBarFill.color = EvidenceYellow;

            xpFillAnchorParentWidth = xpBarBack.GetComponent<RectTransform>();
        }

        RectTransform xpFillAnchorParentWidth;

        // ---------------------------------------------------------------
        // Hint bar
        // ---------------------------------------------------------------

        void BuildHintBar()
        {
            GameObject bar = new GameObject("HintBar", typeof(Image));
            bar.transform.SetParent(transform, false);
            RectTransform rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -102);
            rt.sizeDelta = new Vector2(-20, 44);
            bar.GetComponent<Image>().color = EvidenceYellow;

            hintText = CreateLabel(Data.HintEmptyBoard, bar.transform, 18, TextDark, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(hintText.GetComponent<RectTransform>());
        }

        // ---------------------------------------------------------------
        // Board
        // ---------------------------------------------------------------

        void BuildBoard()
        {
            GameObject boardGO = new GameObject("Board", typeof(Image));
            boardGO.transform.SetParent(transform, false);
            boardRoot = boardGO.GetComponent<RectTransform>();
            boardRoot.anchorMin = new Vector2(0.5f, 1f);
            boardRoot.anchorMax = new Vector2(0.5f, 1f);
            boardRoot.pivot = new Vector2(0.5f, 1f);
            boardRoot.anchoredPosition = new Vector2(0, -152);
            boardRoot.sizeDelta = new Vector2(380, 380);
            boardGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.08f);

            GridLayoutGroup grid = boardGO.AddComponent<GridLayoutGroup>();
            grid.padding = new RectOffset(6, 6, 6, 6);
            grid.spacing = new Vector2(4, 4);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Data.BoardSize;
            float cell = (380f - 12f - 4f * (Data.BoardSize - 1)) / Data.BoardSize;
            grid.cellSize = new Vector2(cell, cell);

            int count = GameState.CellCount;
            cellBg = new Image[count];
            cellTileGO = new GameObject[count];
            cellTileImg = new Image[count];
            cellTileLabel = new Text[count];
            cellTileDots = new Text[count];
            cellTileOutline = new Image[count];

            for (int i = 0; i < count; i++)
            {
                int idx = i;
                GameObject cellGO = new GameObject("Cell_" + i, typeof(Image), typeof(Button));
                cellGO.transform.SetParent(boardGO.transform, false);
                Image cellImg = cellGO.GetComponent<Image>();
                cellImg.color = CellEmptyColor;
                cellBg[i] = cellImg;

                Button btn = cellGO.GetComponent<Button>();
                btn.onClick.AddListener(() => OnCellClicked(idx));

                GameObject outlineGO = new GameObject("Outline", typeof(Image));
                outlineGO.transform.SetParent(cellGO.transform, false);
                StretchFull(outlineGO.GetComponent<RectTransform>());
                Image outlineImg = outlineGO.GetComponent<Image>();
                outlineImg.color = EvidenceYellow;
                outlineImg.enabled = false;
                cellTileOutline[i] = outlineImg;

                GameObject tileGO = new GameObject("Tile", typeof(Image));
                tileGO.transform.SetParent(cellGO.transform, false);
                RectTransform tileRt = tileGO.GetComponent<RectTransform>();
                tileRt.anchorMin = new Vector2(0.08f, 0.08f);
                tileRt.anchorMax = new Vector2(0.92f, 0.92f);
                tileRt.offsetMin = Vector2.zero;
                tileRt.offsetMax = Vector2.zero;
                Image tileImg = tileGO.GetComponent<Image>();
                tileImg.color = Color.clear;
                cellTileImg[i] = tileImg;
                cellTileGO[i] = tileGO;

                Text label = CreateLabel("", tileGO.transform, 13, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
                RectTransform labelRt = label.GetComponent<RectTransform>();
                labelRt.anchorMin = new Vector2(0.05f, 0.35f);
                labelRt.anchorMax = new Vector2(0.95f, 0.85f);
                labelRt.offsetMin = Vector2.zero;
                labelRt.offsetMax = Vector2.zero;
                label.horizontalOverflow = HorizontalWrapMode.Wrap;
                cellTileLabel[i] = label;

                Text dots = CreateLabel("", tileGO.transform, 14, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
                RectTransform dotsRt = dots.GetComponent<RectTransform>();
                dotsRt.anchorMin = new Vector2(0.05f, 0.08f);
                dotsRt.anchorMax = new Vector2(0.95f, 0.32f);
                dotsRt.offsetMin = Vector2.zero;
                dotsRt.offsetMax = Vector2.zero;
                cellTileDots[i] = dots;
            }
        }

        // ---------------------------------------------------------------
        // Generators & file button
        // ---------------------------------------------------------------

        void BuildGeneratorsAndFileButton()
        {
            GameObject row = new GameObject("GeneratorRow", typeof(RectTransform));
            row.transform.SetParent(transform, false);
            RectTransform rowRt = row.GetComponent<RectTransform>();
            rowRt.anchorMin = new Vector2(0, 1);
            rowRt.anchorMax = new Vector2(1, 1);
            rowRt.pivot = new Vector2(0.5f, 1f);
            rowRt.anchoredPosition = new Vector2(0, -540);
            rowRt.sizeDelta = new Vector2(-20, 56);

            HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;

            fabricBtn = CreateButton("FabricBtn", row.transform, Data.Chains[0].color, out fabricBtnLabel, "Fabric");
            printsBtn = CreateButton("PrintsBtn", row.transform, Data.Chains[1].color, out printsBtnLabel, "Prints");
            phoneBtn = CreateButton("PhoneBtn", row.transform, Data.Chains[2].color, out phoneBtnLabel, "Phone");

            fabricBtn.onClick.AddListener(() => OnGeneratorClicked(ChainType.Fabric));
            printsBtn.onClick.AddListener(() => OnGeneratorClicked(ChainType.Prints));
            phoneBtn.onClick.AddListener(() => OnGeneratorClicked(ChainType.Phone));

            GameObject fileRow = new GameObject("FileRow", typeof(RectTransform));
            fileRow.transform.SetParent(transform, false);
            RectTransform fileRt = fileRow.GetComponent<RectTransform>();
            fileRt.anchorMin = new Vector2(0, 1);
            fileRt.anchorMax = new Vector2(1, 1);
            fileRt.pivot = new Vector2(0.5f, 1f);
            fileRt.anchoredPosition = new Vector2(0, -604);
            fileRt.sizeDelta = new Vector2(-20, 48);

            Text fileLabel;
            fileBtn = CreateButton("FileBtn", fileRow.transform, EvidenceYellow, out fileLabel, "File as evidence");
            fileLabel.color = TextDark;
            fileBtn.onClick.AddListener(OnFileClicked);
        }

        Button CreateButton(string name, Transform parent, Color color, out Text label, string text)
        {
            GameObject go = new GameObject(name, typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 44);
            Image img = go.GetComponent<Image>();
            img.color = color;

            Button btn = go.GetComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.disabledColor = DisabledGrey;
            btn.colors = cb;

            label = CreateLabel(text, go.transform, 15, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(label.GetComponent<RectTransform>());
            return btn;
        }

        // ---------------------------------------------------------------
        // Panels: objectives, suspects, HQ shelf
        // ---------------------------------------------------------------

        void BuildPanels()
        {
            GameObject objPanel = CreatePanel("ObjectivesPanel", transform, new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(10, -660), new Vector2(-10, -656));
            RectTransform objRt = objPanel.GetComponent<RectTransform>();
            objRt.anchoredPosition = new Vector2(0, -660);
            objRt.sizeDelta = new Vector2(-20, 70);
            VerticalLayoutGroup objVlg = objPanel.AddComponent<VerticalLayoutGroup>();
            objVlg.padding = new RectOffset(12, 12, 6, 6);
            objVlg.childForceExpandWidth = true;
            objVlg.childForceExpandHeight = false;
            objVlg.spacing = 2;

            Text objTitle = CreateLabel("Case objectives", objPanel.transform, 14, TextDark, FontStyle.Bold, TextAnchor.MiddleLeft);
            objFabricText = CreateLabel("File the Fabric chain", objPanel.transform, 13, TextDark, FontStyle.Normal, TextAnchor.MiddleLeft);
            objPrintsText = CreateLabel("File the Prints chain", objPanel.transform, 13, TextDark, FontStyle.Normal, TextAnchor.MiddleLeft);
            objPhoneText = CreateLabel("File the Phone chain", objPanel.transform, 13, TextDark, FontStyle.Normal, TextAnchor.MiddleLeft);

            GameObject suspectPanel = CreatePanel("SuspectsPanel", transform, new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(10, -738), new Vector2(-10, -734));
            RectTransform susRt = suspectPanel.GetComponent<RectTransform>();
            susRt.anchoredPosition = new Vector2(0, -738);
            susRt.sizeDelta = new Vector2(-20, 58);
            HorizontalLayoutGroup susHlg = suspectPanel.AddComponent<HorizontalLayoutGroup>();
            susHlg.padding = new RectOffset(10, 10, 8, 8);
            susHlg.spacing = 8;
            susHlg.childForceExpandWidth = true;
            susHlg.childForceExpandHeight = true;

            suspectKaranChip = CreateChip(suspectPanel.transform, "Karan: unknown", out suspectKaranBg);
            suspectDevChip = CreateChip(suspectPanel.transform, "Dev: unknown", out suspectDevBg);
            suspectMeenaChip = CreateChip(suspectPanel.transform, "Meena: unknown", out suspectMeenaBg);

            GameObject hqPanel = CreatePanel("HQPanel", transform, new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(10, -804), new Vector2(-10, -800));
            RectTransform hqRt = hqPanel.GetComponent<RectTransform>();
            hqRt.anchoredPosition = new Vector2(0, -804);
            hqRt.sizeDelta = new Vector2(-20, 48);
            VerticalLayoutGroup hqOuterVlg = hqPanel.AddComponent<VerticalLayoutGroup>();
            hqOuterVlg.padding = new RectOffset(10, 10, 4, 4);
            hqOuterVlg.childForceExpandWidth = true;
            hqOuterVlg.childForceExpandHeight = false;
            hqOuterVlg.spacing = 2;

            Text hqTitle = CreateLabel("Detective HQ shelf", hqPanel.transform, 12, TextDark, FontStyle.Bold, TextAnchor.MiddleLeft);
            hqTitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 16;

            GameObject slotsRow = new GameObject("Slots", typeof(RectTransform));
            slotsRow.transform.SetParent(hqPanel.transform, false);
            LayoutElement slotsLe = slotsRow.AddComponent<LayoutElement>();
            slotsLe.preferredHeight = 24;
            HorizontalLayoutGroup slotsHlg = slotsRow.AddComponent<HorizontalLayoutGroup>();
            slotsHlg.spacing = 8;
            slotsHlg.childForceExpandWidth = true;
            slotsHlg.childForceExpandHeight = true;

            hqSlot1Text = CreateShelfSlot(slotsRow.transform, "Locked");
            hqSlot2Text = CreateShelfSlot(slotsRow.transform, "Locked");
            hqSlot3Text = CreateShelfSlot(slotsRow.transform, "Locked");
        }

        Text CreateShelfSlot(Transform parent, string text)
        {
            GameObject go = new GameObject("Slot", typeof(Image));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = new Color(0, 0, 0, 0.06f);
            Text t = CreateLabel(text, go.transform, 11, TextDark, FontStyle.Normal, TextAnchor.MiddleCenter);
            StretchFull(t.GetComponent<RectTransform>());
            return t;
        }

        Text CreateChip(Transform parent, string text, out Image bg)
        {
            GameObject go = new GameObject("Chip", typeof(Image));
            go.transform.SetParent(parent, false);
            bg = go.GetComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.08f);
            Text t = CreateLabel(text, go.transform, 12, TextDark, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(t.GetComponent<RectTransform>());
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            return t;
        }

        GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 posUnused, Vector2 sizeUnused)
        {
            GameObject go = new GameObject(name, typeof(Image));
            go.transform.SetParent(parent, false);
            Image img = go.GetComponent<Image>();
            img.color = PanelWhite;
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            return go;
        }

        // ---------------------------------------------------------------
        // Modal layer
        // ---------------------------------------------------------------

        void BuildModalLayer()
        {
            modalRoot = new GameObject("ModalLayer", typeof(Image));
            modalRoot.transform.SetParent(transform, false);
            StretchFull(modalRoot.GetComponent<RectTransform>());
            modalRoot.GetComponent<Image>().color = new Color(0, 0, 0, 0.55f);

            GameObject panel = new GameObject("ModalPanel", typeof(Image));
            panel.transform.SetParent(modalRoot.transform, false);
            modalPanelBg = panel.GetComponent<Image>();
            modalPanelBg.color = PanelWhite;
            RectTransform panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.5f, 0.5f);
            panelRt.anchorMax = new Vector2(0.5f, 0.5f);
            panelRt.pivot = new Vector2(0.5f, 0.5f);
            panelRt.sizeDelta = new Vector2(400, 480);
            panelRt.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup vlg = panel.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(24, 24, 24, 24);
            vlg.spacing = 16;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childAlignment = TextAnchor.UpperCenter;

            modalTitleText = CreateLabel("Title", panel.transform, 24, TextDark, FontStyle.Bold, TextAnchor.MiddleCenter);
            LayoutElement titleLe = modalTitleText.gameObject.AddComponent<LayoutElement>();
            titleLe.preferredHeight = 40;

            modalBodyText = CreateLabel("Body", panel.transform, 16, TextDark, FontStyle.Normal, TextAnchor.UpperLeft);
            modalBodyText.horizontalOverflow = HorizontalWrapMode.Wrap;
            modalBodyText.verticalOverflow = VerticalWrapMode.Overflow;
            LayoutElement bodyLe = modalBodyText.gameObject.AddComponent<LayoutElement>();
            bodyLe.preferredHeight = 260;
            bodyLe.flexibleHeight = 1;

            GameObject btnRow = new GameObject("ButtonRow", typeof(RectTransform));
            btnRow.transform.SetParent(panel.transform, false);
            LayoutElement btnRowLe = btnRow.AddComponent<LayoutElement>();
            btnRowLe.preferredHeight = 56;
            VerticalLayoutGroup btnRowVlg = btnRow.AddComponent<VerticalLayoutGroup>();
            btnRowVlg.spacing = 10;
            btnRowVlg.childForceExpandWidth = true;
            btnRowVlg.childForceExpandHeight = true;
            modalButtonRow = btnRow.transform;

            modalRoot.SetActive(false);
        }

        void ClearModalButtons()
        {
            for (int i = modalButtonRow.childCount - 1; i >= 0; i--)
                Destroy(modalButtonRow.GetChild(i).gameObject);
        }

        Button AddModalButton(string text, Color color, UnityAction onClick)
        {
            GameObject go = new GameObject("Btn_" + text, typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(modalButtonRow, false);
            go.GetComponent<LayoutElement>().preferredHeight = 48;
            Image img = go.GetComponent<Image>();
            img.color = color;
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(onClick);
            Text label = CreateLabel(text, go.transform, 16, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(label.GetComponent<RectTransform>());
            return btn;
        }

        void ShowModal(string title, string body, params (string text, Color color, UnityAction action)[] buttons)
        {
            modalTitleText.text = title;
            modalBodyText.text = body;
            ClearModalButtons();
            foreach (var b in buttons)
                AddModalButton(b.text, b.color, b.action);
            modalRoot.SetActive(true);
        }

        void HideModal()
        {
            modalRoot.SetActive(false);
        }

        // ---------------------------------------------------------------
        // Modal flows
        // ---------------------------------------------------------------

        void ShowIntroModal()
        {
            string body = Data.IntroStory + "\n\n" + Data.IntroStep1 + "\n" + Data.IntroStep2 + "\n" + Data.IntroStep3;
            ShowModal("Merge Mortem: The Coorg Study", body,
                ("Start investigating", EvidenceYellow, () => HideModal()));
        }

        void ShowRevealModal(ChainDef chain)
        {
            ShowModal(chain.revealTitle, chain.revealBody,
                ("Continue", EvidenceYellow, () =>
                {
                    HideModal();
                    if (state.AllChainsFiled())
                        ShowChapterCompleteModal();
                }));
        }

        void ShowChapterCompleteModal()
        {
            ShowModal(Data.ChapterCompleteTitle, Data.ChapterCompleteBody,
                ("Continue", EvidenceYellow, () =>
                {
                    HideModal();
                    hqSlot1Text.text = "Coffee jar";
                    SaveProgress();
                    ShowDay1HookModal();
                }));
        }

        void ShowDay1HookModal()
        {
            ShowModal(Data.Day1HookTitle, Data.Day1HookBody,
                ("Come back tomorrow", PanelWhite, () =>
                {
                    HideModal();
                }),
                ("Demo: simulate tomorrow", EvidenceYellow, () =>
                {
                    HideModal();
                    SimulateTomorrow();
                }));
            // Tint the "Come back tomorrow" text dark since its background is white.
            TintLastButtonTextDark(0);
        }

        void TintLastButtonTextDark(int childIndex)
        {
            if (childIndex < modalButtonRow.childCount)
            {
                Text t = modalButtonRow.GetChild(childIndex).GetComponentInChildren<Text>();
                if (t != null) t.color = TextDark;
            }
        }

        void ShowSeeYouTomorrowModal()
        {
            ShowModal(Data.SeeYouTomorrowTitle, Data.SeeYouTomorrowBody + "\n\n" + Data.Day1HookBody,
                ("Demo: simulate tomorrow", EvidenceYellow, () =>
                {
                    HideModal();
                    SimulateTomorrow();
                }),
                ("Replay case", PanelWhite, () =>
                {
                    HideModal();
                    ReplayCase();
                }));
            TintLastButtonTextDark(1);
        }

        void ShowChapter2TeaserModal()
        {
            ShowModal(Data.Chapter2TeaserTitle, Data.Chapter2TeaserBody,
                ("Replay case", EvidenceYellow, () =>
                {
                    HideModal();
                    ReplayCase();
                }));
        }

        void ShowAdModal()
        {
            ShowModal(Data.AdModalTitle, Data.AdModalBody,
                ("Watch ad (+5 energy)", EvidenceYellow, () =>
                {
                    state.WatchAdForEnergy();
                    HideModal();
                    RefreshAll();
                }));
        }

        void SimulateTomorrow()
        {
            state.energy += 20;
            RefreshAll();
            ShowChapter2TeaserModal();
        }

        void ReplayCase()
        {
            PlayerPrefs.DeleteAll();
            state.Reset();
            hqSlot1Text.text = "Locked";
            hqSlot2Text.text = "Locked";
            hqSlot3Text.text = "Locked";
            RefreshAll();
            ShowIntroModal();
        }

        void SaveProgress()
        {
            PlayerPrefs.SetInt(PP_CH1, 1);
            PlayerPrefs.SetString(PP_TS, System.DateTime.UtcNow.ToString("o"));
            PlayerPrefs.SetInt(PP_COINS, state.coins);
            PlayerPrefs.SetInt(PP_XP, state.xp);
            PlayerPrefs.Save();
        }

        // ---------------------------------------------------------------
        // Input handlers
        // ---------------------------------------------------------------

        void OnGeneratorClicked(ChainType chain)
        {
            SpawnResultType result = state.SpawnBasic(chain);
            if (result == SpawnResultType.BoardFull)
            {
                hintText.text = Data.HintBoardFull;
                return;
            }
            if (result == SpawnResultType.NoEnergy)
            {
                ShowAdModal();
                return;
            }
            if (result == SpawnResultType.Spawned)
            {
                RefreshAll();
            }
        }

        void OnCellClicked(int index)
        {
            MoveResult result = state.TapCell(index);
            if (result.type == MoveResultType.Merged)
            {
                RefreshAll();
                StartCoroutine(PopAnimation(cellTileGO[result.mergedIntoIndex]));
            }
            else
            {
                RefreshAll();
            }
        }

        void OnFileClicked()
        {
            ChainType? filed = state.FileSelected();
            if (filed == null) return;
            RefreshAll();
            ShowRevealModal(Data.Chains[(int)filed.Value]);
        }

        // ---------------------------------------------------------------
        // Pop animation
        // ---------------------------------------------------------------

        IEnumerator PopAnimation(GameObject tile)
        {
            if (tile == null) yield break;
            Transform t = tile.transform;
            float duration = 0.3f;
            float elapsed = 0f;
            t.localScale = Vector3.one * 0.6f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float p = Mathf.Clamp01(elapsed / duration);
                float scale;
                if (p < 0.6f)
                    scale = Mathf.Lerp(0.6f, 1.25f, p / 0.6f);
                else
                    scale = Mathf.Lerp(1.25f, 1f, (p - 0.6f) / 0.4f);
                t.localScale = Vector3.one * scale;
                yield return null;
            }
            t.localScale = Vector3.one;
        }

        // ---------------------------------------------------------------
        // Rendering / refresh
        // ---------------------------------------------------------------

        void RefreshAll()
        {
            RefreshTopBar();
            RefreshBoard();
            RefreshGenerators();
            RefreshFileButton();
            RefreshHintBar();
            RefreshObjectives();
            RefreshSuspects();
        }

        void RefreshTopBar()
        {
            energyText.text = "Energy: " + state.energy;
            coinsText.text = "Coins: " + state.coins;
            levelText.text = "Level " + state.Level;

            float frac = (float)state.XpIntoLevel / Data.XpPerLevel;
            float fullWidth = xpFillAnchorParentWidth.rect.width;
            RectTransform fillRt = xpBarFill.rectTransform;
            fillRt.sizeDelta = new Vector2(fullWidth * frac, 0);
        }

        void RefreshBoard()
        {
            for (int i = 0; i < GameState.CellCount; i++)
            {
                BoardItem item = state.board[i];
                bool selected = state.selectedIndex.HasValue && state.selectedIndex.Value == i;

                if (item == null)
                {
                    cellBg[i].color = selected ? Color.Lerp(CellEmptyColor, EvidenceYellow, 0.4f) : CellEmptyColor;
                    cellTileImg[i].color = Color.clear;
                    cellTileLabel[i].text = "";
                    cellTileDots[i].text = "";
                    cellTileOutline[i].enabled = false;
                    continue;
                }

                ChainDef def = Data.Chains[(int)item.chain];
                cellBg[i].color = selected ? Color.Lerp(CellEmptyColor, EvidenceYellow, 0.5f) : CellEmptyColor;
                cellTileImg[i].color = def.color;
                cellTileLabel[i].text = def.tierLabels[item.tier - 1];
                cellTileDots[i].text = new string('\u25CF', item.tier);

                bool isTopTier = item.tier == Data.TierCount;
                cellTileOutline[i].enabled = isTopTier;
                if (isTopTier) cellTileOutline[i].color = EvidenceYellow;
            }
        }

        void RefreshGenerators()
        {
            fabricBtn.interactable = !state.chainFiled[0];
            printsBtn.interactable = !state.chainFiled[1];
            phoneBtn.interactable = !state.chainFiled[2];
        }

        void RefreshFileButton()
        {
            fileBtn.interactable = state.CanFileSelected();
        }

        void RefreshHintBar()
        {
            if (state.HasTopTierUnfiledOnBoard())
            {
                hintText.text = Data.HintFileReady;
            }
            else if (state.HasMatchingPairOnBoard())
            {
                hintText.text = Data.HintMatchExists;
            }
            else if (!state.HasAnyItemOnBoard())
            {
                hintText.text = Data.HintEmptyBoard;
            }
            else
            {
                hintText.text = Data.HintKeepDropping;
            }
        }

        void RefreshObjectives()
        {
            objFabricText.text = (state.chainFiled[0] ? "[done] " : "") + "File the Fabric chain";
            objPrintsText.text = (state.chainFiled[1] ? "[done] " : "") + "File the Prints chain";
            objPhoneText.text = (state.chainFiled[2] ? "[done] " : "") + "File the Phone chain";

            SetStrikethroughLook(objFabricText, state.chainFiled[0]);
            SetStrikethroughLook(objPrintsText, state.chainFiled[1]);
            SetStrikethroughLook(objPhoneText, state.chainFiled[2]);
        }

        void SetStrikethroughLook(Text t, bool done)
        {
            t.fontStyle = done ? FontStyle.Italic : FontStyle.Normal;
            t.color = done ? new Color(0.5f, 0.5f, 0.5f, 1f) : TextDark;
        }

        void RefreshSuspects()
        {
            SetSuspectChip(suspectKaranChip, suspectKaranBg, "Karan", state.chainFiled[0], "lied", new Color(0.85f, 0.25f, 0.2f));
            SetSuspectChip(suspectDevChip, suspectDevBg, "Dev", state.chainFiled[1], "cleared", new Color(0.2f, 0.6f, 0.3f));
            SetSuspectChip(suspectMeenaChip, suspectMeenaBg, "Meena", state.chainFiled[2], "unexplained", new Color(0.6f, 0.6f, 0.2f));
        }

        void SetSuspectChip(Text chip, Image bg, string name, bool revealed, string verdict, Color verdictColor)
        {
            if (revealed)
            {
                chip.text = name + ": " + verdict;
                chip.color = Color.white;
                bg.color = verdictColor;
            }
            else
            {
                chip.text = name + ": unknown";
                chip.color = TextDark;
                bg.color = new Color(0, 0, 0, 0.08f);
            }
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        Text CreateLabel(string text, Transform parent, int fontSize, Color color, FontStyle style, TextAnchor anchor)
        {
            GameObject go = new GameObject("Text", typeof(Text));
            go.transform.SetParent(parent, false);
            Text t = go.GetComponent<Text>();
            t.font = uiFont;
            t.fontSize = fontSize;
            t.color = color;
            t.fontStyle = style;
            t.alignment = anchor;
            t.text = text;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            return t;
        }
    }
}
