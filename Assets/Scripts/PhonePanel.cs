using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BapakSimulator
{
    [DisallowMultipleComponent]
    public sealed class PhonePanel : MonoBehaviour
    {
        [Serializable]
        private sealed class Contact
        {
            public UnityEngine.UI.Button button;
            public UnityEvent dial;
        }

        [SerializeField] private PickupDay household;
        [SerializeField] private PlayerController player;
        [SerializeField] private SchoolPickupDialogue school;
        [SerializeField] private FactoryPickupRequest factory;
        [SerializeField] private InputAction togglePhone;
        [SerializeField] private InputAction toggleFullPhone;
        [SerializeField] private GameObject financePage;
        [SerializeField] private GameObject riskPage;
        [SerializeField] private HouseholdFinances finances;
        [SerializeField] private FinanceOverdueTrigger financeReminder;
        [SerializeField] private GameObject screen;
        [SerializeField] private GameObject contactsPage;
        [SerializeField] private GameObject notesPage;
        [SerializeField] private GameObject ojolPage;
        [SerializeField] private UnityEngine.UI.Text ojolStatus;
        [SerializeField] private UnityEngine.UI.Button ojolOffer;
        [SerializeField] private NgojolMechanic ojol;
        [SerializeField] private UnityEngine.UI.Text header;
        [SerializeField] private UnityEngine.UI.Text notes;
        [SerializeField] private UnityEngine.UI.Text incoming;
        [SerializeField] private GameObject callPage;
        [SerializeField] private UnityEngine.UI.Text callerLabel;
        [SerializeField] private UnityEngine.UI.Button answerButton;
        [SerializeField] private UnityEngine.EventSystems.EventSystem eventSystem;
        [SerializeField, Min(.05f)] private float callSlideSeconds;
        [SerializeField] private CommuteLeg[] commutes = Array.Empty<CommuteLeg>();
        [SerializeField] private Contact[] contacts = Array.Empty<Contact>();

        private RectTransform phoneRect;
        private RectTransform phoneCanvasRect;
        private Vector2 compactAnchor;
        private Vector2 compactPivot;
        private Vector2 compactPosition;
        private Vector2 previousCanvasSize;
        private bool fullPhone;
        private CursorLockMode previousLock;
        private bool previousVisibility;
        private enum Caller { None, School, Factory }

        private bool open;
        private bool openedForCall;
        private Caller currentCaller;
        private GameObject previousSelection;
        private float slideStarted = -1f;
        private int renderedMinute = -1;
        private int renderedCash = -1;
        private float nextNotesRefresh;

        private void Awake()
        {
            if (household == null || player == null || school == null || factory == null ||
                togglePhone == null || togglePhone.bindings.Count == 0 || screen == null ||
                toggleFullPhone == null || toggleFullPhone.bindings.Count == 0 || financePage == null || finances == null ||
                contactsPage == null || notesPage == null || ojolPage == null || ojolStatus == null || ojolOffer == null || ojol == null || header == null || notes == null || incoming == null ||
                callPage == null || callerLabel == null || answerButton == null || eventSystem == null || callSlideSeconds <= 0 ||
                Array.Exists(contacts, contact => contact == null || contact.button == null || contact.dial == null) ||
                Array.Exists(commutes, commute => commute == null))
            {
                Debug.LogError($"Phone '{name}' has incomplete references.", this);
                enabled = false;
                return;
            }

            phoneRect = screen.GetComponent<RectTransform>();
            phoneCanvasRect = (RectTransform)phoneRect.parent;
            compactAnchor = phoneRect.anchorMin;
            compactPivot = phoneRect.pivot;
            compactPosition = phoneRect.anchoredPosition;

            foreach (var contact in contacts)
                contact.button.onClick.AddListener(() =>
                {
                    if (!open || player.IsPaused) return;
                    Close();
                    contact.dial.Invoke();
                });
            callPage.SetActive(false);
            screen.SetActive(false);
        }

        private void OnEnable()
        {
            togglePhone?.Enable();
            toggleFullPhone?.Enable();
        }

        private void OnDisable()
        {
            togglePhone?.Disable();
            toggleFullPhone?.Disable();
            Close();
        }

        private void Update()
        {
            if (!player.IsPaused && togglePhone.WasPressedThisFrame())
            {
                if (currentCaller != Caller.None) DeclineIncoming();
                else if (open) Close();
                else Open();
            }

            UpdateIncomingCall();
            if (!open || !household.IsConfigured) return;
            if (!player.IsPaused && toggleFullPhone.WasPressedThisFrame())
            {
                fullPhone = !fullPhone;
                ApplyPhoneSize();
            }
            else if (slideStarted >= 0f || fullPhone && phoneCanvasRect.rect.size != previousCanvasSize)
                ApplyPhoneSize();
            if (renderedMinute != household.Minute || renderedCash != household.Cash)
            {
                renderedMinute = household.Minute;
                renderedCash = household.Cash;
                header.text = $"{FormatTime(renderedMinute)}   •   Rp{renderedCash:N0}";
            }
            if (ojolPage.activeSelf) RefreshOjol();
            if (notesPage.activeSelf && Time.unscaledTime >= nextNotesRefresh)
            {
                nextNotesRefresh = Time.unscaledTime + .2f;
                RefreshNotes();
            }
        }

        private Caller RingingCaller => factory.IsRinging ? Caller.Factory :
            school.HasIncomingCall ? Caller.School : Caller.None;

        private string NameOf(Caller caller) => caller == Caller.Factory ? factory.CallerName : school.CallerName;

        private void UpdateIncomingCall()
        {
            var ringing = RingingCaller;
            if (currentCaller != Caller.None && currentCaller != ringing)
            {
                bool close = openedForCall;
                HideIncoming();
                if (close) Close();
            }

            if (ringing != Caller.None && currentCaller == Caller.None && !player.IsPaused)
            {
                bool wasOpen = open;
                if (!open) Open();
                if (open)
                {
                    openedForCall = !wasOpen;
                    currentCaller = ringing;
                    previousSelection = eventSystem.currentSelectedGameObject;
                    callerLabel.text = NameOf(ringing);
                    callPage.SetActive(true);
                    eventSystem.SetSelectedGameObject(answerButton.gameObject);
                    if (openedForCall)
                    {
                        slideStarted = Time.unscaledTime;
                        ApplyPhoneSize();
                    }
                }
            }

            bool showBanner = ringing != Caller.None && currentCaller == Caller.None;
            if (incoming.gameObject.activeSelf != showBanner) incoming.gameObject.SetActive(showBanner);
            if (showBanner) incoming.text = $"{NameOf(ringing)} menelepon";
        }

        public void AnswerIncoming()
        {
            if (player.IsPaused || currentCaller == Caller.None) return;
            var caller = currentCaller;
            HideIncoming();
            Close();
            if (caller == Caller.Factory) factory.TryAnswerCall();
            else school.TryAnswerCall();
        }

        public void DeclineIncoming()
        {
            if (player.IsPaused || currentCaller == Caller.None) return;
            if (currentCaller == Caller.Factory) factory.SilenceIncomingCall();
            else school.DismissIncomingCall();
            bool close = openedForCall;
            HideIncoming();
            if (close) Close();
        }

        private void HideIncoming()
        {
            if (currentCaller == Caller.None) return;
            currentCaller = Caller.None;
            openedForCall = false;
            callPage.SetActive(false);
            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(previousSelection != null && previousSelection.activeInHierarchy
                    ? previousSelection : null);
            previousSelection = null;
        }

        public void Open()
        {
            if (!isActiveAndEnabled || open || !player.CanOpenDialogue) return;
            open = true;
            previousLock = Cursor.lockState;
            previousVisibility = Cursor.visible;
            player.SetGameplayInputBlocked(true);
            player.SetWorldInteractionBlocked(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            screen.SetActive(true);
            ApplyPhoneSize();
            ShowContacts();
        }

        public void Close()
        {
            if (!open || player.IsPaused && isActiveAndEnabled) return;
            HideIncoming();
            slideStarted = -1f;
            open = false;
            if (screen != null) screen.SetActive(false);
            if (player != null)
            {
                player.SetGameplayInputBlocked(false);
                player.SetWorldInteractionBlocked(false);
            }
            Cursor.lockState = previousLock;
            Cursor.visible = previousVisibility;
        }

        private void ApplyPhoneSize()
        {
            var anchor = fullPhone ? Vector2.one * .5f : compactAnchor;
            phoneRect.anchorMin = phoneRect.anchorMax = anchor;
            phoneRect.pivot = fullPhone ? Vector2.one * .5f : compactPivot;
            phoneRect.anchoredPosition = fullPhone ? Vector2.zero : compactPosition;
            var available = previousCanvasSize = phoneCanvasRect.rect.size;
            var size = phoneRect.rect.size;
            float scale = fullPhone ? .97f * Mathf.Min(available.x / size.x, available.y / size.y) : 1f;
            phoneRect.localScale = new Vector3(scale, scale, 1f);
            if (slideStarted >= 0f)
            {
                float progress = Mathf.Clamp01((Time.unscaledTime - slideStarted) / callSlideSeconds);
                float distance = fullPhone ? available.y : size.y + compactPosition.y;
                phoneRect.anchoredPosition += Vector2.down * distance * (1f - Mathf.SmoothStep(0f, 1f, progress));
                if (progress >= 1f) slideStarted = -1f;
            }
        }

        public void ShowContacts()
        {
            contactsPage.SetActive(true);
            notesPage.SetActive(false);
            ojolPage.SetActive(false);
            financePage.SetActive(false);
            if (riskPage != null) riskPage.SetActive(false);
        }

        public void ShowNotes()
        {
            contactsPage.SetActive(false);
            notesPage.SetActive(true);
            ojolPage.SetActive(false);
            financePage.SetActive(false);
            if (riskPage != null) riskPage.SetActive(false);
            RefreshNotes();
        }

        public void ShowOjol()
        {
            contactsPage.SetActive(false);
            notesPage.SetActive(false);
            ojolPage.SetActive(true);
            financePage.SetActive(false);
            if (riskPage != null) riskPage.SetActive(false);
            RefreshOjol();
        }

        public void ShowFinance()
        {
            contactsPage.SetActive(false);
            notesPage.SetActive(false);
            ojolPage.SetActive(false);
            financePage.SetActive(true);
            if (riskPage != null) riskPage.SetActive(false);
        }

        public void ShowRisk()
        {
            if (riskPage == null) return;
            contactsPage.SetActive(false);
            notesPage.SetActive(false);
            ojolPage.SetActive(false);
            financePage.SetActive(false);
            riskPage.SetActive(true);
        }

        public void OpenRisk()
        {
            Open();
            if (open) ShowRisk();
        }

        public void OpenOjolOffer()
        {
            if (!open || player.IsPaused || !ojol.CanOpenOffer) return;
            Close();
            ojol.OpenPhoneOffer();
        }

        private void RefreshOjol()
        {
            string status = $"{ojol.RiderName}\n\n{ojol.PhoneStatus}";
            if (ojolStatus.text != status) ojolStatus.text = status;
            ojolOffer.interactable = ojol.CanOpenOffer;
        }

        private void RefreshNotes()
        {
            var text = new StringBuilder(household.Objective);
            foreach (var commute in commutes)
            {
                string mark = commute.State switch
                {
                    CommuteLeg.Progress.Complete => "Selesai",
                    CommuteLeg.Progress.Underway => "Dalam perjalanan",
                    _ => "Belum"
                };
                text.Append('\n').Append(commute.Label).Append(" • ").Append(mark);
            }
            if (factory.State != FactoryPickupRequest.RequestState.None)
            {
                string status = factory.State switch
                {
                    FactoryPickupRequest.RequestState.Waiting => $"menunggu di pabrik sampai {FormatTime(factory.Deadline)}",
                    FactoryPickupRequest.RequestState.Promised => $"dijanjikan jemput • {FormatTime(factory.Deadline)}",
                    FactoryPickupRequest.RequestState.Completed => "sudah dijemput",
                    FactoryPickupRequest.RequestState.Expired => "waktu tunggu habis • pulang sendiri",
                    _ => "pulang sendiri"
                };
                text.Append("\n\n").Append(factory.CallerName).Append(" • ").Append(status);
            }

            if (!finances.BillPaid)
                text.Append("\n\n").Append(finances.BillName).Append(" • hari ").Append(finances.BillDueDay)
                    .Append(finances.BillOverdue ? " • Terlambat" : "");
            if (finances.LoanOutstanding > 0)
                text.Append("\nPinjol Rp").Append(finances.LoanOutstanding.ToString("N0")).Append(" • hari ")
                    .Append(finances.LoanDueDay).Append(finances.LoanOverdue ? " • Terlambat" : "");

            if (financeReminder != null && !string.IsNullOrWhiteSpace(financeReminder.CurrentReminder))
                text.Append("\n").Append(financeReminder.CurrentReminder);

            string value = text.ToString();
            if (notes.text != value) notes.text = value;
        }

        private static string FormatTime(int minute) => $"{minute / 60:00}:{minute % 60:00}";
    }
}
