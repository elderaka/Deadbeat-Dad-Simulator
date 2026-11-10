using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BapakSimulator
{
    [DisallowMultipleComponent, RequireComponent(typeof(PickupDay))]
    public sealed class CheckpointSave : MonoBehaviour
    {
        [SerializeField] private string slotId;
        [SerializeField] private ItemInventory inventory;
        [SerializeField] private ItemDefinition[] items = Array.Empty<ItemDefinition>();
        [SerializeField] private CommuteLeg[] trips = Array.Empty<CommuteLeg>();

        private readonly Dictionary<string, ItemDefinition> itemsById = new();
        private readonly Dictionary<string, CommuteLeg> tripsById = new();
        [SerializeField] private VehicleDrive[] vehicles = Array.Empty<VehicleDrive>();
        private readonly Dictionary<string, VehicleDrive> vehiclesById = new();

        private PickupDay day;
        private HouseholdFinances finances;
        private RiskArcade riskArcade;
        private string path;
        private bool recoveredBackup;
        private string titleRunSlot;

        public string DifficultyId { get; private set; }
        public bool HasInheritedHistory { get; private set; }

        private void Start()
        {
            day = GetComponent<PickupDay>();
            finances = GetComponent<HouseholdFinances>();
            riskArcade = GetComponent<RiskArcade>();
            if (!day.IsConfigured || !Guid.TryParseExact(slotId, "N", out var slot) ||
                (inventory == null && items.Length > 0) ||
                !Index(items, item => item.IsValid ? item.PersistentId : null, itemsById) ||
                !Index(trips, trip => trip.PersistentId, tripsById) ||
                !Index(vehicles, vehicle => vehicle.enabled ? vehicle.PersistentId : null, vehiclesById))
            {
                Debug.LogError($"Checkpoint Save on '{name}' needs a slot GUID and unique, valid IDs for its items, trips, and vehicles.", this);
                return;
            }

            var launch = NewGameLaunch.Take(slotId);
            titleRunSlot = launch?.RunSlot;
            path = CheckpointFile.GetPath(titleRunSlot ?? slot.ToString("N"));
            if (launch?.Preset != null)
            {
                if (finances == null || !finances.isActiveAndEnabled)
                {
                    Debug.LogError("A title-screen new game needs configured household finances.", this);
                    return;
                }

                DifficultyId = launch.Preset.Id;
                HasInheritedHistory = launch.IncludeHistory;
                day.Progression.SetStartingCash(launch.Preset.StartingCash);
                finances.SetInheritedLoan(launch.IncludeHistory ? launch.Preset.InheritedDebt : 0,
                    launch.Preset.InheritedDebtDueDay);
            }
            if (Load()) day.DayFinished += Save;
            day.StartAfterCheckpoint();
        }

        private void OnDestroy()
        {
            if (day != null) day.DayFinished -= Save;
        }

        private static bool Index<T>(T[] definitions, Func<T, string> getId, Dictionary<string, T> index)
            where T : UnityEngine.Object
        {
            foreach (var definition in definitions)
            {
                if (definition == null) return false;
                string id = getId(definition);
                if (string.IsNullOrWhiteSpace(id) || !index.TryAdd(id, definition)) return false;
            }
            return true;
        }

        private bool Load()
        {
            try
            {
                var data = Read(path);
                if (data == null)
                {
                    data = Read(path + ".bak");
                    if (data == null) return true;
                    recoveredBackup = true;
                }
                Restore(data);
                return true;
            }
            catch (InvalidDataException exception)
            {
                try
                {
                    var backup = Read(path + ".bak");
                    if (backup != null)
                    {
                        Restore(backup);
                        recoveredBackup = true;
                        Debug.LogWarning($"Recovered checkpoint backup for '{name}'. {exception.Message}", this);
                        return true;
                    }
                }
                catch (Exception backupError) when (IsSaveError(backupError))
                {
                    Debug.LogWarning($"Checkpoint backup for '{name}' could not be loaded. {backupError.Message}", this);
                }
                Debug.LogWarning($"Checkpoint for '{name}' could not be loaded. Saving is disabled for this session. {exception.Message}", this);
                return false;
            }
            catch (Exception exception) when (IsSaveError(exception))
            {
                Debug.LogWarning($"Checkpoint for '{name}' could not be loaded. Saving is disabled for this session. {exception.Message}", this);
                return false;
            }
        }

        private CheckpointData Read(string file)
        {
            string json;
            try { json = File.ReadAllText(file); }
            catch (FileNotFoundException) { return null; }
            catch (DirectoryNotFoundException) { return null; }

            CheckpointData data;
            try { data = CheckpointData.Read(json); }
            catch (ArgumentException exception) { throw new InvalidDataException("Invalid checkpoint JSON.", exception); }

            if (data.version > 0 && data.version != CheckpointData.CurrentVersion)
                throw new NotSupportedException($"Unsupported checkpoint version {data.version}.");
            if (!IsValid(data)) throw new InvalidDataException("Invalid or incomplete checkpoint data.");
            return data;
        }

        private bool IsValid(CheckpointData data)
        {
            if (data.version != CheckpointData.CurrentVersion || data.cash < 0 || data.latest == null ||
                data.loanOutstanding < 0 || (data.loanOutstanding == 0 ? data.loanDueDay != 0 || !string.IsNullOrEmpty(data.loanOfferId) : data.loanDueDay < 1) ||
                (finances == null && (data.billPaid || data.loanOutstanding > 0)) ||
                !data.latest.IsValid || data.previous == null ||
                (data.latest.day == 1 ? data.previous.day != 0 :
                    !data.previous.IsValid || data.previous.day != data.latest.day - 1) ||
                data.relationships == null || data.inventory == null || data.completedTrips == null || data.vehicles == null || data.npcFlags == null)
                return false;

            if (data.risk == null || (riskArcade == null && data.risk.Length > 0)) return false;
            var riskIds = new HashSet<string>();
            if (data.risk.Any(entry => entry == null || !entry.IsValid || !riskIds.Add(entry.id))) return false;

            var relationshipIds = new HashSet<string>();
            var inventoryIds = new HashSet<string>();
            var tripIds = new HashSet<string>();
            var vehicleIds = new HashSet<string>();
            var flags = new HashSet<string>();
            return data.npcFlags.All(flag => !string.IsNullOrWhiteSpace(flag) && flags.Add(flag)) &&
                data.relationships.All(entry => !string.IsNullOrWhiteSpace(entry.id) && relationshipIds.Add(entry.id)) &&
                data.inventory.All(entry => entry.value > 0 && !string.IsNullOrWhiteSpace(entry.id) &&
                    itemsById.ContainsKey(entry.id) && inventoryIds.Add(entry.id)) &&
                data.inventory.Sum(entry => (long)entry.value) <= int.MaxValue &&
                data.completedTrips.All(id => !string.IsNullOrWhiteSpace(id) && tripsById.ContainsKey(id) && tripIds.Add(id)) &&
                data.vehicles.All(vehicle => vehicle.IsValid && !string.IsNullOrWhiteSpace(vehicle.id) &&
                    vehiclesById.ContainsKey(vehicle.id) && vehicleIds.Add(vehicle.id));
        }

        private void Restore(CheckpointData data)
        {
            day.Progression.RestoreCheckpoint(data);
            day.RestoreCheckpoint(data);
            finances?.Restore(data);
            riskArcade?.Restore(data.risk);
            DifficultyId = data.difficultyId;
            HasInheritedHistory = data.inheritedHistory;
            foreach (var entry in data.inventory) inventory.TryAdd(itemsById[entry.id], entry.value);
            foreach (string id in data.completedTrips) tripsById[id].RestoreCompletedDay(day.DayNumber);
            foreach (var vehicle in data.vehicles) vehiclesById[vehicle.id].RestoreCheckpoint(vehicle);
        }

        private void Save()
        {
            if (!isActiveAndEnabled || !day.LastRecap.HasValue) return;

            var data = new CheckpointData
            {
                version = CheckpointData.CurrentVersion,
                cash = day.Cash,
                risk = riskArcade != null ? riskArcade.Capture() : Array.Empty<RiskGameState>(),
                difficultyId = DifficultyId,
                inheritedHistory = HasInheritedHistory,
                billPaid = finances != null && finances.BillPaid,
                loanOutstanding = finances != null ? finances.LoanOutstanding : 0,
                loanDueDay = finances != null ? finances.LoanDueDay : 0,
                loanOfferId = finances != null ? finances.LoanOfferId : null,
                recurringTransport = day.HasRecurringTransport,
                latest = new CheckpointData.RecapData(day.LastRecap.Value),
                previous = new CheckpointData.RecapData(day.PreviousRecap.GetValueOrDefault()),
                relationships = day.Progression.Relationships.Select(entry => new CheckpointData.Entry(entry.Key, entry.Value)).ToArray(),
                inventory = inventory == null ? Array.Empty<CheckpointData.Entry>() :
                    inventory.Stacks.Select(stack => new CheckpointData.Entry(stack.Key.PersistentId, stack.Value)).ToArray(),
                completedTrips = trips.Where(trip => trip.WasCompletedOn(day.DayNumber)).Select(trip => trip.PersistentId).ToArray(),
                npcFlags = day.Progression.NpcFlags.ToArray(),
                vehicles = vehicles.Select(vehicle => vehicle.CaptureCheckpoint()).ToArray()
            };

            try
            {
                if (!IsValid(data) || (inventory != null && inventory.Stacks.Any(stack =>
                    !itemsById.TryGetValue(stack.Key.PersistentId, out var definition) || definition != stack.Key)))
                    throw new InvalidDataException("Checkpoint contains invalid or unregistered data.");
                CheckpointFile.Write(path, data, recoveredBackup);
                if (titleRunSlot != null) NewGameLaunch.Remember(slotId, titleRunSlot);
                recoveredBackup = false;
            }
            catch (Exception exception) when (IsSaveError(exception))
            {
                Debug.LogWarning($"Checkpoint for '{name}' could not be saved. {exception.Message}", this);
            }
        }

        private static bool IsSaveError(Exception error) =>
            error is IOException || error is UnauthorizedAccessException || error is NotSupportedException;
    }
}
