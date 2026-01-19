using System;
using System.IO;
using System.Linq;
using TestScript.DataClass;
using TestScript.Repository;

class Program
{
    static void Main()
    {
        string basePath = Path.Combine(Environment.CurrentDirectory, "data");

        Directory.CreateDirectory(basePath);

        var projektRepo          = new ProjektRepository(Path.Combine(basePath, "projekte.json"));
        var vorgangRepo          = new VorgangRepository(Path.Combine(basePath, "vorgaenge.json"));
        var ressourceRepo        = new RessourceRepository(Path.Combine(basePath, "ressourcen.json"));
        var workerRepo           = new WorkerRepository(Path.Combine(basePath, "worker.json"));
        var revisionRepo         = new VorgangRevisionRepository(Path.Combine(basePath, "revisionen.json"));

        Console.WriteLine("=== TEST START ===");

        // -----------------------------
        // Worker anlegen
        // -----------------------------
        var worker = new Worker
        {
            Name = "Max Mustermann"
        };

        worker = workerRepo.Insert(worker);
        Console.WriteLine($"Worker inserted: UID={worker.Uid}, Name={worker.Name}");

        // -----------------------------
        // Ressource anlegen
        // -----------------------------
        var ressource = new Ressource
        {
            Name = "Laptop",
            Anzahl = 5
        };

        ressource = ressourceRepo.Insert(ressource);
        Console.WriteLine($"Ressource inserted: UID={ressource.Uid}, Name={ressource.Name}, Anzahl={ressource.Anzahl}");

        // -----------------------------
        // Projekt anlegen
        // -----------------------------
        var projekt = new Projekt
        {
            Name = "Projekt A"
        };

        projekt = projektRepo.Insert(projekt);
        Console.WriteLine($"Projekt inserted: UID={projekt.Uid}, Name={projekt.Name}");

        // -----------------------------
        // Vorgang anlegen
        // -----------------------------
        var vorgang = new Vorgang
        {
            Dauer = 10,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(10),
            WorkerUid = worker.Uid
        };

        vorgang = vorgangRepo.Insert(vorgang);
        Console.WriteLine($"Vorgang inserted: UID={vorgang.Uid}, Dauer={vorgang.Dauer}, WorkerUid={vorgang.WorkerUid}");

        // -----------------------------
        // Revision anlegen
        // -----------------------------
        var revision = new VorgangRevision
        {
            VorgangUid = vorgang.Uid,
            OriginalUid = vorgang.Uid,
            RevisionNumber = 1
        };

        revision = revisionRepo.Insert(revision);
        Console.WriteLine($"Revision inserted: UID={revision.Uid}, VorgangUid={revision.VorgangUid}");

        // -----------------------------
        // GetAll Tests
        // -----------------------------
        Console.WriteLine("\n--- GET ALL TESTS ---");

        Console.WriteLine($"Worker count: {workerRepo.GetAll().Count}");
        Console.WriteLine($"Ressource count: {ressourceRepo.GetAll().Count}");
        Console.WriteLine($"Projekt count: {projektRepo.GetAll().Count}");
        Console.WriteLine($"Vorgang count: {vorgangRepo.GetAll().Count}");
        Console.WriteLine($"Revision count: {revisionRepo.GetAll().Count}");

        // -----------------------------
        // GetByUid Tests
        // -----------------------------
        Console.WriteLine("\n--- GET BY UID TESTS ---");

        var loadedWorker = workerRepo.GetByUid(worker.Uid);
        Console.WriteLine($"Loaded Worker: UID={loadedWorker?.Uid}, Name={loadedWorker?.Name}");

        var loadedProjekt = projektRepo.GetByUid(projekt.Uid);
        Console.WriteLine($"Loaded Projekt: UID={loadedProjekt?.Uid}, Name={loadedProjekt?.Name}");

        // -----------------------------
        // Update Tests
        // -----------------------------
        Console.WriteLine("\n--- UPDATE TESTS ---");

        loadedWorker!.Name = "Max Mustermann (Updated)";
        workerRepo.Update(loadedWorker);

        var updatedWorker = workerRepo.GetByUid(worker.Uid);
        Console.WriteLine($"Updated Worker: UID={updatedWorker?.Uid}, Name={updatedWorker?.Name}");

        loadedProjekt!.Name = "Projekt A (Updated)";
        projektRepo.Update(loadedProjekt);

        var updatedProjekt = projektRepo.GetByUid(projekt.Uid);
        Console.WriteLine($"Updated Projekt: UID={updatedProjekt?.Uid}, Name={updatedProjekt?.Name}");

        // -----------------------------
        // Delete Tests
        // -----------------------------
        Console.WriteLine("\n--- DELETE TESTS ---");

        bool workerDeleted = workerRepo.Delete(worker.Uid);
        Console.WriteLine($"Worker deleted: {workerDeleted}");

        var deletedWorker = workerRepo.GetByUid(worker.Uid);
        Console.WriteLine($"Worker after delete: {(deletedWorker == null ? "null" : "exists")}");

        // -----------------------------
        // Final State
        // -----------------------------
        Console.WriteLine("\n--- FINAL STATE ---");

        Console.WriteLine($"Worker count: {workerRepo.GetAll().Count}");
        Console.WriteLine($"Projekt count: {projektRepo.GetAll().Count}");
        Console.WriteLine($"Vorgang count: {vorgangRepo.GetAll().Count}");
        Console.WriteLine($"Revision count: {revisionRepo.GetAll().Count}");

        Console.WriteLine("\n=== TEST END ===");
        Console.ReadLine();
    }
}
