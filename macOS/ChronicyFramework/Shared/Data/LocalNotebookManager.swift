//
//  LocalNotebookManager.swift
//  ChronicyFrameworkMacOS
//
//  Created by Alexandru Istrate on 22/04/2019.
//

import Foundation;
import CoreData;

public class LocalNotebookManager: NotebookManager {
    
    private var context: NSManagedObjectContext = CoreDataStorage.defaultContext;
    
    public init() {

    }
    
    public func getInfo(callback: @escaping NotebookManagerInfoCallback) {
        guard let notebooks: [CoreDataNotebook] = self.getNotebooks() else {
            Log.error(message: "Could not retrieve notebooks!");
            callback(nil, .fetchFailure);
            return;
        }
        
        let info: [NotebookInfo] = notebooks.map { (iter: CoreDataNotebook) -> NotebookInfo in
            return NotebookInfo(name: iter.name, id: iter.name, dateCreated: Date());
        }
        
        callback(info, nil);
    }
    
    public func retrieveNotebook(info: NotebookInfo, callback: @escaping NotebookManagerNotebookCallback) {
        guard let notebooks: [CoreDataNotebook] = self.getNotebooks() else {
            Log.error(message: "Could not retrieve notebooks!");
            callback(nil, .fetchFailure);
            return;
        }
        
        let filtered: [CoreDataNotebook] = notebooks.filter { (iter: CoreDataNotebook) -> Bool in
            // TODO: Compare the other fields
            return iter.name == info.name;
        }
        
//        guard filtered.count == 1 else {
//            callback(nil, .itemNotFound);
//            return;
//        }
        
        callback(filtered.first!.notebook, nil);
    }
    
    public func saveNotebook(notebook: Notebook) throws {
        if let existing: CoreDataNotebook = self.getNotebooks()?.first(where: { (iter: CoreDataNotebook) -> Bool in
            return iter.name == notebook.name;
        }) {
            existing.notebook = notebook;
            try self.saveCoreDataItems();
            return;
        }
        
        if let item: CoreDataNotebook = NSEntityDescription.insertNewObject(forEntityName: "CoreDataNotebook", into: self.context) as? CoreDataNotebook {
            item.notebook = notebook;
            try self.saveCoreDataItems();
            return;
        }
        
        Log.error(message: "Could not save notebook with name \(notebook.name)!");
    }
    
    private func getNotebooks() -> [CoreDataNotebook]? {
        let fetchRequest: NSFetchRequest<CoreDataNotebook> = CoreDataNotebook.fetchRequest();
        
        do {
            let notebooks: [CoreDataNotebook] = try self.context.fetch(fetchRequest);
            return notebooks;
        } catch let e {
            Log.error(message: "Could not fetch notebooks: \(e.localizedDescription)");
        }
        
        return nil;
    }
    
    private func saveCoreDataItems() throws {
        guard self.context.hasChanges else {
            return;
        }
        
        try self.context.save();
    }
}