//
//  ServerNotebookManager.swift
//  ChronicyFrameworkMacOS
//
//  Created by Alexandru Istrate on 18/04/2019.
//

import Foundation;

public class ServerNotebookManager: NotebookManager {
    
    private lazy var api: WebAPI = {
        return WebAPI();
    } ();
    
    private var token: String = "";
    
    public func getInfo(info: NotebookInfo, callback: @escaping NotebookManagerInfoCallback) {
        self.api.getInfo(token: self.token, info: info) { (response: NotebookInfoResponse?, error: RequestError?) in
            guard let response: NotebookInfoResponse = response else {
                callback(nil, .fetchFailure);
                return;
            }
            
            callback(NotebookInfo(name: response.name, id: response.id, dateCreated: response.dateCreated), nil);
        }
    }
    
    public func getInfo(callback: @escaping NotebookManagerInfoAllCallback) {
        self.api.getInfoForAll(token: self.token) { (response: NotebookInfoAllResponse?, error: RequestError?) in
            guard let response: NotebookInfoAllResponse = response else {
                callback(nil, .fetchFailure);
                return;
            }
            
            callback(response.notebooks.map({ (iter: NotebookInfoResponse) -> NotebookInfo in
                return NotebookInfo(name: iter.name, id: iter.id, dateCreated: iter.dateCreated);
            }), nil);
        }
    }
    
    public func retrieveNotebook(info: NotebookInfo, callback: @escaping NotebookManagerNotebookCallback) {
        self.api.getNotebook(token: token, info: info) { (notebook: Notebook?, error: RequestError?) in
            guard let notebook: Notebook = notebook else {
                callback(nil, .fetchFailure);
                return;
            }
            
            callback(notebook, nil);
        }
    }
}
